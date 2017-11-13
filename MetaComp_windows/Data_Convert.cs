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
    public partial class Data_Convert : Form
    {
        public Data_Convert()
        {
            InitializeComponent();
        }

        private void Form15_Load(object sender, EventArgs e)
        {
            this.radioButton1.Checked = true;
            this.radioButton2.Checked = false;
            this.radioButton3.Checked = false;
            this.radioButton4.Checked = false;
            this.radioButton5.Checked = false;
            this.radioButton6.Checked = false;
            this.radioButton7.Checked = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.radioButton1.Checked)
            {
                BLAST_Input BLAST = new BLAST_Input();
                BLAST.MdiParent = this.MdiParent;
                BLAST.Show();
                this.Close();
            }
            else if (this.radioButton2.Checked)
            {
                Kraken_Input Kraken = new Kraken_Input();
                Kraken.MdiParent = this.MdiParent;
                Kraken.Show();
                this.Close();
            }
            else if (this.radioButton3.Checked)
            {
                HMMER_Input HMMER = new HMMER_Input();
                HMMER.MdiParent = this.MdiParent;
                HMMER.Show();
                this.Close();
            }
            else if (this.radioButton4.Checked)
            {
                MG_Input MG = new MG_Input();
                MG.MdiParent = this.MdiParent;
                MG.Show();
                this.Close();
            }
            else if (this.radioButton5.Checked)
            {
                MZmine_Input MZmine = new MZmine_Input();
                MZmine.MdiParent = this.MdiParent;
                MZmine.Show();
                this.Close();
            }
            else if (this.radioButton6.Checked)
            {
                PhymmBL_Input PhymmBL = new PhymmBL_Input();
                PhymmBL.MdiParent = this.MdiParent;
                PhymmBL.Show();
                this.Close();
            }
            else if (this.radioButton7.Checked)
            {
                APM_Input APM = new APM_Input();
                APM.MdiParent = this.MdiParent;
                APM.Show();
                this.Close();
            }
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
