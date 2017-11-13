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
    public partial class HClustering_Ana : Form
    {
        public HClustering_Ana()
        {
            InitializeComponent();
        }

        private void Form11_Load(object sender, EventArgs e)
        {
            this.comboBox1.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] FeatureName = (string[])app.FeaName;
            string[] SampleName = (string[])app.SamName;

            int FeatureNum = FeatureName.GetLength(0);
            int SampleNum = SampleName.GetLength(0);

            REngine.SetEnvironmentVariables();

            REngine CLU = REngine.GetInstance();

            CLU.Initialize();

            NumericMatrix Freq = CLU.CreateNumericMatrix(app.FreqMatrix);
            CLU.SetSymbol("Freq", Freq);
            NumericMatrix Count = CLU.CreateNumericMatrix(app.CountMatrix);
            CLU.SetSymbol("Count", Count);
            NumericVector RFeatureNum = CLU.CreateNumeric(FeatureNum);
            NumericVector RSampleNum = CLU.CreateNumeric(SampleNum);
            CLU.SetSymbol("FeatureNum", RFeatureNum);
            CLU.SetSymbol("SampleNum", RSampleNum);
            CharacterVector RSampleName = CLU.CreateCharacterVector(app.SamName);
            CharacterVector RFeatureName = CLU.CreateCharacterVector(app.FeaName);
            CLU.SetSymbol("FeatureName", RFeatureName);
            CLU.SetSymbol("SampleName", RSampleName);
           
            CLU.Evaluate("CountMatrix <- as.data.frame(Count)");
            CLU.Evaluate("names(CountMatrix) <- SampleName");
            switch (this.comboBox1.SelectedIndex)
            {
                case 0:
                    CLU.Evaluate("d <- dist(t(CountMatrix),method = \"euclidean\")");
                    break;
                case 1:
                    CLU.Evaluate("d <- dist(t(CountMatrix),method = \"maximum\")");
                    break;
                case 2:
                    CLU.Evaluate("d <- dist(t(CountMatrix),method = \"manhattan\")");
                    break;
                case 3:
                    CLU.Evaluate("d <- dist(t(CountMatrix),method = \"canberra\")");
                    break;
                case 4:
                    CLU.Evaluate("d <- dist(t(CountMatrix),method = \"binary\")");
                    break;
                case 5:
                    CLU.Evaluate("d <- dist(t(CountMatrix),method = \"minkowski\")");
                    break;
                default:
                    break;
            }
            switch (this.comboBox2.SelectedIndex)
            {
                case 0:
                    CLU.Evaluate("hc <- hclust(d,method = \"ward\")");
                    break;
                case 1:
                    CLU.Evaluate("hc <- hclust(d,method = \"single\")");
                    break;
                case 2:
                    CLU.Evaluate("hc <- hclust(d,method = \"complete\")");
                    break;
                case 3:
                    CLU.Evaluate("hc <- hclust(d,method = \"average\")");
                    break;
                case 4:
                    CLU.Evaluate("hc <- hclust(d,method = \"mcquitty\")");
                    break;
                case 5:
                    CLU.Evaluate("hc <- hclust(d,method = \"median\")");
                    break;
                case 6:
                    CLU.Evaluate("hc <- hclust(d,method = \"centroid\")");
                    break;
                default:
                    break;
            }

            CLU.Evaluate("plot(hc)");

            this.Close();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
