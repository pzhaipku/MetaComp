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
    public partial class Two_Groups_Ana : Form
    {
        public Two_Groups_Ana()
        {
            InitializeComponent();
        }

        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form9_Load(object sender, EventArgs e)
        {
            this.comboBox1.SelectedIndex = 0;
            this.radioButton1.Enabled = false;
            this.radioButton2.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
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

            REngine.SetEnvironmentVariables();

            REngine TGS = REngine.GetInstance();

            TGS.Initialize();

            NumericMatrix Freq = TGS.CreateNumericMatrix(app.FreqMatrix);
            TGS.SetSymbol("Freq", Freq);
            NumericMatrix Count = TGS.CreateNumericMatrix(app.CountMatrix);
            TGS.SetSymbol("Count", Count);
            NumericVector RFeatureNum = TGS.CreateNumeric(FeatureNum);
            NumericVector RSampleNum = TGS.CreateNumeric(SampleNum);
            TGS.SetSymbol("FeatureNum", RFeatureNum);
            TGS.SetSymbol("SampleNum", RSampleNum);
            CharacterVector SampleName = TGS.CreateCharacterVector(app.SamName);
            CharacterVector FeatureName = TGS.CreateCharacterVector(app.FeaName);
            TGS.SetSymbol("FeatureName", FeatureName);
            TGS.SetSymbol("SampleName", SampleName);
                      
            List<string> SampleNameFreq = new List<string>();
            List<double?> OddRatio = new List<double?>();
            List<double?> absOddRatio = new List<double?>();
            List<List<double>> Freqtemp = new List<List<double>>();
            List<double> FreqSum = new List<double>();
            
           
            int Correct1 = 0;
            int Correct2 = 0;
            int method = 0;
            int GroupNum = int.Parse(this.textBox3.Text.ToString());
            NumericVector RGroupNum = TGS.CreateNumeric(GroupNum);
            TGS.SetSymbol("Groupsep", RGroupNum);
            for (int i = 0; i < SampleNum; i++)
            {
                SampleNameFreq.Add(SampleName[i] + "Freq");
            }
            
            if (this.comboBox1.SelectedIndex == 0)
            {
            

                int effNum1, effNum2;

                TGS.Evaluate("FeatureSums1 <- rowSums(Count[,1:Groupsep])");
                TGS.Evaluate("FeatureSums2 <- rowSums(Count[,Groupsep:SampleNum])");
                TGS.Evaluate("effnum1 <- length(FeatureSums1[FeatureSums1 > Groupsep])");
                TGS.Evaluate("effnum2 <- length(FeatureSums2[FeatureSums2 > (SampleNum - Groupsep)])");
                effNum1 = Convert.ToInt32(TGS.GetSymbol("effnum1").AsNumeric().First());
                effNum2 = Convert.ToInt32(TGS.GetSymbol("effnum2").AsNumeric().First());
                for (int i = 0; i < FeatureNum; i++)
                { 
                    double rowsums1 = 0;
                    double rowsums2 = 0;
                    double[] rowsCount1 = new double[GroupNum];
                    for(int j = 0; j < GroupNum; j++)
                    {
                        rowsums1 = rowsums1 + app.CountMatrix[i,j];
                        rowsCount1[j] = app.CountMatrix[i,j];
                    }
                    if (rowsums1 > GroupNum)
                    {
                        NumericVector rowscount1 = TGS.CreateNumericVector(rowsCount1);
                        TGS.SetSymbol("rowcount1", rowscount1);
                        TGS.Evaluate("result1 <- ks.test(rowcount1,\"pnorm\",mean(rowcount1),sd(rowcount1))");
                        TGS.Evaluate("factor1 <- result1$p");
                        double prows1 = TGS.GetSymbol("factor1").AsNumeric().First();

                        if (prows1 < 0.05)
                            Correct1 = Correct1 + 1;
                    }
                    double[] rowsCount2 = new double[SampleNum - GroupNum];
                    for(int j = GroupNum; j < SampleNum; j++)
                    {
                        rowsums2 = rowsums2 + app.CountMatrix[i,j];
                        rowsCount2[j - GroupNum] = app.CountMatrix[i,j];
                    }
                    if (rowsums2 > (SampleNum - GroupNum))
                    {
                        NumericVector rowscount2 = TGS.CreateNumericVector(rowsCount2);
                        TGS.SetSymbol("rowcount2", rowscount2);
                        TGS.Evaluate("result2 <- ks.test(rowcount2,\"pnorm\",mean(rowcount2),sd(rowcount2))");
                        TGS.Evaluate("factor2 <- result2$p");
                        double prows2 = TGS.GetSymbol("factor2").AsNumeric().First();

                        if (prows2 < 0.05)
                            Correct2 = Correct2 + 1;
                    }

                }
                   
                bool condition1 = (Correct1 >= effNum1 * 0.5) && (Correct2 >= effNum2 * 0.5);
                bool condition2 = GroupNum == SampleNum - GroupNum;
                if (condition1)
                {
                    if (condition2)
                        method = 2;
                    else
                        method = 1;
                }
                else
                {
                    if (condition2)
                        method = 4;
                    else
                        method = 3;
                }
                switch(method)
                {
                    case 1:
                        MessageBox.Show("Statistical Method : t-test");
                        break;
                    case 2:
                        MessageBox.Show("Statistical Method : Pair t-test");
                        break;
                    case 3:
                        MessageBox.Show("Statistical Method : Mann-Whitney U test");
                        break;
                    case 4:
                        MessageBox.Show("Statistical Method : Wilcoxon sign-rank test");
                        break;
                    default:
                        break;
                }
            }
            
            TGS.Evaluate("FreqMatrix <- as.data.frame(Freq)");
            TGS.Evaluate("names(FreqMatrix) <- SampleName");
            TGS.Evaluate("samp1_mean <- apply(FreqMatrix[,1:Groupsep],1,mean)");
            TGS.Evaluate("samp2_mean <- apply(FreqMatrix[,(Groupsep+1):SampleNum],1,mean)");
            TGS.Evaluate("samp1_sd <- apply(FreqMatrix[,1:Groupsep],1,sd)");
            TGS.Evaluate("samp2_sd <- apply(FreqMatrix[,(Groupsep+1):SampleNum],1,sd)");
            TGS.Evaluate("samp1_stat <- paste(samp1_mean,samp1_sd,sep=\"±\")");
            TGS.Evaluate("samp2_stat <- paste(samp2_mean,samp2_sd,sep=\"±\")");

            string[] s1_stat = (string[])TGS.GetSymbol("samp1_stat").AsCharacter().ToArray();
            string[] s2_stat = (string[])TGS.GetSymbol("samp2_stat").AsCharacter().ToArray();
            
            if (this.comboBox1.SelectedIndex != 6)
            {
                switch (this.comboBox1.SelectedIndex + method)
                {
                    case 1:
                        
                        for (int i = 1; i <= FeatureNum; i++)
                        {
                            TGS.SetSymbol("i", TGS.CreateNumeric(i));
                            TGS.Evaluate("group1_freq <- as.numeric(FreqMatrix[i,1:Groupsep])");
                            TGS.Evaluate("group2_freq <- as.numeric(FreqMatrix[i,(Groupsep+1):SampleNum])");
                            TGS.Evaluate("p.value <- t.test(group1_freq,group2_freq, paired=FALSE)$p.value");
                            pvalue.Add(TGS.GetSymbol("p.value").AsNumeric().First());
                        }
                        break;
            
                    case 2:
                        if (GroupNum != SampleNum - GroupNum)
                        {
                            MessageBox.Show("This statistical test must have same number samples in each category!", "WARNIMG");
                            break;
                        }
                        else
                        {
                           
                            for (int i = 1; i <= FeatureNum; i++)
                            {
                                TGS.SetSymbol("i", TGS.CreateNumeric(i));
                                TGS.Evaluate("group1_freq <- as.numeric(FreqMatrix[i,1:Groupsep])");
                                TGS.Evaluate("group2_freq <- as.numeric(FreqMatrix[i,(Groupsep+1):SampleNum])");
                                TGS.Evaluate("p.value <- t.test(group1_freq,group2_freq, paired=TRUE)$p.value");
                                pvalue.Add(TGS.GetSymbol("p.value").AsNumeric().First());
                            }
                            break;
                        }
                    case 3:
                        for (int i = 1; i <= FeatureNum; i++)
                        {
                            TGS.SetSymbol("i", TGS.CreateNumeric(i));
                            TGS.Evaluate("group1_freq <- as.numeric(FreqMatrix[i,1:Groupsep])");
                            TGS.Evaluate("group2_freq <- as.numeric(FreqMatrix[i,(Groupsep+1):SampleNum])");
                            TGS.Evaluate("p.value <- wilcox.test(group1_freq,group2_freq,exact = FALSE)$p.value");
                            pvalue.Add(TGS.GetSymbol("p.value").AsNumeric().First());
                        }
                        break;
                    case 4:
                        if (GroupNum != SampleNum - GroupNum)
                        {
                            MessageBox.Show("This statistical test must have same number samples in each category!", "WARNIMG");
                            break;
                        }
                        else
                        {
                            for (int i = 1; i <= FeatureNum; i++)
                            {
                                TGS.SetSymbol("i", TGS.CreateNumeric(i));
                                TGS.Evaluate("group1_freq <- as.numeric(FreqMatrix[i,1:Groupsep])");
                                TGS.Evaluate("group2_freq <- as.numeric(FreqMatrix[i,(Groupsep+1):SampleNum])");
                                TGS.Evaluate("p.value <- wilcox.test(group1_freq,group2_freq,paired=TRUE,exact = FALSE)$p.value");
                                pvalue.Add(TGS.GetSymbol("p.value").AsNumeric().First());
                            }
                            break;
                        }
                    case 5:
                        double Sum1 = 0;
                        double Sum2 = 0;
                        
            
                        for (int i = 0; i < FeatureNum; i++)
                        {
                            for (int j = 0; j < GroupNum; j++)
                                Sum1 = Sum1 + app.CountMatrix[i,j];
                            for (int j = GroupNum; j < SampleNum; j++)
                                Sum2 = Sum2 + app.CountMatrix[i,j];
                        }
                        for (int i = 0; i < FeatureNum; i++)
                        {
                            double n11 = 0;
                            double n12 = 0;
                            double n21 = 0;
                            double n22 = 0;
                            for (int j = 0; j < GroupNum; j++)
                                n11 = n11 + app.CountMatrix[i,j];
                            for (int j = GroupNum; j < SampleNum; j++)
                                n21 = n21 + app.CountMatrix[i,j];
                            n12 = Sum1 - n11;
                            n22 = Sum2 - n21;
                            NumericVector Rn11 = TGS.CreateNumeric(n11);
                            NumericVector Rn21 = TGS.CreateNumeric(n21);
                            NumericVector Rn12 = TGS.CreateNumeric(n12);
                            NumericVector Rn22 = TGS.CreateNumeric(n22);
                            TGS.SetSymbol("n11", Rn11);
                            TGS.SetSymbol("n12", Rn12);
                            TGS.SetSymbol("n21", Rn21);
                            TGS.SetSymbol("n22", Rn22);
                            TGS.Evaluate("compare <- matrix(c(n11,n12,n21,n22),nr=2)");
                            TGS.Evaluate("p.value <- fisher.test(compare)$p.value");
                            pvalue.Add(TGS.GetSymbol("p.value").AsNumeric().First());
                        }
                        break;
                    default:
                        break;
                }

                NumericVector Rpvalue = TGS.CreateNumericVector(pvalue);
                TGS.SetSymbol("p.value", Rpvalue);
                TGS.Evaluate("NAnum = length(p.value[is.nan(p.value)])");
                NAnum = Convert.ToInt32(TGS.GetSymbol("NAnum").AsNumeric().First());
                TGS.Evaluate("bonferroni.p <- p.adjust(p.value,\"bonferroni\")");
                TGS.Evaluate("fdr.p <- p.adjust(p.value,\"fdr\")");
                for (int i = 0; i < FeatureNum; i++)
                {
                    bonferroni[i] = TGS.GetSymbol("bonferroni.p").AsNumeric()[i];
                    fdr[i] = TGS.GetSymbol("fdr.p").AsNumeric()[i];
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
                    dt.Columns.Add(SampleName[i], typeof(double)); ;
                }
                dt.Columns.Add("group1", typeof(string));
                dt.Columns.Add("group2", typeof(string));
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
                    for (int j = 1; j <= SampleNum; j++)
                    {
                        dr[j] = app.CountMatrix[i, j - 1];
                    }
                    dr[SampleNum + 1] = s1_stat[i];
                    dr[SampleNum + 2] = s2_stat[i];
                    if (double.IsNaN(pvalue[i]))
                    {
                        dr[SampleNum + 3] = DBNull.Value;
                        dr[SampleNum + 4] = DBNull.Value;
                        dr[SampleNum + 5] = DBNull.Value;
                    }
                    else 
                    { 
                        dr[SampleNum + 3] = pvalue[i];
                        dr[SampleNum + 4] = bonferroni[i];
                        dr[SampleNum + 5] = fdr[i];
                    }
                    
                    if (this.checkBox1.Checked)
                    {
                        dr[SampleNum + 6] = Annotation[i];
                    }
                    else
                    {
                        dr[SampleNum + 6] = null;
                    }
            
                    for (int j = 0; j < SampleNum; j++)
                    {
                        dr[j + SampleNum + 7] = app.FreqMatrix[i,j];
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
                int pnum = 0;
                for (int i = 0; i < FeatureNum; i++)
                {
                    try
                    {
                        if (double.Parse(dtCopy.Rows[i][SampleNum + 3].ToString()) < double.Parse(this.textBox1.Text.ToString()))
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
                        df[i, j] = double.Parse(dtCopy.Rows[i][SampleNum + 7 + j].ToString());
                    }
                }
                string[] rownamesdf = new string[Math.Min(10, FeatureNum)];
                for (int i = 0; i < Math.Min(10, FeatureNum); i++)
                {
                    rownamesdf[i] = dtCopy.Rows[i][0].ToString();
                }
                CharacterVector Rrownamesdf = TGS.CreateCharacterVector(rownamesdf);
                TGS.SetSymbol("Rownamedf", Rrownamesdf);

                NumericMatrix Rdf = TGS.CreateNumericMatrix(df);
                TGS.SetSymbol("Freqdf", Rdf);
                NumericVector RRow = TGS.CreateNumeric(Math.Min(10, FeatureNum));
                TGS.SetSymbol("selrow", RRow);
                TGS.Evaluate("Freqdf <- as.data.frame(Freqdf)");
                TGS.Evaluate("rownames(Freqdf) <- Rownamedf");
                TGS.Evaluate("colnames(Freqdf) <- SampleName");
                TGS.Evaluate("colournum <- rainbow(dim(Freqdf)[2])");
                TGS.Evaluate("plotdata <- t(Freqdf)");
                TGS.Evaluate("windows()");
                TGS.Evaluate("barplot(plotdata,main=\"features with top varition\",ylab=\"Freq\",beside=TRUE,horiz=FALSE, cex.names=0.6,col=colournum)");
                TGS.Evaluate("legend(\"topright\",SampleName,fill=colournum)");
                if (pnum > 0)
                {
                    double[,] dfall = new double[pnum, SampleNum];
                    for (int i = 0; i < pnum; i++)
                    {
                        for (int j = 0; j < SampleNum; j++)
                        {
                            dfall[i, j] = double.Parse(dtCopy.Rows[i][SampleNum + 7 + j].ToString());
                        }
                    }
                    string[] rownamesall = new string[pnum];
                    for (int i = 0; i < pnum; i++)
                    {
                        rownamesall[i] = dtCopy.Rows[i][0].ToString();
                    }
                    CharacterVector Rrownamesall = TGS.CreateCharacterVector(rownamesall);
                    TGS.SetSymbol("Rownameall", Rrownamesall);

                    NumericMatrix Rdfall = TGS.CreateNumericMatrix(dfall);
                    TGS.SetSymbol("Freqdfall", Rdfall);
                    NumericVector RRowall = TGS.CreateNumeric(pnum);
                    TGS.SetSymbol("selrowall", RRowall);
                    TGS.Evaluate("Freqdfall <- as.data.frame(Freqdfall)");
                    TGS.Evaluate("rownames(Freqdfall) <- Rownameall");
                    TGS.Evaluate("colnames(Freqdfall) <- SampleName");
                    TGS.Evaluate("distance <- as.dist(1-abs(cor(Freqdfall)))");
                    TGS.Evaluate("fit <- cmdscale(distance, eig=TRUE, k=2)");
                    TGS.Evaluate("x <- fit$points[,1]");
                    TGS.Evaluate("y <- fit$points[,2]");
                    TGS.Evaluate("windows()");
                    TGS.Evaluate("plot(x,y,xlab=\"Coordinate 1\",ylab=\"Coordinate 2\",main=\"MDS\", type=\"p\",col=\"red\")");
                    TGS.Evaluate("text(x,y,labels=SampleName,pos=4)");

                    TGS.Evaluate("windows()");
                    TGS.Evaluate("plot(hclust(distance),main =\"Samples Clust\")");
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
                    tempMean = tempSum / (SampleNum);
                    if (tempSum > 0)
                    {
                        FreqSum.Add(tempSum);
                        List<double> tempRow = new List<double>();
                        for (int j = 0; j < SampleNum; j++)
                        {
                            tempRow.Add(app.FreqMatrix[i, j] / tempMean);
                        }
                        Freqtemp.Add(tempRow);
                        Rownum = Rownum + 1;
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
                        dfhm[i, j] = double.Parse(Freqtemp[i][j].ToString());
                    }
                }
                string[] rownameshm = new string[Math.Min(500, Rownum)];
                for (int i = 0; i < Math.Min(500, Rownum); i++)
                {
                    rownameshm[i] = dtCopy.Rows[i][0].ToString();
                }
                CharacterVector Rrownameshm = TGS.CreateCharacterVector(rownameshm);
                TGS.SetSymbol("Rownamehm", Rrownameshm);

                NumericMatrix Rdfhm = TGS.CreateNumericMatrix(dfhm);
                TGS.SetSymbol("Freqdfhm", Rdfhm);
                NumericVector RRowhm = TGS.CreateNumeric(Math.Min(500, Rownum));
                TGS.SetSymbol("plotnum", RRowhm);
                TGS.Evaluate("Freqdfhm <- as.data.frame(Freqdfhm)");
                TGS.Evaluate("rownames(Freqdfhm) <- Rownamehm");
                TGS.Evaluate("colnames(Freqdfhm) <- SampleName");
                TGS.Evaluate("Freqdfhm <- as.matrix(Freqdfhm)");
                TGS.Evaluate("library(pheatmap)");
                TGS.Evaluate("windows()");
                TGS.Evaluate("pheatmap(Freqdfhm[1:plotnum,],show_rownames=F)");
            
            }
            else if (this.comboBox1.SelectedIndex == 6)
            {
                double Sum1 = 0;
                double Sum2 = 0;
                
                for (int i = 0; i < FeatureNum; i++)
                {
                    for (int j = 0; j < GroupNum; j++)
                        Sum1 = Sum1 + app.CountMatrix[i,j];
                    for (int j = GroupNum; j < SampleNum - 1; j++)
                        Sum2 = Sum2 + app.CountMatrix[i, j];
                }
            
                TGS.SetSymbol("Sum1", TGS.CreateNumeric(Sum1));
                TGS.SetSymbol("Sum2", TGS.CreateNumeric(Sum2));
                TGS.Evaluate("R <- Sum1/Sum2");
                TGS.Evaluate("treatadd <- R/(R+1)");
                TGS.Evaluate("controladd <- 1/(R+1)");
                for (int i = 0; i < FeatureNum; i++)
                {
                    double n11 = 0;
                    double n12 = 0;
                    double n21 = 0;
                    double n22 = 0;
                    for (int j = 0; j < GroupNum; j++)
                        n11 = n11 + app.Count[i][j];
                    for (int j = GroupNum; j < SampleNum - 1; j++)
                        n21 = n21 + app.Count[i][j];
                    if ((n11 < GroupNum) && (n21 < SampleNum - 1 - GroupNum))
                    {
                        OddRatio.Add(null);
                        absOddRatio.Add(null);
                    }    
                    else
                    {
                        n12 = Sum1 - n11;
                        n22 = Sum2 - n21;
                        TGS.SetSymbol("n11", TGS.CreateNumeric(n11));
                        TGS.SetSymbol("n12", TGS.CreateNumeric(n12));
                        TGS.SetSymbol("n21", TGS.CreateNumeric(n21));
                        TGS.SetSymbol("n22", TGS.CreateNumeric(n22));
                        TGS.Evaluate("odd_ratio <- log2(((n11+treatadd)*(n22+controladd))/((n21+controladd)*(n12+treatadd )))");
                        OddRatio.Add(double.Parse(TGS.GetSymbol("odd_ratio").ToString()));
                        absOddRatio.Add(Math.Abs(double.Parse(TGS.GetSymbol("odd_ratio").ToString())));
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
                    dt.Columns.Add(SampleName[i], typeof(double)); ;
                }
                dt.Columns.Add("Odd_Ratio", typeof(double));
                dt.Columns.Add("abs_Odd_Ratio", typeof(double));
                dt.Columns.Add("Annotation", typeof(string));
            
                for (int i = 0; i < FeatureNum; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = FeatureName[i];
                    for (int j = 1; j <= SampleNum; j++)
                    {
                        dr[j] = app.CountMatrix[i, j - 1];
                    }
                    if (OddRatio[i] == null)
                    {
                        dr[SampleNum] = DBNull.Value;
                        dr[SampleNum + 1] = DBNull.Value;
                    }
                    else
                    {
                        dr[SampleNum] = OddRatio[i];
                        dr[SampleNum + 1] = absOddRatio[i];
                    }
                    if (this.checkBox1.Checked)
                    {
                        dr[SampleNum + 2] = Annotation[i];
                    }
                    else
                    {
                        dr[SampleNum + 2] = null;
                    }
                    dt.Rows.Add(dr);
                }
                DataTable dtCopy = dt.Copy();
                DataView dv = dt.DefaultView;
                dv.Sort = "abs_Odd_Ratio DESC";
                dtCopy = dv.ToTable();
            
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
                for (int i = 0; i < dtCopy.Columns.Count; i++)
                {
                    worksheet.Cells[1, i + 1] = dtCopy.Columns[i].ColumnName;
                    range = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, i + 1];
                    range.Interior.ColorIndex = 15;
                    range.Font.Bold = true;
                }
                for (int r = 0; r < dtCopy.Rows.Count; r++)
                {
                    for (int i = 0; i < dtCopy.Columns.Count; i++)
                    {
                        worksheet.Cells[r + 2, i + 1] = dtCopy.Rows[r][i].ToString();
                    }
                    rowRead++;
                    percent = ((float)(100 * rowRead)) / totalCount;
                }
                xlApp.Visible = true;
            }
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Dispose();
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
    }
}
