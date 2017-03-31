using OglVisualizer;
namespace UI
{
    partial class UI2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem tikZToolStripMenuItem;
            System.Windows.Forms.TabControl tabControl1;
            System.Windows.Forms.ToolStripButton consoleButton;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UI2));
            System.Windows.Forms.TabPage customOptimizationTab;
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panelActions = new System.Windows.Forms.FlowLayoutPanel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.drawPointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.doNotDrawToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawWithNormalsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawFaceNormalsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnPoints = new System.Windows.Forms.ToolStripButton();
            this.btnEdges = new System.Windows.Forms.ToolStripButton();
            this.btnFaces = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.algorithmExecutionProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.cancelButton = new System.Windows.Forms.ToolStripStatusLabel();
            this.algorithmExecutionWorker = new System.ComponentModel.BackgroundWorker();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.visualizer = new OglVisualizer.Visualizer();
            toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            tikZToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            tabControl1 = new System.Windows.Forms.TabControl();
            consoleButton = new System.Windows.Forms.ToolStripButton();
            customOptimizationTab = new System.Windows.Forms.TabPage();
            tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            tikZToolStripMenuItem});
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // tikZToolStripMenuItem
            // 
            tikZToolStripMenuItem.Name = "tikZToolStripMenuItem";
            tikZToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            tikZToolStripMenuItem.Text = "TikZ";
            tikZToolStripMenuItem.Click += new System.EventHandler(this.tikZToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(this.tabPage1);
            tabControl1.Controls.Add(this.tabPage2);
            tabControl1.Controls.Add(customOptimizationTab);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Right;
            tabControl1.Location = new System.Drawing.Point(437, 63);
            tabControl1.Multiline = true;
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(203, 370);
            tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panelActions);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(195, 344);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panelActions
            // 
            this.panelActions.AutoScroll = true;
            this.panelActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelActions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.panelActions.Location = new System.Drawing.Point(3, 3);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(189, 338);
            this.panelActions.TabIndex = 5;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.propertyGrid1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(195, 344);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(3, 3);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.SelectedObject = this.menuStrip1;
            this.propertyGrid1.Size = new System.Drawing.Size(189, 338);
            this.propertyGrid1.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolStripMenuItem1,
            toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip1.Size = new System.Drawing.Size(640, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.drawPointsToolStripMenuItem,
            this.drawFaceNormalsToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(44, 20);
            this.toolStripMenuItem1.Text = "View";
            // 
            // drawPointsToolStripMenuItem
            // 
            this.drawPointsToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.drawPointsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.doNotDrawToolStripMenuItem,
            this.drawToolStripMenuItem,
            this.drawWithNormalsToolStripMenuItem});
            this.drawPointsToolStripMenuItem.Name = "drawPointsToolStripMenuItem";
            this.drawPointsToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.drawPointsToolStripMenuItem.Text = "Draw points";
            // 
            // doNotDrawToolStripMenuItem
            // 
            this.doNotDrawToolStripMenuItem.Name = "doNotDrawToolStripMenuItem";
            this.doNotDrawToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.doNotDrawToolStripMenuItem.Text = "Do not draw";
            this.doNotDrawToolStripMenuItem.Click += new System.EventHandler(this.doNotDrawToolStripMenuItem_Click);
            // 
            // drawToolStripMenuItem
            // 
            this.drawToolStripMenuItem.Name = "drawToolStripMenuItem";
            this.drawToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.drawToolStripMenuItem.Text = "Draw";
            this.drawToolStripMenuItem.Click += new System.EventHandler(this.drawToolStripMenuItem_Click);
            // 
            // drawWithNormalsToolStripMenuItem
            // 
            this.drawWithNormalsToolStripMenuItem.Name = "drawWithNormalsToolStripMenuItem";
            this.drawWithNormalsToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.drawWithNormalsToolStripMenuItem.Text = "Draw with normals";
            this.drawWithNormalsToolStripMenuItem.Click += new System.EventHandler(this.drawWithNormalsToolStripMenuItem_Click);
            // 
            // drawFaceNormalsToolStripMenuItem
            // 
            this.drawFaceNormalsToolStripMenuItem.Name = "drawFaceNormalsToolStripMenuItem";
            this.drawFaceNormalsToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.drawFaceNormalsToolStripMenuItem.Text = "Draw face normals";
            this.drawFaceNormalsToolStripMenuItem.Click += new System.EventHandler(this.drawFaceNormalsToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnPoints,
            this.btnEdges,
            this.btnFaces,
            this.toolStripSeparator1,
            consoleButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(640, 39);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnPoints
            // 
            this.btnPoints.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPoints.Image = ((System.Drawing.Image)(resources.GetObject("btnPoints.Image")));
            this.btnPoints.ImageTransparentColor = System.Drawing.Color.White;
            this.btnPoints.Name = "btnPoints";
            this.btnPoints.Size = new System.Drawing.Size(36, 36);
            this.btnPoints.Text = "Select vertices";
            this.btnPoints.Click += new System.EventHandler(this.btnPoints_Click);
            // 
            // btnEdges
            // 
            this.btnEdges.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEdges.Image = ((System.Drawing.Image)(resources.GetObject("btnEdges.Image")));
            this.btnEdges.ImageTransparentColor = System.Drawing.Color.White;
            this.btnEdges.Name = "btnEdges";
            this.btnEdges.Size = new System.Drawing.Size(36, 36);
            this.btnEdges.Text = "Select edges";
            this.btnEdges.Click += new System.EventHandler(this.btnEdges_Click);
            // 
            // btnFaces
            // 
            this.btnFaces.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFaces.Image = ((System.Drawing.Image)(resources.GetObject("btnFaces.Image")));
            this.btnFaces.ImageTransparentColor = System.Drawing.Color.White;
            this.btnFaces.Name = "btnFaces";
            this.btnFaces.Size = new System.Drawing.Size(36, 36);
            this.btnFaces.Text = "Select faces";
            this.btnFaces.Click += new System.EventHandler(this.btnFaces_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.algorithmExecutionProgress,
            this.cancelButton});
            this.statusStrip1.Location = new System.Drawing.Point(0, 433);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.statusStrip1.Size = new System.Drawing.Size(640, 24);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // algorithmExecutionProgress
            // 
            this.algorithmExecutionProgress.Name = "algorithmExecutionProgress";
            this.algorithmExecutionProgress.Size = new System.Drawing.Size(100, 18);
            this.algorithmExecutionProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // cancelButton
            // 
            this.cancelButton.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(47, 19);
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // algorithmExecutionWorker
            // 
            this.algorithmExecutionWorker.WorkerReportsProgress = true;
            this.algorithmExecutionWorker.WorkerSupportsCancellation = true;
            this.algorithmExecutionWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.algorithmExecutionWorker_DoWork);
            this.algorithmExecutionWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.algorithmExecutionWorker_RunWorkerCompleted);
            this.algorithmExecutionWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.algorithmExecutionWorker_ProgressChanged);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox1.Location = new System.Drawing.Point(0, 67);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(206, 361);
            this.textBox1.TabIndex = 7;
            // 
            // visualizer
            // 
            this.visualizer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.visualizer.BackColor = System.Drawing.Color.White;
            this.visualizer.DrawFaceNormals = false;
            this.visualizer.DrawPoints = OglVisualizer.DrawPoints.DoNotDraw;
            this.visualizer.FaceColorEvaluator = null;
            this.visualizer.Location = new System.Drawing.Point(212, 63);
            this.visualizer.Name = "visualizer";
            this.visualizer.Phi = 0;
            this.visualizer.Size = new System.Drawing.Size(219, 372);
            this.visualizer.TabIndex = 3;
            this.visualizer.Theta = 0;
            this.visualizer.VertexColorEvaluator = null;
            this.visualizer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.visualizer_MouseDown);
            // 
            // customOptimizationTab
            // 
            customOptimizationTab.Location = new System.Drawing.Point(4, 22);
            customOptimizationTab.Name = "customOptimizationTab";
            customOptimizationTab.Size = new System.Drawing.Size(195, 344);
            customOptimizationTab.TabIndex = 2;
            customOptimizationTab.Text = "Custom";
            customOptimizationTab.UseVisualStyleBackColor = true;
            // 
            // UI2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 457);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(tabControl1);
            this.Controls.Add(this.visualizer);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "UI2";
            this.Text = "UI2";
            tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnPoints;
        private System.Windows.Forms.ToolStripButton btnEdges;
        private System.Windows.Forms.ToolStripButton btnFaces;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private Visualizer visualizer;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.FlowLayoutPanel panelActions;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem drawPointsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem doNotDrawToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem drawToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem drawWithNormalsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem drawFaceNormalsToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker algorithmExecutionWorker;
        private System.Windows.Forms.ToolStripProgressBar algorithmExecutionProgress;
        private System.Windows.Forms.ToolStripStatusLabel cancelButton;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.TextBox textBox1;
    }
}