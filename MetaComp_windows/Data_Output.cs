using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.Data.SqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;


namespace MetaComp
{
    public partial class Data_Output : Form
    {
        public Data_Output()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.GridLines = true;
            listView1.FullRowSelect = true;

            listView1.View = View.Details;
            listView1.Scrollable = true;
            listView1.MultiSelect = false;


            int SampleNum = app.Profile.Columns.Count - 1;
            int FeatureNum = app.Profile.Rows.Count;

            for (int i = 0; i < SampleNum + 1; i++)
                listView1.Columns.Add(app.Profile.Columns[i].ToString(), 160, HorizontalAlignment.Center);

            for (int i = 0; i < FeatureNum; i++)
            {
                ListViewItem item = new ListViewItem();
                item.SubItems.Clear();

                item.SubItems[0].Text = app.Profile.Rows[i][0].ToString();
                for (int j = 1; j < SampleNum + 1; j++)
                {
                    item.SubItems.Add(app.Profile.Rows[i][j].ToString());
                }
                listView1.Items.Add(item);
            }

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
    
}
