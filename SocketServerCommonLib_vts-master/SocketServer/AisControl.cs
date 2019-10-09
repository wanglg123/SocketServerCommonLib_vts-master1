using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using AISParserLibrary;
using SocketClientCommonLib;
using SocketServerCommonLib;
using System.Net;
using System.Net.Sockets;
namespace SocketServer
{
    public partial class AisControl : UserControl
    {
       
         /// <summary>
        /// AIS目标集合
        /// </summary>
        public Dictionary<int, AISTarget> m_AISCollection = new Dictionary<int, AISTarget>();
        /// <summary>
        /// 动态SQL命令
        /// </summary>
        private static List<string> m_DynamicCommandCollection = new List<string>();
        /// <summary>
        /// 静态SQL命令
        /// </summary>
        private static List<string> m_StaticCommandCollection = new List<string>();

        /// <summary>
        /// 后台保存数据进程对象
        /// </summary>
        private BackgroundWorker m_wndSaveDataWorker = new BackgroundWorker();
        /// <summary>
        /// 数据集合的同步锁
        /// </summary>
        private static Mutex m_StaticLocker = new Mutex();
        /// <summary>
        /// 数据集合的同步锁
        /// </summary>
        private static Mutex m_DynamicLocker = new Mutex();

        /// <summary>
        /// 串口通讯对象
        /// </summary>
        public SerialPort m_SerialPort = new SerialPort();
        /// <summary>
        /// AIS解析对象
        /// </summary>
        AISParser m_AISParser = new AISParser();
        private AsyncUDPServer m_asyncudpServer;

       
        public SocketClientCommonLib.SyncSocketClient m_tcpClient;
     
        public AisControl()
        {
            InitializeComponent();
        }
        public AisControl(PortSettings setting,AsyncUDPServer udpServer)
        {
            InitializeComponent();


            try
            {
                m_asyncudpServer = udpServer;
                m_AISCollection = udpServer.m_np.m_AISCollection;

                //m_tcpClient = new SyncSocketClient();

                //m_tcpClient.Connect("192.168.20.40", 50000);
                //Program.g_client = m_tcpClient;
               
                // m_ais = new jsonParse.AisMessage();

                //    tcpClient.SendCommand("1111111111111111");


                // 设置事件入口
                m_wndSaveDataWorker.DoWork += new DoWorkEventHandler(m_wndSaveDataWorker_DoWork);
                m_wndSaveDataWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_wndSaveDataWorker_RunWorkerCompleted);
                // 绑定消息
                m_SerialPort.PortName = setting.PortName;
                m_SerialPort.BaudRate = setting.BaudRate;
                m_SerialPort.DataBits = setting.DataSize;
                m_SerialPort.StopBits = setting.StopBits;
                m_SerialPort.Parity = setting.Parity;
                m_SerialPort.DataReceived += new SerialDataReceivedEventHandler(m_SerialPort_DataReceived);
                m_AISParser.DynamicInformationReceived += new DynamicInformationReceivedHandler(m_AISParser_DynamicInformationReceived);
                m_AISParser.StaticInformationReceived += new StaticInformationReceivedHandler(m_AISParser_StaticInformationReceived);
                m_AISParser.NMEAStringReceived += new DataReceivedHandler(m_AISParser_NMEAStringReceived);
                m_AISParser.RawStringReceived += new DataReceivedHandler(m_AISParser_RawStringReceived);
	            // 打开串口
	            m_SerialPort.Open();
            }
            catch(System.Exception ex)
            {
                MessageBox.Show("打开串口失败, 串口名称:" + setting.PortName + "\n详细信息:\n" + ex.Message);
            }
        }
        public void udpSendCentrlIP(string str)
        {
            m_asyncudpServer.m_sListen.SendTo(System.Text.Encoding.Default.GetBytes(str), str.Length, SocketFlags.None, m_asyncudpServer.m_ippoint);
        }
        /// <summary>
        /// 关闭串口并释放资源
        /// </summary>
        public void CloseSerialPort()
        {
            try
            {
	            m_SerialPort.Close();
	            m_SerialPort.Dispose();
	            m_AISParser.Dispose();
            }
            finally
            {
                GC.Collect();            	
            }
        }

        void m_AISParser_RawStringReceived(string line)
        {
            AddRawData(line);
        }

        
        /// <summary>
        /// 数据存储工作进程完成或出现错误
        /// </summary>
        void m_wndSaveDataWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // 回收内存
            GC.Collect();
        }

        /// <summary>
        /// 开始数据存储工作进程
        /// </summary>
        void m_wndSaveDataWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //// 获取对象
            //AisControl controlObject = ( AisControl )e.Argument;
            //// 尝试打开数据库连接
            //MySql.Data.MySqlClient.MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection(MainForm.DatabaseConnectionString);
            //connection.Open();
            //if( connection.State == ConnectionState.Open )
            //{
            //    try
            //    {
            //        // 复制数据缓冲区的数据
            //        List<string> dynamicSqlCommandCollection = null;
            //        List<string> staticSqlCommandCollection = null;
            //        CopyBufferCommands(ref dynamicSqlCommandCollection, ref staticSqlCommandCollection);

            //        // 判断是否有数据需要操作
            //        if( ( staticSqlCommandCollection != null ) && ( dynamicSqlCommandCollection != null ) )
            //        {
            //            // 保存所有的数据项
            //            SaveDynamicData(connection, dynamicSqlCommandCollection, controlObject);
            //            SaveStaticData(connection, staticSqlCommandCollection, controlObject);
            //        }
            //        else
            //        {
            //            // 空数据...
            //            controlObject.AddStatusString("数据集合为空, 入库操作无效。");
            //        }
            //    }
            //    catch( Exception ex )
            //    {
            //        System.Diagnostics.Debug.WriteLine(ex.ToString());
            //    }
            //    finally
            //    {
            //        // 关闭数据库连接
            //        connection.Close();
            //    }
            //}
            //// 释放资源
            //connection.Dispose();
        }

        /// <summary>
        /// 静态数据入库操作
        /// </summary>
        private static void SaveStaticData(MySql.Data.MySqlClient.MySqlConnection connection, List<string> staticSqlCommandCollection, AisControl controlObject)
        {
            // 开始事务处理
            MySql.Data.MySqlClient.MySqlTransaction transaction = connection.BeginTransaction();
            MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
            try
            {
                // 批量更新
                foreach( string cmd in staticSqlCommandCollection )
                {
                    command.CommandText = cmd;
                    command.ExecuteNonQuery();
                }

                // 提交数据入库
                transaction.Commit();
                if( controlObject != null )
                {
                    // 提交错误日志
                    controlObject.AddStatusString("静态数据入库操作成功。静态目标数量[" + staticSqlCommandCollection.Count.ToString() + "]");
                }
            }
            catch( Exception ex )
            {
                // 放弃当前的数据入库操作
                transaction.Rollback();
                if( controlObject != null )
                {
                    // 提交错误日志
                    controlObject.AddStatusString("静态数据入库操作失败。\r\n错误信息为：" + ex.ToString());
                }
            }
        }

        /// <summary>
        /// 动态数据入库操作
        /// </summary>
        private static void SaveDynamicData(MySql.Data.MySqlClient.MySqlConnection connection, List<string> dynamicSqlCommandCollection, AisControl controlObject)
        {
            // 开始事务处理
            MySql.Data.MySqlClient.MySqlTransaction transaction = connection.BeginTransaction();
            MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
            try
            {
                // 批量更新
                foreach( string cmd in dynamicSqlCommandCollection )
                {
                    command.CommandText = cmd;
                    command.ExecuteNonQuery();
                }

                // 提交数据入库
                transaction.Commit();
                if( controlObject != null )
                {
                    // 提交错误日志
                    controlObject.AddStatusString("动态数据入库操作成功。动态目标数量[" + dynamicSqlCommandCollection.Count.ToString() + "]");
                }
            }
            catch( Exception ex )
            {
                // 放弃当前的数据入库操作
                transaction.Rollback();
                if( controlObject != null )
                {
                    // 提交错误日志
                    controlObject.AddStatusString("动态数据入库操作失败。\r\n错误信息为：" + ex.ToString());
                }
            }
        }

        /// <summary>
        /// 删除过时的AIS数据操作
        /// </summary>
        private static void DeleteAISdata(MySql.Data.MySqlClient.MySqlConnection connection, AisControl controlObject)
        {
            // 开始事务处理
            MySql.Data.MySqlClient.MySqlTransaction transaction = connection.BeginTransaction();
            MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();

            DateTime lastDynamicTime = DateTime.Now.Subtract(new TimeSpan(0, 4, 0, 0));
            DateTime lastStaticTime = DateTime.Now.Subtract(new TimeSpan(15, 0, 0, 0));
          

            string sqlDeleteAisData = "delete from realtimeaisdynamicinfotable where AISTimeStamp < '" + lastDynamicTime + "';";
            sqlDeleteAisData += "delete from realtimeaisstaticinfotable where Timestamp < '" + lastStaticTime + "';";
            try
            {
                command.CommandText = sqlDeleteAisData;
                command.ExecuteNonQuery();

                // 提交数据入库
                transaction.Commit();
                if( controlObject != null )
                {
                    // 提交错误日志
                    controlObject.AddStatusString("AIS过时数据删除操作成功。");
                }

                command.Dispose();
            }
            catch( Exception ex )
            {
                // 放弃当前的数据入库操作
                transaction.Rollback();
                if( controlObject != null )
                {
                    // 提交错误日志
                    controlObject.AddStatusString("AIS过时数据删除操作失败。\r\n错误信息为：" + ex.ToString());
                }
            }

        }

        /// <summary>
        /// 同步复制已生成的命令数据
        /// </summary>
        /// <param name="dynamicSqlCommandCollection">动态SQL命令</param>
        /// <param name="staticSQLCommandCollection">静态SQL命令</param>
        private static void CopyBufferCommands(ref List<string> dynamicSqlCommandCollection, ref List<string> staticSQLCommandCollection)
        {
            if( m_StaticLocker.WaitOne(1000) )
            {
                // 复制后清除旧数据
                staticSQLCommandCollection = new List<string>(m_StaticCommandCollection);
                m_StaticCommandCollection.Clear();
                // 释放锁
                m_StaticLocker.ReleaseMutex();
            }
            if(m_DynamicLocker.WaitOne(1000))
            {
                // 复制后清除旧数据
                dynamicSqlCommandCollection = new List<string>(m_DynamicCommandCollection);
                m_DynamicCommandCollection.Clear();
                // 释放锁
                m_DynamicLocker.ReleaseMutex();
            }
        }

        void m_AISParser_NMEAStringReceived(string line)
        {
            AddNMEAData(line);
            
          //  m_tcpClient.SendData(line);
        }

        public bool IsOpen()
        {
            return m_SerialPort.IsOpen;
        }

        void m_AISParser_StaticInformationReceived(int iMMIS, int iIMO, string strCallSign, string name, int ShipCargoTyp, int iDimensionA, int iDimensionB, int iDimensionC, int iDimensionD, int iPositionDeviceTyp, string ETA, double dMaxDraught, string destination)
        {
            try
            {
               // MessageBox.Show("aaaaaaaaaaaaaaa");
                // 更新船舶目标
                UpdateData(iMMIS, name, strCallSign, destination);


                string etaMonth = ETA[0].ToString() + ETA[1].ToString();
                string etaDay = ETA[2].ToString() + ETA[3].ToString();
                string etaHour = ETA[4].ToString() + ETA[5].ToString();
                string etaMin = ETA[6].ToString() + ETA[7].ToString();
                jsonParse.AisMessage m_ais = new jsonParse.AisMessage();
                m_ais.ProtocolNo = "01";
              //  m_ais.UniqueCode = iMMIS.ToString();
                m_ais.MMSI = iMMIS.ToString();
               
                m_ais.IMO = iIMO;
                m_ais.CallSign = strCallSign;
                m_ais.Name = name;
                m_ais.ShipCarGoType = ShipCargoTyp.ToString();
                m_ais.DimA = iDimensionA;
                m_ais.DimB = iDimensionB;
                m_ais.DimC = iDimensionC;
                m_ais.DimD = iDimensionD;
                m_ais.PositionDeviceType = iPositionDeviceTyp.ToString();
                //m_ais.EATMinute = (Convert.ToDateTime(ETA)).Minute;
                //m_ais.ETAMonth = (Convert.ToDateTime(ETA)).Month;
                //m_ais.ETADay =(Convert.ToDateTime(ETA)).Day;
                //m_ais.ETAHour = (Convert.ToDateTime(ETA)).Hour;
                m_ais.MaximumDraught = (int)dMaxDraught;
                m_ais.Destination = destination;
                m_ais.ETAMonth= int.Parse(etaMonth);
                m_ais.ETADay = int.Parse(etaDay);
                m_ais.ETAHour = int.Parse(etaHour);
                m_ais.ETAMinute = int.Parse(etaMin);
                

                string str = jsonParse.GetMessage(m_ais);
                str = "~" + str + "~";

                //m_tcpClient.SendData(str);

                udpSendCentrlIP(str);

              

                // 生成更新数据库的SQL命令
                string SqlString = "replace into realtimeaisstaticinfotable set MMSI = " + iMMIS.ToString() + ",";
                SqlString += "IMONumber = " + iIMO.ToString() + "," + "CallSign = '" + strCallSign + "',";
                SqlString += "ShipName = '" + name + "'," + "ShipCargoType = " + ShipCargoTyp.ToString() + ",";
                SqlString += "ShipDimensionA = " + iDimensionA.ToString() + "," + "ShipDimensionB = " + iDimensionB.ToString() + ",";
                SqlString += "ShipDimensionC = " + iDimensionC.ToString() + "," + "ShipDimensionD = " + iDimensionD.ToString() + ",";
                SqlString += "PositionDeviceType = " + iPositionDeviceTyp.ToString() + "," + "EstimatedTimeOfArrival = '" + ETA + "',";
                SqlString += "MaximumDraught = " + dMaxDraught.ToString() + "," + "Destination = '" + destination + "',";
                SqlString += "Timestamp = '" + DateTime.Now + "';";
                // 添加到集合中
                if( m_StaticLocker.WaitOne(1000) )
                {
                    m_StaticCommandCollection.Add(SqlString);
                    m_StaticLocker.ReleaseMutex();
                }

                // 更新船舶目标
             //   UpdateData(iMMIS, name, strCallSign, destination);
            }
            catch( System.Exception ex )
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        void m_AISParser_DynamicInformationReceived(int iMMIS, double dLat, double dLot, double dSOG, double dCOG, int iQI, int iHDG, double dROT, int iageofDiffCorr, int iNavi, int time)
        {
            try
            {
                jsonParse.AisMessage m_ais = new jsonParse.AisMessage();
                m_ais.ProtocolNo = "01";
               // m_ais.UniqueCode = iMMIS.ToString();
                m_ais.MMSI = iMMIS.ToString();
                m_ais.Lat = dLat;
                m_ais.Lon = dLot;



                m_ais.SOG = dSOG;
                m_ais.COG = dCOG;
                m_ais.PositionAccuracy = iQI.ToString();
                m_ais.Heading = (double)iHDG;
                m_ais.ROT = dROT;
                m_ais.AISVersion = iageofDiffCorr.ToString();
                m_ais.NavigationalStatus = iNavi.ToString();
                m_ais.CollTime = DateTime.Now;

                string str = jsonParse.GetMessage(m_ais);
                str = "~" + str + "~";
                //m_tcpClient.SendData(str);
                udpSendCentrlIP(str);

                // 更新船舶目标集合
                UpdatePosition(iMMIS, dLat, dLot, dSOG, dCOG, iHDG);
                //定义SQL语句replace
                string SqlString = "replace into realtimeaisdynamicinfotable set MMSI = " + iMMIS.ToString() + ",";
                SqlString += "Latitude =" + dLat.ToString() + "," + "Longitude = " + dLot.ToString() + ",";
                SqlString += "SpeedOverGround = " + dSOG.ToString() + ",";
                SqlString += "CourseOverGround = " + dCOG.ToString() + ",";
                SqlString += "QuanlityIndicatior = " + iQI.ToString() + ",";
                SqlString += "TrueHeading = " + iHDG.ToString() + "," + "RateOfTurn = " + dROT.ToString() + ",";
                SqlString += "AgeOfDiffCorrection = " + iageofDiffCorr.ToString() + ",";
                SqlString += "NavigationalStatus = " + iNavi.ToString() + ",";
                SqlString += "AISTimeStamp = '" + DateTime.Now.ToString() + "';";

                // 添加到集合中
                if( m_DynamicLocker.WaitOne(1000) )
                {
                    m_DynamicCommandCollection.Add(SqlString);
                    m_DynamicLocker.ReleaseMutex();
                }

          
            }
            catch( System.Exception ex )
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 解析串口中的新数据
        /// </summary>
        void m_SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
	            if( e.EventType == SerialData.Chars )
	            {
	                string message = m_SerialPort.ReadLine();
	                if( ( message != null ) && ( message.Length > 0 ) )
	                {
                     //   MessageBox.Show("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa1111111111111");
	                    m_AISParser.TryParseMemssage(message);
	                }
	            }
            }
            catch(System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        public void AddRawData(string line)
        {
            if (this.InvokeRequired)
            {
                Delegate method = new DataReceivedHandler(AddRawData);
                this.Invoke(method, new object[] { line });
                return;
            }

            if (m_wndRawDataList.Items.Count > 200)
            {
                m_wndRawDataList.Items.RemoveAt(0);
            }
            m_wndRawDataList.SelectedIndex = m_wndRawDataList.Items.Add(line);
        }

        public void AddNMEAData(string line)
        {
            if( this.InvokeRequired )
            {
                Delegate method = new DataReceivedHandler(AddNMEAData);
                this.Invoke(method, new object[] { line });
                return;
            }

            if( m_wndNMEAList.Items.Count > 200 )
            {
                m_wndNMEAList.Items.RemoveAt(0);
            }
            m_wndNMEAList.SelectedIndex = m_wndNMEAList.Items.Add(line);
        }

        delegate void AddStringHandler(string line);
        /// <summary>
        /// 添加状态信息
        /// </summary>
        /// <param name="line">信息行</param>
        private void AddStatusString(string line)
        {
            if(this.InvokeRequired)
            {
                Delegate method = new AddStringHandler(AddStatusString);
                this.Invoke(method, new object[] { line });
                return;
            }

            DateTime localtime = DateTime.Now;
            string currentTimeString = localtime.ToString("\r\n[yyyy-MM-dd  HH:mm:ss] ");
            SaveInformationToFile(localtime, currentTimeString + line + "\r\n");

            if( m_wndLog.Lines.Length > 500 )
            {
                // 清除旧的数据
                List<string> buffer = new List<string>();
                buffer.AddRange(m_wndLog.Lines);
                buffer.RemoveRange(0, buffer.Count - 50);
                buffer.Add(currentTimeString + line);
                // 恢复现场
                m_wndLog.Lines = buffer.ToArray();
            }
            else
            {
                m_wndLog.AppendText(currentTimeString + line + "\r\n");
            }
        }

        //保存操作记录到日志
        public void SaveInformationToFile(DateTime localtime, string line)
        {
            //创建日志名
            string fileName = localtime.ToString("yyyy - MM - dd");
            string path = Application.StartupPath + "/" + fileName + ".log";

            File.AppendAllText(path, line);
        }

        delegate void UpdateDataHandler(int mmsi, string name, string callSign, string destination);

        /// <summary>
        /// 更新静态信息
        /// </summary>
        private void UpdateData(int mmsi, string name, string callSign, string destination)
        {
            if(this.InvokeRequired)
            {
                Delegate method = new UpdateDataHandler(UpdateData);
                this.Invoke(method, new object[] { mmsi, name, callSign, destination });
                return;
            }

            AISTarget target = null;
            if( m_AISCollection.TryGetValue(mmsi, out target) )
            {
                // 更新现有数据项
                target.Name = name;
                target.CallSign = callSign;
                target.Destination = destination;
            }
            else
            {
                // 新增新数据项
                target = new AISTarget();
                target.MMSI = mmsi;
                target.Name = name;
                target.CallSign = callSign;
                target.Destination = destination;

                // 添加到集合中
                m_AISCollection.Add(mmsi, target);
                target.ListItem = new System.Windows.Forms.ListViewItem(mmsi.ToString());
                target.ListItem.SubItems.AddRange(new string[] { "***", "***", "***", "***", "***", "***", "***", "***" });
                m_wndAISList.Items.Add(target.ListItem);
            }

            if( target != null )
            {
                target.UpdateTime = DateTime.Now;
                // 更新显示
                target.UpdateContent();
            }
        }

        delegate void UpdatePositionHandler(int mmsi, double latitude, double longitude, double speed, double course, double heading);

        /// <summary>
        /// 更新位置
        /// </summary>
        private void UpdatePosition(int mmsi, double latitude, double longitude, double speed, double course, double heading)
        {
            if(this.InvokeRequired)
            {
                Delegate method = new UpdatePositionHandler(UpdatePosition);
                this.Invoke(method, new object[] { mmsi, latitude, longitude, speed, course, heading});
                return;
            }

            AISTarget target = null;
            if( m_AISCollection.TryGetValue(mmsi, out target) )
            {
                // 更新现有数据项
                target.Latitude = latitude;
                target.Longitude = longitude;
                target.Speed = speed;
                target.Course = course;
                target.Heading = heading;
            }
            else
            {
                // 新增新数据项
                target = new AISTarget();
                target.MMSI = mmsi;
                target.Latitude = latitude;
                target.Longitude = longitude;
                target.Speed = speed;
                target.Course = course;
                target.Heading = heading;

                // 添加到集合中
                m_AISCollection.Add(mmsi, target);
                target.ListItem = new System.Windows.Forms.ListViewItem(mmsi.ToString());
                target.ListItem.SubItems.AddRange(new string[] { "***", "***", "***", "***", "***", "***", "***", "***" });
                m_wndAISList.Items.Add(target.ListItem);
            }

            if( target != null )
            {
                target.UpdateTime = DateTime.Now;
                // 更新显示
                target.UpdateContent();
            }
        }

        private void m_wndClearTimer_Tick(object sender, EventArgs e)
        {
            // 查找需要清除的目标
            DateTime now = DateTime.Now;
            TimeSpan tenMinutes = new TimeSpan(0, 20, 0);
            List<int> removeKeys = new List<int>();
            foreach( AISTarget target in m_AISCollection.Values )
            {
                if( ( now - target.UpdateTime ) >= tenMinutes )
                {
                    removeKeys.Add(target.MMSI);
                }
            }

            // 开展清除工作
            foreach( int mmsi in removeKeys )
            {
                AISTarget target = null;
                if( m_AISCollection.TryGetValue(mmsi, out target) && ( target != null ) )
                {
                    m_AISCollection.Remove(mmsi);
                    m_wndAISList.Items.Remove(target.ListItem);
                }
            }
        }

        private void m_wndSaveDataTimer_Tick(object sender, EventArgs e)
        {
            //// 开始异步处理线程
            //if( ( !m_wndSaveDataWorker.IsBusy ) && ( MainForm.DatabaseConnectionString != "" ) )
            //{
            //    m_wndSaveDataWorker.RunWorkerAsync(this);
            //}
        }
    }
    public class PortSettings
    {
        string m_PortName = "";
        /// <summary>
        /// 串口名称
        /// </summary>
        public string PortName
        {
            get
            {
                return m_PortName;
            }
            set
            {
                m_PortName = value;
            }
        }
        int m_Raud = 38400;
        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate
        {
            get
            {
                return m_Raud;
            }
            set
            {
                m_Raud = value;
            }
        }
        int m_DataSize = 8;
        /// <summary>
        /// 数据位大小
        /// </summary>
        public int DataSize
        {
            get
            {
                return m_DataSize;
            }
            set
            {
                m_DataSize = value;
            }
        }
        Parity m_Parity = Parity.None;
        /// <summary>
        /// 校验位
        /// </summary>
        public System.IO.Ports.Parity Parity
        {
            get
            {
                return m_Parity;
            }
            set
            {
                m_Parity = value;
            }
        }
        StopBits m_StopBits = StopBits.One;
        /// <summary>
        /// 停止位
        /// </summary>
        public System.IO.Ports.StopBits StopBits
        {
            get
            {
                return m_StopBits;
            }
            set
            {
                m_StopBits = value;
            }
        }
    }
   
}
