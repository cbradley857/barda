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
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();
            this.TopMost = true;
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 48; i++)
            {
                comboBox1.Items.Add(i + 1);
            }

            comboBox1.SelectedIndex = 0;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["hours"] = comboBox1.SelectedIndex + 1;
            Properties.Settings.Default.Save();

            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
