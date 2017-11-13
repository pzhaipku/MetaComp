using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace MetaComp
{
    public partial class HMMER_Input : Form
    {
        public HMMER_Input()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
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

        private void button3_Click(object sender, EventArgs e)
        {
            app.Profile = null;
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] filePath = null;
            filePath = this.textBox1.Text.Split(',');
            for (int i = 0; i < filePath.Length - 1; i++)
            {
                FileStream fs = new FileStream(filePath[i], System.IO.FileMode.Open, System.IO.FileAccess.Read);               
                StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                string strLine = "";
                string[] aryLine = null;
                ArrayList HMMinfo = new ArrayList();
                bool accession = false;
                while ((strLine = sr.ReadLine()) != null)
                {                   
                    aryLine = strLine.Split(' ');
                    
                    if (string.Equals(aryLine[0], "Query:"))
                    {
                        aryLine = aryLine.Where(s => !string.IsNullOrEmpty(s)).ToArray();
                        HMMinfo.Add(aryLine[1].ToString());
                    }
                    else if (string.Equals(aryLine[0], "Accession:"))
                    {
                        accession = true;
                        aryLine = aryLine.Where(s => !string.IsNullOrEmpty(s)).ToArray();
                        HMMinfo.Add(aryLine[1].ToString());
                    }
                    else if ( aryLine.Length > 5 )
                    {
                        if(string.Equals(aryLine[4], "(domZ):"))
                        {
                            aryLine = aryLine.Where(s => !string.IsNullOrEmpty(s)).ToArray(); 
                            HMMinfo.Add(int.Parse(aryLine[4]).ToString());
                        }
                    }
                }

                sr.Close();
                fs.Close();
                int QueryNum;
                DataTable dt = new DataTable();
                if (accession)
                {
                    dt.Columns.Add("Accession", typeof(string));
                    QueryNum = HMMinfo.Count / 3;
                }
                else
                {
                    dt.Columns.Add("Query", typeof(string));
                    QueryNum = HMMinfo.Count / 2;
                }
                dt.Columns.Add("Number", typeof(Int32));

                for (int j = 0; j < QueryNum; j++ )
                {
                    DataRow dr = dt.NewRow();
                    if (accession)
                    {
                        dr[0] = HMMinfo[3 * j + 1].ToString();
                        dr[1] = int.Parse(HMMinfo[3 * j + 2].ToString());
                    }
                    else
                    {
                        dr[0] = HMMinfo[2 * j].ToString();
                        dr[1] = int.Parse(HMMinfo[2 * j + 1].ToString());
                    }
                    dt.Rows.Add(dr);
                }
                if (app.Profile == null)
                {
                    app.Profile = new DataTable();
                    app.Profile.Columns.Add("Feature", typeof(string));
                    app.Profile.Columns.Add("File1", typeof(int));
                    for (int m = 0; m < dt.Rows.Count; m++)
                    {
                        app.Profile.Rows.Add(dt.Rows[m][0], dt.Rows[m][1]);
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
                        bool k = true; ;
                        for (int j = 0; j < app.Profile.Rows.Count; j++)
                        {
                            if (string.Equals(dt.Rows[m][0].ToString(), app.Profile.Rows[j][0].ToString()))
                            {
                                app.Profile.Rows[j][FileNum] = int.Parse(dt.Rows[m][1].ToString());
                                k = false;
                                break;
                            }
                        }
                        if (k)
                        {
                            DataRow drtemp;
                            drtemp = app.Profile.NewRow();
                            drtemp[0] = dt.Rows[m][0].ToString();
                            for (int n = 1; n < FileNum; n++)
                            {
                                drtemp[n] = 0;
                            }
                            drtemp[FileNum] = int.Parse(dt.Rows[m][1].ToString());
                            app.Profile.Rows.Add(drtemp);
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

        private void Form18_Load(object sender, EventArgs e)
        {

        }
    }
}
