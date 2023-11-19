using WebAPI.Struct;
using WebAPI.SpeedPlan;

namespace WebAPI.Function
{
    /* 此类主要封装通用的基础数据查询方法 */

    public static class DataQuery
    {

        public static DateTime GetDataTimeByTimeStr(string TimeStr)
        {
            DateTime dateTime = new DateTime();
            if (TimeStr != null)
            {
                try
                {
                    // 将字符串转换为 DateTime 对象，使用自定义格式
                    dateTime = DateTime.ParseExact(TimeStr, "yyyy-MM-dd HH:mm:ss.fff", null);
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("无法转换字符串到 DateTime: " + ex.Message);
                }
            }
            return dateTime;
        }


        public static string GetTimeStrByDataTime(DateTime Time)
        {

            // 将DateTime对象转换为字符串，使用自定义格式
            string customFormattedDate = Time.ToString("yyyy-MM-dd HH:mm:ss.fff");
            return customFormattedDate;
        }

        public static string GetTimeStr(long TimeStamp)
        {
            DateTime dateTime = DateTimeOffset.FromUnixTimeMilliseconds(TimeStamp).UtcDateTime;

            string formattedString = dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

            return formattedString;

        }

        public static long GetTimeStamp(string TimeStr)
        {
            // 解析日期时间字符串
            DateTimeOffset dateTimeOffset = DateTimeOffset.ParseExact(
                TimeStr,
                "yyyy-MM-dd HH:mm:ss.fff",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.AssumeUniversal
            );

            // 获取时间戳（毫秒）
            long timestampMilliseconds = dateTimeOffset.ToUnixTimeMilliseconds();
            return timestampMilliseconds;

        }

        // 根据公里标查询限速值 cm/s
        public static int GetSpeedLimitByLoc(int Loc)
        {
            int result = 1250; // cm/s    默认限速45km/h
            for (int i = 0; i < GV.limitConfig.Count; i++)
            {
                if (Loc >= GV.limitConfig[i].LimitBeginLoc && Loc <= GV.limitConfig[i].LimitEndLoc)
                {
                    result = (int)(GV.limitConfig[i].LimitValue * 100 / 3.6);
                    break;
                }
            }
            return result;
        }


        // 根据起点，位移和方向计算终点
        public static int GetNewLoc(int LocBegin, int Dis, int Dir)
        {
            int DisResult;
            if (Dir == 2) // 下行 顺时针，公里标递增
            {
                DisResult = LocBegin + Dis;
                while (DisResult > 4225)
                {
                    DisResult = DisResult - 4225;
                }
            }
            else
            {
                DisResult = LocBegin - Dis;
                while (DisResult < 0)
                {
                    DisResult = 4225 + DisResult;
                }
            }
            return DisResult;
        }

        // 根据起点,终点两点公里标和方向计算距离
        public static int GetDisBetweenBeginAndEnd(int LocBegin, int LocEnd, int Dir)
        {
            int DisResult;
            if (Dir == 2) // 下行 顺时针，公里标递增
            {
                DisResult = (LocEnd >= LocBegin) ? (LocEnd - LocBegin) : (4225 - LocBegin + LocEnd);
            }
            else// 上行 逆时针，公里标递减
            {

                DisResult = (LocEnd <= LocBegin) ? (LocBegin - LocEnd) : (4225 - LocEnd + LocBegin);
            }
            return DisResult;
        }

        // 根据接口数据删除多余列车运行信息
        public static void DeleteTrainOperationInfo(List<int> CarCodeList)
        {
            int CarCodeTemp = 0;
            for (int i = GV.trainOperationInfo.Count - 1; i >= 0; i--)
            {
                CarCodeTemp = GV.trainOperationInfo[i].CarCode;
                int FindResult = 0;
                for (int j = 0; j < CarCodeList.Count; j++)
                {
                    if (CarCodeList[j] == CarCodeTemp)
                    {
                        FindResult = 1;
                        break; // 如果找到了，不需要继续搜索
                    }
                }
                // 如果未在列表找找打此车辆，则清除相关列车运行信息
                if (FindResult == 0)
                {
                    GV.trainOperationInfo.RemoveAt(i);
                }
            }
        }


        // 从列车运行信息列表查询车辆Code所在索引
        public static int GetTrainIndexOrCreate(int CarCode)
        {
            int FindIndex = -1;
            int FindResult = 0;
            for (int i = 0; i < GV.trainOperationInfo.Count; i++)
            {
                // 如果在运行信息列表里找到该车辆Code，则返回所在索引
                if (GV.trainOperationInfo[i].CarCode == CarCode)
                {
                    FindResult = 1;
                    FindIndex = i;
                }

            }
            // 如果没有找到该车辆Code,则增加该车存储对象
            if (FindResult == 0)
            {
                TrainOperationInfo temp = new TrainOperationInfo();
                temp.CarCode = CarCode;
                // 从计划运行图中填充运行计划
                for (int i = 0; i < GV.ShiftList.Count; i++)
                {
                    if (temp.CarCode == int.Parse(GV.ShiftList[i].CarCode))
                    {
                        temp.Shift = GV.ShiftList[i];
                        temp.PlanformCodeList.Clear();
                        temp.InTimeList.Clear();
                        temp.OutTimeList.Clear();
                        for (int j = 0; j < GV.ShiftList[i].ShiftDetailList.Count; j++)
                        {
                            TimeAndStamp InTimeTemp = new TimeAndStamp();
                            TimeAndStamp OutTimeTemp = new TimeAndStamp();
                            InTimeTemp.TimeStr = GetTimeStrByDataTime(GV.ShiftList[i].ShiftDetailList[j].InTime);
                            InTimeTemp.TimeStamp = GetTimeStamp(InTimeTemp.TimeStr);
                            OutTimeTemp.TimeStr = GetTimeStrByDataTime(GV.ShiftList[i].ShiftDetailList[j].OutTime);
                            OutTimeTemp.TimeStamp = GetTimeStamp(OutTimeTemp.TimeStr);
                            int StopTimeTemp = (int)(OutTimeTemp.TimeStamp - InTimeTemp.TimeStamp) / 1000;
                            // 停站时间为0的直接跳停
                            if (StopTimeTemp > 0)
                            {
                                temp.PlanformCodeList.Add(int.Parse(GV.ShiftList[i].ShiftDetailList[j].PlatformCode));
                                temp.InTimeList.Add(InTimeTemp);
                                temp.OutTimeList.Add(OutTimeTemp);
                                temp.StopTimeList.Add(StopTimeTemp);
                            }


                        }
                        break;
                    }

                }

                GV.trainOperationInfo.Add(temp);
                FindIndex = GV.trainOperationInfo.Count - 1;
            }
            return FindIndex;
        }


        // 根据方向计算到达站台的距离，负数表示驶过站台
        public static int GetDisFromPlanform(int PlanformCode, int Loc, int Dir)
        {
            int DisResult = 65535;
            for (int i = 0; i < GV.stationConfig.Count; i++)
            {
                if (GV.stationConfig[i].StationCode == PlanformCode)
                {
                    if (Dir == 2) // 下行 顺时针，公里标递增
                    {
                        DisResult = GV.stationConfig[i].StopPointLocUp - Loc;
                        // 线路尽头
                        if (Math.Abs(DisResult) > 3000)
                        {
                            DisResult = 4225 - Loc + GV.stationConfig[i].StopPointLocUp;
                        }
                    }
                    else// 上行 逆时针，公里标递减
                    {
                        DisResult = Loc - GV.stationConfig[i].StopPointLocDown;
                        // 线路尽头
                        if (Math.Abs(DisResult) > 3000)
                        {
                            DisResult = 4225 - GV.stationConfig[i].StopPointLocDown + Loc;
                        }
                    }
                    break;
                }

            }
            return DisResult;
        }

        //根据站台Code和方向查找停车点所在公里标
        public static int GetLocByPlanformAndDir(int PlanformCode, int Dir)
        {
            int LocResult = 65535;
            for (int i = 0; i < GV.stationConfig.Count; i++)
            {
                if (GV.stationConfig[i].StationCode == PlanformCode)
                {
                    if (Dir == 2) // 下行
                    {
                        LocResult = GV.stationConfig[i].StopPointLocUp;
                    }
                    else
                    {
                        LocResult = GV.stationConfig[i].StopPointLocDown;
                    }
                    break;
                }
            }
            return LocResult;
        }

        // 从运行计划中更新当前站、下一站
        public static void GetPlanformCodeByLoc(ref TrainOperationInfo TrainOptInfo)
        {
            // 如果运行计划有效
            if (TrainOptInfo.PlanformCodeList.Count > 0)
            {
                // 从运行计划停车站台查询最近站台
                List<int> PlanformDis = new List<int>();
                for (int i = 0; i < TrainOptInfo.PlanformCodeList.Count; i++)
                {
                    PlanformDis.Add(GetDisFromPlanform(TrainOptInfo.PlanformCodeList[i], TrainOptInfo.CurrentPosition, TrainOptInfo.Direction));
                }
                // 对列表中的每个元素求绝对值
                List<int> AbsolutePlanformDis = PlanformDis.Select(Math.Abs).ToList();
                // 找到最小值
                int MinValue = AbsolutePlanformDis.Min();
                // 找到最小值对应的索引
                int MinIndex = AbsolutePlanformDis.IndexOf(MinValue);

                // 1.当前列车处于站台区域附近（<10m），当前站为本站
                if (Math.Abs(PlanformDis[MinIndex]) < 10)
                {
                    TrainOptInfo.CurrentStationCode = TrainOptInfo.PlanformCodeList[MinIndex];
                    TrainOptInfo.NextStationCode = (MinIndex + 1 < TrainOptInfo.PlanformCodeList.Count) ? TrainOptInfo.PlanformCodeList[MinIndex + 1] : 65535;
                }
                // 2. 当前列车处于区间，当前站为上一站
                else
                {
                    if (PlanformDis[MinIndex] > 0)
                    {
                        TrainOptInfo.CurrentStationCode = (MinIndex > 0) ? TrainOptInfo.PlanformCodeList[MinIndex - 1] : 65535;
                        TrainOptInfo.NextStationCode = TrainOptInfo.PlanformCodeList[MinIndex];

                    }
                    else
                    {
                        TrainOptInfo.CurrentStationCode = TrainOptInfo.PlanformCodeList[MinIndex];
                        TrainOptInfo.NextStationCode = (MinIndex + 1 < TrainOptInfo.PlanformCodeList.Count) ? TrainOptInfo.PlanformCodeList[MinIndex + 1] : 65535;
                    }

                }

            }

        }
    }
}
