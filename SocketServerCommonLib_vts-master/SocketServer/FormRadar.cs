using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SocketClientCommonLib;
using SocketServerCommonLib;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace SocketServer
{

    public partial class FormRadar : Form
    {
        private NmeaParse m_np;
        public FormRadar( ref NmeaParse np)
        {
            InitializeComponent();
            m_np = np;
        }
        public FormRadar()
        {
            InitializeComponent();
        }

        private void FormRadar_Shown(object sender, EventArgs e)
        {
            if (m_np != null)
            {
                pictureBox_radar.Image = m_np.m_radarimg;
            }
        }

        private void FormRadar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("aaaaaaaaaaaaaaaaaaaaa");
        }

        private void pictureBox_radar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("4");
        }

        private void pictureBox_radar_MouseHover(object sender, EventArgs e)
        {
            MessageBox.Show("4");
        }
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        private extern static bool ReleaseCapture();
        [DllImport("user32.dll")]
        private extern static int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        void Form3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();//释放窗体的鼠标焦点
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                //模拟点击窗体的Title
            }
        }
    }
}
