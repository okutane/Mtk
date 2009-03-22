using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.Utilities;
using OglVisualizer;

namespace UI
{
    class Colorers
    {
        private static readonly IDictionary<string, FaceColorEvaluatorDelegate> _faceColorers =
            new Dictionary<string, FaceColorEvaluatorDelegate>();
        private static readonly IDictionary<string, VertexColorEvaluatorDelegate> _vertexColorers =
            new Dictionary<string, VertexColorEvaluatorDelegate>();

        public static readonly IDictionary<string, FaceColorEvaluatorDelegate> FaceColorers = _faceColorers;
        public static readonly IDictionary<string, VertexColorEvaluatorDelegate> VertexColorers = _vertexColorers;

        public static double MaxArea = 1;

        static Colorers()
        {
            _faceColorers.Add("No coloring", null);
            _faceColorers.Add("Face normals", delegate(Face face)
            {
                int value = Math.Max(0, (int)(255 * -0.75 * face.Normal.z));
                return Color.FromArgb(value, value, value);
            });
            _faceColorers.Add("Area", delegate(Face face)
            {
                double t = face.Area() / MaxArea;
                t = Math.Max(0, Math.Min(t, 1));
                return Color.FromArgb((int)(255 * t), (int)(255 * (1 - t)), 0);
            });
            _vertexColorers.Add("Vertex normals", delegate(Vertex vertex)
            {
                int value = Math.Max(0, (int)(255 * -0.75 * vertex.Normal.z));
                return Color.FromArgb(value, value, value);
            });
            _vertexColorers.Add("Regularity", delegate(Vertex vertex)
            {
                return VertexOps.ExternalCurvature(vertex) > 0.5 ? Color.Red : Color.Green;
            });
        }
    }
}
