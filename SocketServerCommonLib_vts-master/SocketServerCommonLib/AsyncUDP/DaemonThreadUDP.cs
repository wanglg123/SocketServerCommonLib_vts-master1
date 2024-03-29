﻿/********************************************************************
 * * 使本项目源码前请仔细阅读以下协议内容，如果你同意以下协议才能使用本项目所有的功能,
 * * 否则如果你违反了以下协议，有可能陷入法律纠纷和赔偿，作者保留追究法律责任的权利。
 * *
 * * Copyright (C) 2014-? cskin Corporation All rights reserved.
 * * 作者： Amos Li    QQ：443061626   .Net项目技术组群:Amos Li 出品
 * * 请保留以上版权信息，否则作者将保留追究法律责任。
 * * 创建时间：2014-08-05
********************************************************************/

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SocketServerCommonLib
{
    /// <summary>
    /// 判断连接是否超时
    /// </summary>
    public class DaemonThreadUDP
    {
        private Thread m_thread;
        private AsyncUDPServer m_asyncudpServer;
   //     private SyncSocketClient m_syncSocketClient;
        int TeartbeatCount = 0;
        public DaemonThreadUDP(AsyncUDPServer asyncudpServer)
        {
            m_asyncudpServer = asyncudpServer;
            m_thread = new Thread(DaemonThreadStart);
            m_thread.Start();
        }

        public void DaemonThreadStart()
        {
            while (m_thread.IsAlive)
            {
                /*
                lock (m_asyncudpServer.m_np.m_dicTargetCollection)
                {
                    if (!m_asyncudpServer.m_np.readerFalg)
                    {
                          
                           try
                           {
                               //等待WriteToCell方法中调用Monitor.Pulse()方法将这个线程唤醒
                               Monitor.Wait(m_asyncudpServer.m_np.m_dicTargetCollection);
                           }
                           catch (SynchronizationLockException e)
                           {
                               Console.WriteLine(e);
                           }

                           foreach (var item in m_asyncudpServer.m_np.m_dicTargetCollection)
                           {
                               Console.WriteLine(item.Key.ToString() + "  " + item.Value.IP);
                               string strHttm = m_asyncudpServer.m_np.CombinHETTM(item.Value);
                              
                               m_asyncudpServer.m_sListen.SendTo(System.Text.Encoding.Default.GetBytes(strHttm), strHttm.Length, SocketFlags.None, m_asyncudpServer.m_ippoint);

                               
                           }
                           m_asyncudpServer.m_np.m_dicTargetCollection.Clear();
                    }

                    m_asyncudpServer.m_np.readerFalg = false;
                    Monitor.Pulse(m_asyncudpServer.m_np.m_dicTargetCollection);
                }
              
                */
              //  m_asyncudpServer.m_sListen.SendTo(BufferData, datalen, SocketFlags.None, RemoteEndPoint);
                //SocketUserUDP[] userUDPArray = new SocketUserUDP[m_asyncudpServer.DeviceInfoList.Count];
                //m_asyncudpServer.DeviceInfoList.CopyTo(userUDPArray);
                //for (int i = 0; i < userUDPArray.Length; i++)
                //{
                //    if (!m_thread.IsAlive)
                //        break;
                //    try
                //    {
                //        if ((DateTime.Now - userUDPArray[i].ActiveDateTime).Milliseconds > m_asyncudpServer.socketTimeoutMs)
                //        {
                //            lock (userUDPArray[i])
                //            {
                //                m_asyncudpServer.DeviceInfoList.Remove(userUDPArray[i]);
                //            }
                //        }
                //        for (int x = 0; x < 60 * 1000 / 10; x++) //每十分钟检测一次
                //        {
                //            if (!m_thread.IsAlive)
                //                break;
                //            Thread.Sleep(100);
                //        }
                //        TeartbeatCount++;
                //        DelegateState.ServerStateInfo("UDP:" + TeartbeatCount + "心跳检测");
                //    }
                //    catch (Exception ex)
                //    {
                //        DelegateState.ServerStateInfo("Error:类[DaemonThreadUDP]" + ex.Message);
                //    }
                //}
                Thread.Sleep(100);
            }
        }
        /// <summary>
        /// 停用线程
        /// </summary>
        public void Close()
        {
            m_thread.Abort();
        }

    }
}
