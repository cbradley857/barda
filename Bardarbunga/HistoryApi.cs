using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bardarbunga
{
    class HistoryApi
    {
        public List<EqObject> GetData(string startTime, string endTime)
        {
            List<EqObject> equakeList = new List<EqObject>();

            JObject obj = JObject.Parse(getResponse(startTime, endTime));

            int qNo = int.Parse((string)obj["count"]);

            if(qNo > 0)
            {
                // For each earthquake 
                for(int i=0;i<qNo;i++)
                {
                    string date = (string)obj["eqs"][i]["date"] + "000";
                    var time = TimeSpan.FromMilliseconds(Int64.Parse(date));
                    DateTime referenceTime = new DateTime(1970, 1, 1);
                    referenceTime = referenceTime + time;

                    string size = (string)obj["eqs"][i]["size"];
                    string depth = (string)obj["eqs"][i]["depth"];
                    string direction = (string)obj["eqs"][i]["direction"];
                    string distance = (string)obj["eqs"][i]["distance"];
                    string volcano = (string)obj["eqs"][i]["volcano"];
                    string quality = (string)obj["eqs"][i]["quality"];

                    equakeList.Add(new EqObject(
                        referenceTime, 
                        depth, 
                        direction, 
                        distance, 
                        volcano, 
                        quality, 
                        size,
                        (double.Parse(quality) == 99) ? "true":"false"
                        ));
                }
            }

            return equakeList;
        }

        public string getResponse(string startTime, string endTime)
        {
            string webAddress = "http://www.ilikeducks.com/LampSim/Historical/historical_json.php?startTime=" + startTime + "&endTime=" + endTime;

            WebRequest request = WebRequest.Create(webAddress);

            WebResponse response = request.GetResponse();

            using(var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }
        }
    }
}
