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
    public partial class OldGraphs : Form
    {
        GraphType currentGraph;
        string startDate;
        string endDate;

        public OldGraphs(string startDate, string endDate)
        {
            InitializeComponent();

            this.startDate = startDate;
            this.endDate = endDate;

            radioButton1.Checked = true;
            loadGraph(GraphType.realTime);
        }

        private void OldGraphs_Load(object sender, EventArgs e)
        {
            
        }

        public void loadGraph(GraphType graphType)
        {
            currentGraph = graphType;

            HistoryApi web = new HistoryApi();
            List<EqObject> quakeList = web.GetData(startDate, endDate);

            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.Titles.Clear();

            chart1.ChartAreas.Add("chartArea");

            if(quakeList.Count !=0)
            {
                if(graphType == GraphType.realTime)
                {
                    DateTime[] times = new DateTime[quakeList.Count];
                    double[] mag = new double[quakeList.Count];

                    for(int i=0; i < quakeList.Count; i++)
                    {
                        times[i] = quakeList[i].date;
                        mag[i] = double.Parse(quakeList[i].size, new CultureInfo("en-GB"));
                    }

                    chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Berry;
                    chart1.Titles.Add("All Quakes between chosen times");

                    chart1.Series.Add("main");

                    DateTime sDate = quakeList[0].date;
                    DateTime eDate = quakeList[quakeList.Count - 1].date;

                    chart1.ChartAreas[0].AxisX.Title = "Time";
                    chart1.ChartAreas[0].AxisX.Title = "Magnitude";

                    chart1.Series["main"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
                    chart1.ChartAreas[0].AxisX.LabelStyle.Enabled = true;
                    chart1.ChartAreas[0].AxisX.LabelStyle.Format = "h:mm tt";
                    chart1.ChartAreas[0].AxisX.Interval = 1;
                    chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Hours;
                    chart1.ChartAreas[0].AxisX.IntervalOffset = 1;

                    chart1.ChartAreas[0].AxisX.Minimum = sDate.ToOADate();
                    chart1.ChartAreas[0].AxisX.Maximum = eDate.ToOADate();


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

                }
                else if(graphType == GraphType.totalPerHour)
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
                        }
                    }


                    foreach (KeyValuePair<DateTime, int> key in totals)
                    {
                        //Create bar.
                        chart1.Series["main"].Points.AddXY(key.Key.ToLocalTime(), key.Value);
                        //Generate tooltip string
                        String toolTipString = "Total: " + key.Value + "\nTime: " + key.Key.ToLocalTime().ToString();
                        //Set tooltop
                        chart1.Series["main"].Points[chart1.Series["main"].Points.Count - 1].ToolTip = toolTipString;
                    }
                }
            }




        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            loadGraph(GraphType.realTime);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            loadGraph(GraphType.totalPerHour);
        }
    }
}
