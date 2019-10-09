using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
namespace SocketServer
{
    public partial class FormControl : Form
    {
        TCPUDPServer m_gui = null;
        
        private static FormControl instance;
        public CRadarData m_radar;

        //string Filepath;       //写文件的路径
        //FileStream fs;
        //Mutex m = new Mutex();    //互斥锁

        public FormControl()
        {
            InitializeComponent();
            
        }
        public FormControl(TCPUDPServer gui)
        {
            InitializeComponent();
               m_gui = gui;
            InitWindow();

            m_radar = new CRadarData();
            timer1.Interval = 1000;
            timer1.Start();

        }
        public static FormControl creatForm(TCPUDPServer gui)
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new FormControl(gui);
            }

          
            return instance;
        }
        public void InitWindow()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            // this.Parent = m_gui;

             this.StartPosition = m_gui.StartPosition;
             this.Opacity = 1;
             this.TopMost = true;

             textBoxStartFreqLF_LO.Text = "5000000";
             textBoxSweepBandWidthLF.Text = "0";
             textBoxIniPhaseLF_LO.Text = "0";
             textBoxPhaseStepLF_DDS.Text = "30";
             
            textBoxStartFreqHF_DDS.Text = "7000000";

            textBoxIniPhaseHF_LO.Text = "0";
            textBoxPhaseStepHF_DDS.Text = "30";

        }
      
        private void buttonControl_Click(object sender, EventArgs e)
        {




            //IPAddress iP = IPAddress.Parse(textBoxUDPip.Text);
            //// 端口号
            //IPEndPoint endPoint = new IPEndPoint(iP, int.Parse(textBoxUDPport.Text));

            //if (int.Parse(textBoxUDPport.Text) == 0)
            //{
            //    return;
            //}


            //////建立与服务器的远程连接
            //m_gui.UdpServer.m_sListen.Connect(endPoint);

            try
            {
                
                this.m_radar.m_controlData.head = System.Net.IPAddress.HostToNetworkOrder(0x1111FFFFEEEE0000);
                this.m_radar.m_controlData.TBPeriod =  System.Net.IPAddress.HostToNetworkOrder((int)((int)(Convert.ToDouble(textBoxTBPeriod.Text) * 96) - 1));
                // this.m_radar.m_controlData.TBPeriod = System.Net.IPAddress.HostToNetworkOrder((int)(Convert.ToDouble(textBoxTBPeriod.Text) * 96) - 1);
                // this.m_radar.m_controlData.head = (0x1111FFFFEEEE0000);
                this.m_radar.m_controlData.PeriodLF = System.Net.IPAddress.HostToNetworkOrder((int)(Convert.ToDouble(textBoxPeriodLF.Text) * 96) - 1);
                this.m_radar.m_controlData.PeriodHF = System.Net.IPAddress.HostToNetworkOrder((int)(Convert.ToDouble(textBoxPeriodHF.Text) * 96 - 1));
                this.m_radar.m_controlData.TBStartLF = System.Net.IPAddress.HostToNetworkOrder((int)(Convert.ToDouble(textBoxTBStartLF.Text) * 96 - 1));
                this.m_radar.m_controlData.TBStartHF = System.Net.IPAddress.HostToNetworkOrder((int)(Convert.ToDouble(textBoxTBStartHF.Text) * 96 - 1));
                this.m_radar.m_controlData.TBEndLF = System.Net.IPAddress.HostToNetworkOrder((int)(Convert.ToDouble(textBoxTBEndLF.Text) * 96 - 1));
                this.m_radar.m_controlData.TBEndHF = System.Net.IPAddress.HostToNetworkOrder((int)(Convert.ToDouble(textBoxTBEndHF.Text) * 96 - 1));
                this.m_radar.m_controlData.TPStart = System.Net.IPAddress.HostToNetworkOrder((int)(Convert.ToDouble(textBoxTPStart.Text) * 96 - 1));
                this.m_radar.m_controlData.TPEnd = System.Net.IPAddress.HostToNetworkOrder((int)(Convert.ToDouble(textBoxTPEnd.Text) * 96 - 1));
                this.m_radar.m_controlData.TSStart = System.Net.IPAddress.HostToNetworkOrder((int)(Convert.ToDouble(textBoxTSStart.Text) * 96 - 1));
                this.m_radar.m_controlData.TSEnd = System.Net.IPAddress.HostToNetworkOrder((int)(Convert.ToDouble(textBoxTSEnd.Text) * 96 - 1));
                this.m_radar.m_controlData.Delay = System.Net.IPAddress.HostToNetworkOrder((int)(Convert.ToDouble(textBoxDelay.Text) * 96 - 1));
                this.m_radar.m_controlData.FMSStart = System.Net.IPAddress.HostToNetworkOrder((int)(Convert.ToDouble(textBoxFMSStart.Text) * 96 - 1));
                this.m_radar.m_controlData.FMTStart = System.Net.IPAddress.HostToNetworkOrder((int)(Convert.ToDouble(textBoxFMTStart.Text) * 96 - 1));
                this.m_radar.m_controlData.StartFreqLF_LO = System.Net.IPAddress.HostToNetworkOrder((long)(0.5 + Convert.ToDouble(textBoxStartFreqLF_LO.Text) * (Math.Pow(2, 48)) / (192000000)));
                this.m_radar.m_controlData.SweepBandWidthLF = System.Net.IPAddress.HostToNetworkOrder((int)Convert.ToDouble(textBoxSweepBandWidthLF.Text));
                
                double lf = Convert.ToDouble(textBox_StartFreqLF_DDS.Text) * (Math.Pow(2, 32)) / (192000000);
                int tmp = (int)(0.5 + lf);
                this.m_radar.m_controlData.StartFreqLF_DDS = System.Net.IPAddress.HostToNetworkOrder(tmp);

                this.m_radar.m_controlData.IniPhaseLF_LO = System.Net.IPAddress.HostToNetworkOrder((int)(0.5 + Convert.ToDouble(textBoxIniPhaseLF_LO.Text) * (Math.Pow(2, 32) / 360)));
                this.m_radar.m_controlData.IniPhaseLF_DDS = System.Net.IPAddress.HostToNetworkOrder((int)(0.5 + Convert.ToDouble(textBox_IniPhaseLF_DDS.Text) * (Math.Pow(2, 16) / 360)));

                this.m_radar.m_controlData.PhaseStepLF_DDS = System.Net.IPAddress.HostToNetworkOrder((int)(0.5 + Convert.ToDouble(textBoxPhaseStepLF_DDS.Text) * (Math.Pow(2, 16) / 360)));
                this.m_radar.m_controlData.StepFreqLF_DDS = System.Net.IPAddress.HostToNetworkOrder((int)(0.5 + Convert.ToDouble(textBoxStepFreqLF_DDS.Text) * (Math.Pow(2, 32)) / (192000000)));


                this.m_radar.m_controlData.OscAmpControlLF_DDS = System.Net.IPAddress.HostToNetworkOrder((int)(Convert.ToDouble(textBoxOscAmpControlLF_DDS.Text) * (Math.Pow(2, 12))));


                this.m_radar.m_controlData.StartFreqHF_LO = System.Net.IPAddress.HostToNetworkOrder((long)(0.5 + Convert.ToDouble(textBox_StartFreqHF_LO.Text) * (Math.Pow(2, 48)) / (192000000)));

                this.m_radar.m_controlData.StartFreqHF_DDS = System.Net.IPAddress.HostToNetworkOrder((int)(0.5 + Convert.ToDouble(textBoxStartFreqHF_DDS.Text) * (Math.Pow(2, 32)) / (192000000)));
                this.m_radar.m_controlData.SweepBandWidthHF = System.Net.IPAddress.HostToNetworkOrder((int)(0.5 + Convert.ToDouble(textBoxSweepBandWidthHF.Text) * (Math.Pow(2, 32)) / (192000000)));



                this.m_radar.m_controlData.IniPhaseHF_LO = System.Net.IPAddress.HostToNetworkOrder((int)(0.5 + Convert.ToDouble(textBoxIniPhaseHF_LO.Text) * (Math.Pow(2, 32) / 360)));
                this.m_radar.m_controlData.IniPhaseHF_DDS = System.Net.IPAddress.HostToNetworkOrder((int)(0.5 + Convert.ToDouble(textBox_IniPhaseHF_DDS.Text) * (Math.Pow(2, 16) / 360)));
                this.m_radar.m_controlData.PhaseStepHF_DDS = System.Net.IPAddress.HostToNetworkOrder((int)(0.5 + Convert.ToDouble(textBoxPhaseStepHF_DDS.Text) * (Math.Pow(2, 16) / 360)));
                this.m_radar.m_controlData.StepFreqHF_DDS = System.Net.IPAddress.HostToNetworkOrder((int)(0.5 + Convert.ToDouble(textBoxStepFreqHF_DDS.Text) * (Math.Pow(2, 32)) / (192000000)));

               
                this.m_radar.m_controlData.OscAmpControlHF_DDS = System.Net.IPAddress.HostToNetworkOrder((int)(Convert.ToDouble(textBoxOscAmpControlHF.Text) * (Math.Pow(2, 12))));
                
                double delta_t = 0;
               // delta_t = (Convert.ToDouble(textBoxSweepBandWidthHF.Text) / Convert.ToDouble(textBoxStepFreqHF_DDS.Text));
                double tmpdata = Convert.ToDouble(textBoxSweepBandWidthHF.Text) / Convert.ToDouble(textBoxPeriodHF.Text);
                delta_t =  Convert.ToDouble(textBoxStepFreqHF_DDS.Text) / tmpdata;
                textBox_StepRateFreqHF_DDS.Text = delta_t.ToString();

                double Value = delta_t * 192/ 4;
                this.m_radar.m_controlData.StepRateFreqHF_DDS = System.Net.IPAddress.HostToNetworkOrder((int)(Value+0.5));


                if (textBoxSweepBandWidthLF.Text == "1")
                {

                    //delta_t = (int)(60000 / Convert.ToDouble(textBoxStepFreqLF_DDS.Text));
                    //textBox_StepRateFreqLF_DDS.Text = delta_t.ToString();

                    //Value = delta_t * 192000000 / 4;
                    //this.m_radar.m_controlData.StepRateFreqLF_DDS = System.Net.IPAddress.HostToNetworkOrder(Value);
                    tmpdata =60000/Convert.ToDouble(textBoxPeriodLF.Text);
                    delta_t = Convert.ToDouble(textBoxStepFreqLF_DDS.Text) / tmpdata;
                    textBox_StepRateFreqLF_DDS.Text = delta_t.ToString();

                    Value = delta_t * 192/ 4;
                    this.m_radar.m_controlData.StepRateFreqHF_DDS = System.Net.IPAddress.HostToNetworkOrder((int)(Value + 0.5));
                }
                else
                {
                    tmpdata = 30000 / Convert.ToDouble(textBoxPeriodLF.Text);
                    delta_t = Convert.ToDouble(textBoxStepFreqLF_DDS.Text) / tmpdata;
                    textBox_StepRateFreqLF_DDS.Text = delta_t.ToString();

                    Value = delta_t * 192/ 4;
                   int tmpValue = (int)(Value + 0.5);
                   this.m_radar.m_controlData.StepRateFreqLF_DDS = System.Net.IPAddress.HostToNetworkOrder(tmpValue);
                }


                this.m_radar.m_controlData.end = System.Net.IPAddress.HostToNetworkOrder(0x5555AAAAAAAA5555);
                //this.m_radar.m_controlData.head = (0x1111FFFFEEEE0000);
                //this.m_radar.m_controlData.TBPeriod = ((int)(Convert.ToDouble(textBoxTBPeriod.Text) * 96) - 1);
                //// this.m_radar.m_controlData.TBPeriod = System.Net.IPAddress.HostToNetworkOrder((int)(Convert.ToDouble(textBoxTBPeriod.Text) * 96) - 1);
                //// this.m_radar.m_controlData.head = (0x1111FFFFEEEE0000);
                //this.m_radar.m_controlData.PeriodLF = ((int)(Convert.ToDouble(textBoxPeriodLF.Text) * 96) - 1);
                //this.m_radar.m_controlData.PeriodHF = ((int)(Convert.ToDouble(textBoxPeriodHF.Text) * 96 - 1));
                //this.m_radar.m_controlData.TBStartLF = ((int)(Convert.ToDouble(textBoxTBStartLF.Text) * 96 - 1));
                //this.m_radar.m_controlData.TBStartHF = ((int)(Convert.ToDouble(textBoxTBStartHF.Text) * 96 - 1));
                //this.m_radar.m_controlData.TBEndLF = ((int)(Convert.ToDouble(textBoxTBEndLF.Text) * 96 - 1));
                //this.m_radar.m_controlData.TBEndHF = ((int)(Convert.ToDouble(textBoxTBEndHF.Text) * 96 - 1));
                //this.m_radar.m_controlData.TPStart = ((int)(Convert.ToDouble(textBoxTPStart.Text) * 96 - 1));
                //this.m_radar.m_controlData.TPEnd = ((int)(Convert.ToDouble(textBoxTPEnd.Text) * 96 - 1));
                //this.m_radar.m_controlData.TSStart = ((int)(Convert.ToDouble(textBoxTSStart.Text) * 96 - 1));
                //this.m_radar.m_controlData.TSEnd = ((int)(Convert.ToDouble(textBoxTSEnd.Text) * 96 - 1));
                //this.m_radar.m_controlData.Delay = ((int)(Convert.ToDouble(textBoxDelay.Text) * 96 - 1));
                //this.m_radar.m_controlData.FMSStart = ((int)(Convert.ToDouble(textBoxFMSStart.Text) * 96 - 1));
                //this.m_radar.m_controlData.FMTStart =((int)(Convert.ToDouble(textBoxFMTStart.Text) * 96 - 1));
                //this.m_radar.m_controlData.StartFreqLF_LO = ((int)(Convert.ToDouble(textBoxStartFreqLF_LO.Text) * (Math.Pow(2, 48)) / (192000000)));
                //this.m_radar.m_controlData.SweepBandWidthLF = ((int)Convert.ToDouble(textBoxSweepBandWidthLF.Text));
                //this.m_radar.m_controlData.StartFreqLF_DDS = ((int)(Convert.ToDouble(textBox_StartFreqLF_DDS.Text) * (Math.Pow(2, 32)) / (192000000)));

                //this.m_radar.m_controlData.IniPhaseLF_LO = ((int)(Convert.ToDouble(textBoxIniPhaseLF_LO.Text) * (Math.Pow(2, 32) / 360)));
                //this.m_radar.m_controlData.IniPhaseLF_DDS =((int)(Convert.ToDouble(textBox_IniPhaseLF_DDS.Text) * (Math.Pow(2, 16) / 360)));

                //this.m_radar.m_controlData.PhaseStepLF_DDS = ((int)(Convert.ToDouble(textBoxPhaseStepLF_DDS.Text) * (Math.Pow(2, 16) / 360)));
                //this.m_radar.m_controlData.StepFreqLF_DDS = ((int)(Convert.ToDouble(textBoxStepFreqLF_DDS.Text) * (Math.Pow(2, 32)) / (192000000)));


                //this.m_radar.m_controlData.OscAmpControlLF_DDS = ((int)(Convert.ToDouble(textBoxOscAmpControlLF_DDS.Text) * (Math.Pow(2, 13))));


                //this.m_radar.m_controlData.StartFreqHF_LO = ((int)(Convert.ToDouble(textBox_StartFreqHF_LO.Text) * (Math.Pow(2, 48)) / (192000000)));

                //this.m_radar.m_controlData.StartFreqHF_DDS = ((int)(Convert.ToDouble(textBoxStartFreqHF_DDS.Text) * (Math.Pow(2, 48)) / (192000000)));
                //this.m_radar.m_controlData.SweepBandWidthHF = (int)Convert.ToDouble(textBoxSweepBandWidthHF.Text);
                //this.m_radar.m_controlData.IniPhaseHF_LO = ((int)(Convert.ToDouble(textBoxIniPhaseHF_LO.Text) * (Math.Pow(2, 32) / 360)));
                //this.m_radar.m_controlData.IniPhaseHF_DDS = ((int)(Convert.ToDouble(textBox_IniPhaseHF_DDS.Text) * (Math.Pow(2, 16) / 360)));
                //this.m_radar.m_controlData.PhaseStepHF_DDS = ((int)(Convert.ToDouble(textBoxPhaseStepHF_DDS.Text) * (Math.Pow(2, 16) / 360)));
                //this.m_radar.m_controlData.StepFreqHF_DDS = ((int)(Convert.ToDouble(textBoxStepFreqHF_DDS.Text) * (Math.Pow(2, 32)) / (192000000)));


                //this.m_radar.m_controlData.OscAmpControlHF_DDS = ((int)(Convert.ToDouble(textBoxOscAmpControlHF.Text) * (Math.Pow(2, 13))));
                //int delta_t = 0;
                //delta_t = (int)(Convert.ToDouble(textBoxSweepBandWidthHF.Text) / Convert.ToDouble(textBoxStepFreqHF_DDS.Text));
                //textBox_StepRateFreqHF_DDS.Text = delta_t.ToString();

                //int Value = delta_t * 192000000 / 4;
                //this.m_radar.m_controlData.StepRateFreqHF_DDS = (Value);


                //if (textBoxSweepBandWidthLF.Text == "1")
                //{
                //    delta_t = (int)(60000 / Convert.ToDouble(textBoxStepFreqLF_DDS.Text));
                //    textBox_StepRateFreqLF_DDS.Text = delta_t.ToString();

                //    Value = delta_t * 192000000 / 4;
                //    this.m_radar.m_controlData.StepRateFreqLF_DDS = (Value);
                //}
                //else
                //{
                //    delta_t = (int)(30000 / Convert.ToDouble(textBoxStepFreqLF_DDS.Text));
                //    textBox_StepRateFreqLF_DDS.Text = delta_t.ToString();

                //    Value = delta_t * 192000000 / 4;
                //    this.m_radar.m_controlData.StepRateFreqLF_DDS = (Value);
                //}


                //this.m_radar.m_controlData.end = (0x5555AAAAAAAA5555);
                byte[] message = CRadarData.StructToBytes(this.m_radar.m_controlData);
                //    byte[] message = { 0x11, 0x11, 0xff, 0xff, 0xee, 0xee, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00 
                //                     , 0x11, 0x11, 0xff, 0xff, 0xee, 0xee, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00
                // ,0x11, 0x11, 0xff, 0xff, 0xee, 0xee, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00
                //, 0x11, 0x11, 0xff, 0xff, 0xee, 0xee, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00};
                // m_gui.UdpServer.m_sListen.SendTo(message, m_gui.UdpServer.RemoteEndPoint);
                // m_gui.UdpServer.m_sListen.SendTo(message, 0, m_gui.UdpServer.RemoteEndPoint);
                //m_gui.UdpServer.m_sListen.Send(message);
                m_gui.TcpServer.SendAsyncEvent(m_gui.TcpServer.AsyncSocketUserList.Userlist[0].m_connectSocket, m_gui.TcpServer.AsyncSocketUserList.Userlist[0].SendEventArgs, message, 0, message.Length);

                //m.WaitOne();      //利用互斥锁，避免冲突
                //Filepath = "E:\\ControlData\\controldata";
                //fs = File.Open(Filepath, FileMode.OpenOrCreate);
                //fs.Write(message, 0, message.Length);
                //fs.Close();
                //m.ReleaseMutex();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
            //client.Connect(endPoint);

           
       
            
        }
        public List<EndPoint> listEndPoint = new List<EndPoint>();
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                
                EndPoint ipPort =  (m_gui.UdpServer.RemoteEndPoint);
                if (listEndPoint.Contains(ipPort))
                {
                    return;
                }
                else
                {
                    textBoxUDPip.Text = ((IPEndPoint)m_gui.UdpServer.RemoteEndPoint).Address.ToString();
                    textBoxUDPport.Text = ((IPEndPoint)m_gui.UdpServer.RemoteEndPoint).Port.ToString();
                    listEndPoint.Add(ipPort);
                }
               
            }
            catch(Exception ex)
            {
            }
            
        }

        private void FormControl_Shown(object sender, EventArgs e)
        {
            try
            {
                textBoxUDPip.Text = ((IPEndPoint)m_gui.UdpServer.RemoteEndPoint).Address.ToString();
                textBoxUDPport.Text = ((IPEndPoint)m_gui.UdpServer.RemoteEndPoint).Port.ToString();
            }
            catch(Exception ex )
            {

            }
            
        }
    }
}
