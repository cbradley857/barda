using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bardarbunga
{
    public partial class Form1 : Form
    {
        List<EqObject> equakes = new List<EqObject>();

        bool firstCheck;
        ThreadStart tStart;
        Thread childThread;

        //int checkUpdateTime;

        DrumPlots drumPlots;

        public Form1()
        {
            InitializeComponent();

            toolStripStatusLabel3.IsLink = true;
            toolStripStatusLabel3.Click += toolStripStatusLabel3_Click;
            toolStripStatusLabel1.Click += toolStripStatusLabel1_Click;

            //checkUpdateTime = 0;

            Properties.Settings.Default.Upgrade();

            firstCheck = true;

            getTime();

            checkBox1.Checked = Properties.Settings.Default.keepOnTop;

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
            dataGridView1.Columns[8].SortMode = DataGridViewColumnSortMode.NotSortable;


            //Populate Magnitude Box
            comboBox1.Items.Add("All"); // 0
            comboBox1.Items.Add(">1");
            comboBox1.Items.Add(">2"); 
            comboBox1.Items.Add(">3");
            comboBox1.Items.Add(">4");
            comboBox1.Items.Add(">5");
            comboBox1.Items.Add(">6");

            comboBox1.SelectedIndex = int.Parse(Properties.Settings.Default["minSize"].ToString());
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

            //Populate Hours Box
            for (int i = 0; i < 48; i++)
            {
                comboBox2.Items.Add(i + 1);
            }

            comboBox2.SelectedIndex = Properties.Settings.Default.hours;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;

            //Minimum Magnitude for Alerts
            comboBox3.Items.Add("All"); // 0
            comboBox3.Items.Add(">1");
            comboBox3.Items.Add(">2");
            comboBox3.Items.Add(">3");
            comboBox3.Items.Add(">4");
            comboBox3.Items.Add(">5");
            comboBox3.Items.Add(">6");

            comboBox3.SelectedIndex = int.Parse(Properties.Settings.Default["minAlert"].ToString());
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;

            HandleCreated += Form1_HandleCreated;
            

        }

        void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            // Shows message with error detail only if message is "network error, check for more detail"
            if(networkErrorMessage != "" && networkErrorMessage != null)
            {
                MessageBox.Show("Detailed error message: " + networkErrorMessage, "Error Detail");
            }
        }

        void toolStripStatusLabel3_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("mailto:barda@ilikeducks.com");
            }catch(Exception error)
            {
                MessageBox.Show("Please email all bugs to barda@ilikeducks.com");

            }
        }

        void Form1_HandleCreated(object sender, EventArgs e)
        {
            // Needs Changed to work?
            if (Properties.Settings.Default.sortOrder != null && Properties.Settings.Default.sortColumn != null)
            {
                dataGridView1.Sort(Properties.Settings.Default.sortColumn, Properties.Settings.Default.sortOrder);
            }

            refreshList();
            //checkUpdates();
        }

        /*
        private void initialCheck()
        {
            

            WebApi web = new WebApi();
            equakes = web.GetData();

            foreach(EqObject eq in equakes)
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

                //Work out time since
                

                dataGridView1.Rows.Add(tempRow);
            }

            

        }
        */


        public void refreshList()
        {
            //refreshCode();
            if (this.IsHandleCreated)
            {
                tStart = new ThreadStart(refreshCode);
                childThread = new Thread(tStart);
                childThread.Start();
            }

            if (graph != null && graph.IsHandleCreated)
                graph.loadGraph(graph.currentGraph);
        }

        string networkErrorMessage;

        public void refreshCode()
        {
            try
            {
                Invoke((MethodInvoker)delegate() 
                {
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    comboBox3.Enabled = false;
                    button1.Enabled = false;
                });

                DataGridViewColumn sortColumn = dataGridView1.SortedColumn;
                
                ListSortDirection direction;
                if (dataGridView1.SortOrder == SortOrder.Ascending) direction = ListSortDirection.Ascending;
                else direction = ListSortDirection.Descending;
                
                Invoke((MethodInvoker)delegate(){getTime();});
                //toolStripStatusLabel1.Text = "Status: Checking for Quakes";
                Invoke((MethodInvoker)delegate() { toolStripStatusLabel1.Text = "Status: Checking for Quakes"; });

                List<EqObject> lastCheck = equakes;
                DateTime lastDate = new DateTime(1970, 1, 1);

                if (equakes.Count > 0)
                    lastDate = equakes[equakes.Count - 1].date;

                equakes.Clear();
                WebApi web = new WebApi();
                equakes = web.GetData();

                foreach (EqObject q in equakes)
                {
                    if (q.date > Properties.Settings.Default.lastTime && double.Parse(q.size, new CultureInfo("en-GB")) >= double.Parse(Properties.Settings.Default["minAlert"].ToString(), new CultureInfo("en-GB")))
                    {
                        
                        //Play sound
                        System.Media.SystemSounds.Asterisk.Play();
                        notifyIcon1.ShowBalloonTip(2000, "New Quake", "Magnitude " + q.size, ToolTipIcon.Info);

                    }
                }

                //Clear Grid
                Invoke((MethodInvoker)delegate() { dataGridView1.Rows.Clear(); });

                //List<EqObject> tempList = new List<EqObject>();
                //tempList = equakes;
                //tempList.Reverse();

                double minSize = double.Parse(Properties.Settings.Default["minSize"].ToString(), new CultureInfo("en-GB"));
                
                foreach (EqObject eq in equakes)
                {
                    if (double.Parse(eq.size,new CultureInfo("en-GB")) >= minSize)
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

                
                Invoke((MethodInvoker)delegate() { toolStripStatusLabel2.Text = "Showing past " + (int.Parse(Properties.Settings.Default["hours"].ToString()) + 1) + " hour(s)"; });
                Invoke((MethodInvoker)delegate() { toolStripStatusLabel1.Text = "Status: Done     Last Checked: " + DateTime.Now.ToString("h:mm:ss tt"); });

                if (sortColumn != null)
                {
                    Invoke((MethodInvoker)delegate()
                    {
                        dataGridView1.Sort(sortColumn, direction);
                    });
                }

                Invoke((MethodInvoker)delegate() { label6.Text = "Quakes: " + (dataGridView1.Rows.Count); });

                Properties.Settings.Default.lastTime = equakes[equakes.Count - 1].date;
                Properties.Settings.Default.Save();
            }
            catch (Exception e)
            {
                try
                {
                    Invoke((MethodInvoker)delegate() { toolStripStatusLabel1.Text = "Status: Network Error (Click for more detail)"; networkErrorMessage = "Error Message:" + e.Message; });
                    
                }catch(Exception error)
                {
                    Console.WriteLine("Error-: " + error.Message);
                }

                Console.WriteLine("Error-: " + e.Message);
                try
                {
                    WebRequest request = WebRequest.Create("http://ilikeducks.com/LampSim/bugReport.php?eMessage=" + e.Message);
                    WebResponse response = request.GetResponse();
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.Message);
                }
            }

            Invoke((MethodInvoker)delegate()
            {
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;

                if (graph != null && !graph.Visible)
                    button1.Enabled = true;
                else if (graph == null)
                    button1.Enabled = true;

            });
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            /*checkUpdateTime++;

            if(checkUpdateTime > 59)
            {
                checkUpdateTime = 0;
                checkUpdates();
            }
            */

            refreshList();
            timer1.Stop();
            timer1.Start();

            if (tremors != null)
                tremors.refreshCharts();

            if (gps != null)
                gps.refreshCharts();

            if (drumPlots != null)
                drumPlots.refreshCharts();
             
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Open Options Menu
            

            OptionsForm optionsForm = new OptionsForm();
            optionsForm.Show();

            optionsForm.FormClosed +=optionsForm_FormClosed;

            //Listen for window Closing
        }

        private void optionsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            refreshList();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void onSelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox send = (ComboBox)sender;
            Properties.Settings.Default["minSize"] = double.Parse(send.SelectedIndex.ToString(), new CultureInfo("en-GB"));
            Properties.Settings.Default.Save();

            refreshList();
        }

        private void onHoursIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["hours"] = comboBox2.SelectedIndex;
            Properties.Settings.Default.Save();

            refreshList();
        }

        public void getTime()
        {
            label4.Text = "Time: " + DateTime.Now.ToShortTimeString();
        }

        private void minAlertChanged(object sender, EventArgs e)
        {
            ComboBox minAlertCombo = (ComboBox)sender;
            int minAlertNo = comboBox3.SelectedIndex;
            Properties.Settings.Default["minAlert"] = double.Parse("" + minAlertNo, new CultureInfo("en-GB"));
            Properties.Settings.Default.Save();

        }

        private void Form1_ResizeBegin(object sender, EventArgs e)
        {
            
        }

        private void onResizeLayout(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {

                notifyIcon1.ShowBalloonTip(1000, "Minimized to Tray", "Click here to show", ToolTipIcon.Info);
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
            }
        }

        private void onClickTrayIcon(object sender, EventArgs e)
        {
            this.Show();
            this.BringToFront();
            this.WindowState = FormWindowState.Maximized;
        }

        GraphScreen graph;
            
        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count != 0)
            {
                try
                {
                    button1.Enabled = false;
                    graph = new GraphScreen();
                    if (Properties.Settings.Default.keepOnTop)
                        graph.Owner = this;
                    else
                        graph.Owner = null;
                    graph.FormClosed += graph_FormClosed;
                    graph.Load += graph_Load;
                    graph.Show();
                }catch(Exception error)
                {
                    MessageBox.Show("Problem Opening Graph Screen: " + error.Message);
                }
            }
            else
            {
                MessageBox.Show("There is no data to show in the graph screen\nPlease try changing your settings in the main window", "No Data To Show");
            }
        }

        void graph_Load(object sender, EventArgs e)
        {
            timer1.Stop();
            timer1.Start();
        }

        void graph_FormClosed(object sender, FormClosedEventArgs e)
        {
            button1.Enabled = true;
        }

        private void onFormClosing(object sender, FormClosingEventArgs e)
        {
            // If currently checking for date, kill the thread to avoid window Handle errors
            if(childThread.IsAlive)
                childThread.Abort();

            //Save settings regarding the sort direction of te list
            DataGridViewColumn sortColumn = dataGridView1.SortedColumn;

            ListSortDirection direction;
            if (dataGridView1.SortOrder == SortOrder.Ascending) direction = ListSortDirection.Ascending;
            else direction = ListSortDirection.Descending;

            Properties.Settings.Default.sortColumn = sortColumn;
            Properties.Settings.Default.sortOrder = direction;
            Properties.Settings.Default.Save();

        }

        Baering baeringWindow;
        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            //Open Window for 3dbulge data
            baeringWindow = new Baering();
            if (Properties.Settings.Default.keepOnTop)
                baeringWindow.Owner = this;
            else
                baeringWindow.Owner = null;
            baeringWindow.FormClosed += baeringWindow_FormClosed;
            baeringWindow.Show();
        }

        void baeringWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            //button2.Enabled = true;
            dBulgeToolStripMenuItem.Enabled = true;
        }

        public void checkUpdates()
        {
            ThreadStart updateTStart = new ThreadStart(UpdateProgram);
            Thread updateThread = new Thread(updateTStart);
            updateThread.Start();
        }

        public void UpdateProgram()
        {
            string updateLocation = "http://www.ilikeducks.com/LampSim/Barda/currentVersion.php";

            WebRequest request = WebRequest.Create(updateLocation);
            WebResponse response = null;

            try
            {
                response = request.GetResponse();
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            String result;

            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            try
            {
                JObject updateResult = JObject.Parse(result);
                double newest = double.Parse((String)updateResult["version"], new CultureInfo("en-GB"));
                double current = double.Parse(Properties.Settings.Default.currentVersion, new CultureInfo("en-GB"));

                if(newest > current)
                {
                    Invoke((MethodInvoker)delegate() 
                    {
                        if(MessageBox.Show(
                            "Bardabunga Version " + newest + " is available to download. Visit download page now?\nDownload: http://www.ilikeducks.com/LampSim/bardarbunga.html",
                            "Update Available",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            try
                            {
                                System.Diagnostics.Process.Start("http://www.ilikeducks.com/LampSim/bardarbunga.html");
                            }catch(Exception excep)
                            {
                                MessageBox.Show("Cannot open web page automatically on your system. Please download the update from http://ilikeducks.com/", "Problem");
                                Console.WriteLine(excep.Message);
                            }
                        }
                    });
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error Checking for updates\n" + e.Message);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.keepOnTop = checkBox1.Checked;
            Properties.Settings.Default.Save();
            setOwners(Properties.Settings.Default.keepOnTop);
        }

        public void setOwners(bool keepOnTop)
        {
            if(keepOnTop)
            {
                if (graph != null)
                {
                    graph.Owner = this;
                    if (graph.Visible)
                        baeringWindow.BringToFront();
                }
                if (baeringWindow != null)
                {
                    baeringWindow.Owner = this;
                    if (baeringWindow.Visible)
                        baeringWindow.BringToFront();
                }
                if(drumPlots != null)
                {
                    drumPlots.Owner = this;
                    if (drumPlots.Visible)
                        drumPlots.BringToFront();
                }
                if(tremors != null)
                {
                    tremors.Owner = this;
                    if (tremors.Visible)
                        tremors.BringToFront();
                }
                if(historical != null)
                {
                    historical.Owner = this;
                    if (historical.Visible)
                        historical.BringToFront();
                }
            }
            else
            {
                if (graph != null)
                    graph.Owner = null;
                if (baeringWindow != null)
                    baeringWindow.Owner = null;
                if (drumPlots != null)
                    drumPlots.Owner = null;
                if (tremors != null)
                    tremors.Owner = null;
                if (historical.Owner != null)
                    historical.Owner = null;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            //Open Window for 3dbulge data
            drumPlots = new DrumPlots();
            if (Properties.Settings.Default.keepOnTop)
                drumPlots.Owner = this;
            else
                drumPlots.Owner = null;
            drumPlots.FormClosed += drumPlotsWindow_FormClosed;
            drumPlots.Show();
            
            //Open Window
        }

        private void drumPlotsWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            button3.Enabled = true;
            drumplotsToolStripMenuItem.Enabled = true;
        }

        TremorCharts tremors;

        private void button4_Click(object sender, EventArgs e)
        {
            button4.Enabled = false;
            tremors = new TremorCharts();
            if (Properties.Settings.Default.keepOnTop)
                tremors.Owner = this;
            else
                tremors.Owner = null;
            tremors.FormClosed += tremors_FormClosed;
            tremors.Show();
        }

        void tremors_FormClosed(object sender, FormClosedEventArgs e)
        {
            //button4.Enabled = true;
            tremorChartsToolStripMenuItem.Enabled = true;
        }

        GPS gps;
        private void button5_Click(object sender, EventArgs e)
        {
            button5.Enabled = false;
            gps = new GPS();
            if (Properties.Settings.Default.keepOnTop)
                gps.Owner = this;
            else
                gps.Owner = null;
            gps.FormClosed += gps_FormClosed;
            gps.Show();
        }

        void gps_FormClosed(object sender, FormClosedEventArgs e)
        {
            //button5.Enabled = true;
            gPSChartsToolStripMenuItem.Enabled = true;
        }

        private void drumplotsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Drumplots
            drumplotsToolStripMenuItem.Enabled = false;
            //Open Window for 3dbulge data
            drumPlots = new DrumPlots();
            if (Properties.Settings.Default.keepOnTop)
                drumPlots.Owner = this;
            else
                drumPlots.Owner = null;
            drumPlots.FormClosed += drumPlotsWindow_FormClosed;
            drumPlots.Show();
        }

        private void tremorChartsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Tremor Charts
            tremorChartsToolStripMenuItem.Enabled = false;
            tremors = new TremorCharts();
            if (Properties.Settings.Default.keepOnTop)
                tremors.Owner = this;
            else
                tremors.Owner = null;
            tremors.FormClosed += tremors_FormClosed;
            tremors.Show();
        }

        private void gPSChartsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // GPS charts
            gPSChartsToolStripMenuItem.Enabled = false;
            gps = new GPS();
            if (Properties.Settings.Default.keepOnTop)
                gps.Owner = this;
            else
                gps.Owner = null;
            gps.FormClosed += gps_FormClosed;
            gps.Show();
        }

        private void dBulgeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Open 3d bulge window
            dBulgeToolStripMenuItem.Enabled = false;
            baeringWindow = new Baering();
            if (Properties.Settings.Default.keepOnTop)
                baeringWindow.Owner = this;
            else
                baeringWindow.Owner = null;
            baeringWindow.FormClosed += baeringWindow_FormClosed;
            baeringWindow.Show();
        }

        Historical historical;

        private void olderDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            historical = new Historical();
            olderDataToolStripMenuItem.Enabled = false;

            if (Properties.Settings.Default.keepOnTop)
                historical.Owner = this;
            else
                historical.Owner = null;
            historical.FormClosed += historical_FormClosed;
            historical.Show();
        }

        void historical_FormClosed(object sender, FormClosedEventArgs e)
        {
            olderDataToolStripMenuItem.Enabled = true;
        }

    }
}
