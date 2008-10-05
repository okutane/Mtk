using System;
using System.Linq;
using System.Collections.Generic;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library.FaceFunctions;

namespace Matveev.Mtk.Library
{
    public static class OptimizeMesh
    {
        public static void ImproveVertexPositions(Mesh mesh, IImplicitSurface surface)
        {
            Vertex[] vertices = mesh.Vertices.ToArray();
            List<int> fixedPoints = new List<int>();
            double[] x = new double[3 * vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                Point point = vertices[i].Point;
                x[3 * i] = point.X;
                x[3 * i + 1] = point.Y;
                x[3 * i + 2] = point.Z;
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

            Func<double[], double> globalEnergy = delegate(double[] globalX)
            {
                double energy = 0;

                foreach (int[] face in faces)
                {
                    // TODO: Refactor, extract converting methods.
                    Point[] points = new Point[3];
                    for (int i = 0; i < points.Length; i++)
                    {
                        int index = face[i];
                        points[i] = new Point(x[3 * index], x[3 * index + 1], x[3 * index + 2]);
                    }
                    energy += faceEnergy(points);
                }

                return energy;
            };

            Func<Point[], Vector[]> localGradient = LocalGradientProvider.GetNumericalGradient2(faceEnergy, 1e-5);

            Func<double[], double[]> globalGradient = delegate(double[] globalX)
            {
                double[] result = new double[globalX.Length];

                foreach (int[] face in faces)
                {
                    // TODO: Refactor, extract converting methods.
                    Point[] points = new Point[3];
                    for (int i = 0; i < face.Length; i++)
                    {
                        int index = face[i];
                        points[i] = new Point(x[3 * index], x[3 * index + 1], x[3 * index + 2]);
                    }
                    Vector[] localGradValue = localGradient(points);
                    for (int i = 0; i < face.Length; i++)
                    {
                        int index = face[i];
                        result[3 * index] += localGradValue[i].x;
                        result[3 * index + 1] += localGradValue[i].y;
                        result[3 * index + 2] += localGradValue[i].z;
                    }
                }
                foreach (int fixedPoint in fixedPoints)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        result[3 * fixedPoint + i] = 0;
                    }
                }

                return result;
            };

            FunctionOptimization.GradientDescent(globalEnergy, globalGradient, x, 1e-8, 300);

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Point = new Point(x[3 * i], x[3 * i + 1], x[3 * i + 2]);
            }
        }

        public static void OptimizeImplicit(Mesh mesh, IImplicitSurface field, double epsilon, double alpha)
        {
            ImproveVertexPositions(mesh, field);
            return;

            EdgeTransform _split = new EdgeSplit(0.5);
            EdgeTransform _swap = new EdgeSwap();
            EdgeTransform _collapseMiddle = new EdgeCollapse(0.5);
            EdgeTransform _collapseBegin = new EdgeCollapse(0.0);
            EdgeTransform _collapseEnd = new EdgeCollapse(1.0);

            //EdgeTransform[] transforms = new EdgeTransform[] { _collapseMiddle, _collapseBegin, _collapseEnd,
            //_swap,_split };
            EdgeTransform[] transforms = new EdgeTransform[] { };

            FaceFunction faceEnergy = new ImplicitLinearApproximationFaceFunction();
            //FaceFunction faceEnergy = new NormalDeviation();
            //FaceFunction faceEnergy = new DihedralAngles();
            Energy energy = new CompositeEnergy(new VertexEnergy(alpha));
            //Energy energy = new CompositeEnergy(new VertexEnergy(alpha), new DihedralEnergy());

            //Этап 1. Проецирование всех вершин сетки на поверхность
            ProjectAll(mesh, field, epsilon);

            //Этап 2. Улучшение позиций всех вершин сетки
            Tools.VertexPositionOptimizer.OptimizeAll(mesh, field, epsilon, energy);

            //Этап 3. Преобразования над структурой сетки
            Random rand = new Random(42);
            List<Edge> candidats = new List<Edge>();
            foreach (Edge edge in mesh.Edges)
                candidats.Add(edge);
            Edge candidat, smCandidat;
            List<Face> surrounding = new List<Face>();
            Mesh submesh;
            Dictionary<Edge, Edge> edgeMap = new Dictionary<Edge, Edge>();
            double E1, E2;
            while (candidats.Count != 0)
            {
                candidat = candidats[rand.Next(candidats.Count - 1)];
                candidats.RemoveAll(delegate(Edge edge)
                {
                    return edge == candidat || edge == candidat.Pair;
                });

                surrounding.Clear();
                foreach (Face face in candidat.Begin.AdjacentFaces)
                    if (!surrounding.Contains(face))
                        surrounding.Add(face);
                foreach (Face face in candidat.End.AdjacentFaces)
                    if (!surrounding.Contains(face))
                        surrounding.Add(face);

                submesh = mesh.CloneSub(surrounding.ToArray(), null, edgeMap, null);
                smCandidat = edgeMap[candidat];
                E1 = energy.Eval(submesh);

                foreach (EdgeTransform transform in transforms)
                {
                    if (!transform.IsPossible(candidat))
                        continue;

                    IDictionary<Edge, Edge> edgeMap2 = new Dictionary<Edge, Edge>();
                    Mesh submesh2 = submesh.Clone(edgeMap2);
                    Edge smCandidat2 = edgeMap2[smCandidat];

                    try
                    {
                        MeshPart smResult = transform.Execute(smCandidat2);

                        foreach (Vertex vertex in smResult.GetVertices(0))
                        {
                            VertexOps.OptimizePosition(vertex, field, epsilon);
                        }

                        E2 = energy.Eval(submesh2);

                        if (E1 > E2)
                        {
                            candidats.RemoveAll(delegate(Edge edge)
                            {
                                return edgeMap.ContainsKey(edge);
                            });

                            MeshPart result = transform.Execute(candidat);

                            foreach (Vertex vertex in smResult.GetVertices(0))
                            {
                                VertexOps.OptimizePosition(vertex, field, epsilon);
                            }
                            foreach (Edge edge in result.GetEdges(1))
                            {
                                candidats.Add(edge);
                            }
                            break;
                        }
                        else
                        {
                            Tools.VertexPositionOptimizer.OptimizeAll(submesh2, field, epsilon, energy);
                            double E3 = energy.Eval(submesh2);
                            if (E1 > E3)
                                throw new Exception("Should optimize vertex positions " + transform.ToString());
                        }
                    }
                    catch
                    {
                    }
                }

                /*
                #region Try collapse
                {

                    if (true)
                    {
                        surrounding.Clear();
                        foreach (Face face in candidat.Begin.AdjacentFaces)
                            if (!surrounding.Contains(face))
                                surrounding.Add(face);
                        foreach (Face face in candidat.End.AdjacentFaces)
                            if (!surrounding.Contains(face))
                                surrounding.Add(face);

                        submesh = mesh.CloneSub(surrounding.ToArray(), null, edgeMap, null);
                        E1 = energy.Eval(submesh);
                        smCandidat = edgeMap[candidat];

                        try
                        {
                            Vertex collapsed = _collapseMiddle.Execute(smCandidat);
                            VertexOps.OptimizePosition(collapsed, field, epsilon);

                            if (FaceOps.MeshSelfIntersectionTest(submesh))
                                throw new Exception("self-intersection!");
                            E2 = energy.Eval(submesh);

                            if (E1 > E2)
                            {
                                candidats.RemoveAll(delegate(Edge edge)
                           {
                               return edgeMap.ContainsKey(edge);
                           });

                                collapsed = _collapseMiddle.Execute(candidat);
                                VertexOps.OptimizePosition(collapsed, field, epsilon);
                                if (collapsed.Type == VertexType.Internal)
                                {
                                    List<Vertex> list = new List<Vertex>();
                                    list.Add(collapsed);
                                    foreach (Vertex vert in collapsed.Adjacent)
                                        if (vert.Type == VertexType.Internal)
                                            list.Add(vert);
                                    OptimizeVertexPositions(field, epsilon, list);
                                }
                                continue;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                #endregion
                
                #region Try swap
                if(candidat.Pair != null)
                {
                    surrounding.Clear();
                    foreach(Edge edge in mesh.Edges)
                    {
                        if(edge.End == candidat.End || edge.End == candidat.Begin)
                            if(!surrounding.Contains(edge.Face))
                                surrounding.Add(edge.Face);
                    }
                    submesh = mesh.CloneSub(surrounding.ToArray(), null, edgeMap, null);
                    smCandidat = edgeMap[candidat];
                    try
                    {
                        foreach(Edge edge in submesh.Edges)
                        {
                            if((edge.End == smCandidat.Next.End && edge.Begin == smCandidat.Pair.Next.End) || (edge.Begin == smCandidat.Next.End && edge.End == smCandidat.Pair.Next.End))
                                throw new Exception("Swap rejected!");
                        }

                        E1 = energy.Eval(submesh);
                        Edge swapped = submesh.EdgeSwap(smCandidat, null);

                        if(FaceOps.MeshSelfIntersectionTest(submesh))
                            throw new Exception("self-intersection!");

                        E2 = energy.Eval(submesh);

                        if(E1 > E2)
                        {
                            //Принимаем преобразование
                            candidats.RemoveAll(delegate(Edge edge)
                                {
                                    return edge.Face == candidat.Face || edge.Face == candidat.Pair.Face;
                                });
                            swapped = mesh.EdgeSwap(candidat, affected);
                            foreach(Face face in affected)
                                foreach(Edge edge in face.Edges)
                                    candidats.Add(edge);
                            continue;
                        }
                    }
                    catch
                    {
                    }
                }

                #endregion
                
                #region Try split
                {
                    if (candidat.Pair != null)
                        submesh = mesh.CloneSub(new Face[] { candidat.Face, candidat.Pair.Face }, null, edgeMap, null);
                    else
                        submesh = mesh.CloneSub(new Face[] { candidat.Face }, null, edgeMap, null);
                    smCandidat = edgeMap[candidat];

                    E1 = energy.Eval(submesh);

                    Vertex splitted = _split.Execute(smCandidat);
                    VertexOps.OptimizePosition(splitted, field, epsilon);
                    Point p = splitted.Point;
                    Vector n = splitted.Normal;

                    E2 = energy.Eval(submesh);

                    if (E1 > E2)
                    {
                        //Принимаем преобразования для нашей сетки
                        candidats.RemoveAll(delegate(Edge edge)
                        {
                            return edgeMap.ContainsKey(edge);
                        });

                        splitted = _split.Execute(candidat);
                        splitted.Point = p;
                        splitted.Normal = n;

                        if (splitted.Type == VertexType.Internal)
                        {
                            List<Vertex> list = new List<Vertex>();
                            list.Add(splitted);
                            foreach (Vertex vert in splitted.Adjacent)
                                if (vert.Type == VertexType.Internal)
                                    list.Add(vert);
                            OptimizeVertexPositions(field, epsilon, list);
                        }
                        continue;
                    }
                }
                #endregion
            */
            }
        }

        public static void ProjectAll(Mesh mesh, IImplicitSurface field, double epsilon)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            Vector i, j, k;
            i = new Vector(1, 0, 0);
            j = new Vector(0, 1, 0);
            k = new Vector(0, 0, 1);

            foreach (Vertex vert in mesh.Vertices)
            {
                Point p = vert.Point;
                VertexOps.ProjectPointOnSurface(ref p, field, epsilon);
                vert.Point = p;
                vert.Normal = Vector.Normalize(field.Grad(p));
            }
        }
    }
}
