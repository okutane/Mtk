using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Matveev.Mtk.Core;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;

namespace Matveev.Mtk.Library
{
    public static class Utils
    {
        public static ICollection<T> MergeCollections<T>(params IEnumerable<T>[] collections)
        {
            ICollection<T> result = new List<T>();

            foreach (IEnumerable<T> collection in collections)
            {
                foreach (T item in collection)
                {
                    if (!result.Contains(item))
                        result.Add(item);
                }
            }

            return result;
        }

        public static void Serialize(IEnumerable<Face> faces, Stream stream)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<int> indices = new List<int>();

            foreach (Face face in faces)
            {
                foreach (Vertex vertex in face.Vertices)
                {
                    int index = vertices.IndexOf(vertex);
                    if (index == -1)
                    {
                        index = vertices.Count;
                        vertices.Add(vertex);
                    }

                    indices.Add(index);
                }
            }

            SoapFormatter formatter = new SoapFormatter();
            formatter.Serialize(stream, Array.ConvertAll<Vertex, double[]>(vertices.ToArray(), To6Doubles));
            formatter.Serialize(stream, indices.ToArray());
        }

        public static double[] To6Doubles(Vertex vertex)
        {
            double[] result = new double[6];
            result[0] = vertex.Point.X;
            result[1] = vertex.Point.Y;
            result[2] = vertex.Point.Z;

            result[3] = vertex.Normal.x;
            result[4] = vertex.Normal.y;
            result[5] = vertex.Normal.z;

            return result;
        }

        public static double Square(Face face)
        {
            Point[] p = face.Vertices.Select(v => v.Point).ToArray();
            for (int i = 1; i < 3; i++)
            {
                p[i].X -= p[0].X;
                p[i].Y -= p[0].Y;
                p[i].Z -= p[0].Z;
            }

            return p[1].Y * p[2].Z + p[1].X * p[2].Y + p[1].Z * p[2].X
                - p[1].Y * p[2].Z - p[1].X * p[2].Z - p[1].X * p[2].Y;
        }
    }
}
