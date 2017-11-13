using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MetaComp
{
    public partial class HCluster_Output : Form
    {
        public HCluster_Output()
        {
            InitializeComponent();
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            int FeatureNum = app.CountMatrix.GetLength(0);
            int SampleNum = app.CountMatrix.GetLength(1);
            
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            
            listView1.View = View.Details;
            listView1.Scrollable = true;
            listView1.MultiSelect = false;

            listView1.Columns.Add("", 10, HorizontalAlignment.Center);
            for (int i = 0; i < app.clusterNum; i++)
            {
                listView1.Columns.Add("Cluster" + (i + 1).ToString(), 160, HorizontalAlignment.Center);
            }
            
            for (int i = 0; i < SampleNum ; i++)
            {
                ListViewItem item = new ListViewItem();
                item.SubItems.Clear();

                item.SubItems[0].Text = null;
                for (int j = 0; j < app.clusterNum; j++)
                {
                    if (app.ClusterResult[j, i] > 0)
                        item.SubItems.Add(app.SamName[app.ClusterResult[j, i] - 1]);
                    else
                        item.SubItems.Add("");
                }                   
                listView1.Items.Add(item);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
