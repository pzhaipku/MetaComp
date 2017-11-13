using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Collections;

namespace MetaComp
{
    public partial class BLAST_Input : Form
    {
        public BLAST_Input()
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

        private void button4_Click(object sender, EventArgs e)
        {
            app.Profile = null;
            this.Dispose();
        }

        private void Form16_Load(object sender, EventArgs e)
        {
            this.radioButton1.Enabled = false;
            this.radioButton2.Enabled = false;
            this.radioButton1.Checked = false;
            this.radioButton2.Checked = false;
            

        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            
            string[] filePath = null;
            filePath = this.textBox1.Text.Split(',');
            for (int i = 0; i < filePath.Length - 1; i++)
            {
                FileStream fs = new FileStream(filePath[i], System.IO.FileMode.Open, System.IO.FileAccess.Read);
                StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                string strLine = "";
                string[] aryLine = null;

                DataTable dt = new DataTable();
                dt.Columns.Add("Subject ID", typeof(string));
                dt.Columns.Add("Hit Num", typeof(int));
                if (this.radioButton1.Checked)
                {
                    for (int j = 0; j < 5665; j++)
                    { 
                        DataRow dr = dt.NewRow();
                        dr["Subject ID"] = "COG" + j.ToString("0000");
                        dr["Hit Num"] = 0;
                        dt.Rows.Add(dr);
                    }
                    
                }
                else if (this.radioButton2.Checked)
                {
                    for (int j = 0; j < 20519; j++)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Subject ID"] = "KO" + j.ToString("00000");
                        dr["Hit Num"] = 0;
                        dt.Rows.Add(dr);
                    }
                }
                bool Newquery = true;
                
                while ((strLine = sr.ReadLine()) != null)
                {
                    aryLine = strLine.Split('\t');

                    if (aryLine.Length > 1)
                    {

                        if (Newquery)
                        {
                            if (this.radioButton1.Checked)
                            {
                                string COGpath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath + @"./database/COG.csv");
                                FileStream COGfs = new FileStream(COGpath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                                StreamReader COGsr = new StreamReader(COGfs, Encoding.UTF8);
                                string COGstrLine = "";
                                string[] COGaryLine = null;
                                app.COGdatabase = new DataTable();
                                app.COGdatabase.Columns.Add("ID", typeof(string));
                                app.COGdatabase.Columns.Add("COG", typeof(string));
                                while ((COGstrLine = COGsr.ReadLine()) != null)
                                {
                                    COGaryLine = COGstrLine.Split(',');
                                    DataRow dr = app.COGdatabase.NewRow();
                                    dr[0] = COGaryLine[0].Replace("\"", "");
                                    dr[1] = COGaryLine[6].Replace("\"", "");
                                    app.COGdatabase.Rows.Add(dr);
                                }
                                int cogNum = app.COGdatabase.Rows.Count;
                                int subject_ID = Convert.ToInt32( aryLine[1].Split('|')[1].Replace("\"", ""));
                                for (int j = 0; j < cogNum; j++)
                                {
                                    if (subject_ID == Convert.ToInt32( app.COGdatabase.Rows[j][0].ToString()))
                                    {
                                        for (int k = 0; k < 5665; k++)
                                        {
                                            if (string.Equals(dt.Rows[k][0], app.COGdatabase.Rows[j][1]))
                                            {
                                                dt.Rows[k][1] = Convert.ToInt32(dt.Rows[k][1]) + 1;
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                    
                                }
                            }
                            else if (this.radioButton2.Checked)
                            {
                                string KEGGpath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath + @"./database/ko_genes.list");
                                FileStream KEGGfs = new FileStream(KEGGpath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                                StreamReader KEGGsr = new StreamReader(KEGGfs, Encoding.UTF8);
                                string KEGGstrLine = "";
                                string[] KEGGaryLine = null;
                                app.KEGGdatabase = new DataTable();
                                app.KEGGdatabase.Columns.Add("ID", typeof(string));
                                app.KEGGdatabase.Columns.Add("KEGG", typeof(string));
                                while ((KEGGstrLine = KEGGsr.ReadLine()) != null)
                                {
                                    KEGGaryLine = KEGGstrLine.Split('\t');
                                    DataRow dr = app.KEGGdatabase.NewRow();
                                    dr[0] = KEGGaryLine[1].Replace("\"", "");
                                    dr[1] = KEGGaryLine[0].Replace("\"", "");
                                    app.KEGGdatabase.Rows.Add(dr);
                                }
                                int keggNum = app.KEGGdatabase.Rows.Count;
                                string subject_ID = aryLine[1].Replace("\"", "");

                                for (int j = 0; j < keggNum; j++)
                                {
                                    if (string.Equals(subject_ID, app.KEGGdatabase.Rows[j][0].ToString()))
                                    {
                                        for (int k = 0; k < 20519; k++)
                                        {
                                            if (string.Equals(dt.Rows[k][0], app.KEGGdatabase.Rows[j][0]))
                                            {
                                                dt.Rows[k][1] = Convert.ToInt32(dt.Rows[k][1]) + 1;
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                string subject_ID = aryLine[1];
                                if (dt.Rows.Count == 0)
                                {
                                    DataRow dr = dt.NewRow();
                                    dr["Subject ID"] = subject_ID;
                                    dr["Hit Num"] = 1;
                                    dt.Rows.Add(dr);
                                }
                                else
                                { 
                                    bool newFea = true;
                                    for (int j = 0; j < dt.Rows.Count; j++)
                                    {
                                        
                                        if (string.Equals(subject_ID, dt.Rows[j][0]))
                                        {
                                            dt.Rows[j][1] = Convert.ToInt32(dt.Rows[j][1]) + 1;
                                            newFea = false;
                                            break;
                                        }
                                       
                                    }
                                    if (newFea)
                                    {
                                        DataRow dr = dt.NewRow();
                                        dr["Subject ID"] = subject_ID;
                                        dr["Hit Num"] = 1;
                                        dt.Rows.Add(dr);
                                    }
                                
                                } 
                            }
                            Newquery = false;
                        }
                    }
                    else
                    {
                        Newquery = true;
                    }
                }

                sr.Close();
                fs.Close();
                if (app.Profile == null)
                {
                    app.Profile = new DataTable();
                    app.Profile.Columns.Add("Feature", typeof(string));
                    app.Profile.Columns.Add("File1", typeof(int));
                    for (int m = 0; m < dt.Rows.Count; m++)
                    {
                        app.Profile.Rows.Add(dt.Rows[m][0].ToString(), Convert.ToInt32(dt.Rows[m][1]));
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

            for(int i = 0; i < FeatureNum; i++)
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
