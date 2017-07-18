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

namespace MetaComp
{
    public partial class Environment_Output : Form
    {
        public Environment_Output()
        {
            InitializeComponent();
        }

        private void Form14_Load(object sender, EventArgs e)
        {
            string[] FactorName_p = new string[app.FinalFactorName.Count];
            string[] FactorName_R = new string[app.FinalFactorName.Count];
            for (int i = 0; i < FactorName_p.GetLength(0); i++)
            {
                FactorName_p[i] = app.FinalFactorName[i] + "pvalue";
                FactorName_R[i] = app.FinalFactorName[i] + "R";
            }

            string[] FeatureName = (string[])app.FeaName;
            string[] SampleName = (string[])app.SamName;

            int FeatureNum = FeatureName.GetLength(0);
            int SampleNum = SampleName.GetLength(0);
            List<string> Annotation = new List<string>();

            if (app.EFAcheck)
            {

                if (app.EFAradio1)
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
                else if (app.EFAradio2)
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

            DataTable dttemp = new DataTable();

            dttemp.Columns.Add("Feature", typeof(string));
            for (int i = 0; i < app.FinalFactorName.Count; i++)
            {
                dttemp.Columns.Add(app.FinalFactorName[i], typeof(double));
                dttemp.Columns.Add(FactorName_p[i], typeof(double));
                dttemp.Columns.Add(FactorName_R[i], typeof(double));
            }

            dttemp.Columns.Add("Equation_p", typeof(double));
            dttemp.Columns.Add("R square", typeof(double));
            dttemp.Columns.Add("Annotation", typeof(string));

            for (int i = 0; i < app.FeaName.GetLength(0); i++)
            {
                bool condition = true;
                condition = condition && (app.P[i] < app.alphaen);
                for (int j = 0; j < app.Allid[i].Count; j++)
                {
                    condition = condition && (app.pvalue[i][j] < app.alphaen);
                }
                if (condition)
                {
                    DataRow dr = dttemp.NewRow();
                    dr[0] = app.FeaName[i];


                    for (int j = 0; j < app.Allid[i].Count; j++)
                    {
                        dr[3 * app.Allid[i][j] - 2] = app.B[i][j + 1][0];
                        dr[3 * app.Allid[i][j] - 1] = app.pvalue[i][j];
                        dr[3 * app.Allid[i][j]] = app.Rvalue[i][j];
                    }
                    dr[3 * app.FinalFactorName.Count + 1] = app.P[i];
                    dr[3 * app.FinalFactorName.Count + 2] = app.R_square[i];

                    if (app.EFAcheck)
                    {
                        dr[3 * app.FinalFactorName.Count + 3] = Annotation[i];
                    }
                    else
                    {
                        dr[3 * app.FinalFactorName.Count + 3] = null;
                    } 

                    dttemp.Rows.Add(dr);
                }
            }

            DataTable dtCopy = dttemp.Copy();
            DataView dv = dttemp.DefaultView;
            dv.Sort = "Equation_p";
            dtCopy = dv.ToTable();

            DataTable dt = new DataTable();

            dt.Columns.Add("Feature", typeof(string));
            dt.Columns.Add("value", typeof(string));
            for (int i = 0; i < app.FinalFactorName.Count; i++)
            {
                dt.Columns.Add(app.FinalFactorName[i], typeof(double));
            }

            dt.Columns.Add("Regression Equation", typeof(double));
            dt.Columns.Add("Annotation", typeof(string));

            for (int i = 0; i < dtCopy.Rows.Count; i++)
            {
                DataRow dr0 = dt.NewRow();
                DataRow dr1 = dt.NewRow();
                DataRow dr2 = dt.NewRow();
                dr0[0] = dtCopy.Rows[i][0];

                dr0[1] = "Coefficient";
                dr1[1] = "Pvalue";
                dr2[1] = "Correlation";
                for (int j = 2; j <= app.FinalFactorName.Count + 1; j++)
                {
                    dr0[j] = dtCopy.Rows[i][3 * j - 5];
                    dr1[j] = dtCopy.Rows[i][3 * j - 4];
                    dr2[j] = dtCopy.Rows[i][3 * j - 3];
                }
                dr1[app.FinalFactorName.Count + 2] = dtCopy.Rows[i][3 * app.FinalFactorName.Count + 1];
                dr2[app.FinalFactorName.Count + 2] = dtCopy.Rows[i][3 * app.FinalFactorName.Count + 2];
                dr0[app.FinalFactorName.Count + 3] = dtCopy.Rows[i][3 * app.FinalFactorName.Count + 3];
                dt.Rows.Add(dr0);
                dt.Rows.Add(dr1);
                dt.Rows.Add(dr2);
            }


            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];
            Microsoft.Office.Interop.Excel.Range range;
            long totalCount = dt.Rows.Count;
            long rowRead = 0;
            float percent = 0;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                worksheet.Cells[1, i + 1] = dt.Columns[i].ColumnName;
                range = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, i + 1];
                range.Interior.ColorIndex = 15;
                range.Font.Bold = true;
            }
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    worksheet.Cells[r + 2, i + 1] = dt.Rows[r][i].ToString();
                }
                rowRead++;
                percent = ((float)(100 * rowRead)) / totalCount;
            }
            xlApp.Visible = true;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;

            listView1.View = View.Details;
            listView1.Scrollable = true;
            listView1.MultiSelect = false;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                listView1.Columns.Add(dt.Columns[i].ColumnName, 160, HorizontalAlignment.Center);
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                item.SubItems.Clear();

                item.SubItems[0].Text = dt.Rows[i][0].ToString();
                for (int j = 1; j < dt.Columns.Count; j++)
                {
                    item.SubItems.Add(dt.Rows[i][j].ToString());
                }
                listView1.Items.Add(item);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
