using System;
using System.Linq;
using System.Collections.Generic;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library.FaceFunctions;
using Matveev.Mtk.Library.Fields;
using Matveev.Mtk.Library.Validators;
using Matveev.Mtk.Library.Utilities;

namespace Matveev.Mtk.Library
{
    public delegate void LocalGradDelegate(Point[] points, Vector[] result);

    public static class OptimizeMesh
    {
        public static void ImproveVertexPositions(Mesh mesh, IImplicitSurface surface)
        {
            Vertex[] vertices = mesh.Vertices.ToArray();
            List<int> fixedPoints = new List<int>();
            for (int i = 0; i < vertices.Length; i++)
            {
                Point point = vertices[i].Point;
                if (vertices[i].Type != VertexType.Internal)
                {
                    fixedPoints.Add(i);
                }
            }
            Func<Vertex, int> indexSelector = vertex => Array.IndexOf(vertices, vertex);
            List<int[]> faces =               
                new List<int[]>(mesh.Faces.Select(face => face.Vertices.Select(indexSelector).ToArray()));

            Func<Point[], double> faceEnergy =
                TriangleImplicitApproximations.GetApproximation(surface.Eval, "square");

            QuadraticForm quadraticForm = surface as QuadraticForm;
            if (quadraticForm != null)
            {
                faceEnergy = quadraticForm.FaceDistance;
            }
            CompactQuadraticForm cqf = surface as CompactQuadraticForm;
            if (cqf != null)
            {
                faceEnergy = cqf.FaceDistance;
            }

            Point[] points = new Point[3];
            Func<double[], double> globalEnergy = delegate(double[] x)
            {
                double energy = 0;

                foreach (int[] face in faces)
                {
                    // TODO: Refactor, extract converting methods.
                    for (int i = 0; i < points.Length; i++)
                    {
                        int index = face[i];
                        points[i] = new Point(x[3 * index], x[3 * index + 1], x[3 * index + 2]);
                    }
                    double weight = points[0].AreaTo(points[1], points[2]);
                    weight = 1;
                    energy += weight * faceEnergy(points);
                }

                return energy;
            };

            LocalGradDelegate localGradient = LocalGradientProvider.GetNumericalGradient2(faceEnergy, 1e-6);

            if (cqf != null)
            {
                localGradient = delegate(Point[] facePoints, Vector[] result)
                {
                    double[] grad = cqf.GradOfFaceDistance(points);
                    result[0] = new Vector(grad[0], grad[1], grad[2]);
                    result[1] = new Vector(grad[3], grad[4], grad[5]);
                    result[2] = new Vector(grad[6], grad[7], grad[8]);
                };
            }

            Vector[] localGradValue = new Vector[3];
            GradDelegate globalGradient = delegate(double[] x, double[] result)
            {
                Array.Clear(result, 0, result.Length);
                foreach (int[] face in faces)
                {
                    // TODO: Refactor, extract converting methods.
                    for (int i = 0; i < face.Length; i++)
                    {
                        int index = face[i];
                        points[i] = new Point(x[3 * index], x[3 * index + 1], x[3 * index + 2]);
                    }
                    localGradient(points, localGradValue);
                    double weight = points[0].AreaTo(points[1], points[2]);
                    weight = 1;
                    for (int i = 0; i < face.Length; i++)
                    {
                        int index = face[i];
                        result[3 * index] += weight * localGradValue[i].x;
                        result[3 * index + 1] += weight * localGradValue[i].y;
                        result[3 * index + 2] += weight * localGradValue[i].z;
                    }
                }
                foreach (int fixedPoint in fixedPoints)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        result[3 * fixedPoint + i] = 0;
                    }
                }
            };

            double[] buffer = new double[3 * vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                Point point = vertices[i].Point;
                buffer[3 * i] = point.X;
                buffer[3 * i + 1] = point.Y;
                buffer[3 * i + 2] = point.Z;
            }

            FunctionOptimization.GradientDescent(globalEnergy, globalGradient, buffer, 1e-8, 300);

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Point = new Point(buffer[3 * i], buffer[3 * i + 1], buffer[3 * i + 2]);
            }
        }

        public static void OptimizeImplicit(Mesh mesh, IImplicitSurface field, double epsilon, double alpha)
        {
            ICollection<EdgeTransform> transforms = Configuration.EdgeTransforms;

            Dictionary<EdgeTransform, int> numbersOfUses = new Dictionary<EdgeTransform, int>();

            Func<Point[], double> faceEnergy =
                TriangleImplicitApproximations.GetApproximation(field.Eval, "square");
            if (field is QuadraticForm)
            {
                faceEnergy = ((QuadraticForm)field).FaceDistance;
            }
            Energy energy = new CompositeEnergy(new VertexEnergy(alpha), new FaceEnergy(faceEnergy));
            BoundingBox constraintsProvider = new BoundingBox(-1, 1, -1, 1, -1, 1);

            List<IMeshValidator> validators = new List<IMeshValidator>();
            validators.Add(new DihedralAnglesValidator(0.5));
            validators.Add(new FaceNormalsValidator());

            //���� 1. ������������� ���� ������ ����� �� �����������
            ProjectAll(mesh, field, epsilon);

            //���� 2. ��������� ������� ���� ������ �����
            ImproveVertexPositions(mesh, field);

            //���� 3. �������������� ��� ���������� �����
            Random rand = new Random(42);
            bool changed;
            do
            {
                changed = false;
                List<Edge> candidats = new List<Edge>(mesh.Edges);
                Edge candidat, smCandidat;
                Mesh submesh;
                Dictionary<Edge, Edge> edgeMap = new Dictionary<Edge, Edge>();
                double E1, E2;
                while (candidats.Count != 0)
                {
                    candidat = candidats[rand.Next(candidats.Count - 1)];
                    candidats.RemoveAll(edge => edge == candidat || edge == candidat.Pair);

                    IEnumerable<Face> surrounding =
                        candidat.Begin.AdjacentFaces.Concat(candidat.End.AdjacentFaces).Distinct();

                    submesh = mesh.CloneSub(surrounding, null, edgeMap, null);
                    smCandidat = edgeMap[candidat];
                    E1 = energy.Eval(submesh);

                    foreach (EdgeTransform transform in transforms)
                    {
                        if (!transform.IsPossible(candidat, constraintsProvider))
                        {
                            continue;
                        }

                        IDictionary<Edge, Edge> edgeMap2 = new Dictionary<Edge, Edge>();
                        Mesh submesh2 = submesh.Clone(edgeMap2);
                        Edge smCandidat2 = edgeMap2[smCandidat];

                        try
                        {
                            MeshPart smResult = transform.Execute(smCandidat2);
                        }
                        catch
                        {
                            continue;
                        }

                        if (!validators.TrueForAll(v => v.IsValid(submesh2)))
                        {
                            continue;
                        }

                        ImproveVertexPositions(submesh2, field);

                        E2 = energy.Eval(submesh2);

                        if (E1 <= E2)
                        {
                            continue;
                        }

                        if (numbersOfUses.ContainsKey(transform))
                        {
                            numbersOfUses[transform] = numbersOfUses[transform] + 1;
                        }
                        else
                        {
                            numbersOfUses.Add(transform, 1);
                        }
                        candidats.RemoveAll(edgeMap.ContainsKey);

                        MeshPart result = transform.Execute(candidat);

                        // TODO: Add local ImproveVertexPositions call here
                        candidats.AddRange(result.GetEdges(1));
                        changed = true;
                        break;
                    }
                }
            }
            while (changed);
        }

        public static void ProjectAll(Mesh mesh, IImplicitSurface field, double epsilon)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            foreach (Vertex vert in mesh.Vertices)
            {
                Point p = vert.Point;
                VertexOps.ProjectPointOnSurface(ref p, field, epsilon);
                vert.Point = p;
                vert.Normal = Vector.Normalize(field.Grad(p));
            }
        }

        private class FaceEnergy : Energy
        {
            private readonly Func<Point[], double> _faceEnergy;

            public FaceEnergy(Func<Point[], double> faceEnergy)
            {
                _faceEnergy = faceEnergy;
            }

            public override double Eval(Mesh mesh)
            {
                return mesh.Faces.Sum(face => _faceEnergy(face.Vertices.Select(vertex => vertex.Point).ToArray()));
            }
        }
    }
}
