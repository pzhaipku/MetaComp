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
using RDotNet;

namespace MetaComp
{
    public partial class Two_Sample_Ana : Form
    {
        public Two_Sample_Ana()
        {
            InitializeComponent();
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            
            this.comboBox1.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 0;
            this.radioButton1.Enabled = false;
            this.radioButton2.Enabled = false;
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
            int NAnum = 0;
            if (SampleNum == 2)
            {
                REngine.SetEnvironmentVariables();

                REngine TSS = REngine.GetInstance();

                TSS.Initialize();

                NumericMatrix Freq = TSS.CreateNumericMatrix(app.FreqMatrix);
                TSS.SetSymbol("Freq", Freq);
                NumericMatrix Count = TSS.CreateNumericMatrix(app.CountMatrix);
                TSS.SetSymbol("Count", Count);
                CharacterVector SampleName = TSS.CreateCharacterVector(app.SamName);
                CharacterVector FeatureName = TSS.CreateCharacterVector(app.FeaName);
                TSS.SetSymbol("FeatureName", FeatureName);
                TSS.SetSymbol("SampleName", SampleName);
                List<string> SampleNameFreq = new List<string>();
                for (int i = 0; i < SampleNum; i++)
                {
                    SampleNameFreq.Add(SampleName[i] + "Freq");
                }

                IntegerVector RFeaNum = TSS.CreateInteger(FeatureNum);
                NumericVector Ralpha = TSS.CreateNumeric(double.Parse(this.textBox1.Text.ToString()));


                TSS.SetSymbol("n", RFeaNum);
                TSS.SetSymbol("alpha", Ralpha);
                TSS.Evaluate("leastnum = ceiling ( qnorm(alpha/2)^2 )");

                double leastnum = TSS.Evaluate("leastnum").AsNumeric().First();
                if (this.comboBox1.SelectedIndex == 0)
                {
                    for (int i = 0; i < FeatureNum; i++)
                    {
                        double temp = app.FreqMatrix[i, 0] - app.FreqMatrix[i, 1];
                        prob.Add(temp);

                        double P = (app.CountMatrix[i, 0] + app.CountMatrix[i, 1]) / (app.SampleTotal[0] + app.SampleTotal[1]);
                        double S = Math.Sqrt(P * (1 - P) * (1 / app.SampleTotal[0] + 1 / app.SampleTotal[1]));
                        double temp0 = prob[i] / S;
                        if (double.IsNaN(temp0))
                            temp0 = 0;
                        stat.Add(temp0);
                    }

                    for (int i = 0; i < FeatureNum; i++)
                    {
                        NumericVector Rstat = TSS.CreateNumeric(stat[i]);
                        TSS.SetSymbol("stat", Rstat);
                        TSS.Evaluate("p.value <- 2*(pnorm(-abs(stat)))");
                        if ((this.comboBox2.SelectedIndex == 1) && (app.CountMatrix[i, 0] < leastnum) && (app.CountMatrix[i, 1] < leastnum))
                            pvalue.Add(100);
                        else
                            pvalue.Add(TSS.GetSymbol("p.value").AsNumeric().First());
                    }
                    NumericVector Rpvalue = TSS.CreateNumericVector(pvalue);
                    TSS.SetSymbol("p.value", Rpvalue);
                    TSS.Evaluate("NAnum = length(p.value[p.value == 100])");
                    NAnum = Convert.ToInt32(TSS.GetSymbol("NAnum").AsNumeric().First());
                    TSS.Evaluate("p.value[p.value == 100] = NA");
                    TSS.Evaluate("bonferroni.p <- p.adjust(p.value,\"bonferroni\")");
                    TSS.Evaluate("bonferroni.p[which(bonferroni.p == NA)] = 100");
                    //double[] temp1 = (double[])hc1.GetSymbol("bonferroni.p");
                    TSS.Evaluate("fdr.p <- p.adjust(p.value,\"fdr\")");
                    TSS.Evaluate("fdr.p[which(fdr.p == NA)] = 100");
                    for (int i = 0; i < FeatureNum; i++)
                    {
                        bonferroni[i] = TSS.GetSymbol("bonferroni.p").AsNumeric()[i];
                        fdr[i] = TSS.GetSymbol("fdr.p").AsNumeric()[i];
                    }
                }
                else if (this.comboBox1.SelectedIndex == 1)
                {

                    double Sum1 = 0;
                    double Sum2 = 0;
                    for (int i = 0; i < FeatureNum; i++)
                    {
                        Sum1 = Sum1 + app.CountMatrix[i, 0];
                        Sum2 = Sum2 + app.CountMatrix[i, 1];
                    }
                    for (int i = 0; i < FeatureNum; i++)
                    {
                        NumericVector n11 = TSS.CreateNumeric(app.CountMatrix[i, 0]);
                        NumericVector n21 = TSS.CreateNumeric(app.CountMatrix[i, 1]);
                        NumericVector n12 = TSS.CreateNumeric(Sum1 - app.CountMatrix[i, 0]);
                        NumericVector n22 = TSS.CreateNumeric(Sum2 - app.CountMatrix[i, 1]);
                        TSS.SetSymbol("n11", n11);
                        TSS.SetSymbol("n12", n12);
                        TSS.SetSymbol("n21", n21);
                        TSS.SetSymbol("n22", n22);
                        TSS.Evaluate("compare <- matrix(c(n11,n12,n21,n22),nr=2)");
                        TSS.Evaluate("p.value <- fisher.test(compare)$p.value");
                        pvalue.Add(TSS.GetSymbol("p.value").AsNumeric().First());

                    }

                    NumericVector Rpvalue = TSS.CreateNumericVector(pvalue);
                    TSS.SetSymbol("p.value", Rpvalue);

                    TSS.Evaluate("bonferroni.p <- p.adjust(p.value,\"bonferroni\")");

                    TSS.Evaluate("fdr.p <- p.adjust(p.value,\"fdr\")");

                    for (int i = 0; i < FeatureNum; i++)
                    {
                        bonferroni[i] = TSS.GetSymbol("bonferroni.p").AsNumeric()[i];
                        fdr[i] = TSS.GetSymbol("fdr.p").AsNumeric()[i];
                    }
                }
                List<string> Annotation = new List<string>();

                if (this.checkBox1.Checked)
                {

                    if (this.radioButton1.Checked)
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
                            if (Annotation.Count < i + 1)
                                Annotation.Add("No Annotation!");
                        }
                    }
                    else if (this.radioButton2.Checked)
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
                        dr[3] = DBNull.Value;
                        dr[4] = DBNull.Value;
                        dr[5] = DBNull.Value;
                    }
                    else
                    {
                        dr[3] = pvalue[i];
                        dr[4] = bonferroni[i];
                        dr[5] = fdr[i];
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

                double[,] df = new double[Math.Min(10, FeatureNum), SampleNum];
                for (int i = 0; i < Math.Min(10, FeatureNum); i++)
                {
                    for (int j = 0; j < SampleNum; j++)
                    {
                       df[i,j] = double.Parse(dtCopy.Rows[i][SampleNum + 5 + j].ToString());
                    }
                }
                string[] rownames = new string[Math.Min(10, FeatureNum)];
                for (int i = 0; i < Math.Min(10, FeatureNum); i++)
                {
                    rownames[i] = dtCopy.Rows[i][0].ToString();
                }
                CharacterVector Rrownames = TSS.CreateCharacterVector(rownames);
                TSS.SetSymbol("Rowname", Rrownames);
                NumericMatrix Rdf = TSS.CreateNumericMatrix(df);
                TSS.SetSymbol("Freqdf", Rdf);
                NumericVector RRow = TSS.CreateNumeric(Math.Min(10, FeatureNum));
                TSS.SetSymbol("selrow", RRow);
                TSS.Evaluate("Freqdf <- as.data.frame(Freqdf)");
                TSS.Evaluate("rownames(Freqdf) <- Rowname");
                TSS.Evaluate("colnames(Freqdf) <- SampleName");
                TSS.Evaluate("colournum <- rainbow(dim(Freqdf)[2])");
                TSS.Evaluate("plotdata <- t(Freqdf)");
                TSS.Evaluate("windows()");
                TSS.Evaluate("barplot(plotdata,main=\"features with top varition\",ylab=\"Freq\",beside=TRUE,horiz=FALSE, cex.names=0.6,col=colournum)");
                TSS.Evaluate("legend(\"topright\",SampleName,fill=colournum)");

            }
            else
            {
                MessageBox.Show("Sample number is more than two!!");
            }

            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
