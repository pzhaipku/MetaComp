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
    public partial class Cluster_Center_Output : Form
    {
        public Cluster_Center_Output()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            
            

            int CenterNum = app.Center.GetLength(0);
            int FeatureNum = app.Center.GetLength(1);

            List<string> CenterName = new List<string>() ;

            for(int i = 1; i<= CenterNum ; i++ )
            {
                CenterName.Add("Center" + i.ToString());
            }

            listView1.GridLines = true;
            listView1.FullRowSelect = true;

            listView1.View = View.Details;
            listView1.Scrollable = true;
            listView1.MultiSelect = false;
  
            listView1.Columns.Add("", 160, HorizontalAlignment.Center);
            for (int i = 0; i < FeatureNum; i++)
                listView1.Columns.Add(app.FeaName[i], 160, HorizontalAlignment.Center);

            for (int i = 0; i < CenterNum; i++)
            {
                ListViewItem item = new ListViewItem();
                item.SubItems.Clear();

                item.SubItems[0].Text = CenterName[i];
                for (int j = 0; j < FeatureNum; j++)
                {
                    item.SubItems.Add(app.Center[i,j].ToString());
                }
                listView1.Items.Add(item);
            }

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
