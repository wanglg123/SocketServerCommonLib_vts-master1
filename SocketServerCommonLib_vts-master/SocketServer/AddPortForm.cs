using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using SocketClientCommonLib;

namespace SocketServer
{
    public partial class AddPortForm : Form
    {
        public AddPortForm()
        {
            InitializeComponent();
        }

        private void AddPortForm_Load(object sender, EventArgs e)
        {
            string[] names = SerialPort.GetPortNames();
            if (names != null)
            {
                m_wndComboxName.Items.AddRange(names);
            }

            string[] stopbits = Enum.GetNames(typeof(System.IO.Ports.StopBits));
            if (stopbits != null)
            {
                m_wndComboxStopbits.Items.AddRange(stopbits);
            }

            string[] paritys = Enum.GetNames(typeof(System.IO.Ports.Parity));
            if (paritys != null)
            {
                m_wndComboxParity.Items.AddRange(paritys);
            }
        }

        PortSettings m_PortSetting = new PortSettings();
        /// <summary>
        /// 设置或获取串口配置
        /// </summary>
        public PortSettings PortSetting
        {
            get
            {
                return m_PortSetting;
            }
            set
            {
                m_PortSetting = value;
            }
        }

        private void m_wndOK_Click(object sender, EventArgs e)
        {
            try
            {
                m_PortSetting.BaudRate = Convert.ToInt32(m_wndComboxRaud.Text.Trim());
                m_PortSetting.PortName = m_wndComboxName.Text;
                m_PortSetting.DataSize = Convert.ToInt32(m_wndComboxDatasize.Text.Trim());
                m_PortSetting.StopBits = (System.IO.Ports.StopBits)Enum.Parse(typeof(System.IO.Ports.StopBits), m_wndComboxStopbits.Text);
                m_PortSetting.Parity = (System.IO.Ports.Parity)Enum.Parse(typeof(System.IO.Ports.Parity), m_wndComboxParity.Text);

                this.DialogResult = DialogResult.OK;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static PortSettings AutoPortSetting
        {
            get
            {
                PortSettings setting = new PortSettings();
                try
                {
                    // 从配置文件中读取自动开始的数值
                    setting.BaudRate = Properties.Settings.Default.Auto_Raud;
                    setting.PortName = Properties.Settings.Default.Auto_Port;
                    setting.DataSize = Properties.Settings.Default.Auto_Datasize;
                    setting.StopBits = (System.IO.Ports.StopBits)Enum.Parse(typeof(System.IO.Ports.StopBits), Properties.Settings.Default.Auto_Stopbits);
                    setting.Parity = (System.IO.Ports.Parity)Enum.Parse(typeof(System.IO.Ports.Parity), Properties.Settings.Default.Auto_Parity);

                   

                    return setting;
                }
                catch (System.Exception)
                {
                    return null;
                }
            }
        }

        private void AddPortForm_MouseMove(object sender, MouseEventArgs e)
        {
            int i = 0;
            i++;

        }
    }
}
