/********************************************************************
 * * 使本项目源码前请仔细阅读以下协议内容，如果你同意以下协议才能使用本项目所有的功能,
 * * 否则如果你违反了以下协议，有可能陷入法律纠纷和赔偿，作者保留追究法律责任的权利。
 * *
 * * Copyright (C) 2014-? cskin Corporation All rights reserved.
 * * 作者： Amos Li    QQ：443061626   .Net项目技术组群:Amos Li 出品
 * * 网站： CSkin界面库 http://www.cskin.net 作者:乔克斯 感谢免费支持
 * * 请保留以上版权信息，否则作者将保留追究法律责任。
 * * 创建时间：2014-08-05
********************************************************************/
using CCWin;
using CCWin.SkinControl;
using System;
using System.Windows.Forms;
using SocketServerCommonLib;
using System.Threading;
using System.Drawing;
using System.IO;
using AISParserLibrary;
using SocketServerCommonLib;
using System.Data.OleDb;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;

namespace SocketServer
{
    public partial class TCPUDPServer : CCSkinMain
   // public partial class TCPUDPServer : Form
    {
       public AsyncSocketServer TcpServer;

       public  AsyncUDPServer UdpServer;

        Thread updateChartThread;
        public AisControl controlPage;

        private NmeaParse m_np;
        public delegate void radarDataDelegate(SocketUserToken user, params byte[] buffer);
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
        MySql.Data.MySqlClient.MySqlConnection m_connection = null; 
        /// <summary>
        /// 用户连接多少次
        /// </summary>
        int TCPUserCount = 0;
        int TCPDeviceCount = 0;
        public TCPUDPServer()
        {
            InitializeComponent();
            DelegateState.ServerStateInfo = ServerShowStateInfo;
            DelegateState.TeartbeatServerStateInfo = TeartbeatShowStateInfo;
            DelegateState.AddTCPuserStateInfo = AddTCPuser;
            DelegateState.AddTCPdeviceStateInfo = AddTCPdevice;
            DelegateState.ReomveTCPStateInfo = ReomveTCP;
            DelegateState.ServerConnStateInfo = ConnStateInfo;
            this.MouseWheel += TCPUDPServer_MouseWheel;
            this.MouseClick += TCPUDPServer_MouseClick;
            this.panel1.MouseWheel += panel1_MouseWheel;
            tabPageEx2.Parent = null;
            tabPageEx3.Parent = null;
            tabPageEx4.Parent = null;
            //tabPageEx5.Parent = null;
            tabPageEx7.Parent = null;
            tabPageEx8.Parent = null;
            tabPageEx6.Parent = null;
            initchart();
            updateChartThread = new Thread(updateChart);
           
            //try
            //{
            //    m_tcpClient = new SyncSocketClient();

            //    m_tcpClient.Connect("192.168.0.169", 6002);
            //}
            //catch (System.Exception ex)
            //{
            	
            //}
          
           // selectRadarId();

        }
        delegate void delegetUpdateChart();
        void updateChart()
        {
            try
            {
                while (true)
                {
                    // initchart();
                    if (tabcontroSelect!= 2)
                    {
                        continue;
                    }
                    this.Invoke(new delegetUpdateChart(updatechart));
                    Thread.Sleep(100);
                    
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
           

        }
        void panel1_MouseWheel(object sender, MouseEventArgs e)
        {
            //webtoPic(webBrowser1);
            //throw new NotImplementedException();
        }

        void TCPUDPServer_MouseClick(object sender, MouseEventArgs e)
        {
           // webtoPic(webBrowser1);
         //   panel2.BackColor = Color.Transparent;
           
          //  throw new NotImplementedException();
        }

        void TCPUDPServer_MouseWheel(object sender, MouseEventArgs e)
        {
           // panel2.Visible = true;
            //webtoPic(webBrowser1);
            
            //panel1.Visible = true;
            //throw new NotImplementedException();

        }

        #region  AmosLi produce <启动服务模块>

        /// <summary>
        /// 启动TCP服务
        /// </summary>
        private void btnTCP_Click(object sender, EventArgs e)
        {
            btnTCP.Enabled = false;

            if (TcpServer == null)
                TcpServer = new AsyncSocketServer();
            if (!TcpServer.IsStartListening)
            {
                TcpServer.Start();
                txtMsg.AppendText(DateTime.Now + Environment.NewLine + "TCP服务器启动" + Environment.NewLine);
                lblTCP.Text = "TCP服务器地址:" + TcpServer.serverconfig.ListenIp + ":" + TcpServer.serverconfig.ListenPort;
                PicBoxTCP.BackgroundImage = Properties.Resources._07822_48x48x8BPP_;
                btnTCP.Text = "TCP停止服务";


                updateChartThread.Start();
            }
            else
            {
                TcpServer.Stop();
                PicBoxTCP.BackgroundImage = Properties.Resources._07821_48x48x8BPP_;
                txtMsg.AppendText(DateTime.Now + Environment.NewLine + "TCP服务器停止" + Environment.NewLine);
            }
            btnTCP.Enabled = true;
        }


        /// <summary>
        /// UDP服务
        /// </summary>
        private void btnUDP_Click(object sender, EventArgs e)
        {
            btnUDP.Enabled = false;
            if (UdpServer == null)
                UdpServer = new AsyncUDPServer();

         // AsyncUDPServer UdpServer2 = new AsyncUDPServer(6001);

          //UdpServer2.Start(ref m_np);

            if (!UdpServer.IsStartListening)
            {
               // UdpServer.Start();
                UdpServer.Start(ref m_np);

                //// 尝试自动开始串口信息处理
                //PortSettings setting = AddPortForm.AutoPortSetting;
                //if (setting != null)
                //{
                //    // 添加新的设备页
                //    controlPage = new AisControl(setting,UdpServer);
                //    if (controlPage.IsOpen())
                //    {
                //        controlPage.Dock = DockStyle.Fill;
                //        TabPage page = new TabPage(setting.PortName);

                //        tabPageEx6.Controls.Add(controlPage);
                      
                //    }
                //}
               // lblUDP.Text = "UDP服务器地址:" + HelpCommonLib.NetworkAddress.GetIPAddress() + ":" + UdpServer.ListenProt;

                string str = "地址:" + HelpCommonLib.NetworkAddress.GetIPAddress() + ":" + UdpServer.ListenProt;
                txtMsg.AppendText(DateTime.Now + Environment.NewLine + "UDP服务器启动 "+str + Environment.NewLine);

                //str = "地址:" + HelpCommonLib.NetworkAddress.GetIPAddress() + ":" + UdpServer2.ListenProt;
                //txtMsg.AppendText(DateTime.Now + Environment.NewLine + "UDP服务器启动 " + str + Environment.NewLine);
               // lblUDP.Text = "UDP服务器地址:" + HelpCommonLib.NetworkAddress.GetIPAddress() + ":" + UdpServer2.ListenProt;

                PicBoxUDP.BackgroundImage = Properties.Resources._07822_48x48x8BPP_;
                btnUDP.Text = "UDP停止服务";
                btnUDP.Enabled = true;

                //timer1.Start();
            }
            else
            {
                txtMsg.AppendText(DateTime.Now + Environment.NewLine + "UDP服务器停止" + Environment.NewLine);
                UdpServer.Close();
                PicBoxUDP.BackgroundImage = Properties.Resources._07821_48x48x8BPP_;
                btnUDP.Text = "UDP端口错误";

               // UdpServer2.Close();
            }
        }
        #region

        #region TCP回调函数操作
        void AddTCPuser(SocketUserToken userToken)
        {
            this.Invoke(new ThreadStart(delegate
            {
                TCPUserCount++;
                ListViewItem lvi = new ListViewItem();
                lvi.Text = TCPUserCount.ToString();
                lvi.SubItems.Add(userToken.ConnectSocket.RemoteEndPoint.ToString());
                lvi.SubItems.Add(userToken.UserName);
                lvi.SubItems.Add(userToken.ConnectDateTime.ToString());
                lvi.SubItems.Add("TCP");
                tpe3list1.Items.Add(lvi);
            }));
        }

        /// <summary>
        /// 删除用户或者设备
        /// </summary>
        /// <param name="userToken"></param>
        void ReomveTCP(SocketUserToken userToken)
        {
            this.Invoke(new ThreadStart(delegate
            {
                if (userToken.isDevice)
                {
                    for (int i = 0; i < tpe2list1.Items.Count; i++)
                    {
                        if (userToken.UserName == tpe2list1.Items[i].SubItems[2].Text)
                        {
                            tpe2list1.Items.Remove(tpe2list1.Items[i]);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < tpe3list1.Items.Count; i++)
                    {
                        if (userToken.UserName == tpe3list1.Items[i].SubItems[2].Text)
                        {
                            tpe3list1.Items.Remove(tpe3list1.Items[i]);
                        }
                    }
                }
            }));
        }

        void AddTCPdevice(SocketUserToken userToken)
        {
            this.Invoke(new ThreadStart(delegate
            {
                TCPDeviceCount++;
                ListViewItem lvi = new ListViewItem();
                lvi.Text = TCPDeviceCount.ToString();
                lvi.SubItems.Add(userToken.ConnectSocket.RemoteEndPoint.ToString());
                lvi.SubItems.Add(userToken.UserName);
                lvi.SubItems.Add(userToken.ConnectDateTime.ToString());
                lvi.SubItems.Add("TCP");
                tpe2list1.Items.Add(lvi);
            }));
        }
        #endregion

        void ConnStateInfo(string RemoteIp, string TCPUDP)
        {
            this.Invoke(new ThreadStart(delegate
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = listAllView.Items.Count.ToString();
                lvi.SubItems.Add(RemoteIp);
                lvi.SubItems.Add(DateTime.Now.ToString());
                lvi.SubItems.Add(TCPUDP);
                listAllView.Items.Add(lvi);
            }));
        }


        /// <summary>
        /// 信息添加
        /// </summary>
        /// <param name="msg"></param>
        void ServerShowStateInfo(string msg)
        {
            this.Invoke(new ThreadStart(delegate
            {
                tpe2txtMsg.AppendText(DateTime.Now + ":" + msg + Environment.NewLine);
            }));
        }

        /// <summary>
        /// 心跳时间
        /// </summary>
        void TeartbeatShowStateInfo(int num)
        {
            this.Invoke(new ThreadStart(delegate
            {
                txtMsg.AppendText(DateTime.Now + ":" + num + "连接检测");
                lblNum1.NormlBack = ImageListAllUpdate(num / 10 % 10);
                lblNum2.NormlBack = ImageListAllUpdate(num % 10);
            }));
        }
        #endregion
        /// <summary>
        /// 图片更换
        /// </summary>
        Image ImageListAllUpdate(int Num)
        {
            switch (Num)
            {
                case 0:
                    return Properties.Resources._00034_17x25x8BPP_;
                case 1:
                    return Properties.Resources._00035_17x25x8BPP_;
                case 2:
                    return Properties.Resources._00036_17x25x8BPP_;
                case 3:
                    return Properties.Resources._00037_17x25x8BPP_;
                case 4:
                    return Properties.Resources._00038_17x25x8BPP_;
                case 5:
                    return Properties.Resources._00039_17x25x8BPP_;
                case 6:
                    return Properties.Resources._00040_17x25x8BPP_;
                case 7:
                    return Properties.Resources._00041_17x25x8BPP_;
                case 8:
                    return Properties.Resources._00042_17x25x8BPP_;
                case 9:
                    return Properties.Resources._00043_17x25x8BPP_;
                default:
                    return null;
            }
        }
        #endregion
        /// <summary>
        /// 刷新设备列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkdeviceRefresh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (TcpServer == null)
                return;

            linkdeviceRefresh.Enabled = false;
            tpe2list1.Items.Clear();
            SocketUserToken[] userTokenArray = null;
            TcpServer.AsyncSocketDeviceList.CopyList(ref userTokenArray);
            int num = 0;
            for (int i = 0; i < userTokenArray.Length; i++)
            {
                if (userTokenArray[i].LoginFlag)
                {
                    AddTCPdevice(userTokenArray[i]);
                }
                num++;
            }
            lbldevice.Text = "上次刷新是在 " + DateTime.Now.Hour + " 点，共连接 " + userTokenArray.Length + " 个客户。";
            linkdeviceRefresh.Enabled = true;
        }

        private void linkuserRefresh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (TcpServer == null)
                return;

            linkuserRefresh.Enabled = false;
            tpe3list1.Items.Clear();
            SocketUserToken[] userTokenArray = null;
            TcpServer.AsyncSocketUserList.CopyList(ref userTokenArray);
            int num = 0;
            for (int i = 0; i < userTokenArray.Length; i++)
            {
                if (userTokenArray[i].LoginFlag)
                {
                    AddTCPuser(userTokenArray[i]);
                    num++;
                }
            }
            lbluser.Text = "上次刷新是在 " + DateTime.Now.Hour + " 点，共连接 " + num + " 个客户。";
            linkuserRefresh.Enabled = true;
        }
        /// <summary>
        /// 保存信息
        /// </summary>
        private void linkSaveMsg_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter write = new StreamWriter(saveFileDialog1.FileName))
                {
                    write.WriteLine(tpe2txtMsg.Text);
                }
            }
        }

        private void TCPUDPServer_Load(object sender, EventArgs e)
        {
            //// TODO: 这行代码将数据加载到表“databaseRadarConfigDataSet2.CenTreIP”中。您可以根据需要移动或删除它。
            //this.cenTreIPTableAdapter1.Fill(this.databaseRadarConfigDataSet2.CenTreIP);
            //// TODO: 这行代码将数据加载到表“databaseRadarConfigDataSet1.CenTreIP”中。您可以根据需要移动或删除它。
            //this.cenTreIPTableAdapter.Fill(this.databaseRadarConfigDataSet1.CenTreIP);
            //// TODO: 这行代码将数据加载到表“databaseRadarConfigDataSet.RadarIP”中。您可以根据需要移动或删除它。
            //this.radarIPTableAdapter.Fill(this.databaseRadarConfigDataSet.RadarIP);
            try
            {
                // TODO: 这行代码将数据加载到表“databaseNetMonitorSysDataSet.ChargeRecord”中。您可以根据需要移动或移除它。
                //  this.chargeRecordTableAdapter.Fill(this.databaseNetMonitorSysDataSet.ChargeRecord);
                // TODO: 这行代码将数据加载到表“databaseNetMonitorSysDataSet.ChargeRecord”中。您可以根据需要移动或移除它。
                //  this.chargeRecordTableAdapter.Fill(this.databaseNetMonitorSysDataSet.ChargeRecord);
                //  freshDataGridviw();
                // 设置默认数据库连接字符串
                string connectionString = "";
                /*
                connectionString += "server=" + global::DatabaseMonitor.Properties.Settings.Default.DatabaseAddress;
                connectionString += ";User Id=" + global::DatabaseMonitor.Properties.Settings.Default.DatabaseAcount;
                connectionString += ";Password=" + global::DatabaseMonitor.Properties.Settings.Default.DatabasePassword;
                connectionString += ";database=" + global::DatabaseMonitor.Properties.Settings.Default.DatabaseName;
                 * */
                // connectionString += "server=10.2.1.9";
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
                timer2.Start();
                //FastTest();
            }
            catch (System.Exception ex)
            {
                // MessageBox.Show(ex.ToString());
            }
           
            
        }

        private void TCPUDPServer_MouseMove(object sender, MouseEventArgs e)
        {
            //int i = 0;
            //i++;
            //i = e.X;
            //Console.WriteLine(e.X);
        }

        private void pictureBox_radar_Paint(object sender, PaintEventArgs e)
        {

            
        }
        private int tabcontroSelect = 0;
        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (TabControl1.SelectedIndex==6)
            //{
            //    FormRadar radar = new FormRadar(ref m_np);
            //    radar.ShowDialog();
            //}
            try
            {
                if (TabControl1.SelectedIndex == 2)
                {
                    tabcontroSelect = 2;
                  //  pictureBox_radar.Image = m_np.m_radarimg;

                    //   webBrowser1.Focus();
                }
                else 
                {
                    tabcontroSelect = 0;
                }
            }
            catch (System.Exception ex)
            {
            	
            }
          
        }
        public void webtoPic(WebBrowser webBrowser)
        {
            // 网页加载完毕才保存
            if (webBrowser.ReadyState == WebBrowserReadyState.Complete)
            {
               
                // 获取网页高度和宽度,也可以自己设置
                int height = webBrowser.Document.Body.ScrollRectangle.Height;

                int width = webBrowser.Document.Body.ScrollRectangle.Width;
                // 调节webBrowser的高度和宽度
                webBrowser.Height = height;
                webBrowser.Width = width;
                Bitmap bitmap = new Bitmap(width, height);  // 创建高度和宽度与网页相同的图片
                Rectangle rectangle = new Rectangle(0, 0, width, height);  // 绘图区域
                webBrowser.DrawToBitmap(bitmap, rectangle);  // 截图
                // 保存图片对话框
                // SaveFileDialog saveFileDialog = new SaveFileDialog();
                // saveFileDialog.Filter = "JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png";
                // saveFileDialog.ShowDialog();

                // bitmap.Save(saveFileDialog.FileName);  // 保存图片
                // panel2.BackgroundImage = bitmap;
                pictureBox_radar.BackgroundImage = bitmap;
            }
        }
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser webBrowser = (WebBrowser)sender;

           // webtoPic(webBrowser);
           
        }

        private void TCPUDPServer_Paint(object sender, PaintEventArgs e)
        {
      //      webtoPic(webBrowser1);
        }

        private void TCPUDPServer_Resize(object sender, EventArgs e)
        {
            //panel2.Visible = false;
          //  webtoPic(webBrowser1);
          //  panel2.Visible = true;
        }
        int picId = 1000000730;
        private void timer1_Tick(object sender, EventArgs e)
        {
           // initchart();

            if (m_np == null)
            {
                return;
            }
            m_np.udpSendJosonRadarIP();
          
            if (TabControl1.SelectedIndex == 6)
            {
                if (m_np == null)
                {
                    return;
                }
               
                pictureBox_radar.BackgroundImage = m_np.m_radarimg;
                pictureBox_radar.BackgroundImageLayout = ImageLayout.Zoom;
                /// webBrowser1.Focus();
            }
         
          //  string path = Application.StartupPath;
          // // int picId = 1000000730;
          //  picId++;
          //  String sPicId = picId.ToString()+".png";
          //  string fullPath = path + "\\images\\" + sPicId;
          ////  ftp.BeginUpload(fullPath, callback);
          // // ftp.Upload(fullPath);
          //  FTP.Put(fullPath);
          //  Debug.WriteLine("--------------%d"+ picId.ToString());
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
          
        }
        private FtpClient ftp = null;
        private FTPClient FTP = null;
       
        private void skinButton1_Click(object sender, EventArgs e)
        {
            AsyncCallback callback = new AsyncCallback(CloseConnection);
         
          //  ftp = new FtpClient("113.108.167.251", "radar_ftp_user", "hheeHHEE2013");

          //  ftp.Login();

         //   FTP = new FTPClient("113.108.167.251","", "radar_ftp_user", "hheeHHEE2013",21);

          


            //ftp.Close();
        }
        private void CloseConnection(IAsyncResult result)
        {
          //  Debug.WriteLine(result.IsCompleted.ToString());

            if ( ftp != null ) ftp.Close();
                ftp = null;
        }
        FormRadar m_radarForm = new FormRadar();
        private void skinButton2_Click(object sender, EventArgs e)
        {
          //  FormRadar radarForm = new FormRadar();
            m_radarForm.StartPosition = FormStartPosition.CenterParent;
            m_radarForm.Show();
            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (m_np == null)
            {
                return;
            }
            m_radarForm.BackgroundImage = m_np.m_radarimg;

          
        }

        private void TCPUDPServer_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();

            UdpServer.Close();
            controlPage.Dispose();

            controlPage.m_SerialPort.Close();
           
            
        }
        void InitMySql()
        {
            // 尝试打开数据库连接
            MySql.Data.MySqlClient.MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection(TCPUDPServer.DatabaseConnectionString);
            connection.Open();
            if (connection.State == ConnectionState.Open)
            {
                m_connection = connection;
            }
        }
        void FastTest()
        {
            List<string> m_sql = new List<string>();
            DateTime now = DateTime.Now;
            byte[] data = new byte[1024000];
            byte[] dataNull = new byte[0];
            string strData = "";
            Stopwatch sw = new Stopwatch();
            for (int i = 0; i < data.Length; i++)
            {

                data[i] = 2;
                //strData = strData + data[i].ToString();

            }
            DateTime Sample = now;
            int FrameIndex = 0;
            int ParamIndex = 0;
            byte AtnDataType = 1;


            int id = 0;
            for (int i = 0; i <1; i++)
            {

                String historyCommandString = "insert into hfradar.radarsample";
                ////  String historyCommandString = "insert into vesseldatabase.carrecordstable";
                historyCommandString += " (Sample,FrameIndex,ParamIndex,AtnDataType,Atn1Data,Atn2Data,Atn3Data,Atn4Data,Atn5Data,Atn6Data,Atn7Data,Atn8Data)";
                historyCommandString += "values (@Sample,@FrameIndex,@ParamIndex,@AtnDataType,@Atn1Data,@Atn2Data,@Atn3Data,@Atn4Data,@Atn5Data,@Atn6Data,@Atn7Data,@Atn8Data)";
                //historyCommandString += i + ",'";
                //historyCommandString += now.ToString() + "','";
                //historyCommandString += strData + "');";
                m_sql.Add(historyCommandString);
            }



            try
            {
                // 获取对象
                
                TCPUDPServer controlObject = (TCPUDPServer)this;
                // 尝试打开数据库连接
                MySql.Data.MySqlClient.MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection(TCPUDPServer.DatabaseConnectionString);
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    try
                    {
                        //// 复制数据缓冲区的数据
                        //List<string> dynamicSqlCommandCollection = null;

                        //CopyBufferCommands(ref dynamicSqlCommandCollection);


                        // 判断是否有数据需要操作
                        if ((m_sql != null))
                        {
                            // 保存所有的数据项
                            sw.Start();
                            int j = 0;
                            SaveData(connection, m_sql, controlObject, now, FrameIndex, ParamIndex, AtnDataType, data, dataNull, dataNull, dataNull, dataNull, dataNull, dataNull, dataNull);
                            sw.Stop();
                            //获取运行时间间隔  
                            //  TimeSpan ts = sw.Elapsed;
                            //获取运行时间[毫秒]  
                            long times = sw.ElapsedMilliseconds;
                            //获取运行的总时间  
                            long times2 = sw.ElapsedTicks;
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
                    }
                    finally
                    {
                        // 关闭数据库连接
                        connection.Close();
                    }
                }
                // 释放资源
                connection.Dispose();
            }
            catch (System.Exception ex)
            {

            }


        }
        private void SaveData(MySql.Data.MySqlClient.MySqlConnection connection, List<string> dynamicSqlCommandCollection,TCPUDPServer controlObject,  DateTime Sample, int FrameIndex, int ParamIndex, byte AtnDataType, byte[] Atn1Data, byte[] Atn2Data, byte[] Atn3Data, byte[] Atn4Data, byte[] Atn5Data, byte[] Atn6Data, byte[] Atn7Data, byte[] Atn8Data)
        {
            // 开始事务处理
            MySql.Data.MySqlClient.MySqlTransaction transaction = connection.BeginTransaction();
            MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
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
        public void initchart()
        {
            // Zoom into the X axis
            chart1.ChartAreas[0].AxisX.ScaleView.Zoom(0, 284);

            // Enable range selection and zooming end user interface
          //  chart1.ChartAreas[0].CursorX.IsUserEnabled = false;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = false;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.Series[0].IsXValueIndexed = false;
            //chart1.ChartAreas[0].AxisX.IsLabelAutoFit = false;
         

            //将滚动内嵌到坐标轴中
            chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chart1.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = System.Windows.Forms.DataVisualization.Charting.ScrollBarButtonStyles.None;
            //Series1 style
            this.chart1.Series[0].ToolTip = "#VALX,#VALY";    //鼠标停留在数据点上，显示XY值
           
            this.chart1.ChartAreas[0].AxisX.Maximum = 12100;
            this.chart1.ChartAreas[0].AxisX.LabelStyle.Interval = 10;
            this.chart1.ChartAreas[0].AxisX.MajorTickMark.Interval = 10;
            this.chart1.ChartAreas[0].AxisX.Minimum = 0;

            this.chart1.Series[0].Color = Color.Green;
            this.chart1.Series[0].Points.AddXY(0, 0);

            this.chart1.Series[1].Color = Color.Blue;
            this.chart1.Series[1].Points.AddXY(0, 0);

            this.chart1.ChartAreas[0].BackColor = Color.FromArgb(10, 200, 1, 1);
            this.chart1.ChartAreas[0].AxisX.ScrollBar.BackColor = Color.Yellow;

            

        }
        int k = 0;
        int loss_f = 0;    //用于检测是否有丢包，丢包计数
        int loss_s = 0;    //慢扫丢包数
        public void updatechart()
        {
            try
            {
                int ilength = 12000;
                int dataLength = 256;
              //  labelFramCount.Text = TcpServer.AsyncSocketUserList.Userlist[0].InvokeElement.m_iFarmCount.ToString();
                int index = Convert.ToInt16(skinComboBoxAntNum.Text);

                int index2 = Convert.ToInt16(skinComboBoxAtnNum2.Text);

                if (TcpServer == null)
                {
                    return;
                }
                if (TcpServer.AsyncSocketUserList.Userlist.Count == 0)
                {
                    return;
                }
                if (TcpServer.AsyncSocketUserList.Userlist[index].InvokeElement.m_radarData.m_listFastFT1.Count == 0)
                {
                   // return;
                }
                else if (TcpServer.AsyncSocketUserList.Userlist[index].InvokeElement.m_radarData.m_listFastFT1.Count != 0)
                {
                    int num1 = TcpServer.AsyncSocketUserList.Userlist[index].InvokeElement.m_radarData.m_listFastFT1.Count;
                    //int num = System.Net.IPAddress.HostToNetworkOrder(TcpServer.AsyncSocketUserList.Userlist[index].InvokeElement.m_radarData.m_listFastFT1[fram].FrameNum);
                    int num = System.Net.IPAddress.HostToNetworkOrder(TcpServer.AsyncSocketUserList.Userlist[index].InvokeElement.m_radarData.m_listFastFT1[num1-1].FrameNum);
                    labelFramNum.Text = num.ToString();
                    labelFramCount.Text = TcpServer.AsyncSocketUserList.Userlist[index].InvokeElement.m_iFarmCount.ToString();
                    // return;

                    //int numh = System.Net.IPAddress.HostToNetworkOrder(TcpServer.AsyncSocketUserList.Userlist[index].InvokeElement.m_radarData.m_listFastFT1[fram].FrameNum);
                    //if (numh - fram != num - (num1 - 1))     //判断从数据包中取出的帧号差和队列中的位置差是否相等
                    //{
                    //    loss_f++;
                    //}
                    //lblloss_f.Text = loss_f.ToString();
                    lblloss_f.Text = TcpServer.AsyncSocketUserList.Userlist[index].InvokeElement.loss_f.ToString();
                }
                if (TcpServer.AsyncSocketUserList.Userlist[index].InvokeElement.m_radarData.m_concurrentQueueSlowData.Count != 0)
                {
                    int num = System.Net.IPAddress.HostToNetworkOrder(TcpServer.AsyncSocketUserList.Userlist[index].InvokeElement.m_radarData.m_slowData.FrameNum);
                    lblSlow.Text = num.ToString();
                    lblSlowTotal.Text = TcpServer.AsyncSocketUserList.Userlist[index].InvokeElement.m_iFarmCount_slow.ToString();
                }


                int count = TcpServer.AsyncSocketUserList.Userlist[index].InvokeElement.m_radarData.m_listFastFT1.Count;
                //if (count >= 80)
                //{

                //    TcpServer.AsyncSocketUserList.Userlist[index].InvokeElement.m_radarData.m_listFastFT1.RemoveRange(0, 40);
                //}
                //if (stopFlag == false)
                //{
                //    return;
                //}

                //Stopwatch sw = new Stopwatch();
                //    sw.Start();

                //   Random rdm = new Random();




                //int length = 284;


                this.chart1.Series[0].Points.Clear();
               // if (skinCheckBoxAtnNum.Checked)
                {
                    this.chart1.Series[1].Points.Clear();
                }
               // for (int k = 0; k < count; k++)
                {
                    //CRadarData.FastFT1 ft1;
                  
                    //while (TcpServer.AsyncSocketUserList.Userlist[0].InvokeElement.m_radarData.m_disFT1.TryDequeue(out ft1))
                    {
                       
                        //for (int i = 0; i < length; i++)
                        //{
                        //    int value = TcpServer.AsyncSocketUserList.Userlist[0].InvokeElement.m_radarData.m_listFastFT1[fram].ft1Data[i];
                        //    //int value = ft1.ft1Data[i];
                        //    //  int value = TcpServer.AsyncSocketUserList.Userlist[0].InvokeElement.m_radarData.m_fastFT1.ft1Data[i];
                        //      //this.chart1.Series[0].Points.AddXY(i,rdm.Next(10) );
                        //     //this.chart1.Series[0].Points.AddXY(i + k * 284, value);
                        //   this.chart1.Series[0].Points.AddXY(i, value);
                        //}
                       // this.chart1.Series[0].Points.DataBindY(TcpServer.AsyncSocketUserList.Userlist[0].InvokeElement.m_radarData.m_listFastFT1[fram].ft1Data);
                        if(skinComboBoxDataType.Text =="快扫FT1")
                        {
                            //Console.WriteLine("滚动条位置：--------------------" + this.chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum);

                            fram = (int)((this.chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum) / dataLength);
                            //fram = 2;。

                            for (int i = 0; i < dataLength; i++)
                            {
                                //int value = TcpServer.AsyncSocketUserList.Userlist[index].InvokeElement.m_radarData.m_listFastFT1[fram].ft1Data[i];
                                int value1 = TcpServer.AsyncSocketUserList.Userlist[index].InvokeElement.m_radarData.m_listFastFT1[fram].ft1Data[i];
                                int value2 = TcpServer.AsyncSocketUserList.Userlist[index].InvokeElement.m_radarData.m_listFastFT1[fram + 1].ft1Data[i];
                                value1 = System.Net.IPAddress.HostToNetworkOrder(value1);
                                value2 = System.Net.IPAddress.HostToNetworkOrder(value2);
                                double value = Math.Sqrt(value1 * value1 + value2 * value2);
                                //int value = ft1.ft1Data[i];
                                //  int value = TcpServer.AsyncSocketUserList.Userlist[0].InvokeElement.m_radarData.m_fastFT1.ft1Data[i];
                                //this.chart1.Series[0].Points.AddXY(i,rdm.Next(10) );
                                //this.chart1.Series[0].Points.AddXY(i + k * 284, value);
                                this.chart1.Series[0].Points.AddXY(i + fram * dataLength, value);
                                 if(skinCheckBoxAtnNum.Checked)
                                 {
                                     value = TcpServer.AsyncSocketUserList.Userlist[index2].InvokeElement.m_radarData.m_listFastFT1[fram].ft1Data[i];
                                     this.chart1.Series[1].Points.AddXY(i + fram * dataLength, value);
                                 }
                                
                                // this.chart1.Update();
                                //this.chart1.Series[0].Points.InsertXY(i + fram * 284, value);
                              
                                
                            }
                          //  this.chart1.Update();
                          //  this.chart1.Series[0].Points.DataBindXY(,TcpServer.AsyncSocketUserList.Userlist[0].InvokeElement.m_radarData.m_listFastFT1[fram].ft1Data);
                            //label3.Text = fram.ToString();
                           
                        } 
                        else if(skinComboBoxDataType.Text == "慢扫原始")
                        {
                            this.chart1.Series[0].Points.DataBindY(TcpServer.AsyncSocketUserList.Userlist[index].InvokeElement.m_radarData.m_slowData.rawData);
                            if (skinCheckBoxAtnNum.Checked)
                            {
                                this.chart1.Series[1].Points.DataBindY(TcpServer.AsyncSocketUserList.Userlist[index2].InvokeElement.m_radarData.m_slowData.rawData);
                            }
                            //this.chart1.Series[1].Points.DataBindY(TcpServer.AsyncSocketUserList.Userlist[index].InvokeElement.m_radarData.m_slowData.rawData);
                        }
                        else if(skinComboBoxDataType.Text == "频谱监测高频")
                        {
                            if (skinCheckBoxAtnNum.Checked)
                            {
                                this.chart1.Series[1].Points.DataBindY(TcpServer.AsyncSocketUserList.Userlist[index2].InvokeElement.m_radarData.m_monitorData.hfMonitorData);
                            }
                            this.chart1.Series[0].Points.DataBindY(TcpServer.AsyncSocketUserList.Userlist[index].InvokeElement.m_radarData.m_monitorData.hfMonitorData);
                        }
                        else if (skinComboBoxDataType.Text == "频谱监测低频")
                        {
                            if (skinCheckBoxAtnNum.Checked)
                            {
                                this.chart1.Series[0].Points.DataBindY(TcpServer.AsyncSocketUserList.Userlist[index2].InvokeElement.m_radarData.m_monitorData.lfMonitorData);
                            }
                            this.chart1.Series[0].Points.DataBindY(TcpServer.AsyncSocketUserList.Userlist[index].InvokeElement.m_radarData.m_monitorData.lfMonitorData);
                        }
                       // this.chart1.Series[0].Points.DataBindY(TcpServer.AsyncSocketUserList.Userlist[0].InvokeElement.m_radarData.m_slowData.rawData);
                        //k++;
                    }
                  // TcpServer.AsyncSocketUserList.Userlist[0].InvokeElement.m_radarData.m_disFT1.TryDequeue(out ft1);
                   // labelFramCount.Text = k.ToString();
                     
                }

               // sw.Stop();
               // Console.WriteLine(sw.ElapsedMilliseconds);
            }
            catch(Exception  ex)
            {
               // MessageBox.Show(ex.ToString());
                //Console.WriteLine(ex.ToString());
            }
            

            
        }
        bool stopFlag = true;
        private void buttonStop_Click(object sender, EventArgs e)
        {
            if(stopFlag)
            {
                stopFlag = false;
                buttonStop.Text = "停止刷新";
            }
            else
            {
                stopFlag = true;
                buttonStop.Text = "实时刷新";
            }
        }
        int fram = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            fram++;
            int length = 256;
            this.chart1.Series[0].Points.Clear();
            for (int i = 0; i < length; i++)
            {
                int value = TcpServer.AsyncSocketUserList.Userlist[0].InvokeElement.m_radarData.m_listFastFT1[fram].ft1Data[i];
                //int value = ft1.ft1Data[i];
                //  int value = TcpServer.AsyncSocketUserList.Userlist[0].InvokeElement.m_radarData.m_fastFT1.ft1Data[i];
                //this.chart1.Series[0].Points.AddXY(i,rdm.Next(10) );
                //this.chart1.Series[0].Points.AddXY(i + k * 284, value);
                this.chart1.Series[0].Points.AddXY(i+fram*284, value);
            }
        }

        private void chart1_AxisScrollBarClicked(object sender, System.Windows.Forms.DataVisualization.Charting.ScrollBarEventArgs e)
        {
           // Console.WriteLine("滚动条位置：--------------------"+this.chart1.ChartAreas[0].AxisX.ScaleView.Position);
           
            //int length = 284;
            //for (int i = 0; i < length; i++)
            //{
            //    int value = TcpServer.AsyncSocketUserList.Userlist[0].InvokeElement.m_radarData.m_listFastFT1[fram].ft1Data[i];
            //    //int value = ft1.ft1Data[i];
            //    //  int value = TcpServer.AsyncSocketUserList.Userlist[0].InvokeElement.m_radarData.m_fastFT1.ft1Data[i];
            //    //this.chart1.Series[0].Points.AddXY(i,rdm.Next(10) );
            //    //this.chart1.Series[0].Points.AddXY(i + k * 284, value);
            //    this.chart1.Series[0].Points.AddXY(i, value);
            //}
        }

        private void skinComboBoxAntNum_MouseClick(object sender, MouseEventArgs e)
        {
            skinComboBoxAntNum.Items.Clear();
            for (int k = 0; k < TcpServer.AsyncSocketUserList.Userlist.Count; k++)
            {
                skinComboBoxAntNum.Items.Add(k.ToString());
            }
        }

        private void skinComboBoxAtnNum2_MouseClick(object sender, MouseEventArgs e)
        {
            skinComboBoxAtnNum2.Items.Clear();
            for (int k = 0; k < TcpServer.AsyncSocketUserList.Userlist.Count; k++)
            {
                skinComboBoxAtnNum2.Items.Add(k.ToString());
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            FormControl form = FormControl.creatForm(this);
            form.Show();
        }

        private void labelFramCount_Click(object sender, EventArgs e)
        {

        }

        private void labelFramNum_Click(object sender, EventArgs e)
        {

        }

        private void tabPageEx9_Click(object sender, EventArgs e)
        {

        }
    }
}
