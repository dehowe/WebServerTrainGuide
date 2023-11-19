using WebAPI.Struct;


namespace WebAPI.Function
{
    /* 此类主要封装正线准入验证逻辑方法 */
    public class AccessVerify
    {
        private static NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();


        // 查询起始点和结束点之间的区段列表（无序）
        public static int GetNextStationSectionList(int BeginLoc,int EndLoc,int Dir,ref List<int> SectionList)
        {
            int Result = 0;
            // 数据验证
            if((BeginLoc > EndLoc&&Dir==2)|| (BeginLoc < EndLoc && Dir == 1))
            {
                Log.Error("SectionList query error");
                return Result;
            }
            SectionList.Clear(); // 区段列表清空
            for (int i =0; i < GV.sectionConfig.Count; i++)
            {
                // 下行
                if(Dir==2)
                {
                    // 区段被部分包含,被完全包含
                    if ((GV.sectionConfig[i].SectionBeginLoc <= BeginLoc && GV.sectionConfig[i].SectionEndLoc >= BeginLoc) ||
                        (GV.sectionConfig[i].SectionBeginLoc <= EndLoc && GV.sectionConfig[i].SectionEndLoc >= EndLoc) ||
                        (GV.sectionConfig[i].SectionBeginLoc >= BeginLoc && GV.sectionConfig[i].SectionEndLoc <= EndLoc))
                    {

                        SectionList.Add(GV.sectionConfig[i].SectionID);
                    }
                }
                else
                {
                    // 区段被部分包含,被完全包含
                    if ((GV.sectionConfig[i].SectionBeginLoc <= BeginLoc && GV.sectionConfig[i].SectionEndLoc >= BeginLoc) ||
                        (GV.sectionConfig[i].SectionBeginLoc <= EndLoc && GV.sectionConfig[i].SectionEndLoc >= EndLoc) ||
                        (GV.sectionConfig[i].SectionBeginLoc >= EndLoc && GV.sectionConfig[i].SectionEndLoc <= BeginLoc))
                    {

                        SectionList.Add(GV.sectionConfig[i].SectionID);
                    }
                }
            }
            return 1;
        }


        // 根据区段ID查询区段占用状态
        public static int GetOccupyStatusBySectionID(int SectionID)
        {
            int OccupyFlag = -1; // 
            for(int i=0;i<GV.InterlockInfo.SectionOccupyList.Count;i++)
            {
                if (GV.InterlockInfo.SectionOccupyList[i].SectionID == SectionID)
                {
                    OccupyFlag = GV.InterlockInfo.SectionOccupyList[i].OccupyStatus;
                    break;
                }
            }
            return OccupyFlag;
        }


        // 查询区段列表的占用状态，返回空闲标志
        public static void GetSectionOccupyList(List<int> SectionList,ref List<int> OccupyList,ref List<string> OccupyMsg)
        {
            int OccupyFlagTemp;
            string OccupyMsgTemp = "";
            OccupyList.Clear();
            OccupyMsg.Clear();
            for (int i = 0; i < SectionList.Count; i++)
            {
                OccupyFlagTemp = GetOccupyStatusBySectionID(SectionList[i]);
                if(OccupyFlagTemp!=0)
                {
                    OccupyList.Add(SectionList[i]);

                    if (OccupyFlagTemp == 1)
                    {
                        OccupyMsgTemp = "区段" + SectionList[i].ToString() + "处于计划占用状态";
                    }
                    else if(OccupyFlagTemp == 2)
                    {
                        OccupyMsgTemp = "区段" + SectionList[i].ToString() + "处于正在占用状态";
                    }
                    else 
                    {
                        OccupyMsgTemp = "区段" + SectionList[i].ToString() + "未查询到状态";
                    }
                    OccupyMsg.Add(OccupyMsgTemp);
                }
            }
        }

        // 查询列车在计划时刻表中的状态
        public static int GetScheduleState(string CarCode,string ForwardStation,DateTime RequestTime)
        {
            int Result=-1;  // 1:时刻表允许发车 -1:在运行计划中未找到该车辆Code，-2：在运行计划中未找到拟前方站台的停车计划，-3：未到发车时间,
            long RequestTimeStamp = DataQuery.GetTimeStamp(DataQuery.GetTimeStrByDataTime(RequestTime));
            int PlatformCodeIndex;
            // 找到车辆code对应的运行计划
            for (int i=0;i<GV.ShiftList.Count;i++)
            {
                // 找到该车计划
                if (String.Compare(CarCode, GV.ShiftList[i].CarCode)==0)
                {
                    // 找到对应站
                    PlatformCodeIndex = -1;
                    for (int j=0;j< GV.ShiftList[i].ShiftDetailList.Count;j++)
                    {
                        if (String.Compare(ForwardStation, GV.ShiftList[i].ShiftDetailList[j].PlatformCode) == 0)
                        {
                            PlatformCodeIndex = j - 1;
                            break;
                        }
                    }
                    // 没有找到下一站的运行计划
                    if(PlatformCodeIndex==-1)
                    {
                        Result = -2; // 未找到前方站的停车计划
                    }
                    else
                    {
                        long LeaveTimeStamp = DataQuery.GetTimeStamp(DataQuery.GetTimeStrByDataTime(GV.ShiftList[i].ShiftDetailList[PlatformCodeIndex].OutTime));
                        if (RequestTimeStamp> LeaveTimeStamp)
                        {
                            Result = 1; // 未找到前方站的停车计划
                        }
                        else
                        {
                            Result = -3; // 未到发车时间

                        }
                    }
                    break;

                }
            }
            return Result;

        }



        // 查询列车距离目标站台或避让线的区段占用列表
        public static int GetAccessState(int CurrentPosition,string NextStationCode,int Direction, ref string AccessFailureResult)
        {
            int AccessState;
            int TargetLoc;  // 目标公里标
            List<int> SectionList = new List<int>();  // 区段列表
            List<int> OccupyList = new List<int>();   // 区段占用列表
            List<string> OccupyMsg = new List<string>(); // 区段占用信息
            // 根据下一站台CODE查询目标公里标
            TargetLoc = DataQuery.GetLocByPlanformAndDir(int.Parse(NextStationCode),Direction);
            // 查询从当前位置到达目标点的区段列表（无序）
            GetNextStationSectionList(CurrentPosition, TargetLoc, Direction,ref SectionList);
            // 查询区段列表的占用状态
            GetSectionOccupyList(SectionList, ref OccupyList, ref OccupyMsg);

            // 如果没有区段占用
            if(OccupyList.Count == 0)
            {
                AccessState = 1; // 准入状态 允许通行
                AccessFailureResult = "";   // 准入失败原因
            }
            else
            {
                AccessState = 2;
                AccessFailureResult = "";
                for (int i=0;i< OccupyMsg.Count;i++)
                {
                    AccessFailureResult = AccessFailureResult + OccupyMsg[i] + ";";
                }
            }
            return AccessState;
        }


        // 车辆正线准入验证
        public static Response726 TrainEnterVerify(Request726 request726)
        {
            int AccessState = 2;
            int Result;  // 1:时刻表允许发车 -1:在运行计划中未找到该车辆Code，-2：在运行计划中未找到拟前方站台的停车计划，-3：未到发车时间,
            string AccessFailureResult="";
            Response726 response726 = new Response726();
            response726.CarGuideDataList.Clear();
            for (int i=0;i< request726.CarStatusList.Count;i++)
            {
                CarGuideData726 CarGuideDataTemp = new CarGuideData726();
                // 查询在时刻表中的状态
                Result = GetScheduleState(request726.CarStatusList[i].CarCode, request726.CarStatusList[i].ForwardStation, request726.RequestTime);
                // 如果时刻表允许发车
                if (Result == 1)
                {
                    AccessState = GetAccessState(request726.CarStatusList[i].CurrentPosition, request726.CarStatusList[i].ForwardStation, request726.CarStatusList[i].Direction, ref AccessFailureResult);
                }
                else
                {
                    AccessState = 2;
                    if (Result == -1)
                    {
                        AccessFailureResult = "在运行计划中未找到该车辆Code";
                    }
                    else if(Result == -2)
                    {
                        AccessFailureResult = "在运行计划中未找到前方站台的停车计划";
                    }
                    else if(Result == -3)
                    {
                        AccessFailureResult = "未到发车时间";
                    }
                    else
                    {
                        AccessFailureResult = "未知原因"; // 正常不会触发此逻辑
                    }


                }
                CarGuideDataTemp.CarCode = request726.CarStatusList[i].CarCode;
                CarGuideDataTemp.AccessState = AccessState;
                CarGuideDataTemp.AccessFailureResult = AccessFailureResult;
                response726.CarGuideDataList.Add(CarGuideDataTemp);

            }
            return response726;
        }

    }
}
