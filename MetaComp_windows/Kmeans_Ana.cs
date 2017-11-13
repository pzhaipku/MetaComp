using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace MetaComp
{
    public partial class Kmeans_Ana : Form
    {
        public Kmeans_Ana()
        {
            InitializeComponent();
        }

        //public object table;

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            app.clusterNum = int.Parse(this.textBox1.Text);
            int FeatureNum = app.CountMatrix.GetLength(0);
            int SampleNum = app.CountMatrix.GetLength(1);
            int ClusterNum = int.Parse(this.textBox1.Text);
            double[,] resultP = app.GetProcess(app.CountMatrix, app.clusterNum);
            app.Center = new double[ClusterNum, FeatureNum - 1];
            app.ClusterResult = new int[ClusterNum, SampleNum];
            for (int i = 0; i < ClusterNum; i++)
            {
                for (int j = 0; j < SampleNum; j++)
                    app.ClusterResult[i, j] = (int)resultP[i, j];
                for (int j = 0; j < FeatureNum - 1; j++)
                    app.Center[i, j] = resultP[i, SampleNum + j];
            }
            this.Hide();
            if ( FeatureNum <= 200 )
            {
                Cluster_Center_Output f3 = new Cluster_Center_Output();
                f3.MdiParent = this.MdiParent;
                f3.Show();
            }
            if (SampleNum <= 200)
            {
                HCluster_Output f5 = new HCluster_Output();
                f5.MdiParent = this.MdiParent;
                f5.Show();
            }
            this.Close();             
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form4_Load(object sender, EventArgs e)
        {
           
        }

    }
}
