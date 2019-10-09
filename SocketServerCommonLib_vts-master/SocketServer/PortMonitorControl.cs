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

namespace DatabaseMonitor
{
    public partial class PortMonitorControl : UserControl
    {
        /// <summary>
        /// AISĿ�꼯��
        /// </summary>
        Dictionary<int, AISTarget> m_AISCollection = new Dictionary<int, AISTarget>();
        /// <summary>
        /// ��̬SQL����
        /// </summary>
        private static List<string> m_DynamicCommandCollection = new List<string>();
        /// <summary>
        /// ��̬SQL����
        /// </summary>
        private static List<string> m_StaticCommandCollection = new List<string>();

        /// <summary>
        /// ��̨�������ݽ��̶���
        /// </summary>
        private BackgroundWorker m_wndSaveDataWorker = new BackgroundWorker();
        /// <summary>
        /// ���ݼ��ϵ�ͬ����
        /// </summary>
        private static Mutex m_StaticLocker = new Mutex();
        /// <summary>
        /// ���ݼ��ϵ�ͬ����
        /// </summary>
        private static Mutex m_DynamicLocker = new Mutex();

        /// <summary>
        /// ����ͨѶ����
        /// </summary>
        SerialPort m_SerialPort = new SerialPort();
        /// <summary>
        /// AIS��������
        /// </summary>
        AISParser m_AISParser = new AISParser();

        public PortMonitorControl(PortSettings setting)
        {
            InitializeComponent();

            // �����¼����
            m_wndSaveDataWorker.DoWork += new DoWorkEventHandler(m_wndSaveDataWorker_DoWork);
            m_wndSaveDataWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_wndSaveDataWorker_RunWorkerCompleted);
            // ����Ϣ
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

            try
            {
	            // �򿪴���
	            m_SerialPort.Open();
            }
            catch(System.Exception ex)
            {
                MessageBox.Show("�򿪴���ʧ��, ��������:" + setting.PortName + "\n��ϸ��Ϣ:\n" + ex.Message);
            }
        }

        /// <summary>
        /// �رմ��ڲ��ͷ���Դ
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
        /// ���ݴ洢����������ɻ���ִ���
        /// </summary>
        void m_wndSaveDataWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // �����ڴ�
            GC.Collect();
        }

        /// <summary>
        /// ��ʼ���ݴ洢��������
        /// </summary>
        //void m_wndSaveDataWorker_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    // ��ȡ����
        //    PortMonitorControl controlObject = ( PortMonitorControl )e.Argument;
        //    // ���Դ����ݿ�����
        //    MySql.Data.MySqlClient.MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection(MainForm.DatabaseConnectionString);
        //    connection.Open();
        //    if( connection.State == ConnectionState.Open )
        //    {
        //        try
        //        {
        //            // �������ݻ�����������
        //            List<string> dynamicSqlCommandCollection = null;
        //            List<string> staticSqlCommandCollection = null;
        //            CopyBufferCommands(ref dynamicSqlCommandCollection, ref staticSqlCommandCollection);

        //            // �ж��Ƿ���������Ҫ����
        //            if( ( staticSqlCommandCollection != null ) && ( dynamicSqlCommandCollection != null ) )
        //            {
        //                // �������е�������
        //                SaveDynamicData(connection, dynamicSqlCommandCollection, controlObject);
        //                SaveStaticData(connection, staticSqlCommandCollection, controlObject);
        //            }
        //            else
        //            {
        //                // ������...
        //                controlObject.AddStatusString("���ݼ���Ϊ��, ��������Ч��");
        //            }
        //        }
        //        catch( Exception ex )
        //        {
        //            System.Diagnostics.Debug.WriteLine(ex.ToString());
        //        }
        //        finally
        //        {
        //            // �ر����ݿ�����
        //            connection.Close();
        //        }
        //    }
        //    // �ͷ���Դ
        //    connection.Dispose();
        //}

        /// <summary>
        /// ��̬����������
        /// </summary>
        private static void SaveStaticData(MySql.Data.MySqlClient.MySqlConnection connection, List<string> staticSqlCommandCollection, PortMonitorControl controlObject)
        {
            // ��ʼ������
            MySql.Data.MySqlClient.MySqlTransaction transaction = connection.BeginTransaction();
            MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
            try
            {
                // ��������
                foreach( string cmd in staticSqlCommandCollection )
                {
                    command.CommandText = cmd;
                    command.ExecuteNonQuery();
                }

                // �ύ�������
                transaction.Commit();
                if( controlObject != null )
                {
                    // �ύ������־
                    controlObject.AddStatusString("��̬�����������ɹ�����̬Ŀ������[" + staticSqlCommandCollection.Count.ToString() + "]");
                }
            }
            catch( Exception ex )
            {
                // ������ǰ������������
                transaction.Rollback();
                if( controlObject != null )
                {
                    // �ύ������־
                    controlObject.AddStatusString("��̬����������ʧ�ܡ�\r\n������ϢΪ��" + ex.ToString());
                }
            }
        }

        /// <summary>
        /// ��̬����������
        /// </summary>
        private static void SaveDynamicData(MySql.Data.MySqlClient.MySqlConnection connection, List<string> dynamicSqlCommandCollection, PortMonitorControl controlObject)
        {
            // ��ʼ������
            MySql.Data.MySqlClient.MySqlTransaction transaction = connection.BeginTransaction();
            MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
            try
            {
                // ��������
                foreach( string cmd in dynamicSqlCommandCollection )
                {
                    command.CommandText = cmd;
                    command.ExecuteNonQuery();
                }

                // �ύ�������
                transaction.Commit();
                if( controlObject != null )
                {
                    // �ύ������־
                    controlObject.AddStatusString("��̬�����������ɹ�����̬Ŀ������[" + dynamicSqlCommandCollection.Count.ToString() + "]");
                }
            }
            catch( Exception ex )
            {
                // ������ǰ������������
                transaction.Rollback();
                if( controlObject != null )
                {
                    // �ύ������־
                    controlObject.AddStatusString("��̬����������ʧ�ܡ�\r\n������ϢΪ��" + ex.ToString());
                }
            }
        }

        /// <summary>
        /// ɾ����ʱ��AIS���ݲ���
        /// </summary>
        private static void DeleteAISdata(MySql.Data.MySqlClient.MySqlConnection connection, PortMonitorControl controlObject)
        {
            // ��ʼ������
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

                // �ύ�������
                transaction.Commit();
                if( controlObject != null )
                {
                    // �ύ������־
                    controlObject.AddStatusString("AIS��ʱ����ɾ�������ɹ���");
                }

                command.Dispose();
            }
            catch( Exception ex )
            {
                // ������ǰ������������
                transaction.Rollback();
                if( controlObject != null )
                {
                    // �ύ������־
                    controlObject.AddStatusString("AIS��ʱ����ɾ������ʧ�ܡ�\r\n������ϢΪ��" + ex.ToString());
                }
            }

        }

        /// <summary>
        /// ͬ�����������ɵ���������
        /// </summary>
        /// <param name="dynamicSqlCommandCollection">��̬SQL����</param>
        /// <param name="staticSQLCommandCollection">��̬SQL����</param>
        private static void CopyBufferCommands(ref List<string> dynamicSqlCommandCollection, ref List<string> staticSQLCommandCollection)
        {
            if( m_StaticLocker.WaitOne(1000) )
            {
                // ���ƺ����������
                staticSQLCommandCollection = new List<string>(m_StaticCommandCollection);
                m_StaticCommandCollection.Clear();
                // �ͷ���
                m_StaticLocker.ReleaseMutex();
            }
            if(m_DynamicLocker.WaitOne(1000))
            {
                // ���ƺ����������
                dynamicSqlCommandCollection = new List<string>(m_DynamicCommandCollection);
                m_DynamicCommandCollection.Clear();
                // �ͷ���
                m_DynamicLocker.ReleaseMutex();
            }
        }

        void m_AISParser_NMEAStringReceived(string line)
        {
            AddNMEAData(line);
        }

        public bool IsOpen()
        {
            return m_SerialPort.IsOpen;
        }

        void m_AISParser_StaticInformationReceived(int iMMIS, int iIMO, string strCallSign, string name, int ShipCargoTyp, int iDimensionA, int iDimensionB, int iDimensionC, int iDimensionD, int iPositionDeviceTyp, string ETA, double dMaxDraught, string destination)
        {
            try
            {
                // ���ɸ������ݿ��SQL����
                string SqlString = "replace into realtimeaisstaticinfotable set MMSI = " + iMMIS.ToString() + ",";
                SqlString += "IMONumber = " + iIMO.ToString() + "," + "CallSign = '" + strCallSign + "',";
                SqlString += "ShipName = '" + name + "'," + "ShipCargoType = " + ShipCargoTyp.ToString() + ",";
                SqlString += "ShipDimensionA = " + iDimensionA.ToString() + "," + "ShipDimensionB = " + iDimensionB.ToString() + ",";
                SqlString += "ShipDimensionC = " + iDimensionC.ToString() + "," + "ShipDimensionD = " + iDimensionD.ToString() + ",";
                SqlString += "PositionDeviceType = " + iPositionDeviceTyp.ToString() + "," + "EstimatedTimeOfArrival = '" + ETA + "',";
                SqlString += "MaximumDraught = " + dMaxDraught.ToString() + "," + "Destination = '" + destination + "',";
                SqlString += "Timestamp = '" + DateTime.Now + "';";
                // ��ӵ�������
                if( m_StaticLocker.WaitOne(1000) )
                {
                    m_StaticCommandCollection.Add(SqlString);
                    m_StaticLocker.ReleaseMutex();
                }

                // ���´���Ŀ��
                UpdateData(iMMIS, name, strCallSign, destination);
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
                //����SQL���replace
                string SqlString = "replace into realtimeaisdynamicinfotable set MMSI = " + iMMIS.ToString() + ",";
                SqlString += "Latitude =" + dLat.ToString() + "," + "Longitude = " + dLot.ToString() + ",";
                SqlString += "SpeedOverGround = " + dSOG.ToString() + ",";
                SqlString += "CourseOverGround = " + dCOG.ToString() + ",";
                SqlString += "QuanlityIndicatior = " + iQI.ToString() + ",";
                SqlString += "TrueHeading = " + iHDG.ToString() + "," + "RateOfTurn = " + dROT.ToString() + ",";
                SqlString += "AgeOfDiffCorrection = " + iageofDiffCorr.ToString() + ",";
                SqlString += "NavigationalStatus = " + iNavi.ToString() + ",";
                SqlString += "AISTimeStamp = '" + DateTime.Now.ToString() + "';";

                // ��ӵ�������
                if( m_DynamicLocker.WaitOne(1000) )
                {
                    m_DynamicCommandCollection.Add(SqlString);
                    m_DynamicLocker.ReleaseMutex();
                }

                // ���´���Ŀ�꼯��
                UpdatePosition(iMMIS, dLat, dLot, dSOG, dCOG, iHDG);
            }
            catch( System.Exception ex )
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// ���������е�������
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
	                    m_AISParser.TryParseMemssage(message);
	                }
	            }
            }
            catch(System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        public void AddRawData(string line)
        {
            if(this.InvokeRequired)
            {
                Delegate method = new DataReceivedHandler(AddRawData);
                this.Invoke(method, new object[] { line });
                return;
            }

            if( m_wndRawDataList.Items.Count > 200 )
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
        /// ���״̬��Ϣ
        /// </summary>
        /// <param name="line">��Ϣ��</param>
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
                // ����ɵ�����
                List<string> buffer = new List<string>();
                buffer.AddRange(m_wndLog.Lines);
                buffer.RemoveRange(0, buffer.Count - 50);
                buffer.Add(currentTimeString + line);
                // �ָ��ֳ�
                m_wndLog.Lines = buffer.ToArray();
            }
            else
            {
                m_wndLog.AppendText(currentTimeString + line + "\r\n");
            }
        }

        //���������¼����־
        public void SaveInformationToFile(DateTime localtime, string line)
        {
            //������־��
            string fileName = localtime.ToString("yyyy - MM - dd");
            string path = Application.StartupPath + "/" + fileName + ".log";

            File.AppendAllText(path, line);
        }

        delegate void UpdateDataHandler(int mmsi, string name, string callSign, string destination);

        /// <summary>
        /// ���¾�̬��Ϣ
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
                // ��������������
                target.Name = name;
                target.CallSign = callSign;
                target.Destination = destination;
            }
            else
            {
                // ������������
                target = new AISTarget();
                target.MMSI = mmsi;
                target.Name = name;
                target.CallSign = callSign;
                target.Destination = destination;

                // ��ӵ�������
                m_AISCollection.Add(mmsi, target);
                target.ListItem = new System.Windows.Forms.ListViewItem(mmsi.ToString());
                target.ListItem.SubItems.AddRange(new string[] { "***", "***", "***", "***", "***", "***", "***", "***" });
                m_wndAISList.Items.Add(target.ListItem);
            }

            if( target != null )
            {
                target.UpdateTime = DateTime.Now;
                // ������ʾ
                target.UpdateContent();
            }
        }

        delegate void UpdatePositionHandler(int mmsi, double latitude, double longitude, double speed, double course, double heading);

        /// <summary>
        /// ����λ��
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
                // ��������������
                target.Latitude = latitude;
                target.Longitude = longitude;
                target.Speed = speed;
                target.Course = course;
                target.Heading = heading;
            }
            else
            {
                // ������������
                target = new AISTarget();
                target.MMSI = mmsi;
                target.Latitude = latitude;
                target.Longitude = longitude;
                target.Speed = speed;
                target.Course = course;
                target.Heading = heading;

                // ��ӵ�������
                m_AISCollection.Add(mmsi, target);
                target.ListItem = new System.Windows.Forms.ListViewItem(mmsi.ToString());
                target.ListItem.SubItems.AddRange(new string[] { "***", "***", "***", "***", "***", "***", "***", "***" });
                m_wndAISList.Items.Add(target.ListItem);
            }

            if( target != null )
            {
                target.UpdateTime = DateTime.Now;
                // ������ʾ
                target.UpdateContent();
            }
        }

        private void m_wndClearTimer_Tick(object sender, EventArgs e)
        {
            // ������Ҫ�����Ŀ��
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

            // ��չ�������
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
            // ��ʼ�첽�����߳�
            if( ( !m_wndSaveDataWorker.IsBusy ) && ( MainForm.DatabaseConnectionString != "" ) )
            {
                m_wndSaveDataWorker.RunWorkerAsync(this);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PortMonitorControl
            // 
            this.Name = "PortMonitorControl";
            this.Size = new System.Drawing.Size(644, 412);
            this.ResumeLayout(false);

        }
    }

    public class AISTarget
    {
        public static string LatitudeToString(double latitude)
        {
            // ��������ֵ����Ч��
            if( ( latitude > -90 ) && ( latitude < 90 ) )
            {
                string latitudeString = "";
                if( latitude >= 0 )
                {
                    // ��γ
                    int degree = ( int )latitude;
                    double minute = ( latitude - degree ) * 60;
                    latitudeString = String.Format("{0}��{1:00.000}'N", degree, minute);
                }
                else
                {
                    // ��γ
                    latitude *= -1;
                    int degree = ( int )latitude;
                    double minute = ( latitude - degree ) * 60;
                    latitudeString = String.Format("{0}��{1:00.000}'S", degree, minute);
                }

                return latitudeString;
            }

            return "****";
        }

        public static bool StringToLatitude(string latitudeString, ref double latitude)
        {
            latitudeString = latitudeString.Trim();
            // �жϲ����Ƿ���Ч
            if( latitudeString.Length > 3 )
            {
                int degree = 91;
                float minute = 60;
                string pared = "";

                string[] fileds = latitudeString.Split(new char[] { '��', '\'' });
                if( fileds.Length == 3 )
                {
                    degree = Convert.ToInt32(fileds[ 0 ]);
                    minute = Convert.ToSingle(fileds[ 1 ]);
                    pared = fileds[ 2 ];

                    latitude = degree + minute / 60.0;
                    if( ( pared == "S" ) || ( pared == "s" ) )
                    {
                        latitude *= -1;

                        // ��������ֵ��Χ
                        if( ( latitude > -90 ) && ( latitude < 90 ) && ( minute < 60 ) && ( degree >= 0 ) )
                        {
                            return true;
                        }
                    }
                    else if( ( pared == "N" ) || ( pared == "n" ) )
                    {
                        // ��������ֵ��Χ
                        if( ( latitude > -90 ) && ( latitude < 90 ) && ( minute < 60 ) && ( degree >= 0 ) )
                        {
                            return true;
                        }
                    }
                }
            }

            // ��Ч����!
            latitude = 91;
            return false;
        }

        public static string LongitudeToString(double longitude)
        {
            // ��������ֵ����Ч��
            if( ( longitude > -180 ) && ( longitude < 180 ) )
            {
                string longitudeString = "";
                if( longitude >= 0 )
                {
                    // ����
                    int degree = ( int )longitude;
                    double minute = ( longitude - degree ) * 60;
                    longitudeString = String.Format("{0}��{1:00.000}'E", degree, minute);
                }
                else
                {
                    // ����
                    longitude *= -1;
                    int degree = ( int )longitude;
                    double minute = ( longitude - degree ) * 60;
                    longitudeString = String.Format("{0}��{1:00.000}'W", degree, minute);
                }

                return longitudeString;
            }

            return "****";
        }

        public static bool StringToLongitude(string longitudeString, ref double longitude)
        {
            longitudeString = longitudeString.Trim();
            // �жϲ����Ƿ���Ч
            if( longitudeString.Length > 3 )
            {
                int degree = 181;
                float minute = 60;
                string pared = "";

                string[] fileds = longitudeString.Split(new char[] { '��', '\'' });
                if( fileds.Length == 3 )
                {
                    degree = Convert.ToInt32(fileds[ 0 ]);
                    minute = Convert.ToSingle(fileds[ 1 ]);
                    pared = fileds[ 2 ];

                    longitude = degree + minute / 60.0;
                    if( ( pared == "W" ) || ( pared == "w" ) )
                    {
                        longitude *= -1;

                        // ��������ֵ��Χ
                        if( ( longitude > -180 ) && ( longitude < 180 ) && ( minute < 60 ) && ( degree >= 0 ) )
                        {
                            return true;
                        }
                    }
                    else if( ( pared == "E" ) || ( pared == "e" ) )
                    {
                        // ��������ֵ��Χ
                        if( ( longitude > -180 ) && ( longitude < 180 ) && ( minute < 60 ) && ( degree >= 0 ) )
                        {
                            return true;
                        }
                    }
                }
            }

            // ��Ч����!
            longitude = 181;
            return false;
        }

        public void UpdateContent()
        {
            if( m_ListItem != null )
            {
                m_ListItem.Text = m_MMSI.ToString("000000000");
                if( Latitude != double.MaxValue )
                {
                    m_ListItem.SubItems[ 1 ].Text = LatitudeToString(Latitude);
                }
                else
                {
                    m_ListItem.SubItems[ 1 ].Text = "***";
                }
                if( Longitude != double.MaxValue )
                {
                    m_ListItem.SubItems[ 2 ].Text = LongitudeToString(Longitude);
                }
                else
                {
                    m_ListItem.SubItems[ 2 ].Text = "***";
                }
                if( m_Course != double.MaxValue )
                {
                    m_ListItem.SubItems[ 3 ].Text = m_Course.ToString("0.0");
                }
                else
                {
                    m_ListItem.SubItems[ 3 ].Text = "***";
                }
                if( m_Speed != double.MaxValue )
                {
                    m_ListItem.SubItems[ 4 ].Text = m_Speed.ToString("0.0");
                }
                else
                {
                    m_ListItem.SubItems[ 4 ].Text = "***";
                }
                if( m_Heading != double.MaxValue )
                {
                    m_ListItem.SubItems[ 5 ].Text = m_Heading.ToString("0.0");
                }
                else
                {
                    m_ListItem.SubItems[ 5 ].Text = "***";
                }
                m_ListItem.SubItems[ 6 ].Text = m_Name;
                m_ListItem.SubItems[ 7 ].Text = m_CallSign;
                m_ListItem.SubItems[ 8 ].Text = m_Destination;
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
