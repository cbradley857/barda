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
    public partial class DrumPlots : Form
    {
        string[] plotLocations;

        public DrumPlots()
        {
            InitializeComponent();

            plotLocations = new String[] {
                "http://ilikeducks.com/LampSim/Historical/drumplots/fissure.png", //Near Fissure
                "http://ilikeducks.com/LampSim/Historical/drumplots/caldera.png", //Near Caldera
                "http://ilikeducks.com/LampSim/Historical/drumplots/askja.png"  //Near Askja
            };



        }

       
        private void DrumPlots_Load(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            webControl1.Source = new Uri(plotLocations[0]);
            toolStripStatusLabel1.Text = "Loading..";
        }



        public void changeGraph(int index)
        {
            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            webControl1.Source = new Uri(plotLocations[0]);
            toolStripStatusLabel1.Text = "Loading..";
        }

        private void radioButton2_checkChanged(object sender, EventArgs e)
        {
            webControl1.Source = new Uri(plotLocations[1]);
            toolStripStatusLabel1.Text = "Loading..";
        }

        private void radioButton3_checkChanged(object sender, EventArgs e)
        {
            webControl1.Source = new Uri(plotLocations[2]);
            toolStripStatusLabel1.Text = "Loading..";
        }

        private void pageLoaded(object sender, Awesomium.Core.UrlEventArgs e)
        {
            toolStripStatusLabel1.Text = "Complete";
        }

        private void targetChanged(object sender, Awesomium.Core.UrlEventArgs e)
        {
            //toolStripStatusLabel1.Text = "Loading..";
        }

        private void addressChanged(object sender, Awesomium.Core.UrlEventArgs e)
        {
        }

        public void refreshCharts()
        {
            if (radioButton1.Checked)
            {
                webControl1.Source = new Uri(plotLocations[0]);
                toolStripStatusLabel1.Text = "Loading..";
            }
            else if (radioButton2.Checked)
            {
                webControl1.Source = new Uri(plotLocations[1]);
                toolStripStatusLabel1.Text = "Loading..";
            }
            else if(radioButton3.Checked)
            {
                webControl1.Source = new Uri(plotLocations[2]);
                toolStripStatusLabel1.Text = "Loading..";
            }
        }
    }
}
