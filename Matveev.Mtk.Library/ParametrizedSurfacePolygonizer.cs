using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class ParametrizedSurfacePolygonizer
    {
        public readonly static ParametrizedSurfacePolygonizer Instance = new ParametrizedSurfacePolygonizer();

        private ParametrizedSurfacePolygonizer()
        {
        }

        public Mesh Create(IParametrizedSurface surface, int n, int m)
        {
            Builder builder = new Builder(surface, n, m);
            builder.Build();
            return builder.Product;
        }

        private class Builder
        {
            private IParametrizedSurface _surface;
            private int _n;
            private int _m;
            private double _minU;
            private double _minV;
            private double _stepU;
            private double _stepV;
            private Dictionary<int, Vertex> _vertices;
            private Mesh _product;

            public Builder(IParametrizedSurface surface, int n, int m)
            {
                this._surface = surface;
                this._n = n;
                this._m = m;
                this._minU = surface.MinU;
                this._minV = surface.MinV;
                this._stepU = (surface.MaxU - this._minU) / m;
                this._stepV = (surface.MaxV - this._minV) / n;
                this._vertices = new Dictionary<int, Vertex>();
                this._product = new HEMesh();
            }

            public Mesh Product
            {
                get
                {
                    return this._product;
                }
            }

            public void Build()
            {
                for (int i = 0; i < this._n; i++)
                {
                    for (int j = 0; j < this._m; j++)
                    {
                        Vertex v0 = GetVertex(i, j);
                        Vertex v1 = GetVertex(i, (j + 1) % _m);
                        Vertex v2 = GetVertex(i + 1, j);
                        Vertex v3 = GetVertex(i + 1, (j + 1) % _m);

                        this._product.CreateFace(v0, v1, v3);
                        this._product.CreateFace(v0, v3, v2);
                    }
                }
            }

            private Vertex GetVertex(int i, int j)
            {
                int n = 1000 * i + j;
                if (!this._vertices.ContainsKey(n))
                {
                    double u = this._minU + this._stepU * j;
                    double v = this._minV + this._stepV * i;
                    this._vertices.Add(n, this._product.AddVertex(this._surface.Eval(u, v),
                        this._surface.Normal(u, v)));
                }
                return this._vertices[n];
            }
        }
    }
}
