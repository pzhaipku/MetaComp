using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RDotNet;
using System.IO;


namespace MetaComp
{
    public partial class Environment_Ana : Form
    {
        public Environment_Ana()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.ShowDialog();
            this.textBox2.Text = this.openFileDialog1.FileName;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string filePath = this.textBox2.Text;
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.UTF8);
            string strLine = "";
            string[] aryLine = null;
            string[] tableHead = null;
            int columnCount = 0;
            bool IsFirst = true;
            while ((strLine = sr.ReadLine()) != null)
            {
                if (IsFirst == true)
                {
                    tableHead = strLine.Split('\t');
                    IsFirst = false;
                    columnCount = tableHead.Length;
                    for (int i = 0; i < columnCount; i++)
                    {
                        tableHead[i] = tableHead[i].Replace("\"", "");
                        DataColumn dc = new DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    aryLine = strLine.Split('\t');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j].Replace("\"", "");
                    }
                    dt.Rows.Add(dr);
                }
            }
            sr.Close();
            fs.Close();
            app.EFProfile = dt;

            app.FactorName = new string[app.EFProfile.Columns.Count - 1];
            for (int i = 0; i < app.EFProfile.Columns.Count - 1; i++)
            {
                app.FactorName[i] = app.EFProfile.Columns[i + 1].ColumnName;
            }
            int FactorNum = app.FactorName.GetLength(0);
            int SampleNum = app.EFProfile.Rows.Count;

            app.EFMatrix = new double[SampleNum, FactorNum];

            for (int i = 0; i < SampleNum; i++)
            {
                for (int j = 0; j < FactorNum; j++)
                {
                    app.EFMatrix[i, j] = Convert.ToDouble(app.EFProfile.Rows[i][j + 1]);
                }
            }

            
            app.Allid = new List<List<int>>();
            app.pvalue = new List<List<double>>();
            app.Rvalue = new List<List<double>>();
            app.Fvalue = new List<double>();
            float alpha = float.Parse(this.textBox1.Text.ToString());
            app.alphaen = alpha;

            if ((app.EFMatrix == null) || (app.SamName.GetLength(0) != SampleNum))
            {
                MessageBox.Show("No Environmental Factors information or improper Environmental Factors information!!", "Warning!!!", MessageBoxButtons.OK);
            }
            else
            {
                REngine.SetEnvironmentVariables();

                REngine EF = REngine.GetInstance();

                EF.Initialize();
                if (this.checkBox1.Checked)
                {
                    app.FinalFactorCount = new List<List<double>>();
                    app.FinalFactorName = new List<string>();
                    for (int i = 0; i < app.EFMatrix.GetLength(0); i++)
                    {
                        List<double> rowfactor = new List<double>();
                        for (int j = 0; j < app.EFMatrix.GetLength(1); j++)
                        {
                            rowfactor.Add(app.EFMatrix[i,j]);
                        }
                        for (int j = 0; j < app.EFMatrix.GetLength(1) - 1; j++)
                        {
                            for (int k = j + 1; k < app.EFMatrix.GetLength(1); k++)
                            {
                                rowfactor.Add(app.EFMatrix[i,j] * app.EFMatrix[i,k]);
                            }
                        }
                        app.FinalFactorCount.Add(rowfactor);
                    }
                    for (int i = 0; i < app.FactorName.GetLength(0); i++)
                    {
                        app.FinalFactorName.Add(app.FactorName[i]);
                    }
                    for (int i = 0; i < app.FactorName.GetLength(0); i++)
                    {
                        for (int j = i + 1; j < app.FactorName.GetLength(0); j++)
                        {
                            app.FinalFactorName.Add(app.FactorName[i] + "&" + app.FactorName[j]);
                        }
                    }
                }
                else
                {
                    app.FinalFactorCount = new List<List<double>>();
                    app.FinalFactorName = new List<string>();
                
                    for (int i = 0; i < app.EFMatrix.GetLength(0); i++)
                    {
                        List<double> rowfactor = new List<double>();
                        for (int j = 0; j < app.EFMatrix.GetLength(1); j++)
                        {
                            rowfactor.Add(app.EFMatrix[i, j]);
                        }
                        app.FinalFactorCount.Add(rowfactor);
                    }
                    for (int i = 0; i < app.FactorName.GetLength(0); i++)
                    {
                        app.FinalFactorName.Add(app.FactorName[i]);
                    }
                }
                
                
                double[][] Factortemp = new double[app.FinalFactorCount.Count][];
                for (int i = 0; i < app.FinalFactorCount.Count; i++)
                {
                    Factortemp[i] = new double[app.FinalFactorCount[i].Count];
                    for (int j = 0; j < app.FinalFactorCount[i].Count; j++)
                    {
                        Factortemp[i][j] = app.FinalFactorCount[i][j];
                    }
                }
                double[][] TFactorCount = app.TMatrix(Factortemp);
                for (int i = 0; i < TFactorCount.GetLength(0); i++)
                {
                    double mean = app.MEAN(TFactorCount[i]);
                    double sd = app.sd(TFactorCount[i]);
                    for (int j = 0; j < TFactorCount[i].GetLength(0); j++)
                    {
                        TFactorCount[i][j] = (TFactorCount[i][j] - mean) / sd;
                    }
                }
                double[][] Factor = app.TMatrix(TFactorCount);
                
                
                int n = Factor.GetLength(0);
                for (int i = 1; i < n; i++)
                {
                    EF.SetSymbol("alpha", EF.CreateNumeric(alpha));
                    EF.SetSymbol("df1value", EF.CreateNumeric(1));
                    EF.SetSymbol("df2value", EF.CreateNumeric(i));
                    EF.Evaluate("Fvalue <- qf(1 - alpha ,df1 = df1value ,df2 = df2value)");
                    app.Fvalue.Add(EF.GetSymbol("Fvalue").AsNumeric().First());
                }
                
                
                for (int l = 0; l < app.FeaName.GetLength(0); l++)
                {
                    double[] Ynor = new double[app.SamName.GetLength(0)];
                    for (int i = 0; i < Ynor.GetLength(0); i++)
                    {
                        Ynor[i] = app.CountMatrix[l, i];
                    }
                    double mean = app.MEAN(Ynor);
                    double sd = app.sd(Ynor);
                    for (int i = 0; i < Ynor.GetLength(0); i++)
                    {
                        Ynor[i] = (Ynor[i] - mean) / sd;
                    }
                
                    double[][] Y = new double[app.SamName.GetLength(0)][];
                    for (int i = 0; i < app.SamName.GetLength(0); i++)
                    {
                        Y[i] = new double[1];
                        Y[i][0] = Ynor[i];
                    }
                
                    List<int> idNum = new List<int>();
                    List<double> rowP = new List<double>();
                    List<double> rowR = new List<double>();
                    double[][] whole = new double[Factor.GetLength(0)][];
                    for (int i = 0; i < whole.GetLength(0); i++)
                    {
                        whole[i] = new double[Factor[i].GetLength(0) + 1];
                        for (int j = 0; j < Factor[i].GetLength(0); j++)
                        {
                            whole[i][j] = Factor[i][j];
                        }
                        whole[i][Factor[i].GetLength(0)] = Y[i][0];
                    }
                    double[][] Rmatrix = new double[Factor[0].GetLength(0) + 1][];
                    for (int i = 0; i < Rmatrix.GetLength(0); i++)
                    {
                        Rmatrix[i] = new double[Rmatrix.GetLength(0)];
                    }
                
                    Rmatrix = app.Cormatrix(whole);
                
                    while (app.Add_Factors(Factor, Y, alpha, Rmatrix, idNum) != 0)
                    {
                        int addindex = app.Add_Factors(Factor, Y, alpha, Rmatrix, idNum);
                        idNum.Add(addindex);
                        Rmatrix = app.Rconvert(Rmatrix, addindex);
                        if (app.Delete_Factors(Factor, Y, alpha, Rmatrix, idNum) != 0)
                        {
                            int deletindex = app.Delete_Factors(Factor, Y, alpha, Rmatrix, idNum);
                            for (int i = 0; i < idNum.Count; i++)
                            {
                                if (idNum[i] == deletindex)
                                    idNum.RemoveAt(i);
                            }
                            Rmatrix = app.Rconvert(Rmatrix, deletindex);
                            while (app.Check_Factors(Factor, Y, alpha, Rmatrix, idNum) != 0)
                            {
                                deletindex = app.Check_Factors(Factor, Y, alpha, Rmatrix, idNum);
                                for (int i = 0; i < idNum.Count; i++)
                                {
                                    if (idNum[i] == deletindex)
                                        idNum.RemoveAt(i);
                                }
                                Rmatrix = app.Rconvert(Rmatrix, deletindex);
                            }
                        }
                    }
                    int p = idNum.Count;
                
                    for (int i = 0; i < idNum.Count; i++)
                    {
                        double u = Math.Pow(Rmatrix[idNum[i] - 1][Rmatrix[0].GetLength(0) - 1], 2) / Rmatrix[idNum[i] - 1][idNum[i] - 1];
                        double f = u / (Rmatrix[Rmatrix.GetLength(0) - 1][Rmatrix[0].GetLength(0) - 1] / (n - p - 1));
                        EF.SetSymbol("Fvalue", EF.CreateNumeric(f));
                        EF.SetSymbol("df1value", EF.CreateNumeric(1));
                        EF.SetSymbol("df2value", EF.CreateNumeric(n - p - 1));
                        EF.Evaluate("pvalue <- 1 - pf(Fvalue , df1 = df1value , df2 = df2value)");
                        rowP.Add(EF.GetSymbol("pvalue").AsNumeric().First());
                        double[] x = new double[n];
                        double[] y = new double[n];
                        double[] xy = new double[n];
                        double[] xx = new double[n];
                        double[] yy = new double[n];
                        for (int k = 0; k < n; k++)
                        {
                            x[k] = app.FinalFactorCount[k][idNum[i] - 1];
                            y[k] = app.CountMatrix[l,k];
                            xx[k] = app.FinalFactorCount[k][idNum[i] - 1] * app.FinalFactorCount[k][idNum[i] - 1];
                            yy[k] = app.CountMatrix[l,k] * app.CountMatrix[l,k];
                            xy[k] = app.FinalFactorCount[k][idNum[i] - 1] * app.CountMatrix[l,k];
                        }
                        double R = (app.MEAN(xy) - app.MEAN(x) * app.MEAN(y)) / Math.Sqrt((app.MEAN(xx) - app.MEAN(x) * app.MEAN(x)) * (app.MEAN(yy) - app.MEAN(y) * app.MEAN(y)));
                        double R_square = R * R;
                        rowR.Add(R_square);
                
                    }
                    app.Allid.Add(idNum);
                    app.pvalue.Add(rowP);
                    app.Rvalue.Add(rowR);
                
                }
                app.B = new double[app.FeaName.GetLength(0)][][];
                for (int i = 0; i < app.FeaName.GetLength(0); i++)
                {
                    app.B[i] = new double[app.FinalFactorName.Count + 1][];
                    for (int j = 0; j < app.FinalFactorName.Count + 1; j++)
                    {
                        app.B[i][j] = new double[1];
                    }
                }
                app.T = new double[app.FeaName.GetLength(0)][];
                for (int i = 0; i < app.FeaName.GetLength(0); i++)
                {
                    app.T[i] = new double[app.FinalFactorName.Count + 1];
                }
                app.P = new double[app.FeaName.GetLength(0)];
                app.R_square = new double[app.FeaName.GetLength(0)];
                
                
                for (int j = 0; j < app.FeaName.GetLength(0); j++)
                {
                    if (app.Allid[j].Count != 0)
                    {
                        double[][] X = new double[app.FinalFactorCount.Count][];
                        for (int i = 0; i < X.Length; i++)
                        {
                            X[i] = new double[app.Allid[j].Count + 1];
                            X[i][0] = 1;
                            for (int k = 1; k <= app.Allid[j].Count; k++)
                            {
                                X[i][k] = app.FinalFactorCount[i][app.Allid[j][k - 1] - 1];
                            }
                        }
                        double[][] Y = new double[app.SamName.GetLength(0)][];
                        for (int i = 0; i < app.SamName.GetLength(0); i++)
                        {
                            Y[i] = new double[1];
                            Y[i][0] = app.CountMatrix[j,i];
                        }
                        double[][] B = new double[app.FinalFactorName.Count + 1][];
                        for (int i = 0; i < app.FinalFactorName.Count + 1; i++)
                        {
                            B[i] = new double[1];
                        }
                        double[][] C = new double[app.FinalFactorName.Count + 1][];
                        for (int i = 0; i < app.FinalFactorName.Count + 1; i++)
                        {
                            C[i] = new double[app.FinalFactorName.Count + 1];
                        }
                        double[][] E = new double[app.SamName.GetLength(0)][];
                        for (int i = 0; i < app.SamName.GetLength(0); i++)
                        {
                            E[i] = new double[1];
                        }
                        C = app.InverseMatrix(app.MultipleMatrix(app.TMatrix(X), X));
                        B = app.MultipleMatrix(app.MultipleMatrix(C, app.TMatrix(X)), Y);
                        app.B[j] = B;
                        E = app.MinusMatrix(Y, app.MultipleMatrix(X, B));
                        double sigma = 0;
                        for (int i = 0; i < app.SamName.GetLength(0); i++)
                        {
                            sigma = sigma + Math.Pow(E[i][0], 2);
                        }
                        sigma = Math.Sqrt(sigma / (app.SamName.GetLength(0) - app.FactorName.GetLength(0) - 1));
                
                        for (int i = 0; i < app.Allid[j].Count + 1; i++)
                        {
                            app.T[j][i] = Math.Abs(B[i][0] / (sigma * Math.Sqrt(C[i][i])));
                        }
                        double y_mean = 0;
                        double[][] y_predict = new double[app.SamName.GetLength(0)][];
                        for (int i = 0; i < app.SamName.GetLength(0); i++)
                        {
                            y_predict[i] = new double[1];
                        }
                        for (int i = 0; i < app.SamName.GetLength(0); i++)
                        {
                            y_mean = y_mean + Y[i][0];
                        }
                        y_mean = y_mean / (app.SamName.GetLength(0));
                
                        y_predict = app.MultipleMatrix(X, B);
                
                        double SSR = 0;
                        double SSE = 0;
                        for (int i = 0; i < app.SamName.GetLength(0); i++)
                        {
                            SSR = SSR + Math.Pow((y_predict[i][0] - y_mean), 2);
                        }
                        for (int i = 0; i < app.SamName.GetLength(0); i++)
                        {
                            SSE = SSE + Math.Pow((Y[i][0] - y_predict[i][0]), 2);
                        }
                        double F = (SSR / (app.FinalFactorName.Count)) / (SSE / (app.SamName.GetLength(0) - app.FinalFactorName.Count - 1));
                        int p = app.Allid[j].Count;
                        EF.SetSymbol("Fvalue", EF.CreateNumeric(F));
                        EF.SetSymbol("df1value", EF.CreateNumeric(p));
                        EF.SetSymbol("df2value", EF.CreateNumeric(n - p - 1));
                        EF.Evaluate("pvalue_whole <- 1 - pf(Fvalue , df1 = df1value , df2 = df2value)");
                        app.P[j] = EF.GetSymbol("pvalue_whole").AsNumeric().First();
                        double SST = SSR + SSE;
                        app.R_square[j] = SSR / SST;
                        if (app.P[j] >= alpha)
                        {
                            app.P[j] = 2;
                            app.R_square[j] = 2;
                        }
                    }
                    else
                    {
                        app.P[j] = 2;
                        app.R_square[j] = 2;
                    }
                }
                app.EFAcheck = this.checkBox2.Checked;
                app.EFAradio1 = this.radioButton1.Checked;
                app.EFAradio2 = this.radioButton2.Checked;

                Environment_Output f14 = new Environment_Output();
                f14.MdiParent = this.MdiParent;
                f14.Show();
                this.Close();
            }
        }

        private void Form10_Load(object sender, EventArgs e)
        {
            this.checkBox1.Checked = true; 
            this.radioButton1.Enabled = false;
            this.radioButton2.Enabled = false;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox2.Checked)
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
