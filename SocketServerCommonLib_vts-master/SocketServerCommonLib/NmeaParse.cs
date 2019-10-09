using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using GeometryTopoLibrary;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using SocketClientCommonLib;
using Newtonsoft.Json;

using System.Data.OleDb;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
namespace SocketServerCommonLib
{
    public class NmeaParse
    {
        public CRadarData m_radarData = new CRadarData();

     
        public List<CRadarData.FastFT1 >  m_listFastFT1= new List<CRadarData.FastFT1>();
        #region 变量
        public bool readerFalg = false;//状态标准；FALSE 可以写人，true;可以读取发出
        public target m_target ;
        public AisArpaTarget m_AisArpaTarge;
        public RadarEchoPic m_radarEchoPic= null;
        /// <summary>
        /// AIS目标集合
        /// </summary>
        
        public Dictionary<int, AISTarget> m_AISCollection = new Dictionary<int, AISTarget>();

        public Dictionary<long, target> m_dicTargetCollection = new Dictionary<long, target>();
        public Dictionary<long, AisArpaTarget> m_dicAisArpaTargetCollection = new Dictionary<long, AisArpaTarget>();
        //public Dictionary<long, RadarEchoPic> m_dicRadarEchoPicCollection = new Dictionary<long, RadarEchoPic>();
        //public Dictionary<long, RadarEchoPic> m_dicRadarEchoPicCollection1 = new Dictionary<long, RadarEchoPic>();//分开维护，各自雷达站的回波各自处理
        //public Dictionary<long, RadarEchoPic> m_dicRadarEchoPicCollection2 = new Dictionary<long, RadarEchoPic>();

        public Dictionary<long, RadarEchoPic>[] m_dicRadarEchoPicCollectionMultil = null;//建立MAP数组管理雷达站图像
        public int m_radarCount;

        public SyncSocketClient m_tcpClient;
         
        private AsyncUDPServer m_asyncudpServer;

        private readonly byte SUM_END = Convert.ToByte('*');
        private readonly byte SUM_START = Convert.ToByte('$');
        private readonly byte NUM_0_ASCII = Convert.ToByte('0');
        private readonly byte NUM_A_ASCII = Convert.ToByte('A' - 10);

        private double m_MainLat = 0;//主雷达的位置
        private double m_MainLng = 0;

        public static CAccess g_CAccess = null;
        public static string m_AppPath = null;
        public static string m_DlgCaption = "雷达融合提示信息";

        public static DataSet g_dataSetRadarIP = new DataSet();
        public static DataSet g_dataSetCenTrlIP = new DataSet();

        public string m_CenTrlIP = "";

        public NmeaParse()
        {

            InitAccess();//初始化数据库配置

            g_dataSetRadarIP = selectRadarId();//获取雷达站IP
            g_dataSetCenTrlIP = selectCentrlIp();//获取中心服务器IP
            m_CenTrlIP = getCenTrlIP(g_dataSetCenTrlIP);
           
            udpSendJosonRadarIP();

          
           
            //InitTcpClient(m_CenTrlIP);//TCP连接中心服务器

            //  TcpSendJosonRadarIP();//根据与中心服务器要求，上电需要将雷达站配置情况发送给中心服务器
            
        }
        public NmeaParse(AsyncUDPServer udpServer)
        {
         
           
            InitAccess();//初始化数据库配置

            g_dataSetRadarIP = selectRadarId();//获取雷达站IP
            g_dataSetCenTrlIP = selectCentrlIp();//获取中心服务器IP
            m_CenTrlIP = getCenTrlIP(g_dataSetCenTrlIP);

            m_CenTrlIP = HelpCommonLib.NetworkAddress.GetIPAddress();
            InitTcpClient(m_CenTrlIP);//TCP连接中心服务器

            //  TcpSendJosonRadarIP();//根据与中心服务器要求，上电需要将雷达站配置情况发送给中心服务器
            m_asyncudpServer = udpServer;
            udpSendJosonRadarIP();

            //int radarCount = g_dataSetRadarIP.Tables[0].Rows.Count;
            int radarCount = 3;//3路FPGA
            m_radarCount = radarCount;

            m_dicRadarEchoPicCollectionMultil = new Dictionary<long, RadarEchoPic>[radarCount];
            for (int i = 0; i < radarCount; i++)
            {
                m_dicRadarEchoPicCollectionMultil[i] = new Dictionary<long, RadarEchoPic>();
            }
        }
        public void TcpSendJosonRadarIP()
        {
            try
            {
                jsonParse.RadarIPMessage radarIPMessage = new jsonParse.RadarIPMessage();
                radarIPMessage.Radars = getRadarIP(g_dataSetRadarIP);
                string sJosonRadarIP = jsonParse.GetRadarIPMessage(radarIPMessage);
                m_tcpClient.SendData(sJosonRadarIP);
            }
            catch (System.Exception ex)
            {
            	
            }
        }
        public void udpSendCentrlIP(string str)
        {
            m_asyncudpServer.m_sListen.SendTo(System.Text.Encoding.Default.GetBytes(str), str.Length, SocketFlags.None, m_asyncudpServer.m_ippoint);
        }
        public void udpSendJosonRadarIP()
        {
            try
            {
                jsonParse.RadarIPMessage radarIPMessage = new jsonParse.RadarIPMessage();
                radarIPMessage.Radars = getRadarIP(g_dataSetRadarIP);
                string sJosonRadarIP = jsonParse.GetRadarIPMessage(radarIPMessage);

                udpSendCentrlIP(sJosonRadarIP);
            }
            catch (System.Exception ex)
            {

            }
        }
        public void InitTcpClient(string StcpServerIP)
        {
            m_tcpClient = new SyncSocketClient();

            m_tcpClient.Connect(StcpServerIP, 16000);
        }
        public void InitAccess()
        {
            m_AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            string ConString = " Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\\DatabaseRadarConfig.accdb";

            CAccess.DBOpError dberror;
            g_CAccess = new CAccess(ConString);
            g_CAccess.AccessConect();
            dberror = g_CAccess.AccessConect();

            if (dberror != CAccess.DBOpError.Error_Success)
            {            
                return;
            }
        }
        public string getRadarIP(DataSet ds)
        {
            if(ds==null)
            {
                return "";
            }
            String strIP = "";
            if (ds.Tables[0].Rows.Count > 0)
            {
                strIP = ds.Tables[0].Rows[0][RadarIP.m_RadarIP].ToString();
                for (int i = 1; i < ds.Tables[0].Rows.Count; i++)
                {
                    strIP = strIP + "," + ds.Tables[0].Rows[i][RadarIP.m_RadarIP].ToString();
                }

            }
            return strIP;
        }
        public string getCenTrlIP(DataSet ds)
        {
            string sCenTrlIP = "";
            //if (ds == null)
            //{
            //    return sCenTrlIP;
            //}
            //if(ds.Tables[0].Rows.Count>0)
            //{
            //    sCenTrlIP = ds.Tables[0].Rows[0]["tcpServerIP"].ToString();
            //}
            return sCenTrlIP;  
        }
        public DataSet selectRadarId()
        {
            try
            {
                DataSet ds = new DataSet();

                g_CAccess.AccessConect();
                string str = "select * from " + RadarIP.m_BiaoMingCheng + ";";
                g_CAccess.ExecuteGetSet(str, ref ds);
                //String strIP = "";
                //if (ds.Tables[0].Rows.Count>0)
                //{
                //    strIP = ds.Tables[0].Rows[0][RadarIP.m_RadarIP].ToString();
                //    for (int i = 1; i < ds.Tables[0].Rows.Count;i++ )
                //    {
                //         strIP = strIP+","+ ds.Tables[0].Rows[i][RadarIP.m_RadarIP].ToString();
                //    }

                //}
                return ds;
            }
            catch (System.Exception ex)
            {
                return null;
            }

        }
        public DataSet selectCentrlIp()
        {
            try
            {
                DataSet ds = new DataSet();

                g_CAccess.AccessConect();
                string str = "select * from CentreIP;";
                g_CAccess.ExecuteGetSet(str, ref ds);
              
                return ds;
            }
            catch (System.Exception ex)
            {
                return null;
            }

        }
        #endregion
        private byte checkXOR(byte[] TxData)
        {
            byte byteResult;
            byteResult = TxData[0];
            for (int i = 1; i < TxData.Length; i++)
            {
                byteResult ^= TxData[i];
            }
            return byteResult;
        }
        public string CombinHETTM(target targetTmp)
        {
            string strHettm = "";
            string xor = "";
            strHettm = "$HETTM," + targetTmp.IP;
            strHettm += "," + targetTmp.radarID.ToString();
            strHettm += "," + targetTmp.targetID.ToString();
            strHettm += "," + targetTmp.ReportTime.ToString("HHmmss");
            strHettm += "," + targetTmp.Latitude.ToString();
            strHettm += "," + targetTmp.Longitude.ToString();
            strHettm += "," + targetTmp.Speed.ToString();
            strHettm += "," + targetTmp.Bearing.ToString();
            strHettm += "," + targetTmp.Bearing2.ToString();
            strHettm += "," + targetTmp.Course.ToString();
            strHettm += "," + targetTmp.TorR.ToString();
            strHettm += "," + targetTmp.isInRegin.ToString();
            strHettm += "," + targetTmp.State.ToString();
            strHettm += "," + targetTmp.Capture.ToString();
            byte[] byteHettm = System.Text.Encoding.Default.GetBytes(strHettm.Remove(0,1));
            xor = Convert.ToString(checkXOR(byteHettm), 16);
            strHettm += "*"+xor;
            strHettm += Environment.NewLine;
            return strHettm;
        }
                /// <summary>
/// 数字转ASCII
 /// </summary>
 /// <param name="Integer">单个位整数</param>
/// <returns>ASCII</returns>
         public byte Integer2Char(int Integer)
         {
             byte lcv_ch = 0;
             if (Integer <= 9)
             {
                  lcv_ch = Convert.ToByte(Integer + NUM_0_ASCII);
             }
             else if ((Integer >= 0x0A) && (Integer <= 0x0F))
             {
                lcv_ch = Convert.ToByte(Integer + NUM_A_ASCII);
             }
             return lcv_ch;
         }
               /// <summary>
             /// sum效验
             /// </summary>
             /// <param name="array">效验数组</param>
             /// <returns>效验值，字符被拆分为两个ASCII码整和为一个Int，高位在int高8位，低后</returns>
         public int CheckSum(byte[] array)
         {
             byte sum = 0;
             int res = 0;
             int i;
             for (i = 1; (array[i] != SUM_END) && (i < array.Length); i++ )
             {
                 sum ^= array[i];
             }

             if (i != array.Length)
             res = (Integer2Char((sum >> 4)) << 8) | Integer2Char(sum & 0xF);
             return res;
         }
         public Image m_radarimg;
         public void receiveEchoPic(ref  Dictionary<long, RadarEchoPic> dicRadarEchoPicCollection, RadarEchoPic targettmp, byte[] PicData)
         {
             try
             {
                 RadarEchoPic device = null;

              //   GC.Collect();//回收内存，尝试能否解决OutOfmemery

                

                 if (dicRadarEchoPicCollection.TryGetValue(targettmp.ComparedKey, out device))
                 {


                     device.picData.AddRange(PicData);


                     device.m_dataCount++;
                   //  Debug.WriteLine("---count=" + device.Count.ToString() + "----index=" + targettmp.Index.ToString() + "-----" + device.m_dataCount.ToString());
                     if (device.Count == device.m_dataCount + 1)
                     {
                         byte[] data = device.picData.ToArray();
                         device.picData.Clear();
                         string path = Application.StartupPath;
                         MemoryStream ms = new MemoryStream();
                         ms.Write(data, 0, data.Length);

                         m_radarimg = Image.FromStream(ms, true);

                         // string fullPath = path + "\\images\\" + Guid.NewGuid().ToString() + ".png";
                         //  string fullPath = path + "\\images\\" + device.ComparedKey.ToString() + ".png";
                         string webPath = "E:\\app\\projects\\vts\\website\\images\\";
                         // string urlPath = "http://172.19.10.177/radarImage/" + device.ComparedKey.ToString() + ".png";
                         string urlPath = "http://172.19.10.35/RadarImage/" + device.ComparedKey.ToString() + ".png";
                         string fullPath = webPath + targettmp.ComparedKey.ToString() + ".png";
                         //  string fullPath = path + "\\images\\" + targettmp.ComparedKey.ToString() + ".png";
                         FileStream file = new FileStream(fullPath, FileMode.Create);
                         ms.WriteTo(file);
                         ms.Close();
                         file.Close();

                         jsonParse.RadarPicPath pathMessage = new jsonParse.RadarPicPath();
                         pathMessage.CollTime = device.ReportTime;
                         pathMessage.Lat = device.Latitude;
                         pathMessage.Lon = device.Longitude;
                         pathMessage.Pixel = (int)device.Range;
                         pathMessage.Radars = device.IP;
                         pathMessage.Path = urlPath;

                         //  pathMessage.UniqueCode = device.IP;

                         string ssendJonMessage = jsonParse.GetJosonMessage(pathMessage);

                         // m_asyncudpServer.m_sListen.SendTo(System.Text.Encoding.Default.GetBytes(ssendJonMessage), ssendJonMessage.Length, SocketFlags.None, m_asyncudpServer.m_ippoint);

                         udpSendCentrlIP(ssendJonMessage);
                         //m_dicRadarEchoPicCollection.Remove(device.ComparedKey);
                         dicRadarEchoPicCollection.Clear();
                        // m_tcpClient.SendData(ssendJonMessage);

                         //ichTextBox1.Text = fullPath;
                         //   img.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);

                     }
                 }
                 else
                 {
                     device = new RadarEchoPic();
                     device = targettmp;
                     //赋值深拷贝
                     // device.ComparedKey = targettmp.ComparedKey;
                     //device.Count = targettmp.Count;
                     //device.EchoDataTotal = targettmp.EchoDataTotal;
                     //device.Index = targettmp.Index;
                     //device.IP = targettmp.IP;
                     //device.Latitude = targettmp.Latitude;
                     //device.Longitude = targettmp.Longitude;
                     //device.picData = targettmp.picData;
                     //device.radarID = targettmp.radarID;
                     //device.radarPngPic = targettmp.radarPngPic;
                     //device.Range = targettmp.Range;
                     //device.ReportTime = targettmp.ReportTime;
                     //device.targetID = targettmp.targetID;

                     dicRadarEchoPicCollection.Add(device.ComparedKey, device);
                     device.picData.AddRange(PicData);
                     if (device.Count == 1)
                     {
                         byte[] data = device.picData.ToArray();
                         device.picData.Clear();//防止时间长内存溢出

                         string path = Application.StartupPath;
                         MemoryStream ms = new MemoryStream();
                         ms.Write(data, 0, data.Length);

                         m_radarimg = Image.FromStream(ms, true);



                         // string urlPath = "http://172.19.10.177/radarImage/" + device.ComparedKey.ToString() + ".png";
                         string urlPath = "http://172.19.10.35/RadarImage/" + device.ComparedKey.ToString() + ".png";
                         string webPath = "E:\\app\\projects\\vts\\website\\images\\";
                         // string fullPath = path + "\\images\\" + Guid.NewGuid().ToString() + ".png";
                         //  string fullPath = path + "\\images\\" + targettmp.ComparedKey.ToString() + ".png";
                         string fullPath = webPath + device.ComparedKey.ToString() + ".png";

                         FileStream file = new FileStream(fullPath, FileMode.Create);
                         ms.WriteTo(file);
                         ms.Close();
                         file.Close();


                         jsonParse.RadarPicPath pathMessage = new jsonParse.RadarPicPath();
                         pathMessage.CollTime = device.ReportTime;
                         pathMessage.Lat = device.Latitude;
                         pathMessage.Lon = device.Longitude;
                         pathMessage.Pixel = (int)device.Range; ;
                         pathMessage.Path = urlPath;
                         pathMessage.Radars = device.IP;
                         //   pathMessage.UniqueCode = device.IP;

                         string ssendJonMessage = jsonParse.GetJosonMessage(pathMessage);

                         // m_asyncudpServer.m_sListen.SendTo(System.Text.Encoding.Default.GetBytes(ssendJonMessage), ssendJonMessage.Length, SocketFlags.None, m_asyncudpServer.m_ippoint);

                         udpSendCentrlIP(ssendJonMessage);

                         // m_dicRadarEchoPicCollection.Remove(device.ComparedKey);
                         dicRadarEchoPicCollection.Clear();//不清空，随着时间累积，内存不断增加。
                         //ichTextBox1.Text = fullPath;
                         //   img.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);

                     }
                 }
             }
             catch (OutOfMemoryException)
             {
                 GC.Collect();
             }
         }
         public void addRadarEchoPic(RadarEchoPic targettmp,byte[] PicData)
         {

             int id = targettmp.radarID;
             if (id>m_radarCount)
             {
                 return;//超出初始化数组，退出
             }
             receiveEchoPic(ref m_dicRadarEchoPicCollectionMultil[id-1], targettmp, PicData);
             //switch (id)
             //{
             //    case 1:
             //        {
             //            receiveEchoPic(ref m_dicRadarEchoPicCollection, targettmp, PicData);
             //            break;
             //        }
             //    case 2:
             //        {
             //            receiveEchoPic(ref m_dicRadarEchoPicCollection1, targettmp, PicData);
             //            break;
             //        }
             //    case 3:
             //        {
             //            receiveEchoPic(ref m_dicRadarEchoPicCollection2, targettmp, PicData);
             //            break;
             //        }
             //    case 4:
             //        {
             //            break;
             //        }
             //    case 5:
             //        {
             //            break;
             //        }
             //    case 6:
             //        {
             //            break;
             //        }
             //    default:
             //        {
             //            MessageBox.Show(id.ToString());
             //            break;
             //        }

             //}
         }
         public void addTarget(target targettmp)
         {
             //target device = null;
             //lock(m_dicTargetCollection)
             //{
             // //   Monitor.Wait(m_dicTargetCollection);
             //    if(readerFalg)
             //    {
             //        try
             //        {
             //            Monitor.Wait(this.m_dicTargetCollection);
             //        }
             //        catch (SynchronizationLockException e)
             //        {
             //            //当同步方法（指Monitor类除Enter之外的方法）在非同步的代码区被调用
             //            Console.WriteLine(e);
             //        }
             //    }

             //    if (m_dicTargetCollection.TryGetValue(targettmp.ComparedKey, out device))
             //    {
                
             //        device = targettmp;
             //    }
             //    else
             //    {
                 
             //        m_dicTargetCollection.Add(targettmp.ComparedKey, targettmp);
                 
             //    }
             //    readerFalg = true;

             //    Monitor.Pulse(m_dicTargetCollection);
             //}
             target device = null;


             if (m_dicTargetCollection.TryGetValue(targettmp.ComparedKey, out device))
             {

                 device = targettmp;
             }
             else
             {

                 m_dicTargetCollection.Add(targettmp.ComparedKey, targettmp);

             }
             
             
         }
         public void addAisArpaTarget(AisArpaTarget targettmp)
         {
             AisArpaTarget device = null;
             if (m_dicAisArpaTargetCollection.TryGetValue(targettmp.ComparedKey, out device))
             {

                 //device.ReportTime = targettmp.ReportTime;

                 //device.Latitude = targettmp.Latitude;
                 //device.Longitude = targettmp.Longitude;
                 //device.Bearing = targettmp.Bearing;
                 device = targettmp;
                

             }
             else
             {

                 m_dicAisArpaTargetCollection.Add(targettmp.ComparedKey, targettmp);

             }
         }
        /// <summary>
        /// lat1,lng1,主雷达圆心位置，lat2,lng2:次雷达目标位置
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lng1"></param>
        /// <param name="lat2"></param>
        /// <param name="lng2"></param>
        /// <returns></returns>
         public bool IsInCircle(double lat1,double lng1,double lat2,double lng2)
         {
             bool res = true;
             double distance = CommonFunctions.GetDistance(lat1, lng1, lat2, lng2);
             double R = 96 * 1852;//1海里等于1852米
             if (distance>R)
             {
                 res = false;//不在主雷达范围内，该点不用去掉。
             }
             else
             {
                 res = true;
             }
          
             return res;
         }
         public void DataTcpSend(byte[] receiveBuffer,int length)
         {
              m_tcpClient.SendDataByte(receiveBuffer,length);
         }
                 /// <summary>
         /// 从接收到的字符串中，取出有用数据
         /// </summary>
         /// <param name="str">接收到的字符串</param>
         public  string DataProcess(string str,byte[] receiveBuffer)
         {

             char[] chSplit = {',','*'};
             string[] strArray = str.Split(chSplit);
             m_target = new target();
             m_AisArpaTarge = new AisArpaTarget();
           
             int length = str.Length;
             switch (strArray[0])
             {
                 case "$HETTM":
                     {
                         m_target.IP = (strArray[1]);
                         m_target.radarID = int.Parse(strArray[2]);
                         m_target.targetID = int.Parse(strArray[3]);
                         m_target.ReportTime = DateTime.ParseExact(strArray[4], "HHmmss", System.Globalization.CultureInfo.CurrentCulture);
                         m_target.Latitude = double.Parse(strArray[5]);
                         m_target.Longitude = double.Parse(strArray[6]);
                         m_target.Speed = double.Parse(strArray[7]);
                         m_target.Bearing = double.Parse(strArray[8]);
                         m_target.Bearing2 = double.Parse(strArray[9]);
                         m_target.Course = double.Parse(strArray[10]);
                         m_target.TorR = char.Parse(strArray[11]);
                         m_target.isInRegin = int.Parse(strArray[12]);
                         m_target.State = char.Parse(strArray[13]);
                         m_target.Capture = char.Parse(strArray[14]);
                         if (m_target.radarID!=1)//雷达编号不为1，表示是次雷达过来的目标
                         {
                              if (IsInCircle(m_MainLat,m_MainLng,m_target.Latitude,m_target.Longitude))//在两部雷达公共区域内去掉
                             {

                                 break;//不加入融合目标的字典里
                             }
                         }
                         jsonParse.RadarMessage radarMessage = new jsonParse.RadarMessage();
                         RadarEchoPic TmpRadarEchoPic = new RadarEchoPic();
                         radarMessage.TargetID = m_target.targetID.ToString();
                         radarMessage.CollTime = m_target.ReportTime;
                         radarMessage.Lat = m_target.Latitude;
                         radarMessage.Lon = m_target.Longitude;
                         radarMessage.VEL = m_target.Speed;
                         radarMessage.Position = m_target.Bearing2;
                         radarMessage.Direction = m_target.Course;
                         radarMessage.PType = m_target.TorR.ToString();
                         radarMessage.IsAlarmArea = m_target.isInRegin;
                         //for (int i = 0; i < m_radarCount; i++)
                         //{
                         //    if (m_dicRadarEchoPicCollectionMultil[i].TryGetValue(m_target.ComparedKey, out TmpRadarEchoPic))
                         //    {
                         //        radarMessage.Mileage = (int)TmpRadarEchoPic.Range;
                         //    }
                         //}
                       

                         radarMessage.Mileage = (int)m_target.Bearing;

                         radarMessage.DataType = m_target.State.ToString();
                         radarMessage.TrackType = m_target.Capture.ToString();
                         radarMessage.ProtocolNo = "00";

                         string jsonMessage = jsonParse.GetRadarMessage(radarMessage);
                       
                         jsonMessage = "~" + jsonMessage + "~";
                       
                       //  m_tcpClient.SendData(jsonMessage);
                         udpSendCentrlIP(jsonMessage);

                         addTarget(m_target);
                        // Console.WriteLine(item.Key.ToString() + "  " + item.Value.IP);
                      
                         
                     //    m_asyncudpServer.m_sListen.SendTo(System.Text.Encoding.Default.GetBytes(strHttm), strHttm.Length, SocketFlags.None, m_asyncudpServer.m_ippoint);
                         break;
                     }
                 case "$HEUNI":
                     {
                         m_AisArpaTarge.IP = (strArray[1]);
                         m_AisArpaTarge.radarID = int.Parse(strArray[2]);
                         m_AisArpaTarge.MMSI = int.Parse(strArray[3]);
                         m_AisArpaTarge.targetID = int.Parse(strArray[4]);
                         m_AisArpaTarge.ReportTime = DateTime.ParseExact(strArray[5], "HHmmss", System.Globalization.CultureInfo.CurrentCulture);
                         m_AisArpaTarge.Latitude = double.Parse(strArray[6]);
                         m_AisArpaTarge.Longitude = double.Parse(strArray[7]);
                         m_AisArpaTarge.Bearing = double.Parse(strArray[8]);
                         m_AisArpaTarge.Speed = double.Parse(strArray[9]);
                         m_AisArpaTarge.Course = double.Parse(strArray[10]);
                         m_AisArpaTarge.TSpeed = double.Parse(strArray[11]);
                         m_AisArpaTarge.TCourse = double.Parse(strArray[12]);
                         m_AisArpaTarge.name = strArray[13];
                         m_AisArpaTarge.type = strArray[14];

                         if (m_target.radarID != 1)//雷达编号不为1，表示是次雷达过来的目标
                         {
                             if (IsInCircle(m_MainLat, m_MainLng, m_target.Latitude, m_target.Longitude))//在两部雷达公共区域内去掉
                             {

                                 break;//不加入融合目标的字典里
                             }
                         }

                         addTarget(m_target);

                         break;
                     }
                 case "$HERIM":
                     {
                         RadarEchoPic radarEchoPic = null;
                         radarEchoPic = new RadarEchoPic();//尝试初始化的实例一个对象，解决长时间运行收到图像数据,LIST导致OutMemeary异常 见网上： C#:.NET陷阱之五：奇怪的OutOfMemoryException----大对象堆引起的问题与对策。
                       
                         radarEchoPic.IP = (strArray[1]);
                         radarEchoPic.radarID = int.Parse(strArray[2]);
                         radarEchoPic.targetID = int.Parse(strArray[3]);
                         radarEchoPic.ReportTime = DateTime.ParseExact(strArray[4], "HHmmss", System.Globalization.CultureInfo.CurrentCulture);
                         radarEchoPic.Latitude = double.Parse(strArray[5]);
                         radarEchoPic.Longitude = double.Parse(strArray[6]);
                         radarEchoPic.Range = double.Parse(strArray[7]);
                         radarEchoPic.Count = int.Parse(strArray[8]);
                         radarEchoPic.Index = int.Parse(strArray[9]);
                         radarEchoPic.EchoDataTotal = int.Parse(strArray[10]);

                         byte[] dataTmp = new byte[radarEchoPic.EchoDataTotal];

                         int index = str.IndexOf('d');
                         Array.Copy(receiveBuffer, index+1, dataTmp, 0, radarEchoPic.EchoDataTotal);
                         
                         //string strData = "";
                         //for (int i=11;i<strArray.Length-2;i++)
                         //{
                         //   // listEchoData.Add(strArray[i]);
                         //    strData = strData + strArray[i];
                         //}
                         //strData = strData.Remove(0, 1); 
                         //strData = strData.Remove(strData.Length - 1, 1);
                         //byte[] dataTmp = System.Text.Encoding.Default.GetBytes(strData);
                       //  int index = str.IndexOf(',')
                        addRadarEchoPic(radarEchoPic, dataTmp);
                   
                        
                         break;
                     }
                 default: break;
             }
             return strArray[0];
        }

    }
 
    public class AisArpaTarget
    {
        /// <summary>
        /// 返回目标与雷达站结合的标识ID
        /// </summary>
        /// <param name="targetID">MMSI</param>
        /// <param name="regionID">区域ID</param>
        /// <returns>标识ID</returns>
        public static Int64 CreateTargetRegionKey(int targetID, int radarID)
        {
            return (Int64)radarID * 1000000000 + targetID;
        }

        /// <summary>
        /// 根据结合的标识ID来分解目标/雷达站                                                     
        /// </summary>
        /// <param name="longKey"></param>
        /// <param name="?"></param>
        /// <param name="?"></param>
        public static void DecodeTargetRegionKey(Int64 longKey, out int targetID, out int radarID)
        {
            radarID = (int)(longKey / 1000000000.0);
            targetID = (int)(longKey - radarID * 1000000000);
        }

        Int64 m_ComparedKey = 0;
        /// <summary>
        /// 获取与对象的合并标识号
        /// </summary>
        public System.Int64 ComparedKey
        {
            get
            {
                if (m_ComparedKey == 0)
                {
                    m_ComparedKey = CreateTargetRegionKey(m_targetID, m_radarID);
                }
                return m_ComparedKey;
            }
        }
        double m_ClosestDistance = -1;
        /// <summary>
        /// 最近的距离值
        /// </summary>
        public double ClosestDistance
        {
            get
            {
                return m_ClosestDistance;
            }
            set
            {
                m_ClosestDistance = value;
            }
        }



        /************************************************* 以下按雷达站与雷达融合服务器协议V0.2****************************************************************************************/
        string m_IP = "";
        public string IP
        {
            get
            {
                return m_IP;
            }
            set
            {
                m_IP = value;
            }
        }
        int m_radarID = 0;
        /// <summary>
        /// 雷达站的ID号
        /// </summary>
        public int radarID
        {
            get
            {
                return m_radarID;
            }
            set
            {
                m_radarID = value;
            }
        }
        int m_MMSI = 0;
        /// <summary>
        /// 船舶的MMSI号
        /// </summary>
        public int MMSI
        {
            get
            {
                return m_MMSI;
            }
            set
            {
                m_MMSI = value;
            }
        }
        int m_targetID = 0;
        /// <summary>
        /// 目标的ID号
        /// </summary>
        public int targetID
        {
            get
            {
                return m_targetID;
            }
            set
            {
                m_targetID = value;
            }
        }
        DateTime m_ReportTime = DateTime.Now;
        /// <summary>
        /// 获取信息的时间
        /// </summary>
        public System.DateTime ReportTime
        {
            get
            {
                return m_ReportTime;
            }
            set
            {
                m_ReportTime = value;
            }
        }
        double m_Latitude = double.MaxValue;
        public double Latitude
        {
            get
            {
                return m_Latitude;
            }
            set
            {
                m_Latitude = value;
            }
        }
        double m_Longitude = double.MaxValue;
        public double Longitude
        {
            get
            {
                return m_Longitude;
            }
            set
            {
                m_Longitude = value;
            }
        }
        private double m_Bearing = 0;
        /// <summary>
        /// 方位
        /// </summary>
        public double Bearing
        {
            get
            {
                return m_Bearing;
            }
            set
            {
                m_Bearing = value;
            }
        }
        private double m_Speed = 0;
        /// <summary>
        /// 航速
        /// </summary>
        public double Speed
        {
            get
            {
                return m_Speed;
            }
            set
            {
                m_Speed = value;
            }
        }
        
      
       
        private double m_Course = 0;
        /// <summary>
        /// 航向2
        /// </summary>
        public double Course
        {
            get
            {
                return m_Course;
            }
            set
            {
                m_Course = value;
            }
        }
        private double m_TSpeed = 0;
        /// <summary>
        /// 绝对航速
        /// </summary>
        public double TSpeed
        {
            get
            {
                return m_TSpeed;
            }
            set
            {
                m_TSpeed = value;
            }
        }
        private double m_TCourse = 0;
        /// <summary>
        /// 航向2
        /// </summary>
        public double TCourse
        {
            get
            {
                return m_TCourse;
            }
            set
            {
                m_TCourse = value;
            }
        }
        string m_name ="";
        public string name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }
        string m_type = "";
        public string type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
            }
        }
    }
    public class RadarEchoPic
    {

        /// <summary>
        /// 返回目标与雷达站结合的标识ID
        /// </summary>
        /// <param name="targetID">MMSI</param>
        /// <param name="regionID">区域ID</param>
        /// <returns>标识ID</returns>
        public static Int64 CreateTargetRegionKey(int targetID, int radarID)
        {
            return (Int64)radarID * 1000000000 + targetID;
        }

        /// <summary>
        /// 根据结合的标识ID来分解目标/雷达站                                                     
        /// </summary>
        /// <param name="longKey"></param>
        /// <param name="?"></param>
        /// <param name="?"></param>
        public static void DecodeTargetRegionKey(Int64 longKey, out int targetID, out int radarID)
        {
            radarID = (int)(longKey / 1000000000.0);
            targetID = (int)(longKey - radarID * 1000000000);
        }

        Int64 m_ComparedKey = 0;
        /// <summary>
        /// 获取与对象的合并标识号
        /// </summary>
        public System.Int64 ComparedKey
        {
            get
            {
                if (m_ComparedKey == 0)
                {
                    m_ComparedKey = CreateTargetRegionKey(m_targetID, m_radarID);
                }
                return m_ComparedKey;
            }
        }
        double m_ClosestDistance = -1;
        /// <summary>
        /// 最近的距离值
        /// </summary>
        public double ClosestDistance
        {
            get
            {
                return m_ClosestDistance;
            }
            set
            {
                m_ClosestDistance = value;
            }
        }



        /************************************************* 以下按雷达站与雷达融合服务器协议V0.2****************************************************************************************/
        string m_IP = "";
        public string IP
        {
            get
            {
                return m_IP;
            }
            set
            {
                m_IP = value;
            }
        }
        int m_radarID = 0;
        /// <summary>
        /// 雷达站的ID号
        /// </summary>
        public int radarID
        {
            get
            {
                return m_radarID;
            }
            set
            {
                m_radarID = value;
            }
        }
        int m_targetID = 0;
        /// <summary>
        ///图像编号
        /// </summary>
        public int targetID
        {
            get
            {
                return m_targetID;
            }
            set
            {
                m_targetID = value;
            }
        }
        DateTime m_ReportTime = DateTime.Now;
        /// <summary>
        /// 获取信息的时间
        /// </summary>
        public System.DateTime ReportTime
        {
            get
            {
                return m_ReportTime;
            }
            set
            {
                m_ReportTime = value;
            }
        }
        double m_Latitude = double.MaxValue;
        public double Latitude
        {
            get
            {
                return m_Latitude;
            }
            set
            {
                m_Latitude = value;
            }
        }
        double m_Longitude = double.MaxValue;
        public double Longitude
        {
            get
            {
                return m_Longitude;
            }
            set
            {
                m_Longitude = value;
            }
        }
        private double m_Range = 0;
        /// <summary>
        /// 量程
        /// </summary>
        public double Range
        {
            get
            {
                return m_Range;
            }
            set
            {
                m_Range = value;
            }
        }
        
        public Image radarPngPic;
        public List<byte> picData = new List<byte>();
        private int m_Count = 0;
        public int Count
        {
            get
            {
                return m_Count;
            }
            set
            {
                m_Count = value;
            }
        }
        private int m_Index = 0;
        public int Index
        {
            get
            {
                return m_Index;
            }
            set
            {
                m_Index = value;
            }
        }
        private int m_EchoDataTotal = 0;
        public int EchoDataTotal
        {
            get
            {
                return m_EchoDataTotal;
            }
            set
            {
                m_EchoDataTotal = value;
            }
        }
        public int m_dataCount;
    }
    public class target
    {
       
        /// <summary>
        /// 返回目标与雷达站结合的标识ID
        /// </summary>
        /// <param name="targetID">MMSI</param>
        /// <param name="regionID">区域ID</param>
        /// <returns>标识ID</returns>
        public static Int64 CreateTargetRegionKey(int targetID, int radarID)
        {
            return (Int64)radarID * 1000000000 + targetID;
        }

        /// <summary>
        /// 根据结合的标识ID来分解目标/雷达站                                                     
        /// </summary>
        /// <param name="longKey"></param>
        /// <param name="?"></param>
        /// <param name="?"></param>
        public static void DecodeTargetRegionKey(Int64 longKey, out int targetID, out int radarID)
        {
            radarID = (int)(longKey / 1000000000.0);
            targetID = (int)(longKey - radarID * 1000000000);
        }

        Int64 m_ComparedKey = 0;
        /// <summary>
        /// 获取与对象的合并标识号
        /// </summary>
        public System.Int64 ComparedKey
        {
            get
            {
                if (m_ComparedKey == 0)
                {
                    m_ComparedKey = CreateTargetRegionKey(m_targetID, m_radarID);
                }
                return m_ComparedKey;
            }
        }
        double m_ClosestDistance = -1;
        /// <summary>
        /// 最近的距离值
        /// </summary>
        public double ClosestDistance
        {
            get
            {
                return m_ClosestDistance;
            }
            set
            {
                m_ClosestDistance = value;
            }
        }


      
/************************************************* 以下按雷达站与雷达融合服务器协议V0.2****************************************************************************************/
        string m_IP = "";
        public string IP
        {
            get
            {
                return m_IP;
            }
            set
            {
                m_IP = value;
            }
        }
        int m_radarID = 0;
        /// <summary>
        /// 雷达站的ID号
        /// </summary>
        public int radarID
        {
            get
            {
                return m_radarID;
            }
            set
            {
                m_radarID = value;
            }
        }
        int m_targetID = 0;
        /// <summary>
        /// 目标的ID号
        /// </summary>
        public int targetID
        {
            get
            {
                return m_targetID;
            }
            set
            {
                m_targetID = value;
            }
        }
        DateTime m_ReportTime = DateTime.Now;
        /// <summary>
        /// 获取信息的时间
        /// </summary>
        public System.DateTime ReportTime
        {
            get
            {
                return m_ReportTime;
            }
            set
            {
                m_ReportTime = value;
            }
        }
        double m_Latitude = double.MaxValue;
        public double Latitude
        {
            get
            {
                return m_Latitude;
            }
            set
            {
                m_Latitude = value;
            }
        }
        double m_Longitude = double.MaxValue;
        public double Longitude
        {
            get
            {
                return m_Longitude;
            }
            set
            {
                m_Longitude = value;
            }
        }
        private double m_Speed = 0;
        /// <summary>
        /// 航速
        /// </summary>
        public double Speed
        {
            get
            {
                return m_Speed;
            }
            set
            {
                m_Speed = value;
            }
        }
        private double m_Bearing = 0;
        /// <summary>
        /// 方位
        /// </summary>
        public double Bearing
        {
            get
            {
                return m_Bearing;
            }
            set
            {
                m_Bearing = value;
            }
        }
         private double m_Bearing2 = 0;
         /// <summary>
        /// 方位
        /// </summary>
        public double Bearing2
        {
            get
            {
                return m_Bearing2;
            }
            set
            {
                m_Bearing2 = value;
            }
        }
        private double m_Course = 0;
       /// <summary>
        /// 航向2
        /// </summary>
        public double Course
        {
            get
            {
                return m_Course;
            }
            set
            {
                m_Course = value;
            }
        }
       
        int m_MMSI = 0;
        /// <summary>
        /// 船舶的MMSI号
        /// </summary>
        public int MMSI
        {
            get
            {
                return m_MMSI;
            }
            set
            {
                m_MMSI = value;
            }
        }
        char m_TorR = 'T';
        public char TorR
        {
            get
            {
                return m_TorR;
            }
            set
            {
                m_TorR = value;
            }
        }
        int m_isInRegin = 0;//0,1,2,其他值表示不在警戒区内
        public int isInRegin
        {
            get
            {
                return m_isInRegin;
            }
            set
            {
                m_isInRegin = value;
            }
        }
        char m_State = 'T';//跟踪状态
        public char State
        {
            get
            {
                return m_State;
            }
            set
            {
                m_State = value;
            }
        }
        char m_Capture ='T';//捕获方式
        public char Capture
        {
            get
            {
                return m_Capture;
            }
            set
            {
                m_Capture= value;
            }
        }
        /// <summary>
        /// 获取或设置使用BASE64编码的状态信息
        /// </summary>
        public string DangourseStatusString
        {
            get
            {
                // 生成二进制数据
                byte[] buffer = new byte[40];
                using (BinaryWriter writer = new BinaryWriter(new MemoryStream(buffer)))
                {

                }
                // 转换成Base64字符串
                return Convert.ToBase64String(buffer);
            }
            set
            {
                try
                {
                    // 转换二进制数据
                    if ((value != null) && (value != ""))
                    {
                        byte[] buffer = Convert.FromBase64String(value);
                        using (BinaryReader reader = new BinaryReader(new MemoryStream(buffer)))
                        {

                        }
                    }
                }
                catch (System.Exception)
                {
                    // 忽略所有错误
                }
            }
        }

    }
    public class CommonFunctions
    {

        //地球半径，单位米

        private const double EARTH_RADIUS = 6378137;

        /// <summary>

        /// 计算两点位置的距离，返回两点的距离，单位 米

        /// 该公式为GOOGLE提供，误差小于0.2米

        /// </summary>

        /// <param name="lat1">第一点纬度</param>

        /// <param name="lng1">第一点经度</param>

        /// <param name="lat2">第二点纬度</param>

        /// <param name="lng2">第二点经度</param>

        /// <returns></returns>

        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {

            double radLat1 = Rad(lat1);

            double radLng1 = Rad(lng1);

            double radLat2 = Rad(lat2);

            double radLng2 = Rad(lng2);

            double a = radLat1 - radLat2;

            double b = radLng1 - radLng2;

            double result = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2))) * EARTH_RADIUS;

            return result;

        }



        /// <summary>

        /// 经纬度转化成弧度

        /// </summary>

        /// <param name="d"></param>

        /// <returns></returns>

        private static double Rad(double d)
        {

            return (double)d * Math.PI / 180d;

        }
        /// <summary>
        /// 求两点间的距离(快速)
        /// </summary>
        /// <param name="point1">坐标点1</param>
        /// <param name="point2">坐标点2</param>
        /// <returns>表示距离的值的比例</returns>
        public static double FastDistance(CoordinatePoint point1, CoordinatePoint point2)
        {
            return (point1.Latitude - point2.Latitude) * (point1.Latitude - point2.Latitude) + (point1.Longitude - point2.Longitude) * (point1.Longitude - point2.Longitude);
        }

        /// <summary>
        /// 求两点间的距离(快速)
        /// </summary>
        /// <param name="point1">坐标点1</param>
        /// <param name="point2">坐标点2</param>
        /// <returns>表示距离的值的比例</returns>
        public static double FastDistance(double x1, double y1, double x2, double y2)
        {
            return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
        }

      

        /// <summary>
        /// 转换指定的对象为字符串表现形式
        /// </summary>
        /// <param name="value">对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>对象的字符串表现形式</returns>
        public static string TryConvertValue(object value, string defaultValue)
        {
            try
            {
                return value.ToString();
            }
            catch (System.Exception)
            {
                return defaultValue;
            }
        }
        /// <summary>
        /// 转换指定的对象为整型值
        /// </summary>
        /// <param name="value">对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>对象的整型值</returns>
        public static int TryConvertInt32(object value, int defaultValue)
        {
            try
            {
                if (value is System.DBNull)
                {
                    return defaultValue;
                }
                return Convert.ToInt32(value.ToString());
            }
            catch (System.Exception)
            {
                return defaultValue;
            }
        }
        /// <summary>
        /// 转换指定的对象为双精度值
        /// </summary>
        /// <param name="value">对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>对象的双精度值</returns>
        public static double TryConvertDouble(object value, double defaultValue)
        {
            try
            {
                return Convert.ToDouble(value.ToString());
            }
            catch (System.Exception)
            {
                return defaultValue;
            }
        }
        /// <summary>
        /// 转换指定的对象为单精度值
        /// </summary>
        /// <param name="value">对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>对象的单精度值</returns>
        public static DateTime TryConvertDateTime(object value, DateTime defaultValue)
        {
            try
            {
                return Convert.ToDateTime(value.ToString());
            }
            catch (System.Exception)
            {
                return defaultValue;
            }
        }
        /// <summary>
        /// 转换指定的对象为单精度值
        /// </summary>
        /// <param name="value">对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>对象的单精度值</returns>
        public static float TryConvertSigle(object value, float defaultValue)
        {
            try
            {
                return Convert.ToSingle(value.ToString());
            }
            catch (System.Exception)
            {
                return defaultValue;
            }
        }
        /// <summary>
        /// 转换指定的对象为布尔值
        /// </summary>
        /// <param name="value">对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>对象的布尔值</returns>
        public static bool TryConvertBoolean(object value, bool defaultValue)
        {
            try
            {
                if (value is int)
                {
                    return Convert.ToBoolean(value);
                }
                else
                {
                    return Convert.ToBoolean(value.ToString());
                }
            }
            catch (System.Exception)
            {
                return defaultValue;
            }
        }
        public static string LatitudeToString(double latitude)
        {
            // 检验数据值的有效性
            if ((latitude > -90) && (latitude < 90))
            {
                string latitudeString = "";
                if (latitude >= 0)
                {
                    // 北纬
                    int degree = (int)latitude;
                    double minute = (latitude - degree) * 60;
                    latitudeString = String.Format("{0}°{1:00.000}'N", degree, minute);
                }
                else
                {
                    // 南纬
                    latitude *= -1;
                    int degree = (int)latitude;
                    double minute = (latitude - degree) * 60;
                    latitudeString = String.Format("{0}°{1:00.000}'S", degree, minute);
                }

                return latitudeString;
            }

            return "****";
        }

        public static bool StringToLatitude(string latitudeString, ref double latitude)
        {
            latitudeString = latitudeString.Trim();
            // 判断参数是否有效
            if (latitudeString.Length > 3)
            {
                int degree = 91;
                float minute = 60;
                string pared = "";

                string[] fileds = latitudeString.Split(new char[] { '°', '\'' });
                if (fileds.Length == 3)
                {
                    degree = Convert.ToInt32(fileds[0]);
                    minute = Convert.ToSingle(fileds[1]);
                    pared = fileds[2];

                    latitude = degree + minute / 60.0;
                    if ((pared == "S") || (pared == "s"))
                    {
                        latitude *= -1;

                        // 检验数据值范围
                        if ((latitude > -90) && (latitude < 90) && (minute < 60) && (degree >= 0))
                        {
                            return true;
                        }
                    }
                    else if ((pared == "N") || (pared == "n"))
                    {
                        // 检验数据值范围
                        if ((latitude > -90) && (latitude < 90) && (minute < 60) && (degree >= 0))
                        {
                            return true;
                        }
                    }
                }
            }

            // 无效输入!
            latitude = 91;
            return false;
        }

        public static string LongitudeToString(double longitude)
        {
            // 检验数据值的有效性
            if ((longitude > -180) && (longitude < 180))
            {
                string longitudeString = "";
                if (longitude >= 0)
                {
                    // 东经
                    int degree = (int)longitude;
                    double minute = (longitude - degree) * 60;
                    longitudeString = String.Format("{0}°{1:00.000}'E", degree, minute);
                }
                else
                {
                    // 西经
                    longitude *= -1;
                    int degree = (int)longitude;
                    double minute = (longitude - degree) * 60;
                    longitudeString = String.Format("{0}°{1:00.000}'W", degree, minute);
                }

                return longitudeString;
            }

            return "****";
        }

        public static bool StringToLongitude(string longitudeString, ref double longitude)
        {
            longitudeString = longitudeString.Trim();
            // 判断参数是否有效
            if (longitudeString.Length > 3)
            {
                int degree = 181;
                float minute = 60;
                string pared = "";

                string[] fileds = longitudeString.Split(new char[] { '°', '\'' });
                if (fileds.Length == 3)
                {
                    degree = Convert.ToInt32(fileds[0]);
                    minute = Convert.ToSingle(fileds[1]);
                    pared = fileds[2];

                    longitude = degree + minute / 60.0;
                    if ((pared == "W") || (pared == "w"))
                    {
                        longitude *= -1;

                        // 检验数据值范围
                        if ((longitude > -180) && (longitude < 180) && (minute < 60) && (degree >= 0))
                        {
                            return true;
                        }
                    }
                    else if ((pared == "E") || (pared == "e"))
                    {
                        // 检验数据值范围
                        if ((longitude > -180) && (longitude < 180) && (minute < 60) && (degree >= 0))
                        {
                            return true;
                        }
                    }
                }
            }

            // 无效输入!
            longitude = 181;
            return false;
        }
    }
    /// <summary>
    /// 坐标值
    /// </summary>
    public class CoordinatePoint
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        public CoordinatePoint(double latitude, double longitude)
        {
            m_Latitude = latitude;
            m_Longitude = longitude;
        }

        public CoordinatePoint()
        {
            m_Latitude = double.MinValue;
            m_Longitude = double.MinValue;
        }

        /// <summary>
        /// 指示数据是否有效
        /// </summary>
        public bool IsVaild
        {
            get
            {
                return (m_Latitude > -90) && (m_Latitude < 90) && (m_Longitude > -180) && (m_Longitude < 180);
            }
        }

        double m_Latitude;
        /// <summary>
        /// 纬度信息
        /// </summary>
        public double Latitude
        {
            get
            {
                return m_Latitude;
            }
            set
            {
                m_Latitude = value;
            }
        }
        double m_Longitude;
        /// <summary>
        /// 经度信息
        /// </summary>
        public double Longitude
        {
            get
            {
                return m_Longitude;
            }
            set
            {
                m_Longitude = value;
            }
        }
    }
    public class AISTarget
    {
        public static string LatitudeToString(double latitude)
        {
            // 检验数据值的有效性
            if ((latitude > -90) && (latitude < 90))
            {
                string latitudeString = "";
                if (latitude >= 0)
                {
                    // 北纬
                    int degree = (int)latitude;
                    double minute = (latitude - degree) * 60;
                    latitudeString = String.Format("{0}°{1:00.000}'N", degree, minute);
                }
                else
                {
                    // 南纬
                    latitude *= -1;
                    int degree = (int)latitude;
                    double minute = (latitude - degree) * 60;
                    latitudeString = String.Format("{0}°{1:00.000}'S", degree, minute);
                }

                return latitudeString;
            }

            return "****";
        }

        public static bool StringToLatitude(string latitudeString, ref double latitude)
        {
            latitudeString = latitudeString.Trim();
            // 判断参数是否有效
            if (latitudeString.Length > 3)
            {
                int degree = 91;
                float minute = 60;
                string pared = "";

                string[] fileds = latitudeString.Split(new char[] { '°', '\'' });
                if (fileds.Length == 3)
                {
                    degree = Convert.ToInt32(fileds[0]);
                    minute = Convert.ToSingle(fileds[1]);
                    pared = fileds[2];

                    latitude = degree + minute / 60.0;
                    if ((pared == "S") || (pared == "s"))
                    {
                        latitude *= -1;

                        // 检验数据值范围
                        if ((latitude > -90) && (latitude < 90) && (minute < 60) && (degree >= 0))
                        {
                            return true;
                        }
                    }
                    else if ((pared == "N") || (pared == "n"))
                    {
                        // 检验数据值范围
                        if ((latitude > -90) && (latitude < 90) && (minute < 60) && (degree >= 0))
                        {
                            return true;
                        }
                    }
                }
            }

            // 无效输入!
            latitude = 91;
            return false;
        }

        public static string LongitudeToString(double longitude)
        {
            // 检验数据值的有效性
            if ((longitude > -180) && (longitude < 180))
            {
                string longitudeString = "";
                if (longitude >= 0)
                {
                    // 东经
                    int degree = (int)longitude;
                    double minute = (longitude - degree) * 60;
                    longitudeString = String.Format("{0}°{1:00.000}'E", degree, minute);
                }
                else
                {
                    // 西经
                    longitude *= -1;
                    int degree = (int)longitude;
                    double minute = (longitude - degree) * 60;
                    longitudeString = String.Format("{0}°{1:00.000}'W", degree, minute);
                }

                return longitudeString;
            }

            return "****";
        }

        public static bool StringToLongitude(string longitudeString, ref double longitude)
        {
            longitudeString = longitudeString.Trim();
            // 判断参数是否有效
            if (longitudeString.Length > 3)
            {
                int degree = 181;
                float minute = 60;
                string pared = "";

                string[] fileds = longitudeString.Split(new char[] { '°', '\'' });
                if (fileds.Length == 3)
                {
                    degree = Convert.ToInt32(fileds[0]);
                    minute = Convert.ToSingle(fileds[1]);
                    pared = fileds[2];

                    longitude = degree + minute / 60.0;
                    if ((pared == "W") || (pared == "w"))
                    {
                        longitude *= -1;

                        // 检验数据值范围
                        if ((longitude > -180) && (longitude < 180) && (minute < 60) && (degree >= 0))
                        {
                            return true;
                        }
                    }
                    else if ((pared == "E") || (pared == "e"))
                    {
                        // 检验数据值范围
                        if ((longitude > -180) && (longitude < 180) && (minute < 60) && (degree >= 0))
                        {
                            return true;
                        }
                    }
                }
            }

            // 无效输入!
            longitude = 181;
            return false;
        }

        public void UpdateContent()
        {
            if (m_ListItem != null)
            {
                m_ListItem.Text = m_MMSI.ToString("000000000");
                if (Latitude != double.MaxValue)
                {
                    m_ListItem.SubItems[1].Text = LatitudeToString(Latitude);
                }
                else
                {
                    m_ListItem.SubItems[1].Text = "***";
                }
                if (Longitude != double.MaxValue)
                {
                    m_ListItem.SubItems[2].Text = LongitudeToString(Longitude);
                }
                else
                {
                    m_ListItem.SubItems[2].Text = "***";
                }
                if (m_Course != double.MaxValue)
                {
                    m_ListItem.SubItems[3].Text = m_Course.ToString("0.0");
                }
                else
                {
                    m_ListItem.SubItems[3].Text = "***";
                }
                if (m_Speed != double.MaxValue)
                {
                    m_ListItem.SubItems[4].Text = m_Speed.ToString("0.0");
                }
                else
                {
                    m_ListItem.SubItems[4].Text = "***";
                }
                if (m_Heading != double.MaxValue)
                {
                    m_ListItem.SubItems[5].Text = m_Heading.ToString("0.0");
                }
                else
                {
                    m_ListItem.SubItems[5].Text = "***";
                }
                m_ListItem.SubItems[6].Text = m_Name;
                m_ListItem.SubItems[7].Text = m_CallSign;
                m_ListItem.SubItems[8].Text = m_Destination;
            }
        }

        double m_Heading = double.MaxValue;
        public double Heading
        {
            get
            {
                return m_Heading;
            }
            set
            {
                m_Heading = value;
            }
        }
        DateTime m_UpdateTime = DateTime.Now;
        public System.DateTime UpdateTime
        {
            get
            {
                return m_UpdateTime;
            }
            set
            {
                m_UpdateTime = value;
            }
        }
        ListViewItem m_ListItem = null;
        public System.Windows.Forms.ListViewItem ListItem
        {
            get
            {
                return m_ListItem;
            }
            set
            {
                m_ListItem = value;
            }
        }
        int m_MMSI = 0;
        public int MMSI
        {
            get
            {
                return m_MMSI;
            }
            set
            {
                m_MMSI = value;
            }
        }
        int m_IMO = 0;
        public int IMO
        {
            get
            {
                return m_IMO;
            }
            set
            {
                m_IMO = value;
            }
        }
        double m_Latitude = double.MaxValue;
        public double Latitude
        {
            get
            {
                return m_Latitude;
            }
            set
            {
                m_Latitude = value;
            }
        }
        double m_Longitude = double.MaxValue;
        public double Longitude
        {
            get
            {
                return m_Longitude;
            }
            set
            {
                m_Longitude = value;
            }
        }
        double m_Speed = double.MaxValue;
        public double Speed
        {
            get
            {
                return m_Speed;
            }
            set
            {
                m_Speed = value;
            }
        }
        double m_Course = double.MaxValue;
        public double Course
        {
            get
            {
                return m_Course;
            }
            set
            {
                m_Course = value;
            }
        }
        double m_Drag = double.MaxValue;
        public double Drag
        {
            get
            {
                return m_Drag;
            }
            set
            {
                m_Drag = value;
            }
        }
        string m_Name = "";
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }
        string m_CallSign = "";
        public string CallSign
        {
            get
            {
                return m_CallSign;
            }
            set
            {
                m_CallSign = value;
            }
        }
        string m_Destination = "";
        public string Destination
        {
            get
            {
                return m_Destination;
            }
            set
            {
                m_Destination = value;
            }
        }
    }
}
