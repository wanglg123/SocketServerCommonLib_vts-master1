/********************************************************************
 * * 使本项目源码前请仔细阅读以下协议内容，如果你同意以下协议才能使用本项目所有的功能,
 * * 否则如果你违反了以下协议，有可能陷入法律纠纷和赔偿，作者保留追究法律责任的权利。
 * *
 * * Copyright (C) 2014-? cskin Corporation All rights reserved.
 * * 作者： Amos Li    QQ：443061626   .Net项目技术组群:Amos Li 出品
 * * 请保留以上版权信息，否则作者将保留追究法律责任。
 * * 创建时间：2014-08-05
********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Threading;
using System.Diagnostics;
using System.IO;         //添加IO流的命名空间

namespace SocketServerCommonLib
{
    /// <summary>
    /// 信息处理-信息发送接收过滤器
    /// </summary>
    public class SocketInvokeElement
    {

        MySql.Data.MySqlClient.MySqlConnection m_connection = null; 
        protected AsyncSocketServer m_asyncSocketServer;
        //客户端
        private SocketUserToken m_socketUserToken;
        protected SocketUserToken socketUserToken {get { return m_socketUserToken; }  set { m_socketUserToken = value; }  }

        //是否有发送异步事件
        protected bool m_sendAsync;
        //过滤器创建时间
        protected DateTime m_connectDT;
        public DateTime ConnectDT { get { return m_connectDT; } }
        //过滤器最后操作时间
        protected DateTime m_activeDT;
        public DateTime ActiveDT { get { return m_activeDT; } }

        public CRadarData m_radarData = new CRadarData();

        private Thread m_saveDataThread = null;
        private Thread m_saveDataFileThread = null;         //声明一个线程，用于对文件进行读写

        //Mutex m = new Mutex();      //声明一个互斥锁
        //string Control_filepath = "E:\\ControlData\\controldata";

        private static string m_DatabaseConnectionString = "";
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string DatabaseConnectionString
        {
            get
            {
                return m_DatabaseConnectionString;
            }
            set
            {
                m_DatabaseConnectionString = value;
            }
        }
    
        /// <summary>
        /// 发送信息时组装信息协议(信息组装后发送客户端)
        /// </summary>
        private AssemblyOutDataParser m_outDataParser;
        public AssemblyOutDataParser OutDataParser { get { return m_outDataParser; } set { m_outDataParser = value; } }

        /// <summary>
        /// 接收时候信息组装信息(确定信息是否合格)
        /// </summary>
        private AssemblyInDataParser m_InDataParser;
        public AssemblyInDataParser InDataParser { get { return m_InDataParser; } set { m_InDataParser = value; } }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="asyncSocketServer"></param>
        /// <param name="socketUserToken"></param>
        public SocketInvokeElement(AsyncSocketServer asyncSocketServer, SocketUserToken socketUserToken)
        {
            m_asyncSocketServer = asyncSocketServer;
            m_socketUserToken = socketUserToken;

            m_outDataParser = new AssemblyOutDataParser();
            m_InDataParser = new AssemblyInDataParser();
            m_sendAsync = false;
            m_connectDT = DateTime.UtcNow;
            m_activeDT = DateTime.UtcNow;
            //InitMySql();
            //m_saveDataThread = new Thread(saveDataFun);
            //m_saveDataThread.Start();
            m_saveDataFileThread = new Thread(saveDataToFileFun);    //利用构造函数，实例化一个线程，并且运行saveDataToFileFun函数
            m_saveDataFileThread.Start();     //开启线程

        }
        void InitMySql()
        {
            // 尝试打开数据库连接
            string connectionString = "";
            connectionString += "server=127.0.0.1";
            connectionString += ";User Id=root";
            connectionString += ";Password=root";
            // connectionString += ";port=3306";
            connectionString += ";port=3306";
            connectionString += ";database=hfradar";
            if (connectionString != null)
            {
                connectionString += ";Persist Security Info=True";
                m_DatabaseConnectionString = connectionString;
            }
            MySql.Data.MySqlClient.MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection(DatabaseConnectionString);
            connection.Open();
            if (connection.State == ConnectionState.Open)
            {
                m_connection = connection;
            }
        }
        public static int getLength(byte type)
        {
            int length = 0;
            switch(type)
            {
                case 0:
                {
                    break;
                }
                case 1:
                {
                    //length = 1200;
                    length = 1100;
                    break;
                }
                case 2:
                {
                    //length = 16064;
                    break;
                }
                case 3:
                {
                    //length = 1344;
                        length = 1100;
                    break;
                }
                case 4:
                {
                    //length = 16064;
                    length = 1100;
                    break;
                }
                case 5:
                {
                    length = 1472;
                    break;
                }
                case 6://控制字
                {
                    length = 132;
                    break;
                }
                default:
                {
                    break;
                }
            }
            return length;
        }
        /// <summary>
        /// 协议组装/继续接收
        /// </summary>
        /// <param name="buffer">byte[]</param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual bool ProcessReceive(byte[] buffer, int offset, int count) //接收异步事件返回的数据，用于对数据进行缓存和分包
        {
            //m_activeDT = DateTime.UtcNow;
            //DynamicBufferManager receiveBuffer = m_socketUserToken.ReceiveBuffer;
            //receiveBuffer.WriteBuffer(buffer, offset, count);//把刚刚接收的信息加入到缓存中
            //bool result = true;

            ////按照长度分包
            //int packetLength = BitConverter.ToInt32(receiveBuffer.Buffer, 0); //获取包长度
            //if ((packetLength > 10 * 1024 * 1024) | (receiveBuffer.DataCount > 10 * 1024 * 1024)) //最大Buffer异常保护
            //    return false;

            //if (receiveBuffer.DataCount >= packetLength) //packetLength收到的数据达到包长度
            //{
            //    //命令 组装-如果组装失败则继续接收：如果组装成功-则删除
            //    result = ProcessPacket(receiveBuffer.Buffer, offset, packetLength);
            //    if (result)
            //        receiveBuffer.Clear(packetLength);//清理已经处理的协议
            //    else
            //        return result;
            //}
            //return true;

            ///////////******下面的程序考虑粘包的情况。******//////
            m_activeDT = DateTime.UtcNow;
            DynamicBufferManager receiveBuffer = m_socketUserToken.ReceiveBuffer;

            receiveBuffer.WriteBuffer(buffer, offset, count);
            bool result = true;

            while (true)
            {
                if (receiveBuffer.Buffer.Length >11)
                {
                   // int packetLength = (int)((receiveBuffer.Buffer[1] << 8) | receiveBuffer.Buffer[2]);

                    int packetLength = getLength(receiveBuffer.Buffer[11]);
                 //   Console.WriteLine(packetLength+"-------------------");
                    //if ((packetLength > 10 * 1024 * 1024) | (receiveBuffer.DataCount > 10 * 1024 * 1024))
                    if ((packetLength > 16064))
                    {
                        DelegateState.ServerStateInfo("(packetLength > 16064 packetLength= " + packetLength.ToString());
                        return false;
                    }
                    if ((receiveBuffer.DataCount > 10 * 1024 * 1024))
                    {
                        DelegateState.ServerStateInfo("(receiveBuffer.DataCount> 10 * 1024* 1024  receiveBuffer.DataCount > 10 * 1024* 1024  ");
                        return false;
                    }
                    if (packetLength == 0)
                    {
                        return false;
                    }
                    if (receiveBuffer.DataCount >= packetLength)
                    {
                        // while (1)
                        {


                            result = ProcessPacket(receiveBuffer.Buffer, offset, packetLength);
                            if (result)
                            {
                                receiveBuffer.Clear(packetLength);
                                if (receiveBuffer.DataCount > 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    break;
                                }


                            }
                            else
                                return result;
                        }

                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }


            }

            return true;

        }
        /// <summary>
        /// 组装命令协议
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual bool ProcessPacket(byte[] buffer, int offset, int count) //处理分完包后的数据，把命令和数据分开，并对命令进行解析
        {

            byte[] tmpBuffer = new byte[count];
            Array.Copy(buffer, 0, tmpBuffer, 0, count); //复制以前的数据
            receiveRadarData(tmpBuffer);
           
          
            return true;
        }
        public int m_iFarmCount = 0;
        public int m_iFarmCount_slow = 0;    //用于记录接收的慢扫数据的帧数

        public int loss_f = 0;      //用于检测是否丢包

        public void receiveRadarData(byte[] responseBuffer)
        {
            switch (responseBuffer[11])
            {
                case 0:
                    {

                        break;
                    }
                case 1:
                    {
                        m_radarData.m_fastFT1 = (CRadarData.FastFT1)CRadarData.BytesToStruct(responseBuffer, m_radarData.m_fastFT1.GetType());

                        m_radarData.m_listFastFT1.Add(m_radarData.m_fastFT1);

                        CRadarData.FastFT1 ft1tmp = new CRadarData.FastFT1();
                        ft1tmp = m_radarData.m_fastFT1;
                        //   ft1tmp.FrameNum = m_iFarmCount;
                        //m_radarData.m_concurrentQueueFastFT1.Enqueue(ft1tmp);
                        m_radarData.m_concurrentQueueFastFT1file.Enqueue(responseBuffer);
                        m_iFarmCount++;


                        if (m_radarData.m_listFastFT1.Count >= 80)
                        {
                            if (System.Net.IPAddress.HostToNetworkOrder(m_radarData.m_listFastFT1[39].FrameNum) - System.Net.IPAddress.HostToNetworkOrder(m_radarData.m_listFastFT1[0].FrameNum)!=39)
                            {
                                loss_f++;
                            }
                            m_radarData.m_listFastFT1.RemoveRange(0, 40);
                        }
                       
                        break;
                    }
                case 2:
                    {
                        //length = 16064;
                        break;
                    }
                case 3:
                    {
                        // length = 1472;
                        m_radarData.m_slowData = (CRadarData.SlowDada)CRadarData.BytesToStruct(responseBuffer, m_radarData.m_slowData.GetType());
                        CRadarData.SlowDada slowData = new CRadarData.SlowDada();
                        slowData = m_radarData.m_slowData;
                        //m_radarData.m_concurrentQueueSlowData.Enqueue(slowData);
                        m_radarData.m_concurrentQueueSlowDatafile.Enqueue(responseBuffer);    //将慢扫数据加入需要写文件的序列
                        break;
                    }
                case 4:
                    {
                        m_radarData.m_slowData = (CRadarData.SlowDada)CRadarData.BytesToStruct(responseBuffer, m_radarData.m_slowData.GetType());
                        CRadarData.SlowDada slowData = new CRadarData.SlowDada();
                        slowData = m_radarData.m_slowData;
                        //m_radarData.m_concurrentQueueSlowData.Enqueue(slowData);
                        m_radarData.m_concurrentQueueSlowDatafile.Enqueue(responseBuffer);    //将慢扫数据加入需要写文件的序列
                        m_iFarmCount_slow++;
                        //Console.WriteLine("慢扫原始数据");
                        break;
                    }
                case 5:
                    {
                        m_radarData.m_monitorData = (CRadarData.MonitorDada)CRadarData.BytesToStruct(responseBuffer, m_radarData.m_monitorData.GetType());
                        CRadarData.MonitorDada monitorData = new CRadarData.MonitorDada();
                        monitorData = m_radarData.m_monitorData;
                        //m_radarData.m_concurrentQueueMonitorData.Enqueue(monitorData);
                        m_radarData.m_concurrentQueueMonitorDatafile.Enqueue(responseBuffer);    //将慢扫数据加入需要写文件的序列
                        //  Console.WriteLine("频谱监测数据");
                        break;
                    }
                case 6://控制字
                    {

                        break;
                    }
                default:
                    {
                        break;
                    }
            }

        }
        int index = 0;
        void saveDataFun()
        {
            CRadarData.FastFT1 ft1 ;  
            CRadarData.SlowDada slowdata;
            List<string> m_sql = new List<string>();
            DateTime now = DateTime.Now;
            int[] data ;
            int[] dataNull = new int[0];
           // string strData = "";
            Stopwatch sw = new Stopwatch();
            //for (int i = 0; i < data.Length; i++)
            //{
            //    data[i] = 2;
            //    //strData = strData + data[i].ToString();
            //}
            DateTime Sample = now;
            int FrameIndex = 0;
            int ParamIndex = 0;
            byte AtnDataType = 1;

            //int id = 0;
            String historyCommandString = "insert into hfradar.radarsample";
            historyCommandString += " (Sample,FrameIndex,ParamIndex,AtnDataType,Atn1Data,Atn2Data,Atn3Data,Atn4Data,Atn5Data,Atn6Data,Atn7Data,Atn8Data)";
            historyCommandString += "values (@Sample,@FrameIndex,@ParamIndex,@AtnDataType,@Atn1Data,@Atn2Data,@Atn3Data,@Atn4Data,@Atn5Data,@Atn6Data,@Atn7Data,@Atn8Data)";
            m_sql.Add(historyCommandString);
           // List<int[]> listData = new List<int[]>();
            byte [] bigData = new byte[256*320*4];

           // byte[] slowBytedata = new byte[4000 * 4];
            byte[] slowBytedata = new byte[256 * 4];
            while (true)
            {
                if (m_connection.State == ConnectionState.Open)
                {
                    try
                    {
                       
                        // 判断是否有数据需要操作
                        if ((m_sql != null))
                        {
                            // 保存所有的数据项
                           
                          //  int j = 0;
                          //  m_radarData.m_concurrentQueueFastFT1.TryDequeue(out ft1);
                            //if ((!m_radarData.m_concurrentQueueFastFT1.TryDequeue(out ft1))&&(!m_radarData.m_concurrentQueueSlowData.TryDequeue(out slowdata)))
                            //{
                            //    continue;
                            //}
                            if (m_radarData.m_concurrentQueueFastFT1.TryDequeue(out ft1))
                            {
                                sw.Start();
                                data = ft1.ft1Data;
                               // listData.Add(data);
                               // data.CopyTo(bigData, index*256*4);
                                Buffer.BlockCopy(data, 0, bigData, index * 256 * 4, 1024);
                                index++;
                                FrameIndex = System.Net.IPAddress.HostToNetworkOrder(ft1.FrameNum);
                                AtnDataType = 1;

                                if(index==320)
                                {


                                    SaveData(m_connection, m_sql, now, FrameIndex, ParamIndex, AtnDataType, bigData, dataNull, dataNull, dataNull, dataNull, dataNull, dataNull, dataNull);
                                    index = 0;
                                }
                                sw.Stop();
                                //获取运行时间间隔  
                                //  TimeSpan ts = sw.Elapsed;
                                //获取运行时间[毫秒]  
                                long times = sw.ElapsedMilliseconds;
                                //获取运行的总时间  
                                long times2 = sw.ElapsedTicks;
                            }
                            if (m_radarData.m_concurrentQueueSlowData.TryDequeue(out slowdata))
                            {
                                data = slowdata.rawData;
                               // data.CopyTo(slowBytedata, 0);
                                Buffer.BlockCopy(data, 0, slowBytedata, 0, slowBytedata.Length);
                                FrameIndex = System.Net.IPAddress.HostToNetworkOrder(slowdata.FrameNum);
                                AtnDataType = (byte)4;
                                now = DateTime.Now;
                                SaveData(m_connection, m_sql, now, FrameIndex, ParamIndex, AtnDataType, slowBytedata, dataNull, dataNull, dataNull, dataNull, dataNull, dataNull, dataNull);
                            }
                            
                        }
                        else
                        {
                            // 空数据...
                            //controlObject.AddStatusString("数据集合为空, 入库操作无效。");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                        // 关闭数据库连接
                        m_connection.Close();
                    }
                    finally
                    {
                       
                    }
                }
                //Thread.Sleep(3);
            }
        }


        int indexfile = 0;    //用于存文件数据帧数的计数
        int indexfile_s = 0;    //用于存文件数据帧数的计数
        int indexfile_m = 0;    //用于存文件数据帧数的计数
        int m_time = -1;//存文件开始
        int m_time_s = -1;//存文件开始
        int m_time_m = -1;//存文件开始
        DateTime CreatFileTime;    //创建文件时的时间
        DateTime CreatFileTime_s;    //创建文件时的时间
        DateTime CreatFileTime_m;    //创建文件时的时间
        FileStream fs;     //声明一个用于写文件的流
        FileStream fs_slow;
        FileStream fs_monitor;

        void saveDataToFileFun()
        {
            CRadarData.FastFT1 ft1;
            byte[] ft1byte = new byte[1100];     //用于存放快扫数据
            CRadarData.SlowDada slowdata;
            byte[] slowbyte = new byte[1100];
            byte[] monitorbyte = new byte[1472];
            DateTime now = new DateTime();    //用于获取当前时间
            int min = 0;       //分别用于存放当前时刻的年月日、时分秒信息
            int sec = 0;
            int hour = 0;

            int year = 0;
            int month = 0;
            int day = 0;

            byte[] bigData = new byte[1100 * 1024 ];    //1024帧数据写一次文件,存快扫的数据
            byte[] bigData_s = new byte[1100 * 1024 ];    //存慢扫的数据
            byte[] bigData_m = new byte[1472 * 1024];    //存频谱监测数据
            //byte[] controlData = new byte[160];

            //m.WaitOne();
            //fs = File.Open(Control_filepath, FileMode.Open);
            //fs.Read(controlData, 0, 160);
            //fs.Close();
            //m.ReleaseMutex();
            //Buffer.BlockCopy(controlData, 0, bigData, 0, 160);   //将控制字作为头文件写入
            //Buffer.BlockCopy(controlData, 0, bigData_s, 0, 160);   //
            //Buffer.BlockCopy(controlData, 0, bigData_m, 0, 160);   //


            string strRemote = this.m_socketUserToken.m_connectSocket.RemoteEndPoint.ToString();      //将远端的端点信息转化为字符串
            strRemote = strRemote.Replace(":", "_");      //将字符串中的的":"用"-"替换
            while (true)
            {
                if (m_radarData.m_concurrentQueueFastFT1file.TryDequeue(out ft1byte))
                {
                    now = DateTime.Now;   //获取当前时间
                    min = now.Minute;
                    hour = now.Hour;
                    sec = now.Second;
                    year = now.Year;
                    month = now.Month;
                    day = now.Day;
                    Buffer.BlockCopy(ft1byte, 0, bigData, indexfile * 1100, 1100);   //将每帧信息放入大容器
                    indexfile++;
                    if (indexfile == 1024)      //积累1024帧数据开始存文件
                    {
                        //string directoryName = DateTime.Now.ToString("yyyyMMddhh");     //把此时的时间当做文件名
                        string directoryName = now.ToString("yyyy-MM-dd") + "_Fast";     //把此时的时间当做文件名
                        string directoryPath = "E:\\TestData\\" + strRemote + "\\" + directoryName + "\\" + now.ToString("yyyy-MM-dd-HH");//不同的连接通道，存在不同的文件夹下
                        if (Directory.Exists(directoryPath) == false)       //首先判断该文件是否存在，若不存在新建一个文件
                        {
                            Directory.CreateDirectory(directoryPath);

                        }
                        //directoryName = directoryPath + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss"); 
                        directoryName = directoryPath + "\\" + now.ToString("yyyy-MM-dd-HH-mm-ss");     //文件所在的路径
                        TimeSpan span = new TimeSpan();      //用来记录时间讲个
                        span = DateTime.Now - CreatFileTime;
                        if (m_time == -1 || span.TotalMinutes > 1)
                        {
                            m_time = min;
                            CreatFileTime = DateTime.Now;
                            if (fs != null)
                            {
                                fs.Close();

                            }
                            fs = File.Open(directoryName, FileMode.Append);       //打开一个文件夹，从该文件后接着写

                            fs.Write(bigData, 0, bigData.Length);
                            //fs.Close();
                        }
                        else
                        {
                            fs.Write(bigData, 0, bigData.Length);
                        }
                        indexfile = 0;
                    }
                }
                else if (m_radarData.m_concurrentQueueSlowDatafile.TryDequeue(out slowbyte))
                {
                    now = DateTime.Now;
                    min = now.Minute;
                    hour = now.Hour;
                    sec = now.Second;
                    year = now.Year;
                    month = now.Month;
                    day = now.Day;
                    Buffer.BlockCopy(slowbyte, 0, bigData_s, indexfile * 1100 , 1100);   //将每帧信息放入大容器
                    indexfile_s++;
                    if (indexfile_s == 1024)         //1024帧文件存一次数据
                    {
                        string directoryName = now.ToString("yyyy-MM-dd") + "_Slow";     //把此时的时间当做文件名
                        string directoryPath = "E:\\TestData\\" + strRemote + "\\" + directoryName;//不同的连接通道，存在不同的文件夹下
                        if (Directory.Exists(directoryPath) == false)       //首先判断该文件是否存在，若不存在新建一个文件
                        {
                            Directory.CreateDirectory(directoryPath);
                        }
                        directoryName = directoryPath + "\\" + now.ToString("yyyy-MM-dd-HH-mm-ss");     //文件所在的路径
                        TimeSpan span = new TimeSpan();      //用来记录时间讲个
                        span = DateTime.Now - CreatFileTime_s;
                        if (m_time_s == -1 || span.TotalMinutes > 1)
                        {
                            m_time_s = min;
                            CreatFileTime_s = DateTime.Now;
                            if (fs_slow != null)
                            {
                                fs_slow.Close();
                            }
                            fs_slow = File.Open(directoryName, FileMode.Append);       //打开一个文件夹，从该文件后接着写
                            fs_slow.Write(bigData_s, 0, bigData_s.Length);
                            //fs.Close();
                        }
                        else
                        {
                            fs_slow.Write(bigData_s, 0, bigData_s.Length);
                        }
                        indexfile_s = 0;
                    }
                }
                else if (m_radarData.m_concurrentQueueMonitorDatafile.TryDequeue(out monitorbyte))
                {
                    now = DateTime.Now;
                    min = now.Minute;
                    hour = now.Hour;
                    sec = now.Second;
                    year = now.Year;
                    month = now.Month;
                    day = now.Day;
                    Buffer.BlockCopy(monitorbyte, 0, bigData_m, indexfile_m * 1472, 1472);   //将每帧信息放入大容器
                    indexfile_m++;
                    if (indexfile_m == 1024)
                    {
                        string directoryName = now.ToString("yyyy-MM-dd") + "_Monitor";     //把此时的时间当做文件名
                        string directoryPath = "E:\\TestData\\" + strRemote + "\\" + directoryName;//不同的连接通道，存在不同的文件夹下
                        if (Directory.Exists(directoryPath) == false)       //首先判断该文件是否存在，若不存在新建一个文件
                        {
                            Directory.CreateDirectory(directoryPath);
                        }
                        directoryName = directoryPath + "\\" + now.ToString("yyyy-MM-dd-HH-mm-ss");     //文件所在的路径
                        TimeSpan span = new TimeSpan();      //用来记录时间讲个
                        span = DateTime.Now - CreatFileTime_m;
                        if (m_time_m == -1 || span.TotalMinutes > 1)
                        {
                            m_time_m = min;
                            CreatFileTime_m = DateTime.Now;
                            if (fs_monitor != null)
                            {
                                fs_monitor.Close();

                             }
                        fs_monitor = File.Open(directoryName, FileMode.Append);       //打开一个文件夹，从该文件后接着写

                        fs_monitor.Write(bigData_m, 0, bigData_m.Length);
                            //fs.Close();
                        }
                        else
                        {
                            fs_monitor.Write(bigData_m, 0, bigData_m.Length);
                        }
                        indexfile_m = 0;
                    }
                }
            }

        }

        private void SaveData(MySql.Data.MySqlClient.MySqlConnection connection, List<string> dynamicSqlCommandCollection, DateTime Sample, int FrameIndex, int ParamIndex, byte AtnDataType, byte[] Atn1Data, int[] Atn2Data, int[] Atn3Data, int[] Atn4Data, int[] Atn5Data, int[] Atn6Data, int[] Atn7Data, int[] Atn8Data)
        {
            // 开始事务处理
            MySql.Data.MySqlClient.MySqlTransaction transaction = connection.BeginTransaction();
            MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
            //byte[] tmp = new byte[1024];
            //int[] tmpw = new int[1024];
            //for (int i = 0; i < 1024; i++)
            //{
            //    tmp[i] = 1;
            //    tmpw[i] = 2;

            //}
            //byte[] tmp;
            //foreach (var value in Atn1Data)
            //{
            //    byte[] byteArray = BitConverter.GetBytes(value);
            //  //  Console.WriteLine("{0,16}{1,10}{2,17}", value,
            //                   //   BitConverter.IsLittleEndian ? "Little" : " Big",
            //                   ///   BitConverter.ToString(byteArray));
            //}
            try
            {
                command.Parameters.AddWithValue("@Sample", Sample);
                command.Parameters.AddWithValue("@FrameIndex", FrameIndex);
                command.Parameters.AddWithValue("@ParamIndex", ParamIndex);
                command.Parameters.AddWithValue("@AtnDataType", AtnDataType);
                command.Parameters.AddWithValue("@Atn1Data", Atn1Data);
                command.Parameters.AddWithValue("@Atn2Data", Atn2Data);
                command.Parameters.AddWithValue("@Atn3Data", Atn3Data);
                command.Parameters.AddWithValue("@Atn4Data", Atn4Data);
                command.Parameters.AddWithValue("@Atn5Data", Atn5Data);
                command.Parameters.AddWithValue("@Atn6Data", Atn6Data);
                command.Parameters.AddWithValue("@Atn7Data", Atn7Data);
                command.Parameters.AddWithValue("@Atn8Data", Atn8Data);
                // 批量更新
                foreach (string cmd in dynamicSqlCommandCollection)
                {
                    command.CommandText = cmd;



                    command.ExecuteNonQuery();
                }

                // 提交数据入库
                transaction.Commit();
                //if (controlObject != null)
                //{
                //    // 提交错误日志
                //    // controlObject.AddStatusString("动态数据入库操作成功。动态目标数量[" + dynamicSqlCommandCollection.Count.ToString() + "]");
                //    if (dynamicSqlCommandCollection.Count != 0)
                //    {
                //        this.Invoke(new updateControlDelegate(updateControl), "动态数据入库操作成功。动态目标数量[" + dynamicSqlCommandCollection.Count.ToString() + "]");
                //    }
                //    //  this.Invoke(new updateControlDelegate(updateControl), "动态数据入库操作成功。动态目标数量[" + dynamicSqlCommandCollection.Count.ToString() + "]");
                //}
            }
            catch (Exception ex)
            {
                // 放弃当前的数据入库操作
                transaction.Rollback();
                //if (controlObject != null)
                //{
                //   // this.Invoke(new updateControlDelegate(updateControl), "动态数据入库操作失败。\r\n错误信息为：" + ex.ToString());
                //    // 提交错误日志
                //    //controlObject.AddStatusString("动态数据入库操作失败。\r\n错误信息为：" + ex.ToString());
                //}
                Console.WriteLine(ex);
            }
        }
        /// <summary>
        /// 协议通过发送客户端信息(被子类重写-执行子类的方法)
        /// </summary>
        /// <param name="buffer">接收到的信息</param>
        /// <param name="offset">从第几位开始</param>
        /// <param name="count">共有几位</param>
        /// <returns>true</returns>
        public virtual bool ProcessCommand(byte[] buffer, int offset, int count)
        {
            return true;
        }

        /// <summary>
        /// 发送回调函数
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public virtual bool SendCompleted(SocketUserToken userToken)
        {
            userToken.m_sendAsync = false;
            AsyncSendBufferManager asyncSendBufferManager = userToken.SendBuffer;
            asyncSendBufferManager.ClearFirstPacket(); //清除已发送的包
            int offset = 0;
            int count = 0;
            if (asyncSendBufferManager.GetFirstPacket(ref offset, ref count))//缓存中是否还有数据没有发送,有就继续发送
            {
                userToken.m_sendAsync = true;
                return m_asyncSocketServer.SendAsyncEvent(userToken.ConnectSocket, userToken.SendEventArgs,
                    asyncSendBufferManager.m_dynamicBufferManager.Buffer, offset, count);
            }
            else
                return true;
        }
    }
}
