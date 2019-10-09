using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
namespace SocketServerCommonLib
{
    public class jsonParse
    {
        public class AisMessage
        {
            public string ProtocolNo = "NA";
            public string UniqueCode = "NA";
            public string MMSI = "NA";
            public DateTime CollTime;
            public int IMO=0;
            public string CallSign = "NA";
            public string Name = "NA";
            public string AISVersion = "NA";
            public string NavigationalStatus = "NA";
            public string PositionDeviceType = "NA";
            public string PositionAccuracy = "NA";
            public double Lat=0.0;
            public double Lon = 0.0;
            public double SOG = 0.0;
            public double COG = 0.0;
            public double ROT = 0.0;
            public double Heading = 0.0;
            public string ShipCarGoType = "NA";
            public int DimA = 0;
            public int DimB = 0;
            public int DimC = 0;
            public int DimD = 0;
            public string Destination = "NA";
            public int ETAMonth = 0;
            public int ETADay = 0;
            public int ETAHour = 0;
            public int ETAMinute = 0;
            public int DTE = 0;
            public double MaximumDraught=0.0;
        }
        public class RadarMessage
        {
            public string ProtocolNo;
            public string UniqueCode = "NA";
            public string TargetID = "NA";
            public DateTime CollTime;
    
            public double Lat;
            public double Lon;
            public double VEL;
            public double Position;//方位
            public double Direction;//行使方向
            public string PType;//”方位类型T/R”
            public int  Mileage ;//量程，单位米
            public int IsAlarmArea;
            public string DataType;//”数据类型Q/T/L”
            public string TrackType; //”跟踪类型M/A”
     
        }

        public class ARPAMessage
        {
            public string ProtocolNo="02";
            public string UniqueCode = "NA";
            public string TargetID = "NA";
            public string MMSI = "NA";
            public DateTime CollTime;
            public string ShipName;
            public string ShipTypeL;
           

            public double Lat;
            public double Lon;
            public double SOG;
            public double COG;
            public double RealativeSOG;
            public double RealativeCOG;
            public double Position;//方位
        }

        public class RadarIPMessage
        {
            public string ProtocolNo = "F0";
            public string UniqueCode = "";
            public string Radars;

        }

        public class RadarPicPath
        {
            public string ProtocolNo = "20";
            public string UniqueCode = "";
            public string Radars;
            public DateTime CollTime;
            public double Lon;
            public double Lat;
            public int Pixel;//图片生成时雷达站的量程或者图片融合时虚拟量程值
            public string Path;//"图片URL"
        }
        public static string GetMessage(AisMessage meg)
        {
            string ans = "";

            ans = JsonConvert.SerializeObject(meg, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            ////去除字符中间的空隙
            ans = ans.Replace("\n", "");
            ans = ans.Replace("\r", "");
            ans = ans.Replace("\t", "");
            return ans;
        }
        public static string GetRadarMessage(RadarMessage meg)
        {
            string ans = "";

            ans = JsonConvert.SerializeObject(meg, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });


            ////去除字符中间的空隙
            ans = ans.Replace("\n", "");
            ans = ans.Replace("\r", "");
            ans = ans.Replace("\t", "");
            return ans;
        }
        public static string GetArpaMessage(ARPAMessage meg)
        {
            string ans = "";

            ans = JsonConvert.SerializeObject(meg, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            ////去除字符中间的空隙
            ans = ans.Replace("\n", "");
            ans = ans.Replace("\r", "");
            ans = ans.Replace("\t", "");

            return ans;
        }
        public static string GetRadarIPMessage(RadarIPMessage meg)
        {
           string ans = "";

           ans = JsonConvert.SerializeObject(meg, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

           ////去除字符中间的空隙
           ans = ans.Replace("\n", "");
           ans = ans.Replace("\r", "");
           ans = ans.Replace("\t", "");

           ans = "~" + ans + "~";
           return ans;
        }
        public static string GetJosonMessage(object meg)
        {
            string ans = "";

            ans = JsonConvert.SerializeObject(meg, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            ////去除字符中间的空隙
            ans = ans.Replace("\n", "");
            ans = ans.Replace("\r", "");
            ans = ans.Replace("\t", "");

            ans = "~" + ans + "~";
            return ans;
        }
        public void test()
        {

        }
    }
}
