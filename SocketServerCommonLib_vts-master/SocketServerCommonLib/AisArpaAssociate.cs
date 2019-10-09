using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace SocketServerCommonLib
{

    class AisArpaAssociate
    {
        public const int UNION_STATE_SENSOR	= 0x01;// 
        public const int UNION_STATE_TTM = 0x02;	// 
        public const int UNION_STATE_NULL = 0x00;	// 

        public const int  UNION_ASSCI	=	0x01;// 处于强联合
        public const int UNION_DISASSCI = 0x02;	// 处于分离
        public const int UNION_BREAK = 0x04;	// 判断为分离

        public const int MAX_UNION_SIZE = 300;			// 最大联合数

        public const int MARK_AIS = 0x01;		// 标记AIS已联合
        public const int MARK_ARPA = 0x02;			// 标记ARPA已联合
        public const float NM = 1852.0f;		// 1海里等于1852米

        public const int	STAT_COUNT	=		24;
        public const float ASSO_RATE = 0.55f;
        public const float DISO_RATE = 0.45f;
        public const int ASSO_MAXFACTOR = 1;
        public const int DISO_MAXFACTOR = 1;
        public class tagCOMBINE_TGT
        {
            public double dRange;
            public double dAzimuth;
            public double dTSpeed;
            public double dTCourse;
            public double dRSpeed;
            public double dRCourse;
            public double dCPA;
            public double dTCPA;
        }

        public class tagTARGET_UNION
         {
             public long  dwArpaID;			//<! ArpaID
             public long  dwAisID;			//<! AisID
             public long  dwUnionStat;		//<! 联合状态

             public double dUnionRng;			//<! 联合距离
             public double dUnionAzi;			//<! 联合方位
             public double dUnionTSpd;			//<! 联合真速度
             public double dUnionTCrs;			//<! 联合真航向
             public double dUnionRSpd;			//<! 联合相对速度
             public double dUnionRCrs;			//<! 联合相对航向
             public double dUnionCPA;			//<! 联合CPA
             public double dUnionTCPA;			//<! 联合TCPA
             public double dUnionLat;			//<! 联合纬度
             public double dUnionLon;			//<! 联合经度

             

	         private int nState;				//<! 稳定的联合状态，有UNION_ASSCI, UNION_DISASSCI和

             private int nSumCount;			//<! 总计数器
             private long dwAssociToken;		//<! 联合判别字

             public int GetState()
             {
                 return nState;
             }
             public void  ResetState(int nNewState)
            {
	            nState	= nNewState;
            }
             //! 将AIS和ARPA的参数计算后，上报瞬时联合状态，得出稳定的联合状态
             /*! 
                 \param		nCurState：当次的状态
                 \return		返回接收瞬时状态后稳定的联合状态
                 \remark 	将AIS和ARPA的参数计算后，上报瞬时联合状态，得出稳定的联合状态
             */
             public int ReportOnce(int nCurState)
             {
                 int nNewState = nState;
                 switch (nState)
                 {
                     case UNION_DISASSCI:
                         if (nCurState == UNION_ASSCI || nCurState == UNION_DISASSCI)
                         {
                             dwAssociToken = SetCountBit(dwAssociToken, nSumCount % STAT_COUNT, (nCurState == UNION_ASSCI));
                             if (nSumCount > STAT_COUNT * 0.5)
                             {
                                 nNewState = (GetBitCount(dwAssociToken) * 1.0f / STAT_COUNT >= ASSO_RATE) ? UNION_ASSCI : UNION_BREAK;
                                 ResetState(nNewState);
                             }
                         }
                         break;

                     case UNION_ASSCI:
                         if (nCurState == UNION_ASSCI || nCurState == UNION_DISASSCI)
                         {
                             dwAssociToken = SetCountBit(dwAssociToken, nSumCount % STAT_COUNT, (nCurState != UNION_DISASSCI));
                             if (nSumCount > STAT_COUNT * 0.5)
                             {
                                 nNewState = (GetBitCount(dwAssociToken) * 1.0f / STAT_COUNT >= DISO_RATE) ? UNION_ASSCI : UNION_DISASSCI;
                                 ResetState(nNewState);
                             }
                         }
                         break;
                 }

                 nSumCount++;
                 return nNewState;
                 
             }
             // 设置或解除统计位
             public long SetCountBit(long dwToken, int nCount, bool bSet)
             {
                 if (bSet)
                 {
                     dwToken |= (0x01 << (nCount - 1));
                 }
                 else
                 {
                     dwToken &= ~(0x01 << (nCount - 1));
                 }
                 return dwToken;
             }
             // 获取有效位的数目
             public int GetBitCount(long dwToken)
             {
                 int n = 0;
                 if (dwToken != 0)
                 {
                     for (int i = 0; i < 32; i++)
                     {
                         if ((dwToken & (0x01 << i)) != 0)
                         {
                             n++;
                         }
                     }
                 }
                 return n;
             }
         }
     
        public List<tagTARGET_UNION> m_vectUnion;
        class tagTARGET_DIFF
        {
	        public float fRngDiff;
            public float fSpdDiff;
	        public float fCrsDiff;
        }
       
       
       
        //! 设置需要统计的AIS和ARPA列表
	    /*! 
		    \param		lpAisTgtList：指向AIS列表指针
		    \param		lpArpaTgtList：指向ARPA列表指针
		    \return		None
		    \remark 	设置需要统计的AIS和ARPA列表
	    */
	    void SetAisArpaTgtList(AISTarget lpAisTgtList, target lpArpaTgtList)
        {

        }
        	//! 初始化，建立AIS和ARPA的联合统计列表
	    /*! 
		    \return		初始化后列表长度
		    \remark 	初始化，建立AIS和ARPA的联合统计列表，一开始或每次使能后，都需要重建统计列表
	    */
	    int InitAssociateList()
        {
            return 0;
        }
        
	    //! 删除统计列表所有数据
	    /*! 
		    \return		None
		    \remark 	删除统计列表所有数据
	    */
	    void DeleteAll()
        {

        }
        
	    //! 上报新的ARPA目标数据
	    /*! 
		    \param		dwArpaID：ARPA目标ID
		    \return		ARPA数据是否进入统计列表
		    \remark 	上报新的ARPA目标数据
	    */ 
	    bool ReportSensorArpa(long dwArpaID)
        {
            bool bResult = false;
            target radarTarget = null;
            AISTarget aisTarget = null;

            m_np.m_dicTargetCollection.TryGetValue(dwArpaID, out radarTarget);
           
            if (radarTarget==null)
            {
                return false;
            }
            int nIndex = -1;
            nIndex = FindUnionAisTarget(dwArpaID, UNION_STATE_SENSOR, UNION_ASSCI | UNION_DISASSCI);
            if (nIndex != -1)
            {
                m_np.m_AISCollection.TryGetValue((int)dwArpaID, out aisTarget);
                if (aisTarget != null)
                {
                    bResult = TryUnionTarget(aisTarget, radarTarget, UNION_STATE_SENSOR, nIndex);
                }
            }
            else
            {
                float fMinRng = 926 * 1.0f;
                long dwMinRngID = -1;

                foreach (var item in m_np.m_AISCollection)
                {
                    if (FindUnionArpaTarget(item.Value.MMSI, UNION_STATE_SENSOR, UNION_ASSCI | UNION_DISASSCI) != -1)
                    {
                        continue;
                    }
                    tagTARGET_DIFF stTargetDiff = new tagTARGET_DIFF();
                    if (RetCurState(aisTarget, radarTarget, stTargetDiff) == UNION_ASSCI)
                    {
                        if (stTargetDiff.fRngDiff < fMinRng)
                        {
                            fMinRng = stTargetDiff.fRngDiff;
                            dwMinRngID = item.Value.MMSI;
                        }
                    }
                    if (dwMinRngID != -1)
                    {
                        m_np.m_AISCollection.TryGetValue((int)dwMinRngID, out aisTarget);
                        bResult = TryUnionTarget(aisTarget, radarTarget, UNION_STATE_SENSOR, nIndex);
                    }
                }
            }
            return true;
        }
        //! 查找与AIS对应的联合ARPA目标
	    /*!
	    \param		lpAisTarget：AIS目标指针
	    \return		返回统计列表索引，-1则为查找失败
	    \remark		查找与AIS对应的联合ARPA目标
	    */
       public int FindUnionArpaTarget(long dwAisID, long dwUnionState, int nState)
        {
            if (dwAisID == -1)
            {
                return -1;
            }
            int nSize = m_vectUnion.Count;

            int i = 0;
            for (i = 0; i < nSize; i++)
            {
                if (dwAisID == m_vectUnion[i].dwAisID && ((m_vectUnion[i].dwUnionStat & dwUnionState) != null) && ((m_vectUnion[i].GetState() & nState) != null))
                {
                   
                    break;
                }
            }



            return (i < nSize ? i : -1);
        }
        
	    //! 查找与ARPA对应的联合AIS目标
	    /*!
	    \param		lpArpaTarget：Arpa目标指针
	    \return		返回统计列表索引，-1则为查找失败
	    \remark		查找与ARPA对应的联合AIS目标
	    */
       public  int  FindUnionAisTarget(long dwArpaID, long dwUnionState, int nState)
        {
            if (dwArpaID == -1)
	        {
		        return -1;
	        }


            int nSize = m_vectUnion.Count;
	        int i = 0;
            for (i = 0; i < nSize; i++)
            {
                if (dwArpaID == m_vectUnion[i].dwArpaID && ((m_vectUnion[i].dwUnionStat & dwUnionState) != null) && ((m_vectUnion[i].GetState() & nState) != null))
                {
                    
                    break;
                }
            }
	        return (i < nSize ? i : -1);

        }
        	//! 尝试将AIS和ARPA目标对放进入统计列表
	    /*! 
		    \param		lpAisTarget：AIS目标指针
		    \param		lpArpaTarget：ARPA目标指针
		    \return		是否成功将AIS和ARPA目标对放入统计列表
		    \remark 	
	    */
        bool TryUnionTarget(AISTarget lpAisTarget, target lpArpaTarget, long dwUnionState, int nIndex)
        {
            bool bResult = false;
            if (lpAisTarget == null|| lpArpaTarget == null)
            {
                return bResult;
            }

            float fCourse = 0.0f, fSpeed = 0.0f;
            tagCOMBINE_TGT stCmbTgt1 = new tagCOMBINE_TGT();
            tagCOMBINE_TGT stCmbTgt2 = new tagCOMBINE_TGT();
            tagCOMBINE_TGT stCmbTgtRet = new tagCOMBINE_TGT();

            int nCurState = RetCurState(lpAisTarget, lpArpaTarget,null);
            if (nIndex == -1)
	        {
		        // 只有同时满足瞬时状态为UNION_ASSCI，统计列表未到上限，AIS和ARPA任何一个都未被联合才能进入联合统计列表
		        if ((nCurState == UNION_ASSCI) && (m_vectUnion.Count() < MAX_UNION_SIZE))
		        {
			        tagTARGET_UNION	stTargetUnion = new tagTARGET_UNION();
			        stTargetUnion.dwAisID		= lpAisTarget.MMSI;
			        stTargetUnion.dwArpaID		= lpArpaTarget.targetID;
			        stTargetUnion.dwUnionStat	= dwUnionState;

                    //stTargetUnion.dUnionRng		= stCmbTgtRet.dRange;
                    //stTargetUnion.dUnionAzi		= stCmbTgtRet.dAzimuth;
                    //stTargetUnion.dUnionTSpd	= stCmbTgtRet.dTSpeed;
                    //stTargetUnion.dUnionTCrs	= stCmbTgtRet.dTCourse;

                  
			    
			        stTargetUnion.dUnionTSpd	= lpAisTarget.Speed;
			        stTargetUnion.dUnionTCrs	= lpAisTarget.Course;
			
			        {
				        double dCurLat = 0.0, dCurLon = 0.0;
                        //dCurLat	= g_EFPSDataManager.GetCurLat();
                        //dCurLon	= g_EFPSDataManager.GetCurLon();
                        //GeoFunc::RefPos2GeoPos(dCurLat, dCurLon, stTargetUnion.dUnionAzi, MToNM(stTargetUnion.dUnionRng), stTargetUnion.dUnionLat, stTargetUnion.dUnionLon);

			        }
			        stTargetUnion.ResetState(UNION_DISASSCI);
			        stTargetUnion.ReportOnce(nCurState);
			        m_vectUnion.Add(stTargetUnion);
			        bResult = true;
		        }
	        }
            else
	        {
		        int		nState	= m_vectUnion[nIndex].ReportOnce(nCurState);
		        tagTARGET_UNION lpTargetUnion = m_vectUnion[nIndex];

		        if (nCurState == UNION_ASSCI)
		        {
			       
			    
			        lpTargetUnion .dUnionTSpd	= lpAisTarget.Speed;
			        lpTargetUnion .dUnionTCrs	= lpAisTarget.Course;

			      

		        }
		        else
		        {
			        

		        }

		        switch(nState)
		        {
		        case UNION_ASSCI:
			        break;

		        case UNION_DISASSCI:
			        break;

		        case UNION_BREAK:
			        m_vectUnion.RemoveAt(nIndex);
			        break;
		        }
		        bResult = true;
	        }
            return bResult;
        }
        //! 计算AIS和ARPA数据，返回瞬时状态
	    /*! 
		    \param		lpAisTarget：AIS目标指针
		    \param		lpArpaTarget：Arpa目标指针
		    \return		瞬时状态
		    \remark 	计算AIS和ARPA数据，返回瞬时状态
	    */
         int RetCurState(AISTarget lpAisTarget, target lpArpaTarget, tagTARGET_DIFF lpTargetDiff)
         {
             int nCurState = UNION_DISASSCI;
             float fRangeDist = 0.0f;
             float fBearDiff = 0.0f;
             float fCourseDist = 0.0f;
             float fSpeedDist = 0.0f;

             double dAisLat = 0.0, dAisLon = 0.0, dArpaLat = 0.0, dArpaLon = 0.0;
             float fAisSpd = 0.0f, fAisCrs = 0.0f, fArpaSpd = 0.0f, fArpaCrs = 0.0f;

           
             // 比较ais与arpa目标的距离、速度的偏差
	        double dDistance = 0;

            dDistance =   CommonFunctions.GetDistance(lpAisTarget.Latitude, lpAisTarget.Longitude, lpArpaTarget.Latitude,lpArpaTarget.Longitude);
	      
	        dDistance *= NM;
	        fRangeDist	= (float)dDistance;
	      
	        fSpeedDist	= (float)Math.Abs((lpAisTarget.Speed - lpArpaTarget.Speed));

            //if (fAisSpd < NM && fArpaSpd < NM)
            //{
            //    fCourseDist = 0.0f;
            //}

            //if (fCourseDist > 180)
            //{
            //    fCourseDist = 360 - fCourseDist;
            //}

	        if (lpTargetDiff != null)
	        {
		        lpTargetDiff.fRngDiff	= fRangeDist;
		        lpTargetDiff.fSpdDiff= fSpeedDist;
		       
	        }
            if (fRangeDist < NM  && fSpeedDist < 1000)
            {
                // 瞬时状态为强联合
                nCurState = UNION_ASSCI;
            }
            else
            {
                // 瞬时状态为分离
                nCurState = UNION_DISASSCI;
            }
            return nCurState;
            
         }
         NmeaParse m_np = null;
         int  OnTargetEvent(int nEventType, int nTargetType,long dwID)
         {
            target radarTarget = null;
            m_np.m_dicTargetCollection.TryGetValue(dwID, out radarTarget);
            if (radarTarget!=null)
            {
                ReportSensorArpa(dwID);
            }

            return 0;
         }
        public AisArpaAssociate()
        {
            m_np = new NmeaParse();
        }
    }
}
