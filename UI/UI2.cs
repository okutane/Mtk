using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Matveev.Common;
using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.FaceFunctions;
using Matveev.Mtk.Library.Fields;

namespace UI
{
    public partial class UI2 : Form
    {
        private static readonly string LB = Environment.NewLine;

        private Mesh _mesh;
        private IEnumerable<Point> _points;

        private IImplicitSurface _field;

        private object _selection;

        int N = 2;
        double epsilon = 1e-2;//max point-to-surface range
        double alpha = 1e-3; //vertex value

        List<Control> meshActions, vertActions, edgeActions, faceActions;

        public UI2()
        {
            InitializeComponent();

            meshActions = new List<Control>();
            vertActions = new List<Control>();
            edgeActions = new List<Control>();
            faceActions = new List<Control>();

            ListMeshActions();
            ListVertexActions();
            ListEdgeActions();
            ListFaceActions();

            Selection = null;
        }

        private void ListFaceActions()
        {
            TextBox txtFaceInfo = new TextBox();
            txtFaceInfo.Multiline = true;
            txtFaceInfo.ReadOnly = true;
            txtFaceInfo.Width = panelActions.Width - 10;
            txtFaceInfo.Height = 150;
            txtFaceInfo.ParentChanged += delegate(object sender, EventArgs e)
            {
                if (Selection is Face)
                {
                    txtFaceInfo.Text = "Face distance:" + LB;
                    txtFaceInfo.Text = new FaceFunctionsCollector().GetInfo((Face)Selection);
                }
            };
            faceActions.Add(txtFaceInfo);
        }

        private void ListEdgeActions()
        {
            TextBox txtEdgeInfo = new TextBox();
            txtEdgeInfo.Multiline = true;
            txtEdgeInfo.ReadOnly = true;
            txtEdgeInfo.Width = panelActions.Width - 10;
            txtEdgeInfo.Height = 250;
            txtEdgeInfo.ParentChanged += delegate(object sender, EventArgs e)
            {
                if (Selection is Edge)
                {
                    txtEdgeInfo.Text = new EdgeFunctionsCollector().GetInfo((Edge)Selection);
                }
            };
            edgeActions.Add(txtEdgeInfo);

            Dictionary<string, EdgeTransform> transforms = InstanceCollector<EdgeTransform>.Instances;
            foreach (string key in transforms.Keys)
            {
                Button btnEdgeTransform = new Button();
                btnEdgeTransform.Text = key;
                btnEdgeTransform.Click += delegate(object sender, EventArgs e)
                {
                    Edge selected = Selection as Edge;
                    if (selected != null)
                    {
                        EdgeTransform transform = transforms[((Button)sender).Text];
                        if (!transform.IsPossible(selected))
                        {
                            MessageBox.Show("Transform is not possible");
                            return;
                        }
                        Selection = transform.Execute(selected);
                        if (Selection is Vertex)
                            this.visualizer.MarkedVerts = new Vertex[] { (Vertex)Selection };
                        else if (Selection is Edge)
                            this.visualizer.SelectedEdge = (Edge)Selection;
                        else if (Selection is Face)
                            this.visualizer.SelectedFace = (Face)Selection;
                    }
                };
                edgeActions.Add(btnEdgeTransform);
            }
        }

        private void ListVertexActions()
        {
            TextBox txtVertInfo = new TextBox();
            txtVertInfo.Multiline = true;
            txtVertInfo.ReadOnly = true;
            txtVertInfo.Width = panelActions.Width - 10;
            txtVertInfo.Height = 250;
            txtVertInfo.ParentChanged += delegate(object sender, EventArgs e)
            {
                if (Selection is Vertex)
                {
                    txtVertInfo.Text = _selection.ToString() + LB;
                    txtVertInfo.Text += "Point:" + LB;
                    txtVertInfo.Text += ((Vertex)_selection).Point + LB;
                    txtVertInfo.Text += "Normal:" + LB;
                    txtVertInfo.Text += ((Vertex)_selection).Normal + LB;
                    txtVertInfo.Text += "Field value:" + LB;
                    txtVertInfo.Text += _field.Eval(((Vertex)_selection).Point) + LB;
                    try
                    {
                        double curvature = VertexOps.Curvature(_selection as Vertex);
                        txtVertInfo.Text += "Curvature:" + LB;
                        txtVertInfo.Text += curvature + LB;
                    }
                    catch (Exception)
                    {
                    }
                }
            };
            vertActions.Add(txtVertInfo);
            Button btnOptimisePos = new Button();
            btnOptimisePos.Text = "Project";
            btnOptimisePos.Click += delegate(object sender, EventArgs e)
            {
                VertexOps.OptimizePosition(_selection as Vertex, _field, 0.01);
                Selection = _selection;
            };
            vertActions.Add(btnOptimisePos);
        }

        private void ListMeshActions()
        {
            Dictionary<string, IParametrizedSurface> parametrizedSurfaces =
                InstanceCollector<IParametrizedSurface>.Instances;
            Dictionary<string, IImplicitSurface> implicitSurfaces =
                InstanceCollector<IImplicitSurface>.Instances;
            Dictionary<string, IImplicitSurfacePolygonizer> implicitPolygonizers =
                InstanceCollector<IImplicitSurfacePolygonizer>.Instances;

            Label lblSurface = new Label();
            lblSurface.Text = "Поверхность";
            meshActions.Add(lblSurface);

            ComboBox cbxSurface = new ComboBox();
            cbxSurface.DataSource = new List<string>(parametrizedSurfaces.Keys.Union(implicitSurfaces.Keys));
            cbxSurface.DropDownStyle = ComboBoxStyle.DropDownList;
            meshActions.Add(cbxSurface);

            Button btnCreateParametrized = new Button();
            btnCreateParametrized.Text = "Parametrized";
            btnCreateParametrized.Click += delegate(object sender, EventArgs e)
            {
                IParametrizedSurface surface;
                if(parametrizedSurfaces.TryGetValue(cbxSurface.SelectedItem.ToString(), out surface))
                {
                    _mesh = ParametrizedSurfacePolygonizer.Instance.Create(surface, 32, 32);
                    visualizer.Mesh = _mesh;
                }
                _field = surface as IImplicitSurface;
            };
            meshActions.Add(btnCreateParametrized);

            ComboBox cbxImplicitPolygonizer = new ComboBox();
            cbxImplicitPolygonizer.DataSource = new List<string>(implicitPolygonizers.Keys);
            cbxImplicitPolygonizer.DropDownStyle = ComboBoxStyle.DropDownList;
            meshActions.Add(cbxImplicitPolygonizer);

            TextBox txtN = new TextBox();
            txtN.Text = N.ToString();
            txtN.TextChanged += delegate(object sender, EventArgs e)
            {
                try
                {
                    N = int.Parse(txtN.Text);
                    if (N < 2)
                        throw new Exception();
                    txtN.ForeColor = System.Drawing.Color.Black;
                }
                catch
                {
                    txtN.ForeColor = System.Drawing.Color.Red;
                }
            };
            meshActions.Add(txtN);

            Button btnImplicit = new Button();
            btnImplicit.Text = "Implicit";
            btnImplicit.Click += delegate(object sender, EventArgs e)
            {
                IImplicitSurface surface;
                IImplicitSurfacePolygonizer polygonizer;
                if (!implicitSurfaces.TryGetValue(cbxSurface.SelectedValue.ToString(), out surface))
                    return;
                if (!implicitPolygonizers.TryGetValue(cbxImplicitPolygonizer.SelectedValue.ToString(),
                    out polygonizer))
                    return;

                _mesh = polygonizer.Create(surface, -1, 1, -1, 1, -1, 1, N, N, N);
                visualizer.Mesh = _mesh;
                _field = surface;
            };
            meshActions.Add(btnImplicit);

            DoubleValueTextBox txtEps = new DoubleValueTextBox();
            txtEps.ValueChanged += (sender, e) => epsilon = e.NewValue;
            txtEps.Text = epsilon.ToString();
            meshActions.Add(txtEps);

            DoubleValueTextBox txtAlpha = new DoubleValueTextBox();
            txtAlpha.ValueChanged += (sender, e) => alpha = e.NewValue;
            txtAlpha.Text = alpha.ToString();
            meshActions.Add(txtAlpha);

            Button btnProjectAll = new Button();
            btnProjectAll.Text = "ProjectAll";
            btnProjectAll.Click += delegate(object sender, EventArgs e)
            {
                OptimizeMesh.ProjectAll(_mesh, _field, epsilon);
                MessageBox.Show("Done");
                this.Invalidate();
            };
            meshActions.Add(btnProjectAll);
            Button btnOptimizeAll = new Button();
            btnOptimizeAll.Text = "Optimize";
            btnOptimizeAll.Click += delegate(object sender, EventArgs e)
            {
                OptimizeMesh.OptimizeImplicit(_mesh, _field, epsilon, alpha);
                MessageBox.Show("Done");
                this.Invalidate();
            };
            meshActions.Add(btnOptimizeAll);

            Button btnMeshToPoints = new Button();
            btnMeshToPoints.Text = "Mesh to points";
            btnMeshToPoints.Click += delegate(object sender, EventArgs e)
            {
                this._points = from vertex in this._mesh.Vertices
                               select vertex.Point;
            };
            meshActions.Add(btnMeshToPoints);

            Button btnHoppeOptimization = new Button();
            btnHoppeOptimization.Text = "Hoppe";
            btnHoppeOptimization.Click += delegate(object sender, EventArgs e)
            {
                HoppeOptimization.OptimizeMesh(_mesh, this._points);
            };
            meshActions.Add(btnHoppeOptimization);

            Button btnMarkNonregular = new Button();
            btnMarkNonregular.Text = "Mark nonreg";
            btnMarkNonregular.Click += delegate(object sender, EventArgs e)
            {
                visualizer.MarkedVerts = from v in _mesh.Vertices
                                         where v.Type == VertexType.Internal
                                         && VertexOps.ExternalCurvature(v) > 0.1
                                         select v;
                Selection = null;
            };
            meshActions.Add(btnMarkNonregular);

            TextBox txtMeshInfo = new TextBox();
            txtMeshInfo.Multiline = true;
            txtMeshInfo.ReadOnly = true;
            txtMeshInfo.Width = panelActions.Width - 10;
            txtMeshInfo.Height = 150;
            txtMeshInfo.ParentChanged += delegate(object sender, EventArgs e)
            {
                if (_mesh == null)
                    return;

                double index = 0;
                int verts = 0, faces = 0;
                double E = 0;
                foreach (Vertex vert in _mesh.Vertices)
                {
                    if (vert.Type == VertexType.Internal)
                    {
                        double regularity = VertexOps.ExternalCurvature(vert);
                        index += regularity;
                    }

                    E += Math.Pow(_field.Eval(vert.Point), 2);

                    verts++;
                }
                faces = _mesh.Faces.Count();

                StringWriter writer = new StringWriter();
                writer.WriteLine("Verts: " + verts);
                writer.WriteLine("Faces: " + faces);
                writer.WriteLine("Index: " + index);
                writer.WriteLine("Energy: " + E);
                //writer.WriteLine("Sphere: "
                //    + _mesh.Faces.Sum(f => ImplicitLinearApproximationFaceFunction.Instance.Evaluate(f)));

                txtMeshInfo.Text = writer.ToString();
            };
            meshActions.Add(txtMeshInfo);
        }

        private object Selection
        {
            get
            {
                return _selection;
            }
            set
            {
                panelActions.Controls.Clear();
                _selection = value;
                if(_selection == null)
                    panelActions.Controls.AddRange(meshActions.ToArray());
                else if(_selection is Vertex)
                    panelActions.Controls.AddRange(vertActions.ToArray());
                else if(_selection is Edge)
                    panelActions.Controls.AddRange(edgeActions.ToArray());
                else if(_selection is Face)
                    panelActions.Controls.AddRange(faceActions.ToArray());
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
        private int _x0, _y0;
        private double _theta0, _phi0;
        private bool rotating;

        private void visualizer_MouseDown(object sender, MouseEventArgs e)
        {
            if(rotating == false && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                _x0 = e.X;
                _y0 = e.Y;
                _phi0 = visualizer.Phi;
                _theta0 = visualizer.Theta;
                rotating = true;
            }
            if(rotating == false && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Ray ray = visualizer.RayThroughScreen(e.X, e.Y);
                if(btnPoints.Checked)
                {
                    Vertex newSelection = null;
                    double min = 15;
                    double minRho = 0.1;
                    foreach(Vertex v in _mesh.Vertices)
                    {
                        double t = (v.Point.X - ray.origin.X) * ray.direction.x + (v.Point.Y - ray.origin.Y) * ray.direction.y + (v.Point.Z - ray.origin.Z) * ray.direction.z;
                        if(t < 0)
                            t = 0;
                        double rho = Math.Sqrt((v.Point - (ray.origin + t * ray.direction)).Norm);
                        if(rho < minRho && t < min)
                        {
                            newSelection = v;
                            min = t;
                        }
                    }

                    Selection = newSelection;
                    if(newSelection != null)
                        visualizer.MarkedVerts = new Vertex[] { newSelection };
                    else
                        visualizer.MarkedVerts = null;
                }
                if(btnEdges.Checked || btnFaces.Checked)
                {
                    Face faceSelection = null;
                    double mint = 45;
                    foreach(Face f in _mesh.Faces)
                    {
                        double t = ray.Trace(f);
                        if(t > 0 && t < mint)
                        {
                            faceSelection = f;
                            mint = t;
                        }
                    }
                    if(btnFaces.Checked)
                    {
                        Selection = faceSelection;
                        visualizer.SelectedFace = faceSelection;
                    }
                    else
                    {
                        Edge edgeSelection = null;
                        if (faceSelection != null)
                        {
                            Point p = ray.origin + mint * ray.direction;
                            double min = 45;
                            foreach (Edge edge in faceSelection.Edges)
                            {
                                double a, b, c, s;
                                a = (edge.Begin.Point - p).Norm;
                                b = (edge.End.Point - p).Norm;
                                c = (edge.End.Point - edge.Begin.Point).Norm;
                                s = (a + b + c) / 2;
                                double crit = Math.Sqrt(s * (s - a) * (s - b) * (s - c));
                                if (crit < min)
                                {
                                    edgeSelection = edge;
                                    min = crit;
                                }
                            }
                        }
                        Selection = edgeSelection;
                        visualizer.SelectedEdge = edgeSelection;
                    }
                }
            }
        }

        private void visualizer_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
                rotating = false;
        }

        private void visualizer_MouseMove(object sender, MouseEventArgs e)
        {
            if(rotating)
            {
                visualizer.Phi = _phi0 + (_x0 - e.X) * 0.01;
                visualizer.Theta = _theta0 + (_y0 - e.Y) * 0.01;
                visualizer.Invalidate();
            }
        }

        private void btnRestartCam_Click(object sender, System.EventArgs e)
        {
            visualizer.Translate = new Matveev.Mtk.Core.Point(0, 0, 0);
            visualizer.Phi = visualizer.Theta = 0;
            visualizer.Invalidate();
        }

        #region Selection mode buttons handlers

        private void btnPoints_Click(object sender, EventArgs e)
        {
            btnPoints.Checked = true;
            btnEdges.Checked = btnFaces.Checked = false;
        }

        private void btnEdges_Click(object sender, EventArgs e)
        {
            btnEdges.Checked = true;
            btnPoints.Checked = btnFaces.Checked = false;
        }

        private void btnFaces_Click(object sender, EventArgs e)
        {
            btnFaces.Checked = true;
            btnPoints.Checked = btnEdges.Checked = false;
        }

        #endregion

        private void enableLightingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            visualizer.EnableLightning = enableLightingToolStripMenuItem.Checked = !enableLightingToolStripMenuItem.Checked;
        }

        #region Draw points menu handlers
        private void drawWithNormalsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doNotDrawToolStripMenuItem.Checked = drawToolStripMenuItem.Checked = false;
            drawWithNormalsToolStripMenuItem.Checked = true;
            visualizer.DrawPoints = OglVisualizer.DrawPoints.DrawWithNormals;
        }

        private void drawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doNotDrawToolStripMenuItem.Checked = drawWithNormalsToolStripMenuItem.Checked = false;
            drawToolStripMenuItem.Checked = true;
            visualizer.DrawPoints = OglVisualizer.DrawPoints.Draw;
        }

        private void doNotDrawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawToolStripMenuItem.Checked = drawWithNormalsToolStripMenuItem.Checked = false;
            doNotDrawToolStripMenuItem.Checked = true;
            visualizer.DrawPoints = OglVisualizer.DrawPoints.DoNotDraw;
        }
        #endregion

        #region Normal selection menu handlers
        private void useVertexNormalsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            useFaceNormalsToolStripMenuItem.Checked = false;
            useVertexNormalsToolStripMenuItem.Checked = true;
            visualizer.NormalsToUse = OglVisualizer.NormalsType.VertexNormals;
        }

        private void useFaceNormalsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            useFaceNormalsToolStripMenuItem.Checked = true;
            useVertexNormalsToolStripMenuItem.Checked = false;
            visualizer.NormalsToUse = OglVisualizer.NormalsType.FaceNormals;
        }
        #endregion

        private void drawFaceNormalsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawFaceNormalsToolStripMenuItem.Checked = visualizer.DrawFaceNormals = !visualizer.DrawFaceNormals;
        }
    }
}