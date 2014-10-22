using Awesomium.Core;
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
    public partial class Baering : Form
    {
        public Baering()
        {
            
            InitializeComponent();
            WebSession session = WebCore.CreateWebSession(new WebPreferences()
            {
                WebGL = true,
                EnableGPUAcceleration = true
            });
            webControl1.WebSession = session;
        }

        private void Baering_Load(object sender, EventArgs e)
        {
            //webBrowser1.ScriptErrorsSuppressed = true;
            
            //webBrowser1.Navigate("http://www.ilikeducks.com/LampSim/Barda/baering/3dbulge.html");

            

            linkLabel1.Links.Add(0, linkLabel1.Text.Length+1, "http://baering.github.io/");

            linkLabel1.Click += linkLabel1_Click;
            linkLabel2.Click += linkLabel2_Click;
            

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Items.Add("3D Bulge");
            comboBox1.Items.Add("Advanced 3D Bulge [Experimental]");
            comboBox1.Items.Add("Location Map");
            comboBox1.SelectedIndex = 0;

            trackBar1.Move += trackBar1_Move;
            //webBrowser1.DocumentCompleted += webBrowser1_Navigated;
            

        }

        void linkLabel1_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://baering.github.io/");
            }
            catch (Exception error)
            {
                Console.WriteLine("Problem Loading Web Page: " + error.Message);
                MessageBox.Show("Please visit Baerings website at http://baering.github.io", "Baerings Website");
            }
        }

        void linkLabel2_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://baering.github.io");
            }catch(Exception error)
            {
                Console.WriteLine("Problem Loading Web Page: " + error.Message);
                MessageBox.Show("Please visit Baerings website at http://baering.github.io", "Baerings Website");
            }
        }

        #region Hidden
        private void webBrowser1_Navigated(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //Depends on which option is selected in comboBox1
            switch (comboBox1.SelectedIndex)
            {
                //3D Bulge
                case 0:
                    display3d(sender);
                    break;
                //Advanced 3D Bulge
                case 1:

                    break;
                //LocationMap
                case 2:

                    break;
            }
        }


        public void display3d(object sender)
        {
            /*WebBrowser tempBrowser = (WebBrowser)sender;
            string script = "function setParams($displayHours, $displaySize, $onlyVerified){$scope.graphDisplayHours = $displayHours; $scope.graphDisplayQuakeSize = $displaySize; $scope.graphDisplayOnlyVerified = $onlyVerified; graphFilterChange();}";
            //tempBrowser.Document.GetElementByTag("head").InnerHtml += script;
            HtmlDocument tempDoc = tempBrowser.Document;
            HtmlElement element = tempDoc.CreateElement("script");
            element.InnerHtml += script;
            tempBrowser.Document. = tempDoc;


            //Change settings to match Sliders
            int displayHours = trackBar2.Value;
            double displaySize = (double)trackBar1.Value/10;
            bool onlyVerified = checkBox1.Checked;

            tempBrowser.Document.InvokeScript("setParams", new object[] { displayHours, displaySize, onlyVerified });*/
            
            //string script = "<script>function setParams($displayHours, $displaySize, $onlyVerified){$scope.graphDisplayHours = $displayHours; $scope.graphDisplayQuakeSize = $displaySize; $scope.graphDisplayOnlyVerified = $onlyVerified; graphFilterChange();}</script>";
            
            
        }
        #endregion

        void trackBar1_Move(object sender, EventArgs e)
        {
        }

        private void onMagValueChanged(object sender, EventArgs e)
        {

            double selectedValue = trackBar1.Value;
            selectedValue = selectedValue / 10;

            label4.Text = "Magnitude greater than " + selectedValue.ToString("0.#");
        }

        private void onLastHoursChanged(object sender, EventArgs e)
        {
            label5.Text = "Showing last " + trackBar2.Value + " hour(s)";

        }

        private void Awesomium_Windows_Forms_WebControl_ShowCreatedWebView(object sender, Awesomium.Core.ShowCreatedWebViewEventArgs e)
        {

        }


        private void onWindowResized(object sender, EventArgs e)
        {
            webControl1.Reload(true);
        }

        private void onResizeControl(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex != 1)
                webControl1.Reload(true);
        }

        private void onMouseUp(object sender, MouseEventArgs e)
        {
            //This changes the value
            //MessageBox.Show("This is a test. Value Changed.");
            reloadPage();
        }

        private void onHoursMouseUp(object sender, MouseEventArgs e)
        {
            reloadPage();
        }

        public void reloadPage()
        {
            //Reloads the page depending on:
            //  - Values of sliders and checkbox
            //  - Value of combobox (Which Page to load)
            string onlyHours = trackBar2.Value.ToString();
            string onlySize = ((double)trackBar1.Value/10).ToString();
            string onlyVerified = (checkBox1.Checked) ? "true" : "false";

            //string script = "alert(\"hello\");$scope.graphDisplayHours = " + 1 + ";$scope.graphDisplayQuakeSize = " + 1 + "; $scope.graphDisplayOnlyVerified = " + true + ";graphFilterChange();";

            //string script = "$(\"hoursSlider\").slider(\"value\"," + onlyHours + "); $(\"magSlider\").slider(\"value\"," + onlySize + "); graphFilterChange();alert(\"hello\");";
            
            //webControl1.ExecuteJavascript(script);

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combo1 = (ComboBox) sender;
            switch(combo1.SelectedIndex)
            {
                case 0:
                    webControl1.Source = new Uri("http://www.ilikeducks.com/LampSim/Barda/baering/3dbulge.html");
                    break;
                case 1:
                    webControl1.Source = new Uri("http://baering.github.io/earthquakes/visualization.html");
                    break;
                case 2:
                    webControl1.Source = new Uri("http://www.ilikeducks.com/LampSim/Barda/baering/3dbulge_map.html");
                    break;
            }
        }

        
    }
}
