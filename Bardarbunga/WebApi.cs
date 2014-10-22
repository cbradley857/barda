using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Bardarbunga
{
    class WebApi
    {
        string baseAddress = "http://isapi.rasmuskr.dk/api/earthquakes/?date=";
        string addressEnd = "-hoursago";
        int hoursAgo;

        public WebApi()
        {
            hoursAgo = 1;
        }

        public List<EqObject> GetData()
        {
            List<EqObject> eqList = new List<EqObject>();

            // Check that hoursAgo is correct
            hoursAgo = Properties.Settings.Default.hours;//int.Parse(Properties.Settings.Default["hours"].ToString());

            JObject obj = JObject.Parse(getResponse());

            int qNo = int.Parse((string)obj["count"]);

            if (qNo > 0)
            {
                //String quakes = (string)obj["items"][0]["date"];

                DateTime cutOff = DateTime.Now - new TimeSpan(hoursAgo+1,0,0);

                for (int i = 0; i < qNo; i++)
                {
                    //Work out time
                    string date = (string)obj["items"][i]["date"] + "000";
                    var time = TimeSpan.FromMilliseconds(Int64.Parse(date));
                    DateTime referenceTime = new DateTime(1970, 1, 1);
                    referenceTime = referenceTime + time;


                    //Check it actually meets the cut off time
                   if(referenceTime.ToLocalTime() > cutOff){
                        //date = referenceTime.ToLocalTime().ToString();

                        string depth = (string)obj["items"][i]["depth"];

                        string direction = (string)obj["items"][i]["loc_dir"];
                        string distance = (string)obj["items"][i]["loc_dist"];
                        string volcano = (string)obj["items"][i]["loc_name"];

                        string quality = (string)obj["items"][i]["quality"];
                        string size = (string)obj["items"][i]["size"];
                        string verified = (string)obj["items"][i]["verified"];

                        eqList.Add(new EqObject(referenceTime, depth, direction, distance, volcano, quality, size, verified));
                    }
                }
            }

            return eqList;
        }

        public string getResponse()
        {
                string webAddress = baseAddress + (hoursAgo+1) + "-hoursago";
                WebRequest request = WebRequest.Create(webAddress);

                WebResponse response = request.GetResponse();

                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return result;
                }
            
        }


    }


    class EqObject
    {
        /*
        private string location;
        private string mag;
        private string depth;
        private string quality;
        private string time;
        */

        public DateTime date;
        public string depth;

        public string direction;
        public string distance;
        public string volcano;

        public string quality;
        public string size;
        public string verified;

        public EqObject(DateTime date, 
            string depth, 
            string dir, 
            string dist, 
            string volc, 
            string qual, 
            string size, 
            string verified)
        {
            this.date = date;
            this.depth = depth;
            direction = dir;
            direction = direction.Replace("V", "W");
            direction = direction.Replace("v", "W");
            direction = direction.Replace("a", "E");
            direction = direction.Replace("A", "E");

            distance = dist;
            volcano = volc;
            quality = qual;
            this.size = size;
            this.verified = verified;
        }

    }
}
