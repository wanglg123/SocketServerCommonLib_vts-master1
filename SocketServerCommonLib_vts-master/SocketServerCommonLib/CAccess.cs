using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace SocketServerCommonLib
{
    public class CAccess
    {
        private static Object thisLock = new Object();

        public enum DBOpError
        {
            Error_Success = 1,

            Error_CreateDBException,

            Error_ConnException,

            Error_NoConnected,

            Error_ExecuteException,

            Error_CloseException,
        };
        public OleDbConnection m_COledbCon = null;
        public OleDbCommand m_COledbCom = null;
        private string m_ConectString = null;
        private bool m_Connected = false;
        public CAccess()
        {
            m_COledbCon= new OleDbConnection();
            m_COledbCom = new OleDbCommand();
           
        }
        public CAccess(string Constring)
        {
            m_COledbCon = new OleDbConnection();

            m_ConectString = Constring;

            m_COledbCom = new OleDbCommand();


        }
        public string ConectSting
        {
            get { return m_ConectString; }
            set {m_ConectString = value;}
        }
        public OleDbConnection con 
        {
           get {return m_COledbCon;}
           set {m_COledbCon = value;}
        }
        public OleDbCommand com
        {
            get { return m_COledbCom; }
            set { m_COledbCom = value ; }
        }
        public CAccess.DBOpError AccessConect()
        {
            if (m_Connected == true)
            {
                return DBOpError.Error_Success;
            }
            try
            {
                m_COledbCon.ConnectionString = m_ConectString;

              //  MessageBox.Show(m_ConectString);
                m_COledbCon.Open();
                m_Connected = true;
            //    Form1.m_form.BaoJinDataInsert(" AccessConect ok", null);
                return DBOpError.Error_Success;

            }
            catch (System.Exception ex)
            {
               // MessageBox.Show(ex.ToString());
                
                return DBOpError.Error_ConnException;
            }
        }
        public CAccess.DBOpError AccessDisconnect()
        {
            if (m_Connected == true)
            {
                return DBOpError.Error_Success;
            }
            try
            {

                m_COledbCon.Close();

                m_Connected = false;

                return DBOpError.Error_Success;
            }
            catch
            {
                m_Connected = false;;

                return CAccess.DBOpError.Error_CloseException;
            }
        }
     
        public DBOpError ExecuteNoQuery(string com)
        {
            if (m_Connected == false)
            {
                return DBOpError.Error_NoConnected;
            }
            try
           {
               m_COledbCom.CommandText = com;
               m_COledbCom.Connection = m_COledbCon;
             //  m_COledbCon.Open();
              lock (thisLock)
              {
               
                // System.Diagnostics.Debug.WriteLine(System.Threading.Thread.CurrentThread.Name);  
                 m_COledbCom.ExecuteNonQuery();
              }
               return DBOpError.Error_Success;

            }
           catch (System.Exception ex)
           {
           	   // MessageBox.Show(ex.ToString());
             //  Form1.m_form.BaoJinDataInsert(ex.ToString(), null);
               return DBOpError.Error_ExecuteException;
           }
           finally
           {
              //  m_COledbCon.Close();
           }

        }
        public DBOpError ExecuteGetSet(string com,ref DataSet set)
        {
            if (m_Connected == false)
            {
                return DBOpError.Error_NoConnected;
            }
            try
            {
                m_COledbCom.Connection = m_COledbCon;
                m_COledbCom.CommandText = com;
                lock (thisLock)
                {
                    OleDbDataAdapter adapter = new OleDbDataAdapter(m_COledbCom);
                    //  OleDbDataAdapter adapet = new OleDbDataAdapter(m_COledbCon);
                    adapter.Fill(set);
                }
                return DBOpError.Error_Success;

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return DBOpError.Error_ExecuteException;
            }
            finally
            {
               // m_COledbCon.Close();
            }
        }
        public DBOpError ExecuteGetTable(string com,ref DataTable table)
        {
            try
            {
                
                m_COledbCom.CommandText = com;
                m_COledbCom.Connection = m_COledbCon;
                OleDbDataAdapter dad = new OleDbDataAdapter(m_COledbCom);
                dad.Fill(table);
                return DBOpError.Error_Success;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return DBOpError.Error_ExecuteException;
            }
            finally
            {
             //   m_COledbCon.Close();
            }
        }
        public void CloseCon()
        {

            m_COledbCon.Close();
            m_COledbCon.Dispose();
        }
      
    }

    public static class RadarIP
    {
        public static string m_BiaoMingCheng = "RadarIP";
        public static string m_RadarID = "radarID";
        public static string m_RadarIP = "radarIP";
        public static string m_RadarType = "radarType";
        public static string m_EchoOrPic = "EchoOrPic";
        public static string m_Protocol = "Protocol";
       
    }

   
}
