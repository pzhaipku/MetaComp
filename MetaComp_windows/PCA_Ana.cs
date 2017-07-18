using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RDotNet;

namespace MetaComp
{
    public partial class PCA_Ana : Form
    {
        public PCA_Ana()
        {
            InitializeComponent();
        }
        
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form6_Load(object sender, EventArgs e)
        {
            this.radioButton2.Checked = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {


            int FeatureNum = app.FeaName.GetLength(0);
            int SampleNum = app.SamName.GetLength(0);

            REngine.SetEnvironmentVariables();
            
            REngine PCA = REngine.GetInstance();
            
            PCA.Initialize();

            NumericMatrix Freq = PCA.CreateNumericMatrix(app.FreqMatrix);
            PCA.SetSymbol("Freq", Freq);
            CharacterVector SampleName = PCA.CreateCharacterVector(app.SamName);
            CharacterVector FeatureName = PCA.CreateCharacterVector(app.FeaName);
            PCA.SetSymbol("FeatureName", FeatureName);
            PCA.SetSymbol("SampleName", SampleName);

            PCA.Evaluate("library(stats)");
            PCA.Evaluate("pr <- prcomp(t(Freq),cor = TRUE)");
            PCA.Evaluate("score <- predict(pr)");
            double[,] Count = PCA.GetSymbol("score").AsNumericMatrix().ToArray();
            app.Score = new double[SampleNum, 2];
            for (int i = 0; i < SampleNum; i++)
            {
                app.Score[i, 0] = Count[i, 0];
                app.Score[i, 1] = Count[i, 1];
            }
            if (this.radioButton1.Checked)
            {
                PCA.Evaluate("windows()");
                PCA.Evaluate("plot(score[,1:2],main=\"PCA\", type=\"p\")");
                PCA.Evaluate("text(score[,1],score[,2],labels = SampleName[],pos =4)");
            }

            else
            {
                if ((app.cluster == null) || (app.cluster.Length != SampleNum))
                {
                    MessageBox.Show("Sample number in input data is not equal to that in cluster information!!", "Warning!!!", MessageBoxButtons.OK);
                }
                else
                {
                    IntegerVector cluster = PCA.CreateIntegerVector(app.cluster);
                    PCA.SetSymbol("cluster", cluster);
                    PCA.Evaluate("clusterNum <- max(cluster)");
                    PCA.Evaluate("clustermin <- min(cluster)");
                    app.clusterNum = (int)PCA.GetSymbol("clusterNum").AsNumeric().First();
                    int clustermin = (int)PCA.GetSymbol("clustermin").AsNumeric().First();
                    if (app.clusterNum > 10)
                        MessageBox.Show("Too many clusters!!", "WARNING!");
                    else if (clustermin < 0)
                        MessageBox.Show("Illegal cluster number!!!", "WARNING!");
                    else
                    {
                        PCA_whole_Output plot = new PCA_whole_Output();
                        plot.MdiParent = this.MdiParent;
                        plot.Show();

                    }
                }

            }
            this.Close();
              
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            
            if (this.radioButton1.Checked)
                this.button1.Enabled = false;
            else
                this.button1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            REngine.SetEnvironmentVariables();

            REngine cluster = REngine.GetInstance();

            cluster.Initialize();
            //try
            //{
                cluster.Evaluate("cluster = read.table(file.choose())");
                cluster.Evaluate("cluster <- as.matrix(cluster)");
                IntegerMatrix clusterinfo = cluster.GetSymbol("cluster").AsIntegerMatrix();


                app.cluster = new int[clusterinfo.AsNumericMatrix().ToArray().GetLength(1)];
                for (int i = 0; i < app.cluster.Length; i++)
                {
                    app.cluster[i] = clusterinfo[0, i];
                }
                
                //foreach (var item in Enumerable.Range(0, clusterinfo.AsNumericMatrix().ToArray().GetLength(0) * clusterinfo.AsNumericMatrix().ToArray().GetLength(1)).Select(i => new { x = i / clusterinfo.AsNumericMatrix().ToArray().GetLength(1), y = i % clusterinfo.AsNumericMatrix().ToArray().GetLength(1) }))
                //{
                //    app.cluster[item.x, item.y] = (int)clusterinfo.AsNumericMatrix().ToArray()[item.x, item.y];
                //}
            //}

            //try
            //{
            //    StatConnector factor = new STATCONNECTORSRVLib.StatConnectorClass();
            //    factor.Init("R");
            //    factor.EvaluateNoReturn("cluster = read.table(file.choose())");
            //    factor.Evaluate("cluster <- as.matrix(cluster)");
            //    object temp = factor.GetSymbol("cluster");
            //    app.cluster = (int[,])temp;
            //}
            //catch { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
