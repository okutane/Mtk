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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UI2));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnPoints = new System.Windows.Forms.ToolStripButton();
            this.btnEdges = new System.Windows.Forms.ToolStripButton();
            this.btnFaces = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.drawPointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.doNotDrawToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawWithNormalsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawFaceNormalsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelActions = new System.Windows.Forms.FlowLayoutPanel();
            this.algorithmExecutionWorker = new System.ComponentModel.BackgroundWorker();
            this.algorithmExecutionProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.visualizer = new OglVisualizer.Visualizer();
            this.cancelButton = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnPoints,
            this.btnEdges,
            this.btnFaces,
            this.toolStripSeparator1});
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
            this.statusStrip1.Location = new System.Drawing.Point(0, 435);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.statusStrip1.Size = new System.Drawing.Size(640, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolStripMenuItem1});
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
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
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
            this.toolStripMenuItem1.Size = new System.Drawing.Size(41, 20);
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
            this.drawPointsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.drawPointsToolStripMenuItem.Text = "Draw points";
            // 
            // doNotDrawToolStripMenuItem
            // 
            this.doNotDrawToolStripMenuItem.Name = "doNotDrawToolStripMenuItem";
            this.doNotDrawToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.doNotDrawToolStripMenuItem.Text = "Do not draw";
            this.doNotDrawToolStripMenuItem.Click += new System.EventHandler(this.doNotDrawToolStripMenuItem_Click);
            // 
            // drawToolStripMenuItem
            // 
            this.drawToolStripMenuItem.Name = "drawToolStripMenuItem";
            this.drawToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.drawToolStripMenuItem.Text = "Draw";
            this.drawToolStripMenuItem.Click += new System.EventHandler(this.drawToolStripMenuItem_Click);
            // 
            // drawWithNormalsToolStripMenuItem
            // 
            this.drawWithNormalsToolStripMenuItem.Name = "drawWithNormalsToolStripMenuItem";
            this.drawWithNormalsToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.drawWithNormalsToolStripMenuItem.Text = "Draw with normals";
            this.drawWithNormalsToolStripMenuItem.Click += new System.EventHandler(this.drawWithNormalsToolStripMenuItem_Click);
            // 
            // drawFaceNormalsToolStripMenuItem
            // 
            this.drawFaceNormalsToolStripMenuItem.Name = "drawFaceNormalsToolStripMenuItem";
            this.drawFaceNormalsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.drawFaceNormalsToolStripMenuItem.Text = "Draw face normals";
            this.drawFaceNormalsToolStripMenuItem.Click += new System.EventHandler(this.drawFaceNormalsToolStripMenuItem_Click);
            // 
            // panelActions
            // 
            this.panelActions.AutoScroll = true;
            this.panelActions.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelActions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.panelActions.Location = new System.Drawing.Point(466, 63);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(174, 372);
            this.panelActions.TabIndex = 5;
            // 
            // algorithmExecutionWorker
            // 
            this.algorithmExecutionWorker.WorkerReportsProgress = true;
            this.algorithmExecutionWorker.WorkerSupportsCancellation = true;
            this.algorithmExecutionWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.algorithmExecutionWorker_DoWork);
            this.algorithmExecutionWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.algorithmExecutionWorker_RunWorkerCompleted);
            this.algorithmExecutionWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.algorithmExecutionWorker_ProgressChanged);
            // 
            // algorithmExecutionProgress
            // 
            this.algorithmExecutionProgress.Name = "algorithmExecutionProgress";
            this.algorithmExecutionProgress.Size = new System.Drawing.Size(100, 16);
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
            this.visualizer.Location = new System.Drawing.Point(0, 63);
            this.visualizer.Name = "visualizer";
            this.visualizer.Phi = 0;
            this.visualizer.Size = new System.Drawing.Size(460, 372);
            this.visualizer.TabIndex = 3;
            this.visualizer.Theta = 0;
            this.visualizer.VertexColorEvaluator = null;
            this.visualizer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.visualizer_MouseMove);
            this.visualizer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.visualizer_MouseDown);
            this.visualizer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.visualizer_MouseUp);
            // 
            // cancelButton
            // 
            this.cancelButton.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(43, 17);
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // UI2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 457);
            this.Controls.Add(this.panelActions);
            this.Controls.Add(this.visualizer);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "UI2";
            this.Text = "UI2";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
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
        private OglVisualizer.Visualizer visualizer;
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
    }
}