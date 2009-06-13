using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Common.Utilities;
using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.EdgeFunctions;
using Matveev.Mtk.Library.FaceFunctions;

namespace UI
{
    static class Globals
    {
        public static readonly Dictionary<string, Func<Vertex, double>> VertexFunctions =
            new Dictionary<string, Func<Vertex, double>>();
        public static readonly Dictionary<string, Func<Edge, double>> EdgeFunctions =
            new Dictionary<string, Func<Edge, double>>();
        public static readonly Dictionary<string, Func<Face, double>> FaceFunctions =
            new Dictionary<string, Func<Face, double>>();


        static Globals()
        {
            VertexFunctions.Add("Implicit", v => Configuration.Default.Surface.Eval(v.Point));
            VertexFunctions.Add("Implicit^2", v => Configuration.Default.Surface.Eval(v.Point).Sq());
            VertexFunctions.Add("Irregularity", VertexOps.ExternalCurvature);
            VertexFunctions.Add("Curvature", VertexOps.Curvature);

            Length edgeLength = new Length();
            EdgeFunctions.Add("Length", edgeLength.Evaluate);
            EdgeFunctions.Add("Length square", e => edgeLength.Evaluate(e).Sq());
            EdgeFunctions.Add("Diherdal angle", DihedralAngle.Instance.Evaluate);

            FaceFunctions.Add("Area", f => Math.Sqrt(AreaSquare.Instance.Evaluate(f.ToPoints())));
            FaceFunctions.Add("Area square", f => AreaSquare.Instance.Evaluate(f.ToPoints()));
            FaceFunctions.Add("Implicit(precise)",
                f => ((IPointsFunctionWithGradient)Configuration.Default.Surface).Evaluate(f.ToPoints()));
            foreach (string item in TriangleImplicitApproximations.AvailableApproximations)
            {
                FaceFunctions.Add(string.Format("Implicit({0})", item), f => TriangleImplicitApproximations.GetApproximation(Configuration.Default.Surface, item).Evaluate(f.ToPoints()));
            }
        }
    }
}
