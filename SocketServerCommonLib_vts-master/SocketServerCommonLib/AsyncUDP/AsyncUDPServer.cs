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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SocketServerCommonLib;
using System.Collections.Concurrent;
namespace SocketServerCommonLib
{
    /// <summary>
    /// Socket UDP-UDP远程连接P2P,不做转发,不做大量数据接收(只做NAT穿透服务器)(未做用户,设备集合未及时清理)(可以直接发送2直接获取设备)
    /// </summary>
    public class AsyncUDPServer
    {
        bool readerFalg = false;//状态标准；FALSE 可以写人，true;可以读取发出
        private string CentreServerIp = "192.168.0.169";

        public IPEndPoint m_ippoint;
        /// <summary>
        /// Socket字节套
        /// </summary>
        public Socket m_sListen;
        /// <summary>
        /// 是否已启动监听
        /// </summary>
        public bool IsStartListening = false;
        /// <summary>
        /// 心跳检测(毫秒)
        /// </summary>
        public int socketTimeoutMs { get; set; }
        /// <summary>
        /// 客户端集合
        /// </summary>
        private List<SocketUserUDP> userInfoList;
        /// <summary>
        /// 设备端集合
        /// </summary>
        public List<SocketUserUDP> DeviceInfoList{get;set;}
        /// <summary>
        /// 服务器绑定地址
        /// </summary>
        private IPEndPoint loclhostIpEndPoint;
        /// <summary>
        /// 客户端
        /// </summary>
        public EndPoint RemoteEndPoint;
        /// <summary>
        /// 缓存地址
        /// </summary>
        public byte[] BufferData;
        /// <summary>
        /// 监听端口
        /// </summary>
        public int ListenProt = 6000;
        /// <summary>
        /// 信号量
        /// </summary>
        private Semaphore semap;
        /// <summary>
        /// 线程监听
        /// </summary>
        Thread thread;
        /// <summary>
        /// 心跳线程
        /// </summary>
        DaemonThreadUDP m_DaemonThread;

        public NmeaParse m_np;
        public ConcurrentQueue<byte[]> dataConcurrent = new ConcurrentQueue<byte[]>();
   
        public  Thread tcpSendThread ;
        public List<SocketUserUDP> UserInfoList
        {
            get { return userInfoList; }
            set { userInfoList = value; }
        }

        public AsyncUDPServer()
        {
            socketTimeoutMs = 6000;
            BufferData = new byte[1024 * 1000];
            loclhostIpEndPoint = new IPEndPoint(IPAddress.Any, ListenProt);
         
            //loclhostIpEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.134"), ListenProt);
            UserInfoList = new List<SocketUserUDP>();
            DeviceInfoList = new List<SocketUserUDP>();
            m_sListen = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            m_sListen.Bind(loclhostIpEndPoint);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            RemoteEndPoint = (EndPoint)(sender);
            //RemoteEndPoint = m_sListen.RemoteEndPoint;
          //  tcpSendThread = new Thread(tcpFun);

          //  tcpSendThread.Start();
        }
        public void tcpFun()
        {
            byte[] bufferTmp;
            while(true)
            {
              // if(dataConcurrent.TryDequeue(out bufferTmp)) 
              //    m_np.DataTcpSend(bufferTmp, bufferTmp.Length);
              //// Thread.Sleep(1);
              //  //m_np.m_tcpClient.SendDataByte
               
            }
        }
        public AsyncUDPServer(int port)
        {
            socketTimeoutMs = 6000;
            BufferData = new byte[1024 * 1000];
            loclhostIpEndPoint = new IPEndPoint(IPAddress.Any, port);
            ListenProt = port;
            //    loclhostIpEndPoint = new IPEndPoint(IPAddress.Parse("172.19.10.222"), ListenProt);
            UserInfoList = new List<SocketUserUDP>();
            DeviceInfoList = new List<SocketUserUDP>();
            m_sListen = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            m_sListen.Bind(loclhostIpEndPoint);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            RemoteEndPoint = (EndPoint)(sender);
          //  tcpSendThread = new Thread(tcpFun);

         //   tcpSendThread.Start();
        }

        /// <summary>
        /// 启动UDP
        /// </summary>
        public void Start()
        {
            if (IsStartListening)
                return;

        

            semap = new Semaphore(30000, 30000);
            IsStartListening = true;

            thread = new Thread(new ThreadStart(StartAccept));
            thread.Start();

        
            //m_DaemonThread = new DaemonThreadUDP(this);
            //DelegateState.ServerStateInfo("UDP服务器启动...");
        }

        /// <summary>
        /// 启动UDP
        /// </summary>
        public void Start(ref NmeaParse np)
        {
            if (IsStartListening)
                return;

            semap = new Semaphore(30000, 30000);
            IsStartListening = true;


            m_np = new NmeaParse(this);
            CentreServerIp = m_np.m_CenTrlIP;

       
            np = m_np;
         //   CentreServerIp = m_np.m_CenTrlIP;  //UDP接收雷达中心服务器接收IP

            thread = new Thread(new ThreadStart(StartAccept));
            thread.Start();
          
          
            //m_DaemonThread = new DaemonThreadUDP(this);
            //DelegateState.ServerStateInfo("UDP服务器启动...");
        }
        /// <summary>
        /// 监控
        /// </summary>
        int index = 0;
        int count = 0;
        public void StartAccept()
        {
            //异步操作
          //  m_sListen.BeginReceiveFrom(m_sListen.Buffer, 0, state.Buffer.Length,m_sListen.None,ref m_sListen.RemoteEP,EndReceiveFromCallback,state);
            ulong head = 0x1111FFFFEEEE0000;
            ulong headh = 0x1111FFFF;
            ulong headl = 0xEEEE0000;
            ulong headTmph;
            ulong headTmpl;
            ulong headTmp;
            byte[] tmp = new byte[1024*100];
            byte[] bigData = new byte[1024*500];
         //   m_ippoint = new IPEndPoint(IPAddress.Parse(CentreServerIp),50000);
            m_sListen.ReceiveBufferSize = 1024 * 2048 * 2;
            while(true)
            {
               
                try
                {

                    int datalen = m_sListen.ReceiveFrom(BufferData, ref RemoteEndPoint);
                    
                    //int datalen = m_sListen.ReceiveFrom(tmp, ref RemoteEndPoint);


                    m_np.DataTcpSend(BufferData, datalen);




                  //  m_np.DataTcpSend(BufferData, datalen);
                 //  Console.WriteLine("-------------------------------"+datalen.ToString());
                    //headTmph = (ulong)BufferData[3] << 32 | (ulong)BufferData[2] << 40 | (ulong)BufferData[1] << 48 | (ulong)BufferData[0] << 56;
                    //headTmpl = (ulong)BufferData[7] | (ulong)BufferData[6] << 8 | (ulong)BufferData[5] << 16 | (ulong)BufferData[4] << 24;
                    //headTmp = headTmpl | headTmph;
                    //if (head == headTmp)
                    //{
                    //    if (datalen >= 11)
                    //    {
                    //        int packetLength = getLength(BufferData[11]);
                    //    }
                    //}

                    ////byte[] bufferTmp = new byte[datalen];
                    ////Buffer.BlockCopy(BufferData, 0, bufferTmp, count, datalen);

                    ////dataConcurrent.Enqueue(bufferTmp);


                    //count = datalen + count;
                    //if (count >= 102400)
                    //{
                    //   // m_np.DataTcpSend(bigData, count);

                    //    dataConcurrent.Enqueue(bigData);
                    //    count = 0;
                    //}

                    
                    //string str = System.Text.Encoding.Default.GetString(BufferData, 0, datalen);
                    ////((IPEndPoint)RemoteEndPoint).Address
                    //if (((IPEndPoint)RemoteEndPoint).Address.ToString() == m_ippoint.Address.ToString())//数据来源于中心服务器
                    //{
                    //    char[] chSplit = { ',', '*' };
                    //    string[] strArray = str.Split(chSplit);
                    //    string sIP = strArray[1];
                    //    IPEndPoint RadarEnnPoint = new IPEndPoint(IPAddress.Parse(sIP), 6000);

                    //    m_sListen.SendTo(BufferData, datalen, SocketFlags.None, RadarEnnPoint);
                    //}
                    //else//来源于雷达站
                    //{

                    //    if (m_np != null)
                    //    {
                    //        string sdataType = m_np.DataProcess(str, BufferData);

                    //        if (sdataType == "$HEALR" || sdataType == "$HETAR")//报警/删除直接转发给中心服务器
                    //        {

                    //         //   m_sListen.SendTo(BufferData, datalen, SocketFlags.None, m_ippoint);
                    //        }
                    //        else if (sdataType == "$HEREC")
                    //        {
                    //           // m_sListen.SendTo(BufferData, datalen, SocketFlags.None, m_ippoint);
                    //        }

                    //    }

                    //}
                   // Thread.Sleep(10);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                
               // m_sListen.SendTo(BufferData, datalen, SocketFlags.None, RemoteEndPoint);
            }
         
            // semap.WaitOne();
            //if (BufferData[0] == 0x1)
            //{
            //    string username = Encoding.UTF8.GetString(BufferData, 1, BufferData.Length);
            //    SocketUserUDP userUdp = new SocketUserUDP();
            //    userUdp.ipEndPoint = RemoteEndPoint;
            //    userUdp.ActiveDateTime = DateTime.Now;
            //    userUdp.UserName = username;
            //    userUdp.password = username;
            //    if (userUdp.password.Length > 4)
            //    {
            //        //密码小于4是设备
            //        DeviceInfoList.Add(userUdp);
            //        DelegateState.ServerStateInfo(RemoteEndPoint.ToString() + "远端设备连接");
            //    }
            //    else
            //    {
            //        UserInfoList.Add(userUdp);
            //        DelegateState.ServerStateInfo(RemoteEndPoint.ToString() + "远端用户连接");
            //    }
            //    m_sListen.SendTo(Encoding.UTF8.GetBytes("连接成功!"), RemoteEndPoint);
            //    DelegateState.ServerConnStateInfo(RemoteEndPoint.ToString(), "UDP");
            //}
            //else if (BufferData[0] == 0x2)
            //{
            //    string username = Encoding.UTF8.GetString(BufferData, 1, BufferData.Length);
            //    foreach (SocketUserUDP user in DeviceInfoList)
            //    {
            //        if (user.UserName == username)
            //        {
            //            user.ActiveDateTime = DateTime.Now;
            //            m_sListen.SendTo(Encoding.UTF8.GetBytes(user.ipEndPoint.ToString()), RemoteEndPoint);
            //            DelegateState.ServerStateInfo(RemoteEndPoint.ToString() + "远端用户:" + user.UserName + "搜索设备.");
            //        }
            //    }
            //}
            //else if (BufferData[0] == 0x3)
            //{
            //    string username = Encoding.UTF8.GetString(BufferData, 1, BufferData.Length);
            //    foreach (SocketUserUDP user in UserInfoList)
            //    {
            //        if (user.UserName == username)
            //        {
            //            DeviceInfoList.Remove(user);
            //            DelegateState.ServerStateInfo("UDP:" + RemoteEndPoint.ToString() + "远端用户退出");
            //            break;
            //        }
            //    }
            //}
            //else
            //{

            //    m_sListen.SendTo(BufferData, datalen, SocketFlags.None,RemoteEndPoint);
            //    //DelegateState.ServerStateInfo("UDP:" + RemoteEndPoint.ToString() + "发送空数据");
            //}
            //semap.Release();
           //StartAccept();
        }
        public static int getLength(byte type)
        {
            int length = 0;
            switch (type)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
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
                        // length = 1344;
                        break;
                    }
                case 4:
                    {
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
        /// 关闭
        /// </summary>
        public void Close()
        {
            thread.Abort();
            m_sListen.Shutdown(SocketShutdown.Both);
            DeviceInfoList.Clear();
            userInfoList.Clear();
            m_sListen.Close();
            IsStartListening = false;
            GC.Collect();
        }
    }
}
