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
    public partial class MG_Input : Form
    {
        public MG_Input()
        {
            InitializeComponent();
        }

        private void Form19_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string filePath = this.textBox1.Text;
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
            app.Profile = dt;

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

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.openFileDialog1.ShowDialog();
            this.textBox1.Text = this.openFileDialog1.FileName;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
