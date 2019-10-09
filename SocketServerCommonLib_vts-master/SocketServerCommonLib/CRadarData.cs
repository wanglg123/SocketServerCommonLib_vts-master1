using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
namespace SocketServerCommonLib
{
     public class CRadarData
    {
        /// <summary>
        /// 快扫FT1
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct FastFT11
        {
            public long head;//帧头 0x1111FFFF 0xEEEE0000
            public Int32 type;//数据类型 1表示快扫FT1数据， 2表示快扫原始数据，3表示慢扫FT1数据， 4表示慢扫原始数据， 5 表示频谱监测数据 
            public Int32 FrameNum;//32bit 的帧号最长计 数 155 天 
            public long time; //yyyymmdd hhmmss  数据采集的时间
            public Int32 syncStatus;//同步状态 1 表示同步 0表示未同步

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 284)]
            public Int32[] ft1Data;// FT1 数据 实部，虚部，。。。。。。。 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public Int32[] reserve;// 预留

            public Int32 antanNum;//天线号
            public long end;//帧尾 0x5555AAAA 0xAAAA5555 
        }
        /// <summary>
        /// 快扫FT1
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct FastFT1
        {
            public long head;//帧头 0x1111FFFF 0xEEEE0000
            public Int32 type;//数据类型 1表示快扫FT1数据， 2表示快扫原始数据，3表示慢扫FT1数据， 4表示慢扫原始数据， 5 表示频谱监测数据 
            public Int32 FrameNum;//32bit 的帧号最长计 数 155 天 
            public Int32 boardNum;//板号
            public long time; //yyyymmdd hhmmss  数据采集的时间
            public Int32 syncStatus;//同步状态 1 表示同步 0表示未同步
            public Int32 secondFramNum;//秒数据帧号
            public Int32 realOrNo;//实数或虚数
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public Int32[] ft1Data;// FT1 数据 实部，虚部，。。。。。。。 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public Int32[] reserve;// 预留

            // public Int32 antanNum;//天线号
            public long end;//帧尾 0x5555AAAA 0xAAAA5555 
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct SlowDada
        {
            public long head;//帧头 0x1111FFFF 0xEEEE0000
            public Int32 type;//数据类型 1表示快扫FT1数据， 2表示快扫原始数据，3表示慢扫FT1数据， 4表示慢扫原始数据， 5 表示频谱监测数据 
            public Int32 FrameNum;//32bit 的帧号最长计 数 155 天 
            public Int32 boardNum;//板号
            public long time; //yyyymmdd hhmmss  数据采集的时间
            public Int32 syncStatus;//同步状态 1 表示同步 0表示未同步
            public Int32 secondFramNum;//秒数据帧号
            public Int32 realOrNo;//实数或虚数
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public Int32[] rawData;// FT1 数据 实部，虚部，。。。。。。。 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public Int32[] reserve;// 预留

            // public Int32 antanNum;//天线号
            public long end;//帧尾 0x5555AAAA 0xAAAA5555 
        }
        /// <summary>
        /// 慢扫原始
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct SlowDada1
        {
            public long head;//帧头 0x1111FFFF 0xEEEE0000
            public Int32 type;//数据类型 1表示快扫FT1数据， 2表示快扫原始数据，3表示慢扫FT1数据， 4表示慢扫原始数据， 5 表示频谱监测数据 
            public Int32 FrameNum;//32bit 的帧号最长计 数 155 天 
            public long time; //yyyymmdd hhmmss  数据采集的时间
            public Int32 syncStatus;//同步状态 1 表示同步 0表示未同步

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4000)]        
            public Int32[] rawData;// 原始 数据 实部，虚部，。。。。。。。 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public Int32[] reserve;// 预留

            public Int32 antanNum;//天线号
            public  long end;//帧尾 0x5555AAAA 0xAAAA5555 
        }
        /// <summary>
        /// 频谱监测
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct MonitorDada
        {
            public long head;//帧头 0x1111FFFF 0xEEEE0000
            public Int32 type;//数据类型 1表示快扫FT1数据， 2表示快扫原始数据，3表示慢扫FT1数据， 4表示慢扫原始数据， 5 表示频谱监测数据 
            public  Int32 FrameNum;//32bit 的帧号最长计 数 155 天 
            public long time; //yyyymmdd hhmmss  数据采集的时间
            public Int32 syncStatus;//同步状态 1 表示同步 0表示未同步

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 160)]
            public Int32[] hfMonitorData;// 高频 实部，虚部，。。。。。。。 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 160)]
            public Int32[] lfMonitorData;// 低频 数据 实部，虚部，。。。。。。。 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public Int32[] reserve;// 预留

            Int32 antanNum;//天线号
            public long end;//帧尾 0x5555A

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Int32[] zeroData;// 预留
        }
        /// <summary>
        /// 控制字
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct ControlData1
        {
            public long head;
            public Int32 type;//
            public Int32 TBPeriod;//脉冲周期 us    
            public Int32 TBStartLF;//低频脉冲起点 us
            public Int32 TBStartHF;//高频脉冲起点
            public Int32 TBEndLF;//脉冲终点
            public Int32 TBEndHF;//脉冲终点
            public Int32 TPStart;//
            public Int32 TPEnd; //TP 脉冲终点
            public Int32 TSStart;//发射机所需TS脉冲起点
            public Int32 TSEnd;//发射机所需TS脉冲终点
            public Int32 Delay;//发射时延
            public Int32 FMSStart;//采样信号起点
            public Int32 FMTStart;//帧同步信号起点
            public Int32 StartFreqLF;//低频快扫本振的起始频率
            public Int32 SweepBandWidthLF ;//低频快扫的带宽

           
            public Int32 IniPhaseLF;//低频快扫的起始相位 0.1 度
            public Int32 PhaseStepLF;//低频快扫的步进相位
            public Int32 StepFreqLF;//低频快扫的步进频率
            public Int32 OscAmpControlLF ;//低频快扫的幅度
            public Int32 StartFreqHF;//高频慢扫的幅度
            public Int32 SweepBandWidthHF ;//高频慢扫的带宽
            public Int32 IniPhaseHF;//高频起始相位
            public Int32 PhaseStepHF;//高频步进相位
            public Int32 StepFreqHF;//高频步进频率
            public Int32 OscAmpControlHF ;//高频幅度
            public Int32 AntanNumber;//天线号
              [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Int32[] reserve;//保留
            public Int32 end;//帧尾
           
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct ControlData
        {
            public long head;

            public Int32 TBPeriod;//脉冲周期 us    
            public Int32 PeriodLF;
            public Int32 PeriodHF;
            public Int32 TBStartLF;//低频脉冲起点 us
            public Int32 TBStartHF;//高频脉冲起点
            public Int32 TBEndLF;//脉冲终点
            public Int32 TBEndHF;//脉冲终点
            public Int32 TPStart;//
            public Int32 TPEnd; //TP 脉冲终点
            public Int32 TSStart;//发射机所需TS脉冲起点
            public Int32 TSEnd;//发射机所需TS脉冲终点
            public Int32 Delay;//发射时延
            public Int32 FMSStart;//采样信号起点
            public Int32 FMTStart;//帧同步信号起点
            public long StartFreqLF_LO;//低频（快扫）本振的起始频率控制字
            public Int32 SweepBandWidthLF;//低频快扫的带宽

            public Int32 StartFreqLF_DDS;//低频（快扫）发射的起始频率控制字
            public Int32 IniPhaseLF_LO;//低频快扫的起始相位 0.1 度
            public Int32 IniPhaseLF_DDS;//低频（快扫）发射的步进相位控制字


            public Int32 PhaseStepLF_DDS;//低频快扫的步进相位
            public Int32 StepFreqLF_DDS;//低频快扫的步进频率
            public Int32 StepRateFreqLF_DDS;//低频（快扫）发射的步进速率
            public Int32 OscAmpControlLF_DDS;//低频快扫的幅度
            public long StartFreqHF_LO;//高频（慢扫）本振的起始频率
            public Int32 SweepBandWidthHF;//高频慢扫的带宽

            public Int32 StartFreqHF_DDS;//高频（慢扫）发射的起始频率，单位HZ
            public Int32 IniPhaseHF_LO;//高频起始相位
            public Int32 IniPhaseHF_DDS;//高频（慢扫）发射的起始相位控制字
            public Int32 PhaseStepHF_DDS;//高频步进相位
            public Int32 StepFreqHF_DDS;//高频步进频率
            public Int32 StepRateFreqHF_DDS;//高频（慢扫）发射的步进速率，单位s
            public Int32 OscAmpControlHF_DDS;//高频幅度
            public Int32 AntanNumber;//天线号
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public Int32[] reserve;//保留
            public long end;//帧尾

        }
        public static Byte[] StructToBytes(Object structure)
        {
            Int32 size = Marshal.SizeOf(structure);
            Console.WriteLine(size);
            //   size = 4;
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structure, buffer, false);
                Byte[] bytes = new Byte[size];
                Marshal.Copy(buffer, bytes, 0, size);
                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
        //byte[]转换为struct
        public static object BytesToStruct(byte[] bytes, Type strcutType)
        {
            int size = Marshal.SizeOf(strcutType);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, buffer, size);
                return Marshal.PtrToStructure(buffer, strcutType);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
        public FastFT1 m_fastFT1;
        public SlowDada m_slowData;
        public MonitorDada m_monitorData;
        public ControlData m_controlData;
        public List<CRadarData.FastFT1> m_listFastFT1 = new List<CRadarData.FastFT1>();
         public List<int > m_ft1data = new List<int>();
   
        public ConcurrentQueue<CRadarData.FastFT1> m_concurrentQueueFastFT1 = new ConcurrentQueue<FastFT1>();
        public ConcurrentQueue<CRadarData.SlowDada> m_concurrentQueueSlowData = new ConcurrentQueue<SlowDada>();
        public ConcurrentQueue<CRadarData.MonitorDada> m_concurrentQueueMonitorData = new ConcurrentQueue<MonitorDada>();

        public ConcurrentQueue<byte[]> m_concurrentQueueFastFT1file = new ConcurrentQueue<byte[]>();     //用于存放写文件的数据
        public ConcurrentQueue<byte[]> m_concurrentQueueSlowDatafile = new ConcurrentQueue<byte[]>();
        public ConcurrentQueue<byte[]> m_concurrentQueueMonitorDatafile = new ConcurrentQueue<byte[]>();

        public ConcurrentQueue<CRadarData.FastFT1> m_disFT1 = new ConcurrentQueue<FastFT1>();
     

    }
}
