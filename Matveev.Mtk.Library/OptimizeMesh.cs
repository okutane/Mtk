using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

using Matveev.Common;
using Matveev.Common.Utilities;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library.FaceFunctions;
using Matveev.Mtk.Library.Fields;
using Matveev.Mtk.Library.Validators;
using Matveev.Mtk.Library.Utilities;
using Matveev.Mtk.Library.VertexFunctions;
using GslNet;
using GslNet.MultiMin;

namespace Matveev.Mtk.Library
{
    public static class OptimizeMesh
    {
        private static IPointsFunctionWithGradient GetLocalFunctions(IImplicitSurface surface)
        {
            IPointsFunctionWithGradient energyProvider =
                TriangleImplicitApproximations.GetApproximation(surface, "square");
            if (false)
            {
                IPointsFunctionWithGradient preciseEnergyProvider = surface as IPointsFunctionWithGradient;
                if (preciseEnergyProvider != null)
                {
                    energyProvider = preciseEnergyProvider;
                }
            }
            return new FaceAreaDecorator(energyProvider);
            return energyProvider;
        }

        //public static void ClassifyVertices(IEnumerable<Vertex>

        public static void ImproveVertexPositions(Configuration configuration,
            IEnumerable<Vertex> verticesCollection, IProgressMonitor monitor,
            FunctionList functions)
        {
            Vertex[] verticesArray = verticesCollection.ToArray();

            int size = 0;
            var vertexToStrategyMap = new Dictionary<Vertex, IPointStrategy<GslVector, GslVector>>();
            Func<Vertex, IPointStrategy<GslVector, GslVector>> vertexToStrategyUncached = delegate(Vertex vertex)
            {
                int index = Array.IndexOf(verticesArray, vertex);
                if(index >= 0)
                {
                    if(vertex.Type == VertexType.Boundary)
                    {
                        var dof = new List<Vector>();
                        for(int i = 0 ; i < 3 ; i++)
                        {
                            Vector direction = new Vector(0, 0, 0);
                            direction[i] = 1;
                            if(configuration.BoundingBox.IsMovable(vertex, direction))
                            {
                                dof.Add(direction);
                            }
                        }
                        if(dof.Count == 1)
                        {
                            return new GslFixedOnVector(ref size, vertex.Point, dof[0]);
                        }
                        if(dof.Count == 2)
                        {
                            return new GslFixedOnPlane(ref size, vertex.Point, dof[0], dof[1]);
                        }
                        return new FixedPointStrategy(vertex.Point);
                    }
                    return new GslPointStrategy(ref size);
                }
                return new FixedPointStrategy(vertex.Point);
            };
            Func<Vertex, IPointStrategy<GslVector, GslVector>> vertexToStrategy = delegate(Vertex vertex)
            {
                IPointStrategy<GslVector, GslVector> result;
                if(!vertexToStrategyMap.TryGetValue(vertex, out result))
                {
                    result = vertexToStrategyUncached(vertex);
                    vertexToStrategyMap.Add(vertex, result);
                }
                return result;
            };

            var functionsToStrategies
                = new Dictionary<Pair<double, IPointsFunctionWithGradient>, IPointStrategy<GslVector, GslVector>[][]>();
            Func<IEnumerable<Vertex>, IPointStrategy<GslVector, GslVector>[]> verticesToStrategies =
                verts => verts.Select(vertexToStrategy).ToArray();
            foreach(var function in functions)
            {
                var linkedSets = function.Second.PointSelectionStrategy.GetAllLinkedSets(verticesArray);
                functionsToStrategies.Add(function, linkedSets.Select(verticesToStrategies).ToArray());
            }

            GslVector buffer = new GslVector(size);
            foreach(var vertex in verticesArray)
            {
                vertexToStrategyMap[vertex][buffer] = vertex.Point;
            }

            GslGlobalPointsFunctionWithGradient globalFunction =
                new GslGlobalPointsFunctionWithGradient(functionsToStrategies);

            try
            {
                if(Parameters.Instance.UseGradient)
                {
                    buffer = UGslMultimin.Optimize(globalFunction, buffer, size, monitor);
                }
                else
                {
                    buffer = UGslMultimin.Optimize((IFunction)globalFunction, buffer, size, monitor);
                }
            }
            finally
            {
                for(int i = 0 ; i < verticesArray.Length ; i++)
                {
                    verticesArray[i].Point = vertexToStrategyMap[verticesArray[i]][buffer];
                }
            }
        }

        public static double Eval(this IEnumerable<Pair<double, IPointsFunctionWithGradient>> energy, Mesh mesh)
        {
            double result = 0;
            foreach (var item in energy)
            {
                double partialResult = 0;
                foreach (var set in item.Second.PointSelectionStrategy.GetAllLinkedSets(mesh.Vertices.ToArray()))
                {
                    partialResult += item.Second.Evaluate(set.Select(v => v.Point).ToArray());
                }
                result += item.First * partialResult;
            }
            return result;
        }

        public static void OptimizeImplicit(Mesh mesh, IImplicitSurface surface, double epsilon, double alpha,
            IProgressMonitor monitor, Configuration configuration)
        {
            FunctionList functions = new FunctionList();
            functions.Add(surface);
            OptimizeImplicit(mesh, epsilon, alpha, monitor, configuration, functions);
        }

        public static void OptimizeImplicit(Mesh mesh, double epsilon, double alpha, IProgressMonitor monitor,
            Configuration configuration, FunctionList energy)
        {
            Func<Vertex, IEnumerable<Vertex>> Grow = v => v.Adjacent;
            NullProgressMonitor nullMonitor = new NullProgressMonitor();

            ICollection<EdgeTransform> transforms = configuration.EdgeTransforms;

            var transformsToUses = transforms.ToDictionary(transform => transform, transform => 0);

            energy.Add(alpha, VertexEnergy.Instance);

            List<IMeshValidator> validators = new List<IMeshValidator>();
            validators.Add(new DihedralAnglesValidator(0.5));
            validators.Add(new FaceNormalsValidator());

            //Этап 1. Проецирование всех вершин сетки на поверхность
            //ProjectAll(mesh, field, epsilon);

            //Этап 2. Улучшение позиций всех вершин сетки
            ImproveVertexPositions(configuration, mesh.Vertices, monitor, energy);

            //Этап 3. Преобразования над структурой сетки
            Random rand = new Random(42);
            bool changed;
            do
            {
                changed = false;
                var candidats = mesh.Edges.ToList();
                Edge candidat, smCandidat;
                Mesh submesh;
                Dictionary<Edge, Edge> edgeMap = new Dictionary<Edge, Edge>();
                double E1, E2;
                double oldFullEnergy = energy.Eval(mesh);
                while (candidats.Count != 0 && !monitor.IsCancelled)
                {
                    monitor.ReportProgress(100 - 100 * candidats.Count / mesh.EdgesCount);
                    candidat = candidats[rand.Next(candidats.Count - 1)];
                    candidats.RemoveAll(edge => edge == candidat || edge == candidat.Pair);

                    //var surrounding = candidat.Begin.AdjacentFaces.Concat(candidat.End.AdjacentFaces).Distinct();
                    var surrounding =
                        candidat.GetVertices(0).SelectMany(Grow).SelectMany(Grow).SelectMany(v => v.AdjacentFaces).Distinct().ToArray();

                    submesh = mesh.CloneSub(surrounding, null, edgeMap, null);
                    //submesh = mesh.Clone(edgeMap);
                    smCandidat = edgeMap[candidat];
                    E1 = energy.Eval(submesh);

                    foreach (EdgeTransform transform in transforms)
                    {
                        if (!transform.IsPossible(candidat, configuration.BoundingBox))
                        {
                            continue;
                        }

                        IDictionary<Edge, Edge> edgeMap2 = new Dictionary<Edge, Edge>();
                        Mesh submesh2 = submesh.Clone(edgeMap2);
                        Edge smCandidat2 = edgeMap2[smCandidat];

                        MeshPart smResult;
                        try
                        {
                            smResult = transform.Execute(smCandidat2);
                        }
                        catch
                        {
                            continue;
                        }

                        //ImproveVertexPositions(configuration, smResult.GetVertices(0), nullMonitor, energy);
                        OptimizeSubmesh(configuration, submesh2, energy);

                        if (!validators.TrueForAll(v => v.IsValid(submesh2)))
                        {
                            continue;
                        }

                        E2 = energy.Eval(submesh2);
                        double localBoost = E1 - E2;
                        if (localBoost <= 1e-5)
                        {
                            continue;
                        }

                        candidats.RemoveAll(edgeMap.ContainsKey);

                        //MeshPart result = transform.Execute(candidat);
                        transformsToUses[transform]++;

                        //var vertices = result.GetVertices(0);
                        //var vertices =
                          //  result.GetVertices(0).SelectMany(Grow).SelectMany(Grow).Distinct();
                        //ImproveVertexPositions(configuration, vertices, nullMonitor, energy);
                        foreach(var key in edgeMap.Keys.ToList())
                        {
                            edgeMap[key] = edgeMap2[edgeMap[key]];
                        }
                        mesh.Attach(submesh2, edgeMap);
                        double newFullEnergy = energy.Eval(mesh);
                        double globalBoost = oldFullEnergy - newFullEnergy;
                        Contract.Assert(globalBoost > 0,
                            "Energy has been grown during optimization");
                        //Contract.Assert(globalBoost >= local11Boost,
                          //  "Global boost is smaller than local");
                        oldFullEnergy = newFullEnergy;
                        candidats.AddRange(submesh2.Edges);
                        changed = true;
                        break;
                    }
                }
            }
            while (changed && !monitor.IsCancelled);

            foreach (var item in transformsToUses)
            {
                Console.WriteLine("{0} used {1} time(s).", item.Key, item.Value);
            }
        }

        internal static void OptimizeSubmesh(Configuration configuration, Mesh mesh, FunctionList energy)
        {
            ImproveVertexPositions(configuration, mesh.Vertices, NullProgressMonitor.Instance, energy);
            //ImproveVertexPositions(configuration, mesh.Vertices.Where(VertexOps.IsInternal), NullProgressMonitor.Instance, energy);
        }

        public static void ProjectAll(Mesh mesh, IImplicitSurface field, double epsilon)
        {
            Contract.Requires(mesh != null);
            Contract.Requires(field != null);

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

        private class LocalPointsFunctionWithGradient : AbstractPointsFunctionWithGradient
        {
            private readonly Func<Point[], double> _evaluate;
            private readonly GradientDelegate<Point, Vector> _evaluateGradient;

            public LocalPointsFunctionWithGradient(Func<Point[], double> evaluate,
                GradientDelegate<Point, Vector> evaluateGradient)
            {
                _evaluate = evaluate;
                _evaluateGradient = evaluateGradient;
            }

            public override double Evaluate(Point[] argument)
            {
                return _evaluate(argument);
            }

            public override void EvaluateGradient(Point[] argument, Vector[] result)
            {
                _evaluateGradient(argument, result);
            }
        }
    }

    public class FunctionList : List<Pair<double, IPointsFunctionWithGradient>>
    {
        public void Add(IPointsFunctionWithGradient function)
        {
            Add(new Pair<double, IPointsFunctionWithGradient>
            {
                First = 1,
                Second = function
            });
        }

        public void Add(IImplicitSurface surface)
        {
            Add(Parameters.Instance.SurfaceRangeValue, TriangleImplicitApproximations.GetApproximation(surface, "square"));
            Add(Parameters.Instance.SurfaceNormalsValue, new NormalDeviation(surface));
        }

        public void Add(double weight, IPointsFunctionWithGradient function)
        {
            if(weight == 0)
            {
                return;
            }
            Add(new Pair<double, IPointsFunctionWithGradient>
            {
                First = weight,
                Second = function
            });
        }
    }

    enum VertexCategory
    {
        Normal,
        Fixed,
        Constrained,
    }
}
