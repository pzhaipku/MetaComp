namespace MetaComp
{
    partial class MetaComp
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetaComp));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFlieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.anaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.kmeansClusterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hierarchicalClusterHToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mMultipleSamplesStatsticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMultipleSamplesStaticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gTwoGroupsOfSamplesStaticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eEnvironmentalFactorsAnalysisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.anaToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(632, 25);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadFlieToolStripMenuItem,
            this.exitEToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(53, 21);
            this.fileToolStripMenuItem.Text = "File(&F)";
            this.fileToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
            // 
            // loadFlieToolStripMenuItem
            // 
            this.loadFlieToolStripMenuItem.Name = "loadFlieToolStripMenuItem";
            this.loadFlieToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.loadFlieToolStripMenuItem.Text = "Load Data...(&L)";
            this.loadFlieToolStripMenuItem.Click += new System.EventHandler(this.loadFlieToolStripMenuItem_Click_1);
            // 
            // exitEToolStripMenuItem
            // 
            this.exitEToolStripMenuItem.Name = "exitEToolStripMenuItem";
            this.exitEToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.exitEToolStripMenuItem.Text = "Exit(&X)";
            this.exitEToolStripMenuItem.Click += new System.EventHandler(this.exitEToolStripMenuItem_Click);
            // 
            // anaToolStripMenuItem
            // 
            this.anaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem1,
            this.mMultipleSamplesStatsticsToolStripMenuItem,
            this.mMultipleSamplesStaticsToolStripMenuItem,
            this.gTwoGroupsOfSamplesStaticsToolStripMenuItem,
            this.eEnvironmentalFactorsAnalysisToolStripMenuItem});
            this.anaToolStripMenuItem.Name = "anaToolStripMenuItem";
            this.anaToolStripMenuItem.Size = new System.Drawing.Size(82, 21);
            this.anaToolStripMenuItem.Text = "Analysis(&A)";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.kmeansClusterToolStripMenuItem,
            this.hierarchicalClusterHToolStripMenuItem});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(296, 22);
            this.toolStripMenuItem2.Text = "Clustering Analysis(&C)";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // kmeansClusterToolStripMenuItem
            // 
            this.kmeansClusterToolStripMenuItem.Name = "kmeansClusterToolStripMenuItem";
            this.kmeansClusterToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.kmeansClusterToolStripMenuItem.Text = "K-means Clustering...(&K)";
            this.kmeansClusterToolStripMenuItem.Click += new System.EventHandler(this.kmeansClusterToolStripMenuItem_Click);
            // 
            // hierarchicalClusterHToolStripMenuItem
            // 
            this.hierarchicalClusterHToolStripMenuItem.Name = "hierarchicalClusterHToolStripMenuItem";
            this.hierarchicalClusterHToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.hierarchicalClusterHToolStripMenuItem.Text = "Hierarchical Clustering...(&H)";
            this.hierarchicalClusterHToolStripMenuItem.Click += new System.EventHandler(this.hierarchicalClusterHToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(296, 22);
            this.toolStripMenuItem1.Text = "Principal Component Analysis...(&P)";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // mMultipleSamplesStatsticsToolStripMenuItem
            // 
            this.mMultipleSamplesStatsticsToolStripMenuItem.Name = "mMultipleSamplesStatsticsToolStripMenuItem";
            this.mMultipleSamplesStatsticsToolStripMenuItem.Size = new System.Drawing.Size(296, 22);
            this.mMultipleSamplesStatsticsToolStripMenuItem.Text = "Two Samples Statistics...(&T)";
            this.mMultipleSamplesStatsticsToolStripMenuItem.Click += new System.EventHandler(this.mMultipleSamplesStatsticsToolStripMenuItem_Click);
            // 
            // mMultipleSamplesStaticsToolStripMenuItem
            // 
            this.mMultipleSamplesStaticsToolStripMenuItem.Name = "mMultipleSamplesStaticsToolStripMenuItem";
            this.mMultipleSamplesStaticsToolStripMenuItem.Size = new System.Drawing.Size(296, 22);
            this.mMultipleSamplesStaticsToolStripMenuItem.Text = "Multiple Samples Statistics...(&M)";
            this.mMultipleSamplesStaticsToolStripMenuItem.Click += new System.EventHandler(this.mMultipleSamplesStaticsToolStripMenuItem_Click);
            // 
            // gTwoGroupsOfSamplesStaticsToolStripMenuItem
            // 
            this.gTwoGroupsOfSamplesStaticsToolStripMenuItem.Name = "gTwoGroupsOfSamplesStaticsToolStripMenuItem";
            this.gTwoGroupsOfSamplesStaticsToolStripMenuItem.Size = new System.Drawing.Size(296, 22);
            this.gTwoGroupsOfSamplesStaticsToolStripMenuItem.Text = "Two Groups of Samples Statistics...(&G)";
            this.gTwoGroupsOfSamplesStaticsToolStripMenuItem.Click += new System.EventHandler(this.gTwoGroupsOfSamplesStaticsToolStripMenuItem_Click);
            // 
            // eEnvironmentalFactorsAnalysisToolStripMenuItem
            // 
            this.eEnvironmentalFactorsAnalysisToolStripMenuItem.Name = "eEnvironmentalFactorsAnalysisToolStripMenuItem";
            this.eEnvironmentalFactorsAnalysisToolStripMenuItem.Size = new System.Drawing.Size(296, 22);
            this.eEnvironmentalFactorsAnalysisToolStripMenuItem.Text = "Environmental Factor Analysis...(&E)";
            this.eEnvironmentalFactorsAnalysisToolStripMenuItem.Click += new System.EventHandler(this.eEnvironmentalFactorsAnalysisToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(64, 21);
            this.helpToolStripMenuItem.Text = "Help(&H)";
            // 
            // aToolStripMenuItem
            // 
            this.aToolStripMenuItem.Name = "aToolStripMenuItem";
            this.aToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.aToolStripMenuItem.Text = "About...(&A)";
            this.aToolStripMenuItem.Click += new System.EventHandler(this.aToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // MDIParent1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(632, 418);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MDIParent1";
            this.Text = "MetaComp";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MDIParent1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFlieToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem anaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem kmeansClusterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hierarchicalClusterHToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mMultipleSamplesStatsticsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMultipleSamplesStaticsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gTwoGroupsOfSamplesStaticsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eEnvironmentalFactorsAnalysisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;

    }
}



