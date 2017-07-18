using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RDotNet;
using System.Data.OleDb;
using System.IO;

namespace MetaComp
{
    public partial class Multiple_Samples_Ana : Form
    {
        public Multiple_Samples_Ana()
        {
            InitializeComponent();
        }

        private void Form8_Load(object sender, EventArgs e)
        {
            this.comboBox1.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 0;
            this.radioButton1.Enabled = false;
            this.radioButton2.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int FeatureNum = app.FeaName.GetLength(0);
            int SampleNum = app.SamName.GetLength(0);
            List<double> prob = new List<double>();
            List<double> stat = new List<double>();
            List<double> pvalue = new List<double>();
            double[] bonferroni = new double[FeatureNum];
            double[] fdr = new double[FeatureNum];
            //int length;
            int NAnum = 0;
            
            REngine.SetEnvironmentVariables();
            
            REngine MSS = REngine.GetInstance();
            
            MSS.Initialize();
            
            NumericMatrix Freq = MSS.CreateNumericMatrix(app.FreqMatrix);
            MSS.SetSymbol("Freq", Freq);
            NumericMatrix Count = MSS.CreateNumericMatrix(app.CountMatrix);
            MSS.SetSymbol("Count", Count);
            CharacterVector SampleName = MSS.CreateCharacterVector(app.SamName);
            CharacterVector FeatureName = MSS.CreateCharacterVector(app.FeaName);
            MSS.SetSymbol("FeatureName", FeatureName);
            MSS.SetSymbol("SampleName", SampleName);
            
            List<string> SampleNameFreq = new List<string>();

            for (int i = 0; i < SampleNum; i++)
            {
                SampleNameFreq.Add(SampleName[i] + "Freq");
            }
            
            IntegerVector RFeaNum = MSS.CreateInteger(FeatureNum);
            NumericVector Ralpha = MSS.CreateNumeric(double.Parse(this.textBox1.Text.ToString()));
            
            
            MSS.SetSymbol("n", RFeaNum);
            MSS.SetSymbol("alpha", Ralpha);
            

            List<List<double>> Freqtemp = new List<List<double>>();
            List<double> FreqSum = new List<double>();


            MSS.Evaluate("leastnum = ceiling ( qnorm(alpha/2)^2 )");
            double leastnum = MSS.Evaluate("leastnum").AsNumeric().First();

            if (this.comboBox1.SelectedIndex == 0)
            {
                for (int i = 0; i < FeatureNum; i++)
                {
                    pvalue.Add(100);
                    for (int j = 0; j < SampleNum - 1; j++)
                    {
                        for (int k = j + 1; k < SampleNum; k++)
                        {

                            double probtemp = (app.CountMatrix[i,j] + app.CountMatrix[i,k]) / (app.SampleTotal[j] + app.SampleTotal[k]);


                            double stattemp = (app.FreqMatrix[i,j] - app.FreqMatrix[i,k]) / Math.Sqrt(probtemp * (1 - probtemp) * (1 / app.SampleTotal[j] + 1 / app.SampleTotal[k]));
                            if (double.IsNaN(stattemp))
                                stattemp = 0;


                            NumericVector Rstat = MSS.CreateNumeric(stattemp);
                            MSS.SetSymbol("stat", Rstat);
                            MSS.Evaluate("p.value <- 2*(pnorm(-abs(stat)))");
                            double pvaluetemp;
                            if ((this.comboBox2.SelectedIndex == 1) && (app.CountMatrix[i, j] < leastnum) && (app.CountMatrix[i, k] < leastnum))
                                pvaluetemp = 100;
                            else
                                pvaluetemp = MSS.GetSymbol("p.value").AsNumeric().First();

                           
                            if (pvaluetemp != 100)
                                pvalue[i] = Math.Min((double)pvalue[i], (double)pvaluetemp);

                        }
                    }
                    
                }
                NumericVector Rpvalue = MSS.CreateNumericVector(pvalue);
                MSS.SetSymbol("p.value", Rpvalue);
                MSS.Evaluate("NAnum = length(p.value[p.value == 100])");
                NAnum = Convert.ToInt32(MSS.GetSymbol("NAnum").AsNumeric().First());
                MSS.Evaluate("p.value[p.value == 100] = NA");
                MSS.Evaluate("bonferroni.p <- p.adjust(p.value,\"bonferroni\")");
                MSS.Evaluate("bonferroni.p[which(bonferroni.p == NA)] = 100");
                MSS.Evaluate("fdr.p <- p.adjust(p.value,\"fdr\")");
                MSS.Evaluate("fdr.p[which(fdr.p == NA)] = 100");
                for (int i = 0; i < FeatureNum; i++)
                {
                    bonferroni[i] = MSS.GetSymbol("bonferroni.p").AsNumeric()[i];
                    fdr[i] = MSS.GetSymbol("fdr.p").AsNumeric()[i];
                }
            }
            else if (this.comboBox1.SelectedIndex == 1)
            {
                for (int i = 0; i < FeatureNum; i++)
                {
                    pvalue.Add(100);
                }
                for (int j = 0; j < SampleNum - 1; j++)
                { 
                    for (int k = 1; k < SampleNum; k++)
                    {
                        double Sum1 = 0;
                        double Sum2 = 0;
                        for (int i = 0; i < FeatureNum; i++)
                        {
                            Sum1 = Sum1 + app.CountMatrix[i,j];
                            Sum2 = Sum2 + app.CountMatrix[i,k];
                        }
                        for (int i = 0; i < FeatureNum; i++)
                        {
                            NumericVector n11 = MSS.CreateNumeric(app.CountMatrix[i, j]);
                            NumericVector n21 = MSS.CreateNumeric(app.CountMatrix[i, k]);
                            NumericVector n12 = MSS.CreateNumeric(Sum1 - app.CountMatrix[i, j]);
                            NumericVector n22 = MSS.CreateNumeric(Sum2 - app.CountMatrix[i, k]);
                            MSS.SetSymbol("n11", n11);
                            MSS.SetSymbol("n12", n12);
                            MSS.SetSymbol("n21", n21);
                            MSS.SetSymbol("n22", n22);
                            MSS.Evaluate("compare <- matrix(c(n11,n12,n21,n22),nr=2)");
                            MSS.Evaluate("p.value <- fisher.test(compare)$p.value");
                            double pvaluetemp = MSS.GetSymbol("p.value").AsNumeric().First();
                            pvalue[i] = Math.Min((double)pvalue[i], (double)pvaluetemp);
                        }
                    }
                }
                MSS.Evaluate("bonferroni.p <- p.adjust(p.value,\"bonferroni\")");
                
                MSS.Evaluate("fdr.p <- p.adjust(p.value,\"fdr\")");

                for (int i = 0; i < FeatureNum; i++)
                {
                    bonferroni[i] = MSS.GetSymbol("bonferroni.p").AsNumeric()[i];
                    fdr[i] = MSS.GetSymbol("fdr.p").AsNumeric()[i];
                }

            }

            List<string> Annotation = new List<string>();

            if (this.checkBox1.Checked)
            {

                if (this.radioButton2.Checked)
                {
                    string strConnCOG;

                    strConnCOG = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + System.Windows.Forms.Application.StartupPath + "/COG.xlsx" + ";" + "Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";
                    OleDbConnection OleConnCOG = new OleDbConnection(strConnCOG);
                    OleConnCOG.Open();
                    String sqlCOG = "SELECT * FROM  [Sheet1$]";  

                    OleDbDataAdapter OleDaExcelCOG = new OleDbDataAdapter(sqlCOG, OleConnCOG);
                    app.OleDsExcleCOG = new DataSet();
                    OleDaExcelCOG.Fill(app.OleDsExcleCOG, "Sheet1");
                    OleConnCOG.Close();

                    
                    for (int i = 0; i < FeatureNum; i++)
                    {
                        for (int j = 0; j < app.OleDsExcleCOG.Tables[0].Rows.Count; j++)
                        {
                            if (string.Equals(FeatureName[i], app.OleDsExcleCOG.Tables[0].Rows[j][0].ToString()))
                                Annotation.Add(app.OleDsExcleCOG.Tables[0].Rows[j][1].ToString());
                        }
                        if(Annotation.Count < i + 1)
                            Annotation.Add("No Annotation!");
                    }
                }
                else if (this.radioButton1.Checked)
                {

                    string strConnPFAM;
                    strConnPFAM = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + System.Windows.Forms.Application.StartupPath + "/PFAM.xlsx" + ";" + "Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";
                    OleDbConnection OleConnPFAM = new OleDbConnection(strConnPFAM);
                    OleConnPFAM.Open();
                    String sqlPFAM = "SELECT * FROM  [Sheet1$]";

                    OleDbDataAdapter OleDaExcelPFAM = new OleDbDataAdapter(sqlPFAM, OleConnPFAM);
                    app.OleDsExclePFAM = new DataSet();
                    OleDaExcelPFAM.Fill(app.OleDsExclePFAM, "Sheet1");
                    OleConnPFAM.Close();

                    for (int i = 0; i < FeatureNum; i++)
                    {
                        for (int j = 0; j < app.OleDsExclePFAM.Tables[0].Rows.Count; j++)
                        {
                            if (string.Equals(FeatureName[i], app.OleDsExclePFAM.Tables[0].Rows[j][0].ToString()))
                                Annotation.Add(app.OleDsExclePFAM.Tables[0].Rows[j][1].ToString());
                        }
                        if (Annotation.Count < i + 1)
                            Annotation.Add("No Annotation!");
                    }
                }
            }

            DataTable dt = new DataTable();
            
            
            
            dt.Columns.Add("Feature", typeof(string));
            
            
            
            
            for (int i = 0; i < SampleNum; i++)
            {
                dt.Columns.Add(app.SamName[i], typeof(double)); 
            }
            dt.Columns.Add("p.value", typeof(double));
            dt.Columns.Add("bonferroni.p", typeof(double));
            dt.Columns.Add("fdr.p", typeof(double));
            
            dt.Columns.Add("Annotation", typeof(string));
            
            for (int i = 0; i < SampleNum; i++)
            {
                dt.Columns.Add(SampleNameFreq[i], typeof(double)); 
            }
            
            
            
            for (int i = 0; i < FeatureNum; i++)
            {
                DataRow dr = dt.NewRow();
                dr[0] = FeatureName[i];
                for (int j = 1; j < SampleNum + 1; j++)
                {
                    dr[j] = app.CountMatrix[i, j - 1];
                }
                if (pvalue[i] == 100)
                {
                    dr[SampleNum + 1] = DBNull.Value;
                    dr[SampleNum + 2] = DBNull.Value;
                    dr[SampleNum + 3] = DBNull.Value;
                }
                else 
                {
                    dr[SampleNum + 1] = pvalue[i];
                    dr[SampleNum + 2] = bonferroni[i];
                    dr[SampleNum + 3] = fdr[i];

                }
                if (this.checkBox1.Checked)
                {
                    dr[SampleNum + 4] = Annotation[i];
                }
                else
                {
                    dr[SampleNum + 4] = null;
                }
                for (int j = 0; j < SampleNum; j++)
                {
                    dr[j + SampleNum + 5] = app.FreqMatrix[i, j];
                }
            
                
                dt.Rows.Add(dr);
            }
            
            
            DataTable dtCopy = dt.Copy();
            DataTable dttemp = dt.Copy();
            dttemp.Clear();
            DataView dv = dt.DefaultView;
            dv.Sort = "p.value";
            dtCopy = dv.ToTable();
            for (int i = 0; i < NAnum; i++)
            {
                DataRow row = dtCopy.Rows[i];
                dttemp.Rows.Add(row.ItemArray);
            }
            for (int i = 0; i < NAnum; i++)
            {
                dtCopy.Rows.RemoveAt(0);
            }
            
            dtCopy.Merge(dttemp);
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];
            Microsoft.Office.Interop.Excel.Range range;
            long totalCount = dtCopy.Rows.Count;
            long rowRead = 0;
            float percent = 0;
            for (int i = 0; i < dtCopy.Columns.Count - SampleNum; i++)
            {
                worksheet.Cells[1, i + 1] = dtCopy.Columns[i].ColumnName;
                range = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, i + 1];
                range.Interior.ColorIndex = 15;
                range.Font.Bold = true;
            }
            for (int r = 0; r < dtCopy.Rows.Count; r++)
            {
                for (int i = 0; i < dtCopy.Columns.Count - SampleNum; i++)
                {
                    worksheet.Cells[r + 2, i + 1] = dtCopy.Rows[r][i].ToString();
                }
                rowRead++;
                percent = ((float)(100 * rowRead)) / totalCount;
            }
            xlApp.Visible = true;
            int pnum = 0;
            for (int i = 0; i < FeatureNum; i++)
            {
                try
                {
                    if (double.Parse(dtCopy.Rows[i][SampleNum].ToString()) < double.Parse(this.textBox1.Text.ToString()))
                        pnum++;
                }
                catch 
                { }
            }

            double[,] df = new double[Math.Min(10, FeatureNum), SampleNum];
            for (int i = 0; i < Math.Min(10, FeatureNum); i++)
            {
                for (int j = 0; j < SampleNum; j++)
                {
                    df[i, j] = double.Parse(dtCopy.Rows[i][SampleNum + 5 + j].ToString());
                }
            }
            string[] rownamesdf = new string[Math.Min(10, FeatureNum)];
            for (int i = 0; i < Math.Min(10, FeatureNum); i++)
            {
                rownamesdf[i] = dtCopy.Rows[i][0].ToString();
            }
            CharacterVector Rrownamesdf = MSS.CreateCharacterVector(rownamesdf);
            MSS.SetSymbol("Rownamedf", Rrownamesdf);
            NumericMatrix Rdf = MSS.CreateNumericMatrix(df);
            MSS.SetSymbol("Freqdf", Rdf);
            NumericVector RRow = MSS.CreateNumeric(Math.Min(10, FeatureNum));
            MSS.SetSymbol("selrow", RRow);
            MSS.Evaluate("Freqdf <- as.data.frame(Freqdf)");
            MSS.Evaluate("rownames(Freqdf) <- Rownamedf");
            MSS.Evaluate("colnames(Freqdf) <- SampleName");
            
            MSS.Evaluate("colournum <- rainbow(dim(Freqdf)[2])");
            MSS.Evaluate("plotdata <- t(Freqdf)");
            MSS.Evaluate("windows()");
            MSS.Evaluate("barplot(plotdata,main=\"features with top varition\",ylab=\"Freq\",beside=TRUE,horiz=FALSE, cex.names=0.6,col=colournum)");
            MSS.Evaluate("legend(\"topright\",SampleName,fill=colournum)");
            
            
            if (pnum > 0)
            {
                double[,] dfall = new double[pnum, SampleNum];
                for (int i = 0; i < pnum; i++)
                {
                    for (int j = 0; j < SampleNum; j++)
                    {
                        dfall[i, j] = double.Parse(dtCopy.Rows[i][SampleNum + 5 + j].ToString());
                    }
                }
                string[] rownamesall = new string[pnum];
                for (int i = 0; i < pnum; i++)
                {
                    rownamesall[i] = dtCopy.Rows[i][0].ToString();
                }
                CharacterVector Rrownamesall = MSS.CreateCharacterVector(rownamesall);
                MSS.SetSymbol("Rownameall", Rrownamesall);
                NumericMatrix Rdfall = MSS.CreateNumericMatrix(dfall);
                MSS.SetSymbol("Freqdfall", Rdfall);
                NumericVector RRowall = MSS.CreateNumeric(pnum);
                MSS.SetSymbol("selrowall", RRowall);
                MSS.Evaluate("Freqdfall <- as.data.frame(Freqdfall)");

                MSS.Evaluate("rownames(Freqdfall) <- Rownameall");
                MSS.Evaluate("colnames(Freqdfall) <- SampleName");
                
                MSS.Evaluate("distance <- as.dist(1-abs(cor(Freqdfall)))");
                MSS.Evaluate("fit <- cmdscale(distance, eig=TRUE, k=2)");
                MSS.Evaluate("x <- fit$points[,1]");
                MSS.Evaluate("y <- fit$points[,2]");
                MSS.Evaluate("windows()");
                MSS.Evaluate("plot(x,y,xlab=\"Coordinate 1\",ylab=\"Coordinate 2\",main=\"MDS\", type=\"p\",col=\"red\")");
                MSS.Evaluate("text(x,y,labels=SampleName,pos=4)");

                MSS.Evaluate("windows()");
                MSS.Evaluate("plot(hclust(distance),main =\"Samples Clust\")");
            }
            else
            {
                MessageBox.Show("No differntially abundant features!!");
            }

            int Rownum = 0;
            for (int i = 0; i < FeatureNum; i++)
            {
                double tempSum = 0;
                double tempMean = 0;
                for (int j = 0; j < SampleNum; j++)
                {
                    tempSum = tempSum + app.FreqMatrix[i, j];
                }
                tempMean = tempSum/(SampleNum);
                if(tempSum > 0)
                {
                    FreqSum.Add(tempSum);
                    List<double> tempRow = new List<double>();
                    for(int j  =0; j < SampleNum; j++)
                    {
                        tempRow.Add(app.FreqMatrix[i, j]/tempMean);
                    }
                    Freqtemp.Add(tempRow);
                    Rownum= Rownum + 1;
                }
            }

            for (int i = 0; i < Rownum; i++)
            {
                for (int j = 0; j < SampleNum; j++)
                {
                    Freqtemp[i][j] = Math.Log(Freqtemp[i][j], 2);
                    if (Freqtemp[i][j] > 1)
                        Freqtemp[i][j] = 1;
                    else if (Freqtemp[i][j] < -1)
                        Freqtemp[i][j] = -1;
                }
            }


            double[,] dfhm = new double[Math.Min(500, Rownum), SampleNum];

            for (int i = 0; i < Math.Min(500, Rownum); i++)
            {

                for (int j = 0; j < SampleNum; j++)
                {
                   dfhm[i,j] = double.Parse(Freqtemp[i][j].ToString());
                }
            }
            string[] rownameshm = new string[Math.Min(500, Rownum)];
            for (int i = 0; i < Math.Min(500, Rownum); i++)
            {
                rownameshm[i] = dtCopy.Rows[i][0].ToString();
            }
            CharacterVector Rrownameshm = MSS.CreateCharacterVector(rownameshm);
            MSS.SetSymbol("Rownamehm", Rrownameshm);

            NumericMatrix Rdfhm = MSS.CreateNumericMatrix(dfhm);
            MSS.SetSymbol("Freqdfhm", Rdfhm);
            NumericVector RRowhm = MSS.CreateNumeric(Math.Min(500, Rownum));
            MSS.SetSymbol("plotnum", RRowhm);
            MSS.Evaluate("Freqdfhm <- as.data.frame(Freqdfhm)");
            MSS.Evaluate("rownames(Freqdfhm) <- Rownamehm");
            MSS.Evaluate("colnames(Freqdfhm) <- SampleName");
            MSS.Evaluate("Freqdfhm <- as.matrix(Freqdfhm)");
            MSS.Evaluate("library(pheatmap)");
            MSS.Evaluate("windows()");
            MSS.Evaluate("pheatmap(Freqdfhm[1:plotnum,],show_rownames=F)");
            
            this.Close();

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                this.radioButton1.Enabled = true;
                this.radioButton2.Enabled = true;
            }
            else
            {
                this.radioButton1.Enabled = false;
                this.radioButton2.Enabled = false;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

    }
}
