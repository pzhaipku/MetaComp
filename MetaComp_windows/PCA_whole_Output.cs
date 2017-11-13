using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using ZedGraph.Web;

namespace MetaComp
{
    public partial class PCA_whole_Output : Form
    {
        public PCA_whole_Output()
        {
            InitializeComponent();
        } 
        
        private void Form13_Load(object sender, EventArgs e)
        {
            List<PointPairList> list = new List<PointPairList>();
            this.zedGraphControl1.GraphPane.Title.Text = "PCA"; 
            this.zedGraphControl1.GraphPane.XAxis.Title.Text = "PC1";
            this.zedGraphControl1.GraphPane.YAxis.Title.Text = "PC2";
            
            for (int i = 0; i < app.clusterNum; i++)
            {
                PointPairList temp = new PointPairList();
                for (int j = 0; j < app.cluster.Length; j++)
                {
                    
                    if (app.cluster[j] == i+1)
                    {
                        double x = app.Score[j,0];
                        double y = app.Score[j,1];
                        temp.Add(x, y); 
                    }                    
                }
                list.Add(temp);
            }
            for(int i = 0; i < app.clusterNum; i++)
            {
                switch(i)
                {
                    case 0:
                        LineItem myCurve1;
                        myCurve1 = zedGraphControl1.GraphPane.AddCurve("Cluster1", list[i], Color.Black, SymbolType.Circle);
                        myCurve1.Line.IsVisible = false;

                        break;
                    case 1:
                        LineItem myCurve2;
                        myCurve2 = zedGraphControl1.GraphPane.AddCurve("Cluster2", list[i], Color.Peru, SymbolType.Plus);
                        myCurve2.Line.IsVisible = false;

                        break;
                    case 2:
                        LineItem myCurve3;
                        myCurve3 = zedGraphControl1.GraphPane.AddCurve("Cluster3", list[i], Color.Orange, SymbolType.Diamond);
                        myCurve3.Line.IsVisible = false;

                        break;
                    case 3:
                        LineItem myCurve4;
                        myCurve4 = zedGraphControl1.GraphPane.AddCurve("Cluster4", list[i], Color.GreenYellow, SymbolType.Square);
                        myCurve4.Line.IsVisible = false;

                        break;
                    case 4:
                        LineItem myCurve5;
                        myCurve5 = zedGraphControl1.GraphPane.AddCurve("Cluster5", list[i], Color.DeepPink, SymbolType.Star);
                        myCurve5.Line.IsVisible = false;

                        break;
                    case 5:
                        LineItem myCurve6;
                        myCurve6 = zedGraphControl1.GraphPane.AddCurve("Cluster6", list[i], Color.Green, SymbolType.Triangle);
                        myCurve6.Line.IsVisible = false;

                        break;
                    case 6:
                        LineItem myCurve7;
                        myCurve7 = zedGraphControl1.GraphPane.AddCurve("Cluster7", list[i], Color.Blue, SymbolType.TriangleDown);
                        myCurve7.Line.IsVisible = false;

                        break;
                    case 7:
                        LineItem myCurve8;
                        myCurve8 = zedGraphControl1.GraphPane.AddCurve("Cluster8", list[i], Color.Purple, SymbolType.VDash);
                        myCurve8.Line.IsVisible = false;

                        break;
                    case 8:
                        LineItem myCurve9;
                        myCurve9 = zedGraphControl1.GraphPane.AddCurve("Cluster9", list[i], Color.Red, SymbolType.XCross);
                        myCurve9.Line.IsVisible = false;

                        break;
                    case 9:
                        LineItem myCurve10;
                        myCurve10 = zedGraphControl1.GraphPane.AddCurve("Cluster10", list[i], Color.Yellow, SymbolType.HDash);
                        myCurve10.Line.IsVisible = false;

                        break;
                    default:
                        break;
                }
            } 
            this.zedGraphControl1.AxisChange(); 
            this.zedGraphControl1.Refresh(); 
        }

        private void zedGraphControl1_Load(object sender, EventArgs e)
        {

        }
    }
}
