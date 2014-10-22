using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bardarbunga
{
    public partial class Historical : Form
    {
        public Historical()
        {
            InitializeComponent();

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.ColumnCount = 9;
            dataGridView1.Columns[0].Name = "Date";
            dataGridView1.Columns[1].Name = "Depth";
            dataGridView1.Columns[2].Name = "Direction";
            dataGridView1.Columns[3].Name = "Distance";
            dataGridView1.Columns[4].Name = "Volcano";
            dataGridView1.Columns[5].Name = "Quality";
            dataGridView1.Columns[6].Name = "Size";
            dataGridView1.Columns[7].Name = "Verified";
            dataGridView1.Columns[8].Name = "Long Ago";

            dataGridView1.Columns[6].DisplayIndex = 1;
            dataGridView1.Columns[8].DisplayIndex = 1;

            dataGridView1.Columns[6].DefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);

            comboBox1.Items.Add("All");
            comboBox1.Items.Add(">1");
            comboBox1.Items.Add(">2");
            comboBox1.Items.Add(">3");
            comboBox1.Items.Add(">4");
            comboBox1.Items.Add(">5");

            comboBox1.SelectedIndex = 0;

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

            //loadQuakesThread();
        }

        private void Historical_Load(object sender, EventArgs e)
        {
            for(int i=1; i <25;i++)
            {
                startHour.Items.Add(i);
                endHour.Items.Add(i);
            }

            startHour.SelectedIndex = 0;
            endHour.SelectedIndex = 0;

            startHour.DropDownStyle = ComboBoxStyle.DropDownList;
            endHour.DropDownStyle = ComboBoxStyle.DropDownList;

            label3.Text = "Larger times may take \n     a while";

            //Start Date 13th September//
            startDatePicker.MinDate = new DateTime(2014, 9, 13);
            startDatePicker.Value = DateTime.Now;
            startDatePicker.MaxDate = DateTime.Now;


        }

        List<EqObject> earthquakes;
        DataGridViewColumn sortColumn;
        ListSortDirection sortDirection;

        public void loadQuakes()
        {
            try
            {
                Invoke((MethodInvoker)delegate() 
                {
                    button1.Enabled = false;
                    button2.Enabled = false;
                    toolStripStatusLabel1.Text = "Loading..";
                });

                if(earthquakes != null)
                    earthquakes.Clear();
                
                Invoke((MethodInvoker)delegate()
                {
                    dataGridView1.Rows.Clear();

                    sortColumn = dataGridView1.SortedColumn;
                    if (dataGridView1.SortOrder == SortOrder.Ascending) sortDirection = ListSortDirection.Ascending;
                    else sortDirection = ListSortDirection.Descending;
                });
                //Get Date and times
                DateTime startDate = DateTime.Now;
                Invoke((MethodInvoker)delegate() { startDate = startDatePicker.Value; });
                int hours = 0;
                Invoke((MethodInvoker)delegate() { hours = startHour.SelectedIndex + 1; });
                startDate = startDate + new TimeSpan(hours, 0, 0);

                DateTime endDate = DateTime.Now;
                Invoke((MethodInvoker)delegate() { endDate = endDatePicker.Value; });
                int endHours = 0;
                Invoke((MethodInvoker)delegate() { endHours = endHour.SelectedIndex + 1; });
                endDate = endDate + new TimeSpan(endHours, 0, 0);
                

                Int32 startTimestamp = (Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                Int32 endTimestamp = (Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                HistoryApi history = new HistoryApi();
                earthquakes = history.GetData(startTimestamp.ToString(), endTimestamp.ToString());

                double minMag = 0.0;

                Invoke((MethodInvoker)delegate()
                {
                    switch (comboBox1.SelectedIndex)
                    {
                        case 0:
                            minMag = 0;
                            break;
                        case 1:
                            minMag = 1;
                            break;
                        case 2:
                            minMag = 2;
                            break;
                        case 3:
                            minMag = 3;
                            break;
                        case 4:
                            minMag = 4;
                            break;
                        case 5:
                            minMag = 5;
                            break;
                    }
                });

                foreach(EqObject eq in earthquakes)
                {
                    if(double.Parse(eq.size, new CultureInfo("en-GB")) >= minMag)
                    {
                        DataGridViewRow tempRow = new DataGridViewRow();

                        DataGridViewCell cellDateName = new DataGridViewTextBoxCell();
                        cellDateName.Value = eq.date.ToLocalTime().ToString();
                        tempRow.Cells.Add(cellDateName);

                        DataGridViewCell cellDepthName = new DataGridViewTextBoxCell();
                        cellDepthName.Value = eq.depth;
                        tempRow.Cells.Add(cellDepthName);

                        DataGridViewCell cellDirectionName = new DataGridViewTextBoxCell();
                        cellDirectionName.Value = eq.direction;
                        tempRow.Cells.Add(cellDirectionName);

                        DataGridViewCell cellDistanceName = new DataGridViewTextBoxCell();
                        cellDistanceName.Value = eq.distance;
                        tempRow.Cells.Add(cellDistanceName);

                        DataGridViewCell cellVolcanoName = new DataGridViewTextBoxCell();
                        cellVolcanoName.Value = eq.volcano;
                        tempRow.Cells.Add(cellVolcanoName);

                        DataGridViewCell cellQualityName = new DataGridViewTextBoxCell();
                        cellQualityName.Value = eq.quality;
                        tempRow.Cells.Add(cellQualityName);

                        DataGridViewCell cellSizeName = new DataGridViewTextBoxCell();
                        cellSizeName.Value = eq.size;
                        tempRow.Cells.Add(cellSizeName);

                        DataGridViewCell cellVerifiedName = new DataGridViewTextBoxCell();
                        cellVerifiedName.Value = eq.verified;
                        tempRow.Cells.Add(cellVerifiedName);

                        DateTime now = DateTime.Now;
                        Double difference = (now - eq.date.ToLocalTime()).TotalMinutes;
                        int finalDiff = (int)Math.Round(difference);

                        int totalHours = finalDiff / 60;
                        int totalMins = finalDiff % 60;

                        DataGridViewCell cellLongAgoName = new DataGridViewTextBoxCell();
                        cellLongAgoName.Value = totalHours + " hrs " + totalMins + " mins";
                        tempRow.Cells.Add(cellLongAgoName);

                        Invoke((MethodInvoker)delegate() { dataGridView1.Rows.Add(tempRow); });
                        //dataGridView1.Rows.Add(tempRow);

                        if (double.Parse(eq.size, new CultureInfo("en-GB")) >= 4)
                            Invoke((MethodInvoker)delegate() { dataGridView1.Rows[dataGridView1.Rows.Count - 1].DefaultCellStyle.BackColor = Color.Red; });
                        else if (double.Parse(eq.size, new CultureInfo("en-GB")) >= 3)
                            Invoke((MethodInvoker)delegate() { dataGridView1.Rows[dataGridView1.Rows.Count - 1].DefaultCellStyle.BackColor = Color.Orange; });
                        else if (double.Parse(eq.size, new CultureInfo("en-GB")) >= 2)
                            Invoke((MethodInvoker)delegate() { dataGridView1.Rows[dataGridView1.Rows.Count - 1].DefaultCellStyle.BackColor = Color.Yellow; });


                    }
                }

                if(sortColumn != null && sortDirection != null)
                    dataGridView1.Sort(sortColumn, sortDirection);

            }catch(Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            Invoke((MethodInvoker)delegate() 
            {
                button1.Enabled = true;
                button2.Enabled = true;
                toolStripStatusLabel1.Text = "Complete";
            });
        }

        ThreadStart tStart;
        Thread thread;

        private void button1_Click(object sender, EventArgs e)
        {
            loadQuakesThread();
        }

        private void loadQuakesThread()
        {
            //loadQuakes()
            tStart = new ThreadStart(loadQuakes);
            thread = new Thread(tStart);
            thread.Start();
        }

        OldGraphs oldGraphs;

        private void button2_Click(object sender, EventArgs e)
        {
            DateTime startDate = DateTime.Now;
            Invoke((MethodInvoker)delegate() { startDate = startDatePicker.Value; });
            int hours = 0;
            Invoke((MethodInvoker)delegate() { hours = startHour.SelectedIndex + 1; });
            startDate = startDate + new TimeSpan(hours, 0, 0);

            DateTime endDate = DateTime.Now;
            Invoke((MethodInvoker)delegate() { endDate = endDatePicker.Value; });
            int endHours = 0;
            Invoke((MethodInvoker)delegate() { endHours = endHour.SelectedIndex + 1; });
            endDate = endDate + new TimeSpan(endHours, 0, 0);

            Int32 startTimestamp = (Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            Int32 endTimestamp = (Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;


            button2.Enabled = false;
            oldGraphs = new OldGraphs(startTimestamp.ToString(), endTimestamp.ToString());
            oldGraphs.FormClosed += oldGraphs_FormClosed;
            oldGraphs.Owner = this;
            oldGraphs.Show();
        }

        void oldGraphs_FormClosed(object sender, FormClosedEventArgs e)
        {
            button2.Enabled = true;
        }
    }
}
