using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bardarbunga
{
    public partial class TremorCharts : Form
    {
        Uri[] addresses;

        public TremorCharts()
        {
            InitializeComponent();

            addresses = new Uri[] {
                new Uri("http://ilikeducks.com/LampSim/Historical/tremor/caldera.gif"), // Near Bardarbunga Caldera
                new Uri("http://ilikeducks.com/LampSim/Historical/tremor/fissure.gif"), // Near Fissure
                new Uri("http://ilikeducks.com/LampSim/Historical/tremor/fissure_filtered.gif"), //Near Fissure (Median Filtered)
                new Uri("http://ilikeducks.com/LampSim/Historical/tremor/askja.gif"), //Near Askja
                new Uri("http://ilikeducks.com/LampSim/Historical/tremor/grimsfjall.gif"), //Grimsfjäll (Median Filtered)

            };

            toolStripStatusLabel1.Text = "Loading..";
            radioButton2.Enabled = true;
            webControl1.Source = addresses[1];
        }

        private void TremorCharts_Load(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            webControl1.Source = addresses[1];
            toolStripStatusLabel1.Text = "Loading..";
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            webControl1.Source = addresses[2];
            toolStripStatusLabel1.Text = "Loading..";
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            webControl1.Source = addresses[0];
            toolStripStatusLabel1.Text = "Loading..";
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            webControl1.Source = addresses[3];
            toolStripStatusLabel1.Text = "Loading..";
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            webControl1.Source = addresses[4];
            toolStripStatusLabel1.Text = "Loading..";
        }

        private void documentReady(object sender, Awesomium.Core.UrlEventArgs e)
        {
            toolStripStatusLabel1.Text = "Complete";
        }

        public void refreshCharts()
        {
            if(radioButton1.Checked)
            {
                webControl1.Source = addresses[0];
                toolStripStatusLabel1.Text = "Loading..";
            }
            else if(radioButton2.Checked)
            {
                webControl1.Source = addresses[1];
                toolStripStatusLabel1.Text = "Loading..";
            }
            else if(radioButton3.Checked)
            {
                webControl1.Source = addresses[2];
                toolStripStatusLabel1.Text = "Loading..";
            }
            else if(radioButton4.Checked)
            {
                webControl1.Source = addresses[3];
                toolStripStatusLabel1.Text = "Loading..";
            }
            else if(radioButton5.Checked)
            {
                webControl1.Source = addresses[4];
                toolStripStatusLabel1.Text = "Loading..";
            }

        }
    }
}
