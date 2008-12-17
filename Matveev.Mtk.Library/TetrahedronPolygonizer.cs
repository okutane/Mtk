using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public abstract class TetrahedronPolygonizer : IImplicitSurfacePolygonizer 
    {
        protected IImplicitSurface _surface;
        private List<Vertex> _nonInternal;
        private Mesh _mesh;

        #region IImplicitSurfacePolygonizer Members

        public Mesh Create(ISimpleFactory<Mesh> factory, IImplicitSurface surface,
            double x0, double x1, double y0, double y1, double z0, double z1,
            int n, int m, int l)
        {
            _surface = surface;
            _mesh = factory.Create();
            _nonInternal = new List<Vertex>();

            double hx, hy, hz;

            hx = (x1 - x0) / n;
            hy = (y1 - y0) / m;
            hz = (z1 - z0) / l;

            for (int k = 0; k < l; k++)
            {
                for (int j = 0; j < m; j++)
                {
                    for (int i = 0; i < n; i++)
                    {
                        Point[] points = new Point[8];
                        double[] values = new double[8];
                        int bitsnum = 0;

                        for (int i2 = 0; i2 < 8; i2++)
                        {
                            points[i2] = new Point((i2 % 2) == 1 ? x0 + (i + 1) * hx : x0 + i * hx,
                                (i2 / 2 % 2) == 1 ? y0 + (j + 1) * hy : y0 + j * hy,
                                (i2 / 4) == 1 ? z0 + (k + 1) * hz : z0 + k * hz);
                            values[i2] = _surface.Eval(points[i2]);
                            if (values[i2] < 0)
                                bitsnum++;
                        }
                        CellSubroutine(_mesh, points, values, bitsnum);
                        _nonInternal.RemoveAll(vertex => vertex.Type == VertexType.Internal);
                    }
                }
            }

            _nonInternal.Clear();
            /* TODO: Mesh shouldn't contain any isolated vertices at this point,
             * fix this issue and remove that workaround. */
            _mesh.Vertices.Where(v => v.Type == VertexType.Isolated).ToList().ForEach(_mesh.RemoveVertex);
            return _mesh;
        }

        #endregion

        protected abstract void CellSubroutine(Mesh mesh, Point[] points, double[] values, int bitsnum);

        protected void TetrahedronSubroutine(Mesh mesh, Point[] p, double[] v)
        {
            bool[] flags = new bool[4];
            for (int i = 0; i < 4; i++)
                if (v[i] < 0)
                    flags[i] = true;

            //Sort
            Point pp;
            double vv;
            bool ccw = false;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (!flags[j] && flags[j + 1])
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

            if (flags[0] && !flags[1])
            {
                Point p1 = Point.Interpolate(p[0], v[0], p[1], v[1]);
                Point p2 = Point.Interpolate(p[0], v[0], p[2], v[2]);
                Point p3 = Point.Interpolate(p[0], v[0], p[3], v[3]);
                Vertex i1 = AddVertex(p1, Vector.Normalize(this._surface.Grad(p1)));
                Vertex i2 = AddVertex(p2, Vector.Normalize(this._surface.Grad(p2)));
                Vertex i3 = AddVertex(p3, Vector.Normalize(this._surface.Grad(p3)));
                if (ccw)
                    mesh.CreateFace(i1, i2, i3);
                else
                    mesh.CreateFace(i1, i3, i2);
            }
            else if (flags[1] && !flags[2])
            {
                Point p1 = Point.Interpolate(p[0], v[0], p[2], v[2]);
                Point p2 = Point.Interpolate(p[0], v[0], p[3], v[3]);
                Point p3 = Point.Interpolate(p[1], v[1], p[2], v[2]);
                Point p4 = Point.Interpolate(p[1], v[1], p[3], v[3]);
                Vertex i1 = AddVertex(p1, Vector.Normalize(this._surface.Grad(p1)));
                Vertex i2 = AddVertex(p2, Vector.Normalize(this._surface.Grad(p2)));
                Vertex i3 = AddVertex(p3, Vector.Normalize(this._surface.Grad(p3)));
                Vertex i4 = AddVertex(p4, Vector.Normalize(this._surface.Grad(p4)));
                if (ccw)
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
            else if (flags[2] && !flags[3])
            {
                Point p1 = Point.Interpolate(p[3], v[3], p[0], v[0]);
                Point p2 = Point.Interpolate(p[3], v[3], p[1], v[1]);
                Point p3 = Point.Interpolate(p[3], v[3], p[2], v[2]);
                Vertex i1 = AddVertex(p1, Vector.Normalize(this._surface.Grad(p1)));
                Vertex i2 = AddVertex(p2, Vector.Normalize(this._surface.Grad(p2)));
                Vertex i3 = AddVertex(p3, Vector.Normalize(this._surface.Grad(p3)));
                if (ccw)
                    mesh.CreateFace(i1, i2, i3);
                else
                    mesh.CreateFace(i1, i3, i2);
            }
        }

        private Vertex AddVertex(Point p, Vector n)
        {
            Vertex result = _nonInternal.Find(vertex => vertex.Point == p);
            if (result == null)
            {
                result = _mesh.AddVertex(p, n);
                _nonInternal.Add(result);
            }
            return result;
        }
    }
}
