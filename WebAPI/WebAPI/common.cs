using WebAPI.Struct;
using WebAPI.SpeedPlan;

namespace WebAPI.Common
{

    public static class GV  // 全局变量
    {
        public static List<StationConfig> stationConfig = new List<StationConfig>();    // 站台配置数据
        public static List<TrainConfig> trainConfig = new List<TrainConfig>();          // 列车配置数据
        public static List<SectionConfig> sectionConfig = new List<SectionConfig>();    // 联锁区段配置数据
        public static List<LimitConfig> limitConfig = new List<LimitConfig>();          // 限速配置数据
        public static List<TrainOperationInfo> trainOperationInfo = new List<TrainOperationInfo>();          // 列车运行数据
        public static List<Shift> ShiftList = new List<Shift>();                // 全天班次数据集合
        public static TimeAndStamp GlobalTime = new TimeAndStamp();                      // 全局时间和时间戳
    }


    public class ConfigData  // 配置数据 from 基础数据规划
    {
        // 站台配置离线数据
        public static string[,] stationData = new string[,]
        {
            {"序号","站台Code","站台名称","站台类型","隶属区段","站台长度m","下行起点公里标m","上行起点公里标m"},
            {"1","81","E区森林小镇站","站台-正线","J0503-J0504G","35","2023","2058" },
            {"2","82","E区站","站台-侧线","J0505-J0506G","35","2023","2058"},
            {"3","83","VIP2站","避让点-侧线","J0405-J0406G","36","1747","1783"},
            {"5","84","VIP2站-正线(虚拟站台)","避让点-正线","J0403-J0404G","36","1747","1783"},
            {"6","85","D区城市大学站","站台-正线","J0302-J0401G","36","1495","1531"},
            {"7","87","CD区间站","避让点-侧线","J0305-J0306G","36","1341","1377"},
            {"8","88","CD区间站-正线(虚拟站台)","避让点-正线","J0303-J0304G","36","1341","1377"},
            {"9","89","C区大桥南站","站台-正线","J0202-J0301G","36","1103","1139"},
            {"10","91","VIP1站","避让点-侧线","J0205-J0206G","36","723","759"},
            {"11","92","VIP1站-正线(虚拟站台)","避让点-正线","J0203-J0204G","36","723","759"},
            {"12","93","B区水镇站","站台-正线","J0102-J0201G","36","412","448"},
            {"13","95","A区站","避让点-侧线","J0105-J0106G","37","96","133"},
            {"14","96","A区站-正线(虚拟站台)","避让点-正线","J0103-J0104G","37","96","133"},
            {"15","97","A区山顶聚落站","站台-正线","J0802-J0101G","36","4207","18"},
            {"16","99","H区大桥北站","站台-正线","J0803-J0804G","36","3971","4007"},
            {"17","100","H区站","站台-侧线","J0805-J0806G","36","3971","4007"},
            {"18","101","G区城市街区站","站台-正线","J0703-J0704G","36","3424","3460"},
            {"19","102","G区站","站台-侧线","J0705-J0706G","36","3424","3460"},
            {"20","103","F10站","避让点-侧线","J0605-J0606G","28","2781","2809"},
            {"21","104","F10站-正线(虚拟站台)","避让点-正线","J0603-J0604G","28","2781","2809"},
            {"22","105","F区小镇广场站","站台-正线","J0502-J0601G","36","2483","2519"},
        };
        // 列车配置离线数据
        public static string[,] trainData = new string[,]
        {
            {"序号","车辆Code","车辆名称"},
            {"1","1401","QPYG101"},
            {"2","1402","QPYG102"},
            {"3","1403","QPYG103"},
            {"4","1404","QPYG104"},
            {"5","1405","QPYG105"},
            {"6","1406","QPYG106"},
            {"7","1407","QPYG107"},
            {"8","1408","QPYG108"},
            {"9","1409","QPYG109"},
            {"10","1410","QPYG110"},
            {"11","1411","QPYG111"},
            {"12","1412","QPYG112"},
            {"13","1413","QPYG113"},
            {"14","1414","QPYG114"},
            {"15","1415","QPYG115"},
            {"16","1416","QPYG116"},
        };
        // 联锁区段配置离线数据
        public static string[,] sectionData = new string[,]
        {
            {"序号","联锁区段名称","始端公里标m","末端公里标m"},
            {"1","J0802-J0101G","4100","4225"},
            {"2","J0802-J0101G","0","57"},
            {"3","C0101DG","57","96"},
            {"4","J0103-J0104G","96","133"},
            {"5","J0105-J0106G","96","133"},
            {"6","C0102DG","133","171"},
            {"7","J0102-J0201G","171","670"},
            {"8","C0201DG","670","710"},
            {"9","J0205-J0206G","710","775"},
            {"10","J0203-J0204G","710","775"},
            {"11","C0202DG","775","815"},
            {"12","J0202-J0301G","815","1291"},
            {"13","C0301DG","1291","1341"},
            {"14","J0305-J0306G","1341","1371"},
            {"15","J0303-J0304G","1341","1371"},
            {"16","C0302DG","1371","1417"},
            {"17","J0302-J0401G","1417","1692"},
            {"18","C0401DG","1692","1741"},
            {"19","J0405-J0406G","1741","1797"},
            {"20","J0403-J0404G","1741","1797"},
            {"21","C0402DG","1797","1838"},
            {"22","J0402-J0501G","1838","1983"},
            {"23","C0501DG","1983","2023"},
            {"24","J0505-J0506G","2023","2058"},
            {"25","J0503-J0504G","2023","2058"},
            {"26","C0502-C0503DG","2058","2118"},
            {"27","J0502-J0507DG","2118","2208"},
            {"28","C0504DG","2208","2221"},
            {"29","J0502-J0601G","2221","2732"},
            {"30","C0601DG","2732","2781"},
            {"31","J0603-J0604G","2781","2809"},
            {"32","J0605-J0606G","2781","2809"},
            {"33","C0602DG","2809","2848"},
            {"34","J0602-J0701G","2848","3372"},
            {"35","C0701DG","3372","3412"},
            {"36","J0703-J0704G","3412","3467"},
            {"37","J0705-J0706G","3412","3467"},
            {"38","C0702DG","3467","3508"},
            {"39","J0702-J0801G","3508","3866"},
            {"40","C0801DG","3866","3944"},
            {"41","J0803-J0804G","3944","4019"},
            {"42","J0805-J0806G","3944","4019"},
            {"43","C0802DG","4019","4100"},
            {"44","J0802-J0101G","4100","4225"},
            {"45","J0802-J0101G","0","57"},
        };
        // 限速配置离线数据
        public static string[,] limitData = new string[,]
        {
            {"序号","交点号","始端公里标m","末端公里标m","曲线限速km/h"},
            {"1","SJD1","192","286","20"},
            {"2","SJD1-2","310","393","15"},
            {"3","SJD2","456","551","20"},
            {"4","SJD3","566","615","25"},
            {"5","SJD4","817","916","30"},
            {"6","SJD5","1026","1097","20"},
            {"7","SJD6","1222","1230","25"},
            {"8","SJD7","1431","1480","20"},
            {"9","S.D8","1558","1682","25"},
            {"10","SJD9","1842","1981","20"},
            {"11","SJD10","2131","2180","30"},
            {"12","SID11","2248","2315","25"},
            {"13","SJD12","2331","2439","30"},
            {"14","SJD13","2545","2730","25"},
            {"15","SJD14","2853","3104","40"},
            {"16","SJD15","3172","3370","40"},
            {"17","SJD16","3624","3817","45"},
            {"18","SJD17","3900","3959","20"},
            {"19","SJD18","4017","4064","20"},
            {"20","SJD19","4103","4176","20"},

        };

    }

    public class Common
    {



        public static void ProgramInit()  // 程序初始化
        {
            // 基础数据初始化
            DataInit();  

        }


        public static int DataInit()  // 基础数据初始化
        {
            int result = 1;
            try
            {
                // 站台数据初始化
                for (int i = 1; i < ConfigData.stationData.GetLength(0); i++)
                {
                    StationConfig stationConfigTemp = new StationConfig(); ;
                    stationConfigTemp.StationID = int.Parse(ConfigData.stationData[i, 0]);
                    stationConfigTemp.StationCode = int.Parse(ConfigData.stationData[i, 1]);
                    stationConfigTemp.StationName = ConfigData.stationData[i, 2];
                    stationConfigTemp.StationType = ConfigData.stationData[i, 3]; ;
                    stationConfigTemp.SectionID = ConfigData.stationData[i, 4]; ;
                    stationConfigTemp.StationLength = int.Parse(ConfigData.stationData[i, 5]);
                    stationConfigTemp.StationLocUp = int.Parse(ConfigData.stationData[i, 6]);
                    stationConfigTemp.StationLocDown = int.Parse(ConfigData.stationData[i, 7]);
                    // 上行停车点为站台下行起点，反之亦然
                    stationConfigTemp.StopPointLocUp = stationConfigTemp.StationLocDown;
                    stationConfigTemp.StopPointLocDown = stationConfigTemp.StationLocUp;


                    GV.stationConfig.Add(stationConfigTemp);
                }
                // 列车数据初始化
                for (int i = 1; i < ConfigData.trainData.GetLength(0); i++)
                {
                    TrainConfig trainConfigTemp = new TrainConfig();
                    trainConfigTemp.TrainID = int.Parse(ConfigData.trainData[i, 0]);
                    trainConfigTemp.TrainCode = int.Parse(ConfigData.trainData[i, 1]);
                    trainConfigTemp.TrainName = ConfigData.trainData[i, 2];
                    trainConfigTemp.TrainLength = 32;
                    trainConfigTemp.TractionAcc = 82;
                    trainConfigTemp.BrakeAcc = 146;
                    GV.trainConfig.Add(trainConfigTemp);
                }
                // 联锁区段数据初始化
                for (int i = 1; i < ConfigData.sectionData.GetLength(0); i++)
                {
                    SectionConfig sectionConfigTemp = new SectionConfig();
                    sectionConfigTemp.SectionID = int.Parse(ConfigData.sectionData[i, 0]);
                    sectionConfigTemp.SectionName = ConfigData.sectionData[i, 1];
                    sectionConfigTemp.SectionBeginLoc = int.Parse(ConfigData.sectionData[i, 2]);
                    sectionConfigTemp.SectionEndLoc = int.Parse(ConfigData.sectionData[i, 3]);
                    GV.sectionConfig.Add(sectionConfigTemp);
                }
                // 限速区段数据初始化
                for (int i = 1; i < ConfigData.limitData.GetLength(0); i++)
                {
                    LimitConfig limitConfigTemp = new LimitConfig();
                    limitConfigTemp.LimitID = int.Parse(ConfigData.limitData[i, 0]);
                    limitConfigTemp.LimitName = ConfigData.limitData[i, 1];
                    limitConfigTemp.LimitBeginLoc = int.Parse(ConfigData.limitData[i, 2]);
                    limitConfigTemp.LimitEndLoc = int.Parse(ConfigData.limitData[i, 3]);
                    limitConfigTemp.LimitValue = int.Parse(ConfigData.limitData[i, 4]);
                    GV.limitConfig.Add(limitConfigTemp);
                }
            }
            catch(Exception e)
            {
                result = 0; // 异常
                Console.WriteLine("ERROR_1 Base data init error");
                Console.WriteLine(e.Message);
            }
            return result;
        }



        public static long GetTimeStamp(string TimeStr)
        {
            // 解析日期时间字符串
            DateTimeOffset dateTimeOffset = DateTimeOffset.ParseExact(
                TimeStr,
                "yyyy-MM-ddTHH:mm:ss.fffZ",
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
            for(int i = 0; i < GV.limitConfig.Count; i++)
            {
                if(Loc >= GV.limitConfig[i].LimitBeginLoc && Loc <= GV.limitConfig[i].LimitEndLoc)
                {
                    result  = (int)(GV.limitConfig[i].LimitValue * 100 / 3.6);
                    break;
                }
            }
            return result;
        }

        // 根据起点,终点两点公里标和方向计算距离
        public static int GetDisBetweenBeginAndEnd(int LocBegin,int LocEnd,int Dir)
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
            for (int i = GV.trainOperationInfo.Count-1; i >=0; i--)
            {
                CarCodeTemp = GV.trainOperationInfo[i].CarCode;
                int FindResult = 0;
                for(int j=0;j<CarCodeList.Count;j++)
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
            for(int i = 0; i < GV.trainOperationInfo.Count; i++)
            {
                // 如果在运行信息列表里找到该车辆Code，则返回所在索引
                if (GV.trainOperationInfo[i].CarCode == CarCode)
                {
                    FindResult = 1;
                    FindIndex = i;
                }

            }
            // 如果没有找到该车辆Code,则增加该车存储对象
            if(FindResult == 0)
            {
                TrainOperationInfo temp = new TrainOperationInfo();
                temp.CarCode = CarCode;
                // 从计划运行图中填充运行计划
                for(int i=0;i<GV.ShiftList.Count;i++)
                {
                    if(temp.CarCode == int.Parse(GV.ShiftList[i].CarCode))
                    {
                        temp.Shift = GV.ShiftList[i];
                        temp.PlanformCodeList.Clear();
                        temp.InTimeList.Clear();
                        temp.OutTimeList.Clear();
                        for (int j=0;j<GV.ShiftList[i].ShiftDetailList.Count;j++)
                        {
                            TimeAndStamp InTimeTemp = new TimeAndStamp();
                            TimeAndStamp OutTimeTemp = new TimeAndStamp();
                            InTimeTemp.TimeStr = GV.ShiftList[i].ShiftDetailList[j].InTime;
                            InTimeTemp.TimeStamp = GetTimeStamp(InTimeTemp.TimeStr);
                            OutTimeTemp.TimeStr = GV.ShiftList[i].ShiftDetailList[j].OutTime;
                            OutTimeTemp.TimeStamp = GetTimeStamp(OutTimeTemp.TimeStr);
                            int StopTimeTemp = (int)(OutTimeTemp.TimeStamp - InTimeTemp.TimeStamp) / 1000;
                            temp.PlanformCodeList.Add(int.Parse(GV.ShiftList[i].ShiftDetailList[j].PlatformCode));
                            temp.InTimeList.Add(InTimeTemp);
                            temp.OutTimeList.Add(OutTimeTemp);
                            temp.StopTimeList.Add(StopTimeTemp);
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
        public static int GetDisFromPlanform(int PlanformCode,int Loc,int Dir)
        {
            int DisResult = 65535;
            for(int i=0;i<GV.stationConfig.Count;i++)
            {
                if (GV.stationConfig[i].StationCode == PlanformCode)
                {
                    if(Dir == 2) // 下行 顺时针，公里标递增
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
                        LocResult =  GV.stationConfig[i].StopPointLocDown;
                    }
                    break;
                }
            }
            return LocResult;
        }

        // 从运行计划中更新当前站、下一站
        public static void GetPlanformCodeByLoc(ref TrainOperationInfo TrainOptInfo)
        {
            // 从运行计划停车站台查询最近站台
            List<int> PlanformDis = new List<int>();
            for(int i=0;i< TrainOptInfo.PlanformCodeList.Count;i++)
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
            if (Math.Abs(PlanformDis[MinIndex])<10)
            {
                TrainOptInfo.CurrentStationCode = TrainOptInfo.PlanformCodeList[MinIndex];
                TrainOptInfo.NextStationCode = (MinIndex + 1 < TrainOptInfo.PlanformCodeList.Count) ? TrainOptInfo.PlanformCodeList[MinIndex + 1] : 65535;
            }
            // 2. 当前列车处于区间，当前站为上一站
            else
            {
                if(PlanformDis[MinIndex]>0)
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


        // 计算运行计划的离线曲线
        public static void CalAllSpeedOptOffline()
        {
            int[] PlanFormUpList = new int[] { 81, 84, 85, 88, 89, 92, 93, 96, 97, 99, 101, 104, 105, 82 };
            int[] PlanFormDownList = new int[] { 82, 105, 104, 102, 100, 97, 95, 93, 91, 89, 87, 85, 84, 81 };
            List<int> ParkPointUpList = new List<int>();
            List<int> ParkPointDownList = new List<int>();

            for (int i = 0; i < PlanFormUpList.Length; i++)
            {
                ParkPointUpList.Add(GetLocByPlanformAndDir(PlanFormUpList[i], 1));
            }
            for (int i = 0; i < PlanFormDownList.Length; i++)
            {
                ParkPointDownList.Add(GetLocByPlanformAndDir(PlanFormDownList[i], 2));
            }

            // 
            TrainOperationInfo TrainTemp = new TrainOperationInfo();
            for (int i = 0; i < PlanFormDownList.Length - 1; i++)
            {
                // 上行 打印变量也要修改
                //TrainTemp.Direction = 1;
                //TrainTemp.CurrentPosition = ParkPointUpList[i];
                //TrainTemp.CurrentStationCode = PlanFormUpList[i];
                //TrainTemp.NextStationCode = PlanFormUpList[i+1];

                // 下行 打印变量也要修改
                TrainTemp.Direction = 2;
                TrainTemp.CurrentPosition = ParkPointDownList[i];
                TrainTemp.CurrentStationCode = PlanFormDownList[i];
                TrainTemp.NextStationCode = PlanFormDownList[i + 1];
                TrainTemp.DriveStageFlag = 1;
                TrainTemp.OfflineSpeedOptFlag = 0;
                TrainTemp.TargetOperationTime = 20;


                CalSpeedOptData(ref TrainTemp);
                Console.WriteLine("站台{0}({1}m)->站台{2}({3}m),方向{4},区间长度{5}cm", TrainTemp.CurrentStationCode, TrainTemp.CurrentPosition, TrainTemp.NextStationCode, ParkPointDownList[i + 1], TrainTemp.Direction, TrainTemp.IntervalLength);
                SpeedOptParameter parameters = new SpeedOptParameter { speed = 0, targetTime = (ushort)TrainTemp.TargetOperationTime, intervalLength = TrainTemp.IntervalLength, gradient = TrainTemp.Gradient, limit = TrainTemp.Limit, CarCode = 0};
                SpeedOptManager.SpeedOptStart(parameters);
            }

        }

        public static void RefreshTrainOperationInfo()  // 更新列车运行信息
        {
            TrainOperationInfo TrainOptTemp = new TrainOperationInfo();
            for (int i = 0; i < GV.trainOperationInfo.Count; i++)
            {
                TrainOptTemp = GV.trainOperationInfo[i];
                // 如果列车在正线运行
                if(TrainOptTemp.IsOperatingLine == 2)
                {
                    // 更新驾驶阶段,连续两周期速度为0，默认进入停车阶段,否则为区间运行
                    TrainOptTemp.DriveStageLastFlag = TrainOptTemp.DriveStageFlag;
                    TrainOptTemp.DriveStageFlag = (TrainOptTemp.CarSpeed == 0 && TrainOptTemp.CarSpeedLast == 0)?1:2;

                    // 如果区间运行转停车阶段，且运行计划有效
                    if (TrainOptTemp.DriveStageFlag == 1 && TrainOptTemp.DriveStageFlag == 2 && TrainOptTemp.PlanformCodeList.Count > 0)
                    {
                        // 更新当前站和下一站
                        GetPlanformCodeByLoc(ref TrainOptTemp);

                        // 更新区间起点
                        TrainOptTemp.IntervalBeginLoc = TrainOptTemp.CurrentPosition;
                        // 曲线优化相关标志复位
                        TrainOptTemp.OfflineSpeedOptFlag = 0; // 曲线未优化
                        TrainOptTemp.LeaveFlag = 0; // 不允许发车
                        TrainOptTemp.IntervalCurrentDis = 0; // 相对区间起点位移清0
                        TrainOptTemp.Gradient.Clear();
                        TrainOptTemp.OptimalLoc.Clear();
                        TrainOptTemp.OptimalSpeed.Clear();
                        TrainOptTemp.LevelFlag.Clear();
                        if(TrainOptTemp.CurrentStationCode != 65535 && TrainOptTemp.NextStationCode != 65535)
                        {
                            // 停站时间更新,找到当前站对应的索引
                            TrainOptTemp.ArriveTimeStamp = GV.GlobalTime.TimeStamp;
                            int StationIndex = TrainOptTemp.PlanformCodeList.IndexOf(TrainOptTemp.CurrentStationCode);
                            // 期望停站时间=出站时间-到站时间
                            TrainOptTemp.StopCutDowm = (TrainOptTemp.OutTimeList[StationIndex].TimeStamp - TrainOptTemp.InTimeList[StationIndex].TimeStamp);
                            if (TrainOptTemp.StopCutDowm < 1000 * 10)
                            {
                                Console.WriteLine("ERROR_104:stop time error");
                            }


                        }

                        Console.WriteLine("INFO_:arrive station,plan refresh,current:{0},next:{1},stoptime:{2}", TrainOptTemp.CurrentStationCode, TrainOptTemp.NextStationCode, TrainOptTemp.StopCutDowm/1000);
                    }

                    // 如果停车阶段
                    if (TrainOptTemp.DriveStageFlag == 1)
                    {
                        if(TrainOptTemp.StopCutDowm!=65535)
                        {
                            // 停站倒计时检查
                            if (GV.GlobalTime.TimeStamp> TrainOptTemp.ArriveTimeStamp + TrainOptTemp.StopCutDowm)
                            {
                                TrainOptTemp.LeaveFlag = 1; // 倒计时结束，可以发车
                            }
                        }
                    }


                        

                }
            }
        }

        // 速度优化基础数据计算
        public static int CalSpeedOptData(ref TrainOperationInfo TrainOptInfo)
        {
            int result = 1;
            // 1.计算区间长度
            int IntervalLengthTemp = GetDisFromPlanform(TrainOptInfo.NextStationCode, TrainOptInfo.CurrentPosition, TrainOptInfo.Direction);
            if (IntervalLengthTemp > 0)
            {
                TrainOptInfo.IntervalLength = (UInt32)IntervalLengthTemp*100;
            }
            else
            {
                TrainOptInfo.IntervalLength = 0; 
                Console.WriteLine("ERROR_102:IntervalLength cal error,result:{0}", IntervalLengthTemp);
                return 0;
            }
            TrainOptInfo.Gradient.Clear();

            int LocTemp = 0;
            int LimitTemp = 0;
            List<int> SpeedLimitList = new List<int>();
            List<int> LimitChangeLoc = new List<int>();     // 限速转换点相对区间起点位置
            List<int> LimitChangeVal = new List<int>();     // 限速转换点对应限速值

            for (int i=0;i< IntervalLengthTemp; i++)
            {
                // 2.添加区间坡度
                TrainOptInfo.Gradient.Add(0); // todo 添加坡度数据

                // 3.计算区间限速
                if (TrainOptInfo.Direction == 2) // 下行,公里标递增
                {
                    LocTemp = TrainOptInfo.CurrentPosition + i;
                    if(LocTemp > 4225)
                    {
                        LocTemp = TrainOptInfo.CurrentPosition + i - 4225;
                    }
                }
                else if (TrainOptInfo.Direction == 1)
                {
                    LocTemp = TrainOptInfo.CurrentPosition - i;
                    if (LocTemp <= 0)
                    {
                        LocTemp = TrainOptInfo.CurrentPosition - i + 4225;
                    }
                }
                else
                {
                    Console.WriteLine("ERROR_103:Direction error");
                    return 0;
                }

                LimitTemp = GetSpeedLimitByLoc(LocTemp);

                // 找到限速转换点
                if ( i > 0)
                {
                    if(LimitTemp != SpeedLimitList[i - 1])
                    {
                        LimitChangeLoc.Add(i*100);
                        LimitChangeVal.Add(SpeedLimitList[i - 1]);
                    }
                }
                // 添加最后终点
                if(i == IntervalLengthTemp - 1)
                {
                    LimitChangeLoc.Add(IntervalLengthTemp * 100);
                    LimitChangeVal.Add(SpeedLimitList[i - 1]);
                }

                SpeedLimitList.Add(LimitTemp);
            }

            if (LimitChangeLoc.Count < 1)
            {
                Console.WriteLine("ERROR_104:Limit error");
                return 0;
            }

            UInt32[,] LimitChangeArray = new UInt32[2, LimitChangeLoc.Count];

            for(int j=0;j< LimitChangeLoc.Count; j++)
            {
                LimitChangeArray[0, j] = (UInt32)LimitChangeLoc[j];
                LimitChangeArray[1, j] = (UInt32)LimitChangeVal[j];
            }
            TrainOptInfo.Limit = LimitChangeArray;


            // 4. 计算期望运行时分
            // 找到目的站对应的索引
            int StationIndex = TrainOptInfo.PlanformCodeList.IndexOf(TrainOptInfo.NextStationCode);
            // 期望运行时分=目的站到站时间-当前站出发时间
            if(TrainOptInfo.InTimeList.Count>0)
            {
                TrainOptInfo.TargetOperationTime = (TrainOptInfo.InTimeList[StationIndex].TimeStamp - TrainOptInfo.OutTimeList[StationIndex - 1].TimeStamp) / 1000;
                if (TrainOptInfo.TargetOperationTime < 10)
                {
                    Console.WriteLine("ERROR_104:operation time error");
                    return 0;
                }
            }

            return result;
        }

        // 曲线优化触发检查
        public static void CheckSpeedOptimal()
        {
            TrainOperationInfo TrainOptTemp = new TrainOperationInfo();
            for (int i = 0; i < GV.trainOperationInfo.Count; i++)
            {
                TrainOptTemp = GV.trainOperationInfo[i];
                // 如果是停站阶段、未优化曲线、当前站、目的站有效
                if (TrainOptTemp.DriveStageFlag == 1 && TrainOptTemp.OfflineSpeedOptFlag == 0 && TrainOptTemp.NextStationCode != 65535 && TrainOptTemp.CurrentStationCode != 65535)
                {
                    // 计算前方区间曲线优化所需数据
                    int result =  CalSpeedOptData(ref TrainOptTemp);
                    
                    // 数据处理正常，新建线程进行曲线优化
                    if(result == 1)
                    {
                        TrainOptTemp.OfflineSpeedOptFlag = 1; // 正在进行曲线优化
                        SpeedOptParameter parameters = new SpeedOptParameter{ speed = 0,targetTime = (ushort)(TrainOptTemp.TargetOperationTime + 30), intervalLength = TrainOptTemp.IntervalLength , gradient = TrainOptTemp.Gradient, limit = TrainOptTemp.Limit, CarCode = TrainOptTemp.CarCode};
                        // 创建新线程，并指定要执行的方法
                        //Thread myThread = new Thread(new ParameterizedThreadStart(SpeedOptManager.SpeedOptStart));
                        //myThread.Start(parameters);
                        // 使用 Task.Run 启动异步任务，并传递参数
                        Task.Run(() => SpeedOptManager.SpeedOptStart(parameters));
                    }
                }
            }
        }

        // 更新推荐速度
        public static void RefreshRecommendSpeed()
        {
            TrainOperationInfo TrainOptTemp = new TrainOperationInfo();
            for (int i = 0; i < GV.trainOperationInfo.Count; i++)
            {
                TrainOptTemp = GV.trainOperationInfo[i];
                
                // 如果允许发车，曲线优化完成，目的地一致
                if (TrainOptTemp.LeaveFlag == 1 && TrainOptTemp.OfflineSpeedOptFlag == 2 && TrainOptTemp.SpeedOptStationCode == TrainOptTemp.NextStationCode)
                {
                    // 计算当前相对起点距离
                    TrainOptTemp.IntervalCurrentDis = GetDisBetweenBeginAndEnd(TrainOptTemp.IntervalBeginLoc, TrainOptTemp.CurrentPosition, TrainOptTemp.Direction);
                    if(TrainOptTemp.IntervalCurrentDis >= 0 && TrainOptTemp.IntervalCurrentDis < TrainOptTemp.OptimalSpeed.Count)
                    {
                        TrainOptTemp.SuggestCarSpeed = TrainOptTemp.OptimalSpeed[i] / 100 ; // m/s
                        TrainOptTemp.MaxCarSpeed = TrainOptTemp.SuggestCarSpeed + 1.5; // m/s
                        TrainOptTemp.MinCarSpeed = TrainOptTemp.SuggestCarSpeed - 1.5; // m/s
                    }
                    else
                    {
                        TrainOptTemp.SuggestCarSpeed = 0;
                        TrainOptTemp.MaxCarSpeed = 0;
                        TrainOptTemp.MinCarSpeed = 0;
                    }

                }
                // 如果停车阶段转区间运行
                if (TrainOptTemp.LeaveFlag == 1 && TrainOptTemp.PlanformCodeList.Count > 0)
                {
                    int NextStationIndex = TrainOptTemp.PlanformCodeList.IndexOf(TrainOptTemp.NextStationCode);
                    // 更新运行计划指导数据
                    TrainOptTemp.ForwardStopStation = TrainOptTemp.NextStationCode.ToString();
                    TrainOptTemp.InTime = TrainOptTemp.InTimeList[NextStationIndex].TimeStr;
                    TrainOptTemp.OutTime = TrainOptTemp.OutTimeList[NextStationIndex].TimeStr;
                }

            }
        }


            // 更新列车运行信息
        public static Response725 SetTrainOperationInfo(Request725 request725)
        {
            // 同步全局时间
            GV.GlobalTime.TimeStr = request725.RequestTime;
            GV.GlobalTime.TimeStamp = GetTimeStamp(GV.GlobalTime.TimeStr);

            // 同步车辆数据
            List<int> CarCodeList = new List<int>();
            for (int i = 0; i < request725.CarStatusList.Count; i++)
            {
                CarCodeList.Add(int.Parse(request725.CarStatusList[i].CarCode));
            }
            DeleteTrainOperationInfo(CarCodeList);

            // 更新列车运行状态
            int CarCodeTemp = 0;
            int CarIndex = -1;
            for(int i=0;i<request725.CarStatusList.Count;i++)
            {
                CarCodeTemp = int.Parse(request725.CarStatusList[i].CarCode);
                CarIndex = GetTrainIndexOrCreate(CarCodeTemp);
                // 上周期状态更新
                GV.trainOperationInfo[CarIndex].CarSpeedLast = GV.trainOperationInfo[CarIndex].CarSpeed;
                // 本周期状态更新
                GV.trainOperationInfo[CarIndex].ShiftCode = request725.CarStatusList[i].ShiftCode;
                GV.trainOperationInfo[CarIndex].ShiftType = request725.CarStatusList[i].ShiftType;
                GV.trainOperationInfo[CarIndex].Direction = request725.CarStatusList[i].Direction;
                GV.trainOperationInfo[CarIndex].CarSpeed = request725.CarStatusList[i].CarSpeed;
                GV.trainOperationInfo[CarIndex].CurrentSection = request725.CarStatusList[i].CurrentSection;
                GV.trainOperationInfo[CarIndex].CurrentPosition = request725.CarStatusList[i].CurrentPosition;
                GV.trainOperationInfo[CarIndex].NextSection = request725.CarStatusList[i].NextSection;
                GV.trainOperationInfo[CarIndex].NextSectionStatus = request725.CarStatusList[i].NextSectionStatus;
                GV.trainOperationInfo[CarIndex].ForwardSignalDistance = request725.CarStatusList[i].ForwardSignalDistance;
                GV.trainOperationInfo[CarIndex].ForwardSignalStatus = request725.CarStatusList[i].ForwardSignalStatus;
                GV.trainOperationInfo[CarIndex].ForwardCrossDistance = request725.CarStatusList[i].ForwardCrossDistance;
                GV.trainOperationInfo[CarIndex].CrossCountdown = request725.CarStatusList[i].CrossCountdown;
                GV.trainOperationInfo[CarIndex].ForwardStation = request725.CarStatusList[i].ForwardStation;
                GV.trainOperationInfo[CarIndex].ForwardStationDistance = request725.CarStatusList[i].ForwardStationDistance;
                GV.trainOperationInfo[CarIndex].VehicleCommState = request725.CarStatusList[i].VehicleCommState;
                GV.trainOperationInfo[CarIndex].IsOperatingLine = request725.CarStatusList[i].IsOperatingLine;
            }

            // 状态自更新
            RefreshTrainOperationInfo();

            // 曲线优化触发检查
            CheckSpeedOptimal();

            // 更新指导速度
            RefreshRecommendSpeed();


            // 填充接口发送数据
            Response725 response725 = new Response725();
            for (int i = 0; i < GV.trainOperationInfo.Count; i++)
            {
                CarGuideData carGuideDataTemp = new CarGuideData();
                carGuideDataTemp.CarCode = GV.trainOperationInfo[i].CarCode.ToString();
                carGuideDataTemp.InTime = GV.trainOperationInfo[i].InTime;
                carGuideDataTemp.OutTime = GV.trainOperationInfo[i].OutTime;
                carGuideDataTemp.MaxCarSpeed = GV.trainOperationInfo[i].MaxCarSpeed;
                carGuideDataTemp.MinCarSpeed = GV.trainOperationInfo[i].MinCarSpeed;
                carGuideDataTemp.SuggestCarSpeed = GV.trainOperationInfo[i].SuggestCarSpeed;
                response725.CarGuideDataList.Add(carGuideDataTemp);
            }
            return response725;
        }



    }








}
