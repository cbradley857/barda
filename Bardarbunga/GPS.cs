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
    public partial class GPS : Form
    {
        Uri[] addresses;

        public GPS()
        {
            InitializeComponent();

            addresses = new Uri[] { 
                new Uri("http://ilikeducks.com/LampSim/Historical/gps/von.png"), // Von
                new Uri("http://ilikeducks.com/LampSim/Historical/gps/dyn.png"), // Dyn
            };

            radioButton1.Checked = true;
            webControl1.Source = addresses[0];
            toolStripStatusLabel1.Text = "Loading..";
        
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            webControl1.Source = addresses[0];
            toolStripStatusLabel1.Text = "Loading..";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            webControl1.Source = addresses[1];
            toolStripStatusLabel1.Text = "Loading..";
        }

        private void documentReady(object sender, Awesomium.Core.UrlEventArgs e)
        {
            toolStripStatusLabel1.Text = "Complete";
        }

        public void refreshCharts()
        {
            if (radioButton1.Checked)
            {
                webControl1.Source = addresses[0];
                toolStripStatusLabel1.Text = "Loading..";
            }
            else if (radioButton2.Checked)
            {
                webControl1.Source = addresses[1];
                toolStripStatusLabel1.Text = "Loading..";
            }
        }
    }
}
