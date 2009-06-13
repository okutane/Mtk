using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Color = System.Drawing.Color;

using Matveev.Common;
using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.FaceFunctions;
using Matveev.Mtk.Library.Fields;
using Matveev.Mtk.Library.Utilities;
using System.Globalization;
using OglVisualizer;
using Matveev.Mtk.Library.EdgeFunctions;
using Matveev.Mtk.Library.VertexFunctions;

namespace UI
{
    public partial class UI2 : Form
    {
        private static readonly string LB = Environment.NewLine;

        private Mesh _mesh;
        private IEnumerable<Point> _points;

        private MeshPart _selection;

        private Button btnImprovePositions;

        int N = 2;
        double epsilon = 1e-2;//max point-to-surface range
        double alpha = 1e-3; //vertex value

        private readonly Control[] _meshActions, _vertActions, _edgeActions, _faceActions;

        public UI2()
        {
            InitializeComponent();

            ControlListBuilder builder = new ControlListBuilder(this);
            ListMeshActions(builder);
            _meshActions = builder.Result;
            builder = new ControlListBuilder(this);
            ListVertexActions(builder);
            _vertActions = builder.Result;
            builder = new ControlListBuilder(this);
            ListEdgeActions(builder);
            _edgeActions = builder.Result;
            builder = new ControlListBuilder(this);
            ListFaceActions(builder);
            _faceActions = builder.Result;

            btnImprovePositions = new Button();
            btnImprovePositions.Text = "Improve position";
            btnImprovePositions.Click += delegate(object sender, EventArgs e)
            {
                var functions = new FunctionList();
                functions.Add(Configuration.Default.Surface);
                OptimizeMesh.ImproveVertexPositions(Configuration.Default, _selection.GetVertices(0),
                    NullProgressMonitor.Instance, functions);
                Selection = _selection;
                Invalidate(true);
            };

            Selection = null;

            var menu = toolStripMenuItem1;
            menu.DropDownItems.Add("-");
            List<ToolStripMenuItem> colorers = new List<ToolStripMenuItem>();
            foreach (var item in Colorers.FaceColorers)
            {
                var colorer = item.Value;
                colorers.Add(
                    (ToolStripMenuItem)menu.DropDownItems.Add(item.Key, null, delegate(object sender, EventArgs e)
                {
                    colorers.ForEach(mi => mi.Checked = false);
                    ((ToolStripMenuItem)sender).Checked = true;
                    visualizer.FaceColorEvaluator = colorer;
                }));
            }
            foreach (var item in Colorers.VertexColorers)
            {
                var colorer = item.Value;
                colorers.Add(
                    (ToolStripMenuItem)menu.DropDownItems.Add(item.Key, null, delegate(object sender, EventArgs e)
                {
                    colorers.ForEach(mi => mi.Checked = false);
                    ((ToolStripMenuItem)sender).Checked = true;
                    visualizer.VertexColorEvaluator = colorer;
                }));
            }
            colorers[0].Checked = true;

            propertyGrid1.SelectedObject = Parameters.Instance;
        }

        private void ListFaceActions(IControlBuilder builder)
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
            builder.AddControl(txtFaceInfo);
        }

        private void ListEdgeActions(IControlBuilder builder)
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
            builder.AddControl(txtEdgeInfo);

            Dictionary<string, EdgeTransform> transforms = InstanceCollector<EdgeTransform>.Instances;
            foreach (string key in transforms.Keys)
            {
                builder.AddButton(key, delegate()
                {
                    Edge selected = Selection as Edge;
                    if (selected != null)
                    {
                        EdgeTransform transform = transforms[key];
                        if (!transform.IsPossible(selected, null))
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
                });
            }
            builder.AddButton("Select next",
                () => Selection = visualizer.SelectedEdge = ((Edge)Selection).Next);
            builder.AddButton("Select pair",
                () => Selection = visualizer.SelectedEdge = ((Edge)Selection).Pair);
        }

        private void ListVertexActions(IControlBuilder builder)
        {
            TextBox txtVertInfo = new TextBox();
            txtVertInfo.Multiline = true;
            txtVertInfo.ReadOnly = true;
            txtVertInfo.Width = panelActions.Width - 10;
            txtVertInfo.Height = 250;
            txtVertInfo.ParentChanged += delegate(object sender, EventArgs e)
            {
                Vertex selection = Selection as Vertex;
                if (selection != null)
                {
                    // TODO: Use StringWriter or StringBuilder here
                    txtVertInfo.Text = selection.ToString() + LB;
                    txtVertInfo.Text += "Point:" + LB;
                    txtVertInfo.Text += selection.Point + LB;
                    txtVertInfo.Text += "Normal:" + LB;
                    txtVertInfo.Text += selection.Normal + LB;
                    txtVertInfo.Text += "Field value:" + LB;
                    txtVertInfo.Text += Configuration.Default.Surface.Eval(selection.Point) + LB;
                    try
                    {
                        double curvature = VertexOps.Curvature(selection);
                        txtVertInfo.Text += "Curvature:" + LB;
                        txtVertInfo.Text += curvature + LB;
                        double sphereImageArea =
                            Spherical.PolygonArea(selection.AdjacentFaces.Select(f => f.Normal).ToArray());
                        txtVertInfo.Text += string.Format("Sphere image area:\n{0}\n", sphereImageArea);
                    }
                    catch
                    {
                    }
                }
            };
            builder.AddControl(txtVertInfo);
            builder.AddButton("Project", delegate()
            {
                VertexOps.OptimizePosition((Vertex)_selection,
                    Configuration.Default.Surface, 0.01);
                Selection = _selection;
            });
        }

        private void ListMeshActions(IControlBuilder builder)
        {
            Dictionary<string, IParametrizedSurface> parametrizedSurfaces =
                InstanceCollector<IParametrizedSurface>.Instances;
            Dictionary<string, IImplicitSurface> implicitSurfaces =
                InstanceCollector<IImplicitSurface>.Instances;
            Dictionary<string, IImplicitSurfacePolygonizer> implicitPolygonizers =
                InstanceCollector<IImplicitSurfacePolygonizer>.Instances;

            builder.AddLabel("Поверхность");

            ComboBox cbxSurface = new ComboBox();
            cbxSurface.DataSource = new List<string>(parametrizedSurfaces.Keys.Union(implicitSurfaces.Keys));
            cbxSurface.DropDownStyle = ComboBoxStyle.DropDownList;
            builder.AddControl(cbxSurface);

            builder.AddButton("Parametrized", delegate()
            {
                IParametrizedSurface surface;
                if (parametrizedSurfaces.TryGetValue(cbxSurface.SelectedItem.ToString(), out surface))
                {
                    _mesh = ParametrizedSurfacePolygonizer.Instance.Create(surface, 32, 32);
                    visualizer.Mesh = _mesh;
                }
                Configuration.Default.Surface = surface as IImplicitSurface;
            });

            ComboBox cbxImplicitPolygonizer = new ComboBox();
            cbxImplicitPolygonizer.DataSource = new List<string>(implicitPolygonizers.Keys);
            cbxImplicitPolygonizer.DropDownStyle = ComboBoxStyle.DropDownList;
            builder.AddControl(cbxImplicitPolygonizer);

            TextBox txtN = new TextBox();
            txtN.Text = N.ToString();
            txtN.TextChanged += delegate(object sender, EventArgs e)
            {
                try
                {
                    N = int.Parse(txtN.Text);
                    if (N < 2)
                        throw new Exception();
                    txtN.ForeColor = Color.Black;
                }
                catch
                {
                    txtN.ForeColor = Color.Red;
                }
            };
            builder.AddControl(txtN);

            builder.AddButton("Implicit", delegate()
            {
                Configuration.Default.Surface = implicitSurfaces[cbxSurface.SelectedValue.ToString()];
                IImplicitSurfacePolygonizer polygonizer =
                    implicitPolygonizers[cbxImplicitPolygonizer.SelectedValue.ToString()];
                _mesh = polygonizer.Create(Configuration.Default, N, N, N);
                visualizer.Mesh = _mesh;
                if (_mesh.Faces.IsEmpty())
                {
                    Colorers.MaxArea = 0;
                }
                else
                {
                    Colorers.MaxArea = _mesh.Faces.Max(f => f.Area());
                }
                Selection = null;
            });

            DoubleValueTextBox txtEps = new DoubleValueTextBox();
            txtEps.ValueChanged += (sender, e) => epsilon = e.NewValue;
            txtEps.Text = epsilon.ToString();
            builder.AddControl(txtEps);

            DoubleValueTextBox txtAlpha = new DoubleValueTextBox();
            txtAlpha.ValueChanged += (sender, e) => alpha = e.NewValue;
            txtAlpha.Text = alpha.ToString();
            builder.AddControl(txtAlpha);

            builder.AddButton("ImproveVertexPositions", delegate(IProgressMonitor pm)
            {
                var functions = new FunctionList();
                functions.Add(Configuration.Default.Surface);
                OptimizeMesh.ImproveVertexPositions(Configuration.Default, _mesh.Vertices, pm, functions);
            });
            builder.AddButton("ImproveVertexPositionsGsl", delegate(IProgressMonitor pm)
            {
                var functions = new FunctionList();
                functions.Add(Configuration.Default.Surface);
                OptimizeMesh.ImproveVertexPositionsGsl(Configuration.Default, _mesh.Vertices, pm, functions);
            });
            builder.AddButton("Optimize", delegate(IProgressMonitor pm)
            {
                var functions = new FunctionList();
                functions.Add(Configuration.Default.Surface);
                //functions.Add(EdgeLengthSquare.Instance);
                functions.Add(AreaSquare.Instance);
                OptimizeMesh.OptimizeImplicit(_mesh, epsilon, alpha, pm, Configuration.Default, functions);
            });

            builder.AddButton("Mesh to points",
                () => _points = _mesh.Vertices.Select(v => v.Point));

            builder.AddButton("Hoppe",
                () => HoppeOptimization.OptimizeMesh(_mesh, _points));

            builder.AddButton("Dynamic", delegate(IProgressMonitor pm)
            {
                DynamicMeshOptimization.Optimize(_mesh,
                    Configuration.Default.Surface, pm);
            });

            builder.AddButton("Edge length square", delegate(IProgressMonitor pm)
            {
                var functions = new FunctionList();
                functions.Add(1E+5, new SquareDecorator(Configuration.Default.Surface));
                functions.Add(EdgeLengthSquare.Instance);
                OptimizeMesh.ImproveVertexPositionsGsl(Configuration.Default, _mesh.Vertices, pm, functions);
            });

            builder.AddButton("Area square", delegate(IProgressMonitor pm)
            {
                var functions = new FunctionList();
                functions.Add(new SquareDecorator(Configuration.Default.Surface));
                functions.Add(1E-5, AreaSquare.Instance);
                OptimizeMesh.ImproveVertexPositions(Configuration.Default, _mesh.Vertices, pm, functions);
            });

            builder.AddButton("Regularity", delegate(IProgressMonitor pm)
            {
                var functions = new FunctionList();
                functions.Add(new SquareDecorator(Configuration.Default.Surface));
                functions.Add(Regularity.Instance);
                OptimizeMesh.ImproveVertexPositions(Configuration.Default, _mesh.Vertices, pm, functions);
            });

            builder.AddButton("Mark nonreg", delegate()
            {
                visualizer.MarkedVerts = from v in _mesh.Vertices
                                         where v.Type == VertexType.Internal
                                         && VertexOps.ExternalCurvature(v) > 0.1
                                         select v;
                Selection = null;
            });

            TextBox txtMeshInfo = new TextBox();
            txtMeshInfo.Multiline = true;
            txtMeshInfo.ReadOnly = true;
            txtMeshInfo.Width = panelActions.Width - 10;
            txtMeshInfo.Height = 75;
            txtMeshInfo.ParentChanged += delegate(object sender, EventArgs e)
            {
                if (_mesh == null)
                {
                    return;
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Verts: " + _mesh.Vertices.Count());
                sb.AppendLine("Faces: " + _mesh.Faces.Count());
                sb.AppendLine("Index: "
                    + _mesh.Vertices.Where(VertexOps.IsInternal).Sum(v => VertexOps.ExternalCurvature(v)));
                sb.AppendLine("Energy: "
                    + _mesh.Faces.Sum(face => new ImplicitApproximations.NumericalIntegration().Evaluate(face)));

                txtMeshInfo.Text = sb.ToString();
            };
            builder.AddControl(txtMeshInfo);
        }

        private MeshPart Selection
        {
            get
            {
                return _selection;
            }
            set
            {
                panelActions.Controls.Clear();
                _selection = value;
                if (_selection == null)
                {
                    panelActions.Controls.AddRange(_meshActions);
                    textBox1.Text = _mesh != null ? BuildDescription(_mesh) : string.Empty;
                    return;
                }
                if (_selection is Vertex)
                {
                    panelActions.Controls.AddRange(_vertActions);
                    textBox1.Text = BuildDescription((Vertex)_selection, Globals.VertexFunctions);
                }
                else if (_selection is Edge)
                {
                    panelActions.Controls.AddRange(_edgeActions);
                    textBox1.Text = BuildDescription((Edge)_selection, Globals.EdgeFunctions);
                }
                else if (_selection is Face)
                {
                    panelActions.Controls.AddRange(_faceActions);
                    textBox1.Text = BuildDescription((Face)_selection, Globals.FaceFunctions);
                }
                panelActions.Controls.Add(btnImprovePositions);
            }
        }

        private string BuildDescription(Mesh arg)
        {
            StringBuilder sb = new StringBuilder();
            AppendStatistics(sb, "Vertices", arg.Vertices, Globals.VertexFunctions);
            AppendStatistics(sb, "Edges", arg.Edges, Globals.EdgeFunctions);
            AppendStatistics(sb, "Faces", arg.Faces, Globals.FaceFunctions);

            return sb.ToString();
        }

        private void AppendStatistics<T>(StringBuilder sb, string name, IEnumerable<T> enumerable,
            IDictionary<string, Func<T, double>> functions)
        {
            sb.AppendLine(name + ":");
            sb.AppendLine();

            AppendValue(sb, "Count", enumerable.Count());
            foreach (var item in functions)
            {
                sb.AppendLine(item.Key + ":");
                sb.AppendLine();
                AppendValue(sb, "Sum", enumerable.SafeSum(item.Value));
                AppendValue(sb, "Mean", enumerable.SafeMean(item.Value));
                AppendValue(sb, "Variance", enumerable.SafeVariance(item.Value));
            }
        }

        private string BuildDescription<T>(T arg,
            IDictionary<string, Func<T, double>> functions)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in functions)
            {
                try
                {
                    AppendValue(sb, item.Key, item.Value(arg));
                }
                catch
                {
                }
            }
            return sb.ToString();
        }

        private void AppendValue(StringBuilder sb, string name, IConvertible value)
        {
            sb.AppendLine(name + ":");
            sb.AppendLine(value.ToString(NumberFormatInfo.InvariantInfo));
            sb.AppendLine();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void visualizer_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Ray ray = visualizer.RayThroughScreen(e.X, e.Y);
                if (btnPoints.Checked)
                {
                    Vertex newSelection = null;
                    double min = 15;
                    double minRho = 0.1;
                    foreach (Vertex v in _mesh.Vertices)
                    {
                        double t = (v.Point.X - ray.origin.X) * ray.direction.x
                            + (v.Point.Y - ray.origin.Y) * ray.direction.y
                            + (v.Point.Z - ray.origin.Z) * ray.direction.z;
                        if (t < 0)
                        {
                            t = 0;
                        }
                        double rho = Math.Sqrt((v.Point - (ray.origin + t * ray.direction)).Norm);
                        if (rho < minRho && t < min)
                        {
                            newSelection = v;
                            min = t;
                        }
                    }

                    Selection = newSelection;
                    if (newSelection != null)
                    {
                        visualizer.MarkedVerts = new Vertex[] { newSelection };
                    }
                    else
                    {
                        visualizer.MarkedVerts = null;
                    }
                }
                if (btnEdges.Checked || btnFaces.Checked)
                {
                    Face faceSelection = null;
                    double mint = 45;
                    foreach (Face f in _mesh.Faces)
                    {
                        double t = ray.Trace(f);
                        if (t > 0 && t < mint)
                        {
                            faceSelection = f;
                            mint = t;
                        }
                    }
                    if (btnFaces.Checked)
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

        private void btnRestartCam_Click(object sender, System.EventArgs e)
        {
            visualizer.Translate = new Point(0, 0, 0);
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

        #region Draw points menu handlers

        private void drawWithNormalsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doNotDrawToolStripMenuItem.Checked = drawToolStripMenuItem.Checked = false;
            drawWithNormalsToolStripMenuItem.Checked = true;
            visualizer.DrawPoints = DrawPoints.DrawWithNormals;
        }

        private void drawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doNotDrawToolStripMenuItem.Checked = drawWithNormalsToolStripMenuItem.Checked = false;
            drawToolStripMenuItem.Checked = true;
            visualizer.DrawPoints = DrawPoints.Draw;
        }

        private void doNotDrawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawToolStripMenuItem.Checked = drawWithNormalsToolStripMenuItem.Checked = false;
            doNotDrawToolStripMenuItem.Checked = true;
            visualizer.DrawPoints = DrawPoints.DoNotDraw;
        }

        #endregion

        private void drawFaceNormalsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawFaceNormalsToolStripMenuItem.Checked = visualizer.DrawFaceNormals = !visualizer.DrawFaceNormals;
        }

        private void algorithmExecutionWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var algorithm = (Action<IProgressMonitor>)e.Argument;
            algorithm(new BackgroundWorkerProgressMonitor(algorithmExecutionWorker));
        }

        private void algorithmExecutionWorker_ProgressChanged(object sender,
            ProgressChangedEventArgs e)
        {
            algorithmExecutionProgress.Value = e.ProgressPercentage;
        }

        private void algorithmExecutionWorker_RunWorkerCompleted(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            visualizer.Invalidate();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            algorithmExecutionWorker.CancelAsync();
        }

        private void tikZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\begin{tikzpicture}");
            foreach (Edge edge in _mesh.Edges)
            {
                sb.AppendLine(string.Format(NumberFormatInfo.InvariantInfo,
                    @"\draw {4} ({0:F5},{1:F5}) -- ({2:F5},{3:F5});",
                    edge.Begin.Point.X, edge.Begin.Point.Y, edge.End.Point.X, edge.End.Point.Y,
                    (Selection != null && (edge == Selection || edge.Pair == Selection)) ? "[red]" : ""));
            }
            sb.AppendLine(@"\end{tikzpicture}");
            Clipboard.SetDataObject(sb.ToString());
            MessageBox.Show("Copied generated code to clipboard", "Tikz code");
        }

        private class ControlListBuilder : IControlBuilder
        {
            private readonly ICollection<Control> _result = new List<Control>();
            private readonly UI2 _mainWindow;

            public ControlListBuilder(UI2 mainWindow)
            {
                _mainWindow = mainWindow;
            }

            public Control[] Result
            {
                get
                {
                    return _result.ToArray();
                }
            }

            #region IControlBuilder Members

            public void AddButton(string text, Action action)
            {
                Button button = new Button();
                button.Text = text;
                button.Click += (s, e) => action();
                _result.Add(button);
            }

            public void AddButton(string text, Action<IProgressMonitor> action)
            {
                Button button = new Button();
                button.Text = text;
                button.Click += delegate(object sender, EventArgs e)
                {
                    if (_mainWindow.algorithmExecutionWorker.IsBusy)
                    {
                        MessageBox.Show("Worker is busy!");
                        return;
                    }
                    Action<IProgressMonitor> runnedAction = delegate(IProgressMonitor pm)
                    {
                        action(pm);
                        MessageBox.Show("Done");
                    };
                    _mainWindow.algorithmExecutionWorker.RunWorkerAsync(runnedAction);
                };
                _result.Add(button);
            }

            public void AddLabel(string text)
            {
                Label label = new Label();
                label.Text = text;
                _result.Add(label);
            }

            public void AddControl(object control)
            {
                _result.Add((Control)control);
            }

            #endregion
        }

        private void ConsoleButton_CheckedChanged(object sender, EventArgs e)
        {
            if (((ToolStripButton)sender).Checked)
            {
                Win32.AllocConsole();
            }
            else
            {
                Win32.FreeConsole();
            }
        }
    }
}
