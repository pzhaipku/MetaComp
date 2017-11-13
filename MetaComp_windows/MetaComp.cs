using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;

namespace MetaComp
{
    public partial class MetaComp : Form
    {

        public MetaComp()
        {
            InitializeComponent();
        }

        private void loadFlieToolStripMenuItem_Click_1(object sender, EventArgs e)
        {        
            app.Profile = null;
            Data_Convert DataConvert = new Data_Convert();
            DataConvert.MdiParent = this;
            DataConvert.Show();
        }

        private void savePlotToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void kmeansClusterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            Kmeans_Ana kmeans = new Kmeans_Ana();
            kmeans.MdiParent = this;
            kmeans.Show();
        }

        private void hierarchicalClusterHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HClustering_Ana hcluster = new HClustering_Ana();
            hcluster.MdiParent = this;
            hcluster.Show();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
           
            PCA_Ana PCA = new PCA_Ana();
            PCA.MdiParent = this;
            PCA.Show();
        }

        private void mMultipleSamplesStatsticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Two_Sample_Ana TwoSamplesTest = new Two_Sample_Ana();
            TwoSamplesTest.MdiParent = this;
            TwoSamplesTest.Show();
        }

        private void mMultipleSamplesStaticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Multiple_Samples_Ana MultipleSamplesTest = new Multiple_Samples_Ana();
            MultipleSamplesTest.MdiParent = this;
            MultipleSamplesTest.Show();
        }

        private void gTwoGroupsOfSamplesStaticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Two_Groups_Ana TwoGroupsTest = new Two_Groups_Ana();
            TwoGroupsTest.MdiParent = this;
            TwoGroupsTest.Show();
        }

        private void eEnvironmentalFactorsAnalysisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment_Ana EnvironmentalFactor = new Environment_Ana();
            EnvironmentalFactor.MdiParent = this;
            EnvironmentalFactor.Show();        
        }

        private void aToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About aboutbox = new About();
            aboutbox.MdiParent = this;
            aboutbox.Show();
        }

        private void MDIParent1_Load(object sender, EventArgs e)
        {
            
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void menuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
