using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class Bloomenthal
    {
        private static List<Vertex> nonInternal = new List<Vertex>();
        private static Mesh mesh;

        private static Vertex AddVertex(Point p, Vector n)
        {
            Vertex result = nonInternal.Find(delegate(Vertex vert)
            {
                return vert.Point == p;
            });
            if(result == null)
            {
                result = mesh.AddVertex(p, n);
                nonInternal.Add(result);
            }
            return result;
        }

        public static Mesh Create(IImplicitSurface field,
            double x0, double x1, double y0, double y1, double z0, double z1,
            int n, int m, int l)
        {
            mesh = new HEMesh();

            double hx, hy, hz;

            hx = (x1 - x0) / n;
            hy = (y1 - y0) / m;
            hz = (z1 - z0) / l;

            for(int k = 0 ; k < l ; k++)
            {
                for(int j = 0 ; j < m ; j++)
                {
                    for(int i = 0 ; i < n ; i++)
                        CellSubroutine(mesh, field, x0 + i * hx, x0 + (i + 1) * hx,
                            y0 + j * hy, y0 + (j + 1) * hy, z0 + k * hz, z0 + (k + 1) * hz);
                }
            }

            nonInternal.Clear();
            return mesh;
        }

        private static void CellSubroutine(Mesh mesh, IImplicitSurface field,
            double x0, double x1, double y0, double y1, double z0, double z1)
        {
            Point[] p = new Point[9];
            double[] v = new double[9];
            int bitsnum = 0;

            for(int i = 0 ; i < 8 ; i++)
            {
                p[i].X = (i % 2) == 1 ? x1 : x0;
                p[i].Y = (i / 2 % 2) == 1 ? y1 : y0;
                p[i].Z = (i / 4) == 1 ? z1 : z0;

                v[i] = field.Eval(p[i]);
                if(v[i] < 0)
                    bitsnum++;
            }

            p[8].X = (x0 + x1) / 2;
            p[8].Y = (y0 + y1) / 2;
            p[8].Z = (z0 + z1) / 2;
            v[8] = 0;
            for(int i = 0 ; i < 8 ; i++)
                v[8] += v[i] / 8;
            v[8] = field.Eval(p[8]);

            if(bitsnum > 0 && bitsnum < 8)
            {
                TetrahedronSubroutine(mesh, field, new Point[] { p[0], p[1], p[2], p[8] },
                    new double[] { v[0], v[1], v[2], v[8] });
                TetrahedronSubroutine(mesh, field, new Point[] { p[1], p[3], p[2], p[8] },
                    new double[] { v[1], v[3], v[2], v[8] });
                TetrahedronSubroutine(mesh, field, new Point[] { p[4], p[6], p[5], p[8] },
                    new double[] { v[4], v[6], v[5], v[8] });
                TetrahedronSubroutine(mesh, field, new Point[] { p[5], p[6], p[7], p[8] },
                    new double[] { v[5], v[6], v[7], v[8] });
                TetrahedronSubroutine(mesh, field, new Point[] { p[0], p[2], p[4], p[8] },
                    new double[] { v[0], v[2], v[4], v[8] });
                TetrahedronSubroutine(mesh, field, new Point[] { p[2], p[6], p[4], p[8] },
                    new double[] { v[2], v[6], v[4], v[8] });
                TetrahedronSubroutine(mesh, field, new Point[] { p[1], p[5], p[3], p[8] },
                    new double[] { v[1], v[5], v[3], v[8] });
                TetrahedronSubroutine(mesh, field, new Point[] { p[3], p[5], p[7], p[8] },
                    new double[] { v[3], v[5], v[7], v[8] });
                TetrahedronSubroutine(mesh, field, new Point[] { p[0], p[4], p[1], p[8] },
                    new double[] { v[0], v[4], v[1], v[8] });
                TetrahedronSubroutine(mesh, field, new Point[] { p[1], p[4], p[5], p[8] },
                    new double[] { v[1], v[4], v[5], v[8] });
                TetrahedronSubroutine(mesh, field, new Point[] { p[2], p[3], p[6], p[8] },
                    new double[] { v[2], v[3], v[6], v[8] });
                TetrahedronSubroutine(mesh, field, new Point[] { p[3], p[7], p[6], p[8] },
                    new double[] { v[3], v[7], v[6], v[8] });
            }
            nonInternal.RemoveAll(delegate(Vertex vert)
            {
                return vert.Type == VertexType.Internal;
            });
        }
        /*private static void CellSubroutine(Mesh mesh, Field field,
            double x0, double x1, double y0, double y1, double z0, double z1)
        {
            Point[] p = new Point[8];
            double[] v = new double[8];
            int bitsnum = 0;

            for(int i = 0 ; i < 8 ; i++)
            {
                p[i].x = (i % 2) == 1 ? x1 : x0;
                p[i].y = (i / 2 % 2) == 1 ? y1 : y0;
                p[i].z = (i / 4) == 1 ? z1 : z0;

                v[i] = field.Eval(p[i]) - field.Isovalue;
                if(v[i] < 0)
                    bitsnum++;
            }

            if(bitsnum > 0 && bitsnum < 8)
            {
                TetrahedronSubroutine(mesh, field,new Point[] { p[0], p[1], p[2], p[5] },
                    new double[] { v[0], v[1], v[2], v[5] });
                TetrahedronSubroutine(mesh, field, new Point[] { p[1], p[3], p[2], p[5] },
                    new double[] { v[1], v[3], v[2], v[5] });
                TetrahedronSubroutine(mesh, field, new Point[] { p[2], p[5], p[6], p[4] },
                    new double[] { v[2], v[5], v[6], v[4] });
                TetrahedronSubroutine(mesh, field, new Point[] { p[2], p[7], p[6], p[5] },
                    new double[] { v[2], v[7], v[6], v[5] });
                TetrahedronSubroutine(mesh, field, new Point[] { p[0], p[4], p[5], p[2] },
                    new double[] { v[0], v[4], v[5], v[2] });
                TetrahedronSubroutine(mesh, field, new Point[] { p[2], p[3], p[7], p[5] },
                    new double[] { v[2], v[3], v[7], v[5] });
            }
            nonInternal.RemoveAll(delegate(Vertex vert)
            {
                return vert.Type == VertexType.Internal;
            });
        }*/

        private static void TetrahedronSubroutine(Mesh mesh, IImplicitSurface field,Point[] p, double[] v)
        {
            bool[] flags = new bool[4];
            for (int i = 0; i < 4; i++)
                if(v[i] < 0)
                    flags[i] = true;

            //Sort
            Point pp;
            double vv;
            bool ccw = false;
            for(int i = 0 ; i < 4 ; i++)
            {
                for(int j = 0 ; j < 3 ; j++)
                {
                    if(!flags[j] && flags[j + 1])
                    {
                        flags[j] = true;
                        flags[j + 1] = false;
                        pp = p[j];
                        p[j] = p[j + 1];
                        p[j + 1] = pp;
                        vv = v[j];
                        v[j] = v[j + 1];
                        v[j + 1] = vv;
                        ccw = !ccw;
                    }
                }
            }

            if(flags[0] && !flags[1])
            {
                Point p1 = Point.Interpolate(p[0], v[0], p[1], v[1]);
                Point p2 = Point.Interpolate(p[0], v[0], p[2], v[2]);
                Point p3 = Point.Interpolate(p[0], v[0], p[3], v[3]);
                Vertex i1 = AddVertex(p1, Vector.Normalize(field.Grad(p1)));
                Vertex i2 = AddVertex(p2, Vector.Normalize(field.Grad(p2)));
                Vertex i3 = AddVertex(p3, Vector.Normalize(field.Grad(p3)));
                if(ccw)
                    mesh.CreateFace(i1, i2, i3);
                else
                    mesh.CreateFace(i1, i3, i2);
            }
            else if(flags[1] && !flags[2])
            {
                Point p1 = Point.Interpolate(p[0], v[0], p[2], v[2]);
                Point p2 = Point.Interpolate(p[0], v[0], p[3], v[3]);
                Point p3 = Point.Interpolate(p[1], v[1], p[2], v[2]);
                Point p4 = Point.Interpolate(p[1], v[1], p[3], v[3]);
                Vertex i1 = AddVertex(p1, Vector.Normalize(field.Grad(p1)));
                Vertex i2 = AddVertex(p2, Vector.Normalize(field.Grad(p2)));
                Vertex i3 = AddVertex(p3, Vector.Normalize(field.Grad(p3)));
                Vertex i4 = AddVertex(p4, Vector.Normalize(field.Grad(p4)));
                if(ccw)
                {
                    mesh.CreateFace(i1, i2, i4);
                    mesh.CreateFace(i1, i4, i3);
                }
                else
                {
                    mesh.CreateFace(i1, i4, i2);
                    mesh.CreateFace(i1, i3, i4);
                }

            }
            else if(flags[2] && !flags[3])
            {
                Point p1 = Point.Interpolate(p[3], v[3], p[0], v[0]);
                Point p2 = Point.Interpolate(p[3], v[3], p[1], v[1]);
                Point p3 = Point.Interpolate(p[3], v[3], p[2], v[2]);
                Vertex i1 = AddVertex(p1, Vector.Normalize(field.Grad(p1)));
                Vertex i2 = AddVertex(p2, Vector.Normalize(field.Grad(p2)));
                Vertex i3 = AddVertex(p3, Vector.Normalize(field.Grad(p3)));
                if(ccw)
                    mesh.CreateFace(i1, i2, i3);
                else
                    mesh.CreateFace(i1, i3, i2);
            }
        }
    }
}
