using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Bardarbunga
{
    public enum GraphType
    {
        totalPerHour,
        realTime,
        splitHours
    }

    public partial class GraphScreen : Form
    {
        public GraphType currentGraph;

        public GraphScreen(GraphType graphType)
        {
            InitializeComponent();

            loadGraph(graphType);
        }

        public GraphScreen()
        {
            InitializeComponent();

            loadGraph(GraphType.realTime);
            button1.Enabled = false;
            radioButton1.Checked = true;
        }

        //Function is severely in need of refactoring
        public void loadGraph(GraphType graphType)
        {
            currentGraph = graphType;

            WebApi web = new WebApi();
            List<EqObject> quakeList = web.GetData();

            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.Titles.Clear();

            chart1.ChartAreas.Add("chartArea");

            if (quakeList.Count != 0)
            {
                if (graphType == GraphType.realTime)
                {

                    DateTime[] times = new DateTime[quakeList.Count];
                    double[] mag = new double[quakeList.Count];

                    for (int i = 0; i < quakeList.Count; i++)
                    {
                        times[i] = quakeList[i].date;
                        mag[i] = double.Parse(quakeList[i].size, new CultureInfo("en-GB"));
                    }

                    chart1.Palette = ChartColorPalette.Berry;
                    chart1.Titles.Add("All Quakes in past " + (Properties.Settings.Default.hours + 1) + " hour(s)");

                    /*
                    Series series = new Series();
                    series.ChartType = SeriesChartType.FastLine;

                    for(int i = 0; i < times.Length; i++)
                    {
                    

                        try
                        {
                            series = chart1.Series.Add(times[i].ToString());
                        }catch(Exception e)
                        {
                            series = chart1.Series.Add(times[i].ToString() + "-1");
                        }

                        series.Points.Add(mag[i]);
                    }
                    */

                    chart1.Series.Add("main");

                    DateTime startDate = quakeList[0].date;
                    DateTime lastDate = quakeList[quakeList.Count - 1].date;

                    //Populate X Axis Date Labels
                    //chart1.ChartAreas[0].AxisX.CustomLabels.Add(startDate.ToOADate(), lastDate.ToOADate(), startDate.ToLocalTime().ToString("h:mm tt"), 0, LabelMarkStyle.None);
                    //chart1.ChartAreas[0].AxisX.CustomLabels.Add(startDate.ToOADate(), lastDate.ToOADate(), "Hello", )

                    chart1.ChartAreas[0].AxisX.Title = "Time";
                    chart1.ChartAreas[0].AxisY.Title = "Magnitude";

                    chart1.Series["main"].XValueType = ChartValueType.DateTime;
                    chart1.ChartAreas[0].AxisX.LabelStyle.Enabled = true;
                    chart1.ChartAreas[0].AxisX.LabelStyle.Format = "h:mm tt";
                    chart1.ChartAreas[0].AxisX.Interval = 1;
                    chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Hours;
                    chart1.ChartAreas[0].AxisX.IntervalOffset = 1;

                    chart1.ChartAreas[0].AxisX.Minimum = startDate.ToOADate();
                    chart1.ChartAreas[0].AxisX.Maximum = lastDate.ToOADate();


                    chart1.ChartAreas[0].AxisX.Name = "Time";

                    chart1.Series["main"].ChartType = SeriesChartType.Line;
                    chart1.Series["main"].MarkerStyle = MarkerStyle.Diamond;
                    chart1.Series["main"].MarkerSize = 5;

                    for (int i = 0; i < times.Length; i++)
                    {
                        if (mag[i] > Properties.Settings.Default.minSize)
                        {
                            chart1.Series["main"].Points.AddXY(times[i], mag[i]);
                            chart1.Series["main"].Points[chart1.Series["main"].Points.Count - 1].ToolTip = "#VALY" + "M \n" + times[i];
                        }
                    }

                    chart1.Legends.Clear();

                    /*
                    for(int i = 0; i < times.Length; i++)
                    {
                        try
                        {
                            chart1.Series["main"].Points.AddXY()
                        }
                        catch(Exception e)
                        {

                        }
                    }
                    */
                }
                else if (graphType == GraphType.totalPerHour)
                {
                    DateTime[] times = new DateTime[quakeList.Count];
                    double[] mag = new double[quakeList.Count];

                    for (int i = 0; i < quakeList.Count; i++)
                    {
                        times[i] = quakeList[i].date;
                        mag[i] = double.Parse(quakeList[i].size, new CultureInfo("en-GB"));
                    }

                    chart1.Palette = ChartColorPalette.Berry;
                    chart1.Titles.Add("Total Quakes Per Hour");

                   // DateTime startDate = quakeList[0].date;
                    //DateTime endDate = quakeList[quakeList.Count - 1].date;

                    /////////
                    chart1.ChartAreas[0].AxisX.Title = "Time";
                    chart1.ChartAreas[0].AxisY.Title = "Total Earthquakes";

                    chart1.Series.Add("main");

                    //chart1.Series["main"].ChartType = SeriesChartType.Bar;
                    foreach (Series series in chart1.Series)
                    {
                        series.ChartType = SeriesChartType.Column;
                    }


                    //KeyValuePair<float, int> totals = new KeyValuePair<float, int>();
                    //List<KeyValuePair<DateTime, int>> totals = new List<KeyValuePair<DateTime,int>>();
                    Dictionary<DateTime, int> totals = new Dictionary<DateTime, int>();


                    foreach(EqObject e in quakeList)
                    {
                        if (double.Parse(e.size, new CultureInfo("en-GB")) >= Properties.Settings.Default.minSize)
                        {
                            //d.time = new DateTime(d.time.Year, d.time.Month, d.time.Day, d.time.Hour, 0, 0)
                            DateTime roundedTime = new DateTime(e.date.Year, e.date.Month, e.date.Day, e.date.Hour, 0, 0);

                            if(totals.ContainsKey(roundedTime))
                                totals[roundedTime]++;
                            else
                                totals.Add(roundedTime, 1);
                        }
                    }


                    foreach(KeyValuePair<DateTime, int> key in totals)
                    {
                        //Create bar.
                        chart1.Series["main"].Points.AddXY(key.Key.ToLocalTime(), key.Value);
                        //Generate tooltip string
                        String toolTipString = "Total: " + key.Value + "\nHour: " + key.Key.ToLocalTime().ToString("hh tt");
                        //Set tooltop
                        chart1.Series["main"].Points[chart1.Series["main"].Points.Count - 1].ToolTip = toolTipString;
                    }
                }
                else if (graphType == GraphType.splitHours)
                {
                    DateTime[] times = new DateTime[quakeList.Count];
                    double[] mag = new double[quakeList.Count];

                    for (int i = 0; i < quakeList.Count; i++)
                    {
                        times[i] = quakeList[i].date;
                        mag[i] = double.Parse(quakeList[i].size, new CultureInfo("en-GB"));
                    }

                    chart1.Palette = ChartColorPalette.Berry;
                    chart1.Titles.Add("Total Quakes Per Hour");

                    // DateTime startDate = quakeList[0].date;
                    //DateTime endDate = quakeList[quakeList.Count - 1].date;

                    /////////
                    chart1.ChartAreas[0].AxisX.Title = "Time";
                    chart1.ChartAreas[0].AxisY.Title = "Total Earthquakes";

                    
                    //chart1.Series["main"].ChartType = SeriesChartType.Bar;
                    foreach (Series series in chart1.Series)
                    {
                        series.ChartType = SeriesChartType.StackedColumn;
                    }


                    //KeyValuePair<float, int> totals = new KeyValuePair<float, int>();
                    //List<KeyValuePair<DateTime, int>> totals = new List<KeyValuePair<DateTime,int>>();
                    Dictionary<DateTime, int> totals = new Dictionary<DateTime, int>();

                    Dictionary<DateTime, int> total0 = new Dictionary<DateTime, int>();
                    Dictionary<DateTime, int> total1 = new Dictionary<DateTime, int>();
                    Dictionary<DateTime, int> total2 = new Dictionary<DateTime, int>();
                    Dictionary<DateTime, int> total3 = new Dictionary<DateTime, int>();
                    Dictionary<DateTime, int> total4 = new Dictionary<DateTime, int>();
                    Dictionary<DateTime, int> total5 = new Dictionary<DateTime,int>();


                    foreach (EqObject e in quakeList)
                    {
                        if (double.Parse(e.size, new CultureInfo("en-GB")) >= Properties.Settings.Default.minSize)
                        {
                            //d.time = new DateTime(d.time.Year, d.time.Month, d.time.Day, d.time.Hour, 0, 0)
                            DateTime roundedTime = new DateTime(e.date.Year, e.date.Month, e.date.Day, e.date.Hour, 0, 0);

                            if (totals.ContainsKey(roundedTime))
                                totals[roundedTime]++;
                            else
                                totals.Add(roundedTime, 1);

                            //Totals for each size
                            double currentSize = double.Parse(e.size, new CultureInfo("en-GB"));

                            if(currentSize < 1)
                            {
                                if (total0.ContainsKey(roundedTime))
                                    total0[roundedTime]++;
                                else
                                    total0.Add(roundedTime, 1);
                            }
                            else if(currentSize < 2)
                            {
                                if (total1.ContainsKey(roundedTime))
                                    total1[roundedTime]++;
                                else
                                    total1.Add(roundedTime, 1);
                            }
                            else if(currentSize < 3)
                            {
                                if (total2.ContainsKey(roundedTime))
                                    total2[roundedTime]++;
                                else
                                    total2.Add(roundedTime, 1);
                            }
                            else if(currentSize < 4)
                            {
                                if (total3.ContainsKey(roundedTime))
                                    total3[roundedTime]++;
                                else
                                    total3.Add(roundedTime, 1);
                            }
                            else if(currentSize < 5)
                            {
                                if (total4.ContainsKey(roundedTime))
                                    total4[roundedTime]++;
                                else
                                    total4.Add(roundedTime, 1);
                            }
                            else if(currentSize >=5)
                            {
                                if (total5.ContainsKey(roundedTime))
                                    total5[roundedTime]++;
                                else
                                    total5.Add(roundedTime, 1);
                            }
                        }
                    }



                    foreach (KeyValuePair<DateTime, int> key in totals)
                    {
                        /*
                        //Create bar.
                        chart1.Series["main"].Points.AddXY(key.Key.ToLocalTime(), key.Value);
                        //Generate tooltip string
                        String toolTipString = "Total: " + key.Value + "\nHour: " + key.Key.ToLocalTime().ToString("hh tt");
                        //Set tooltop
                        chart1.Series["main"].Points[chart1.Series["main"].Points.Count - 1].ToolTip = toolTipString;
                         * */

                        string sName = key.Key.ToLocalTime().ToString();

                        chart1.Series.Add(sName);

                        if (total0.ContainsKey(key.Key))
                        {
                            chart1.Series[sName].Points.AddXY(key.Key.ToLocalTime(), total0[key.Key]);
                            chart1.Series[sName].Points[chart1.Series[sName].Points.Count - 1].Color = Color.LightBlue;
                        }
                        if (total1.ContainsKey(key.Key))
                        {
                            chart1.Series[sName].Points.AddXY(key.Key.ToLocalTime(), total1[key.Key]);
                            chart1.Series[sName].Points[chart1.Series[sName].Points.Count - 1].Color = Color.Blue;
                        }
                        if (total2.ContainsKey(key.Key))
                        {
                            chart1.Series[sName].Points.AddXY(key.Key.ToLocalTime(), total2[key.Key]);
                            chart1.Series[sName].Points[chart1.Series[sName].Points.Count - 1].Color = Color.Yellow;
                        }
                        if (total3.ContainsKey(key.Key))
                        {
                            chart1.Series[sName].Points.AddXY(key.Key.ToLocalTime(), total3[key.Key]);
                            chart1.Series[sName].Points[chart1.Series[sName].Points.Count - 1].Color = Color.Orange;
                        }
                        if (total4.ContainsKey(key.Key))
                        {
                            chart1.Series[sName].Points.AddXY(key.Key.ToLocalTime(), total4[key.Key]);
                            chart1.Series[sName].Points[chart1.Series[sName].Points.Count - 1].Color = Color.Red;
                        }
                        if (total5.ContainsKey(key.Key))
                        {
                            chart1.Series[sName].Points.AddXY(key.Key.ToLocalTime(), total5[key.Key]);
                            chart1.Series[sName].Points[chart1.Series[sName].Points.Count - 1].Color = Color.Purple;
                        }
                    }
                }

            }
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void GraphScreen_Load(object sender, EventArgs e)
        {
            
        }

        private void onFormClosed(object sender, FormClosedEventArgs e)
        {


        }

        private void button1_Click(object sender, EventArgs e)
        {
            loadGraph(GraphType.realTime);
            buttonAccess(GraphType.realTime);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            loadGraph(GraphType.totalPerHour);
            buttonAccess(GraphType.totalPerHour);   
        }

        #region Hidden
        private void buttonAccess(GraphType g)
        {
            switch(g)
            {
                case GraphType.totalPerHour:
                    button1.Enabled = true;
                    button2.Enabled = false;
                    break;
                case GraphType.realTime:
                    button1.Enabled = false;
                    button2.Enabled = true;
                    break;
            }
        }
        #endregion
        private void timer1_Tick(object sender, EventArgs e)
        {
            loadGraph(currentGraph);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            loadGraph(GraphType.realTime);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            loadGraph(GraphType.totalPerHour);
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            loadGraph(GraphType.splitHours);
        }
        

    }
}
