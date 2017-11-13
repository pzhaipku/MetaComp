using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace MetaComp
{
    public partial class Kraken_Input : Form
    {
        public Kraken_Input()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string fileNames = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
                {
                    fileNames += openFileDialog1.FileNames.GetValue(i).ToString() + ',';
                }
                this.textBox1.Text = fileNames;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            app.Profile = null;
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] filePath = null;
            filePath = this.textBox1.Text.Split(',');
            for (int i = 0; i < filePath.Length - 1; i++)
            { 
                FileStream fs = new FileStream(filePath[i], System.IO.FileMode.Open, System.IO.FileAccess.Read);
                DataTable dt = new DataTable();
                dt.Columns.Add("Query", typeof(string));
                dt.Columns.Add("Result", typeof(string));
                StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                string strLine = "";
                string[] aryLine = null;

                while ((strLine = sr.ReadLine()) != null)
                {
                    aryLine = strLine.Split('\t');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < aryLine.Length; j++)
                    {
                        dr[j] = aryLine[j].Replace("\"", "");
                    }
                    dt.Rows.Add(dr);
                }

                sr.Close();
                fs.Close();
                if (app.Profile == null)
                {
                    app.Profile = new DataTable();
                    app.Profile.Columns.Add("Feature", typeof(string));
                    app.Profile.Columns.Add("File1", typeof(int));
                    DataRow drfirst;
                    drfirst = app.Profile.NewRow();
                    app.Profile.Rows.Add(dt.Rows[0][1].ToString(), 1);
                    for (int m = 1; m < dt.Rows.Count; m++)
                    {
                        int k = 0;
                        for (int j = 0; j < app.Profile.Rows.Count; j++)
                        {
                            if (string.Equals(dt.Rows[m][1].ToString(), app.Profile.Rows[j][0].ToString()))
                            {
                                app.Profile.Rows[j][1] = (int)app.Profile.Rows[j][1] + 1;
                                k = 1;
                                break;
                            }
                        }
                        if (k == 0)
                        {
                            app.Profile.Rows.Add(dt.Rows[m][1].ToString(), 1);
                        }
                    }

                }
                else
                {
                    int FileNum = app.Profile.Columns.Count;
                    string newfilename = "File" + FileNum.ToString();
                    DataColumn newfile = new DataColumn(newfilename, typeof(int));
                    newfile.DefaultValue = 0;
                    app.Profile.Columns.Add(newfile);
                    for (int m = 0; m < dt.Rows.Count; m++)
                    {
                        int k = 0;
                        for (int j = 0; j < app.Profile.Rows.Count; j++)
                        {
                            if (string.Equals(dt.Rows[m][1].ToString(), app.Profile.Rows[j][0].ToString()))
                            {
                                app.Profile.Rows[j][FileNum] = (int)app.Profile.Rows[j][FileNum] + 1;
                                k = 1;
                                break;
                            }
                        }
                        if (k == 0)
                        {
                            DataRow dr;
                            dr = app.Profile.NewRow();
                            dr[0] = dt.Rows[m][1].ToString();
                            for (int n = 1; n < FileNum; n++)
                            {
                                dr[n] = 0;
                            }
                            dr[FileNum] = 1;
                            app.Profile.Rows.Add(dr);
                        }
                    }
                }
            }
            app.FeaName = new string[app.Profile.Rows.Count];
            for (int i = 0; i < app.Profile.Rows.Count; i++)
            {
                app.FeaName[i] = app.Profile.Rows[i][0].ToString();
            }
            app.SamName = new string[app.Profile.Columns.Count - 1];
            for (int i = 0; i < app.Profile.Columns.Count - 1; i++)
            {
                app.SamName[i] = app.Profile.Columns[i + 1].ColumnName;
            }

            int FeatureNum = app.FeaName.GetLength(0);
            int SampleNum = app.SamName.GetLength(0);

            app.CountMatrix = new double[FeatureNum, SampleNum];

            for (int i = 0; i < FeatureNum; i++)
            {
                for (int j = 0; j < SampleNum; j++)
                {
                    app.CountMatrix[i, j] = Convert.ToDouble(app.Profile.Rows[i][j + 1]);
                }
            }

            app.SampleTotal = new double[SampleNum];
            for (int i = 0; i < SampleNum; i++)
            {
                for (int j = 0; j < FeatureNum; j++)
                {
                    app.SampleTotal[i] = app.SampleTotal[i] + Convert.ToDouble(app.Profile.Rows[j][i + 1]);
                }
            }

            app.FreqMatrix = new double[FeatureNum, SampleNum];
            for (int i = 0; i < FeatureNum; i++)
            {
                for (int j = 0; j < SampleNum; j++)
                {
                    app.FreqMatrix[i, j] = app.CountMatrix[i, j] / app.SampleTotal[j];
                }
            }

            Data_Output loaddata = new Data_Output();
            loaddata.MdiParent = this.MdiParent;
            loaddata.Show();
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form17_Load(object sender, EventArgs e)
        {

        }
    }
}
