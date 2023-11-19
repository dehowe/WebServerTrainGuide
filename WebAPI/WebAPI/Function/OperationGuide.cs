using WebAPI.Struct;
using WebAPI.SpeedPlan;

namespace WebAPI.Function
{

    /* 此类主要封装行车速度指导逻辑方法 */

    public static class OperationGuide
    {
        private static NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        // 计算运行计划的离线曲线
        public static void CalAllSpeedOptOffline()
        {
            int[] PlanFormUpList = new int[] { 81, 84, 85, 88, 89, 92, 93, 96, 97, 99, 101, 104, 105, 82 };
            int[] PlanFormDownList = new int[] { 82, 105, 104, 102, 100, 97, 95, 93, 91, 89, 87, 85, 84, 81 };
            List<int> ParkPointUpList = new List<int>();
            List<int> ParkPointDownList = new List<int>();

            for (int i = 0; i < PlanFormUpList.Length; i++)
            {
                ParkPointUpList.Add(DataQuery.GetLocByPlanformAndDir(PlanFormUpList[i], 1));
            }
            for (int i = 0; i < PlanFormDownList.Length; i++)
            {
                ParkPointDownList.Add(DataQuery.GetLocByPlanformAndDir(PlanFormDownList[i], 2));
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
                SpeedOptParameter parameters = new SpeedOptParameter { speed = 0, targetTime = (ushort)TrainTemp.TargetOperationTime, intervalLength = TrainTemp.IntervalLength, gradient = TrainTemp.Gradient, limit = TrainTemp.Limit, CarCode = 0 };
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
                if (TrainOptTemp.IsOperatingLine == 2)
                {
                    // 更新驾驶阶段,连续两周期速度为0，默认进入停车阶段,否则为区间运行
                    TrainOptTemp.DriveStageLastFlag = TrainOptTemp.DriveStageFlag;
                    TrainOptTemp.DriveStageFlag = (TrainOptTemp.CarSpeed == 0 && TrainOptTemp.CarSpeedLast == 0) ? 1 : 2;


                    if (TrainOptTemp.DriveStageFlag == 1)
                    {
                        // 更新当前站和下一站
                        DataQuery.GetPlanformCodeByLoc(ref TrainOptTemp);
                    }

                    // 如果区间运行转停车阶段，且运行计划有效
                    if (TrainOptTemp.DriveStageFlag == 1 && TrainOptTemp.DriveStageLastFlag == 2 && TrainOptTemp.PlanformCodeList.Count > 0)
                    {
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
                        if (TrainOptTemp.CurrentStationCode != 65535 && TrainOptTemp.NextStationCode != 65535)
                        {
                            // 停站时间更新,找到当前站对应的索引
                            TrainOptTemp.ArriveTimeStamp = GV.GlobalTime.TimeStamp;
                            int StationIndex = TrainOptTemp.PlanformCodeList.IndexOf(TrainOptTemp.CurrentStationCode);
                            // 期望停站时间=出站时间-到站时间
                            TrainOptTemp.StopCutDowm = (TrainOptTemp.OutTimeList[StationIndex].TimeStamp - TrainOptTemp.InTimeList[StationIndex].TimeStamp);
                            if (TrainOptTemp.StopCutDowm < 1000 * 10)
                            {
                                Console.WriteLine("ERROR_104:stop time error");
                                Log.Error("ERROR_104:stop time error");
                            }


                        }
                        Console.WriteLine("arrive station,plan refresh,current:{0},next:{1},stoptime:{2}", TrainOptTemp.CurrentStationCode, TrainOptTemp.NextStationCode, TrainOptTemp.StopCutDowm / 1000);
                        Log.Info("arrive station,plan refresh,current:{0},next:{1},stoptime:{2}", TrainOptTemp.CurrentStationCode, TrainOptTemp.NextStationCode, TrainOptTemp.StopCutDowm / 1000);

                    }

                    // 如果停车阶段
                    if (TrainOptTemp.DriveStageFlag == 1)
                    {
                        if (TrainOptTemp.StopCutDowm != 65535)
                        {
                            // 停站倒计时检查
                            if (GV.GlobalTime.TimeStamp > TrainOptTemp.ArriveTimeStamp + TrainOptTemp.StopCutDowm)
                            {
                                TrainOptTemp.LeaveFlag = 1; // 倒计时结束，可以发车
                                Console.WriteLine("stop cutdown over,allow leave");
                                Log.Info("stop cutdown over,allow leave");
                            }
                            else
                            {

                                Console.WriteLine("current station:{0},next station:{1},stop cutdown {2}", TrainOptTemp.CurrentStationCode, TrainOptTemp.NextStationCode, (TrainOptTemp.ArriveTimeStamp + TrainOptTemp.StopCutDowm - GV.GlobalTime.TimeStamp) / 1000);
                                Log.Info("current station:{0},next station:{1},stop cutdown {2}", TrainOptTemp.CurrentStationCode, TrainOptTemp.NextStationCode, (TrainOptTemp.ArriveTimeStamp + TrainOptTemp.StopCutDowm - GV.GlobalTime.TimeStamp) / 1000);
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
            int IntervalLengthTemp = DataQuery.GetDisFromPlanform(TrainOptInfo.NextStationCode, TrainOptInfo.CurrentPosition, TrainOptInfo.Direction);
            if (IntervalLengthTemp > 0)
            {
                TrainOptInfo.IntervalLength = (UInt32)IntervalLengthTemp * 100;
            }
            else
            {
                TrainOptInfo.IntervalLength = 0;
                Console.WriteLine("ERROR_102:IntervalLength cal error,result:{0}", IntervalLengthTemp);
                Log.Error("ERROR_102:IntervalLength cal error,result:{0}", IntervalLengthTemp);
                return 0;
            }
            TrainOptInfo.Gradient.Clear();

            int LocTemp = 0;
            int LimitTemp = 0;
            List<int> SpeedLimitList = new List<int>();
            List<int> LimitChangeLoc = new List<int>();     // 限速转换点相对区间起点位置
            List<int> LimitChangeVal = new List<int>();     // 限速转换点对应限速值

            for (int i = 0; i < IntervalLengthTemp; i++)
            {
                // 2.添加区间坡度
                TrainOptInfo.Gradient.Add(0); // todo 添加坡度数据

                // 3.计算区间限速
                if (TrainOptInfo.Direction == 2) // 下行,公里标递增
                {
                    LocTemp = TrainOptInfo.CurrentPosition + i;
                    if (LocTemp > 4225)
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
                    Log.Error("ERROR_103:Direction error");
                    return 0;
                }

                LimitTemp = DataQuery.GetSpeedLimitByLoc(LocTemp);

                // 找到限速转换点
                if (i > 0)
                {
                    if (LimitTemp != SpeedLimitList[i - 1])
                    {
                        LimitChangeLoc.Add(i * 100);
                        LimitChangeVal.Add(SpeedLimitList[i - 1]);
                    }
                }
                // 添加最后终点
                if (i == IntervalLengthTemp - 1)
                {
                    LimitChangeLoc.Add(IntervalLengthTemp * 100);
                    LimitChangeVal.Add(SpeedLimitList[i - 1]);
                }

                SpeedLimitList.Add(LimitTemp);
            }

            if (LimitChangeLoc.Count < 1)
            {
                Console.WriteLine("ERROR_104:Limit error");
                Log.Error("ERROR_104:Limit error");
                return 0;
            }

            UInt32[,] LimitChangeArray = new UInt32[2, LimitChangeLoc.Count];

            for (int j = 0; j < LimitChangeLoc.Count; j++)
            {
                LimitChangeArray[0, j] = (UInt32)LimitChangeLoc[j];
                LimitChangeArray[1, j] = (UInt32)LimitChangeVal[j];
            }
            TrainOptInfo.Limit = LimitChangeArray;


            // 4. 计算期望运行时分
            // 找到目的站对应的索引
            int StationIndex = TrainOptInfo.PlanformCodeList.IndexOf(TrainOptInfo.NextStationCode);
            // 期望运行时分=目的站到站时间-当前站出发时间
            if (TrainOptInfo.InTimeList.Count > 0)
            {
                TrainOptInfo.TargetOperationTime = (TrainOptInfo.InTimeList[StationIndex].TimeStamp - TrainOptInfo.OutTimeList[StationIndex - 1].TimeStamp) / 1000;
                if (TrainOptInfo.TargetOperationTime < 10)
                {
                    Console.WriteLine("ERROR_104:operation time error");
                    Log.Error("ERROR_104:operation time error");
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
                    int result = CalSpeedOptData(ref TrainOptTemp);

                    // 数据处理正常，新建线程进行曲线优化
                    if (result == 1)
                    {
                        TrainOptTemp.OfflineSpeedOptFlag = 1; // 正在进行曲线优化
                        SpeedOptParameter parameters = new SpeedOptParameter { speed = 0, targetTime = (ushort)(TrainOptTemp.TargetOperationTime), intervalLength = TrainOptTemp.IntervalLength, gradient = TrainOptTemp.Gradient, limit = TrainOptTemp.Limit, CarCode = TrainOptTemp.CarCode };
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
                    TrainOptTemp.IntervalCurrentDis = DataQuery.GetDisBetweenBeginAndEnd(TrainOptTemp.IntervalBeginLoc, TrainOptTemp.CurrentPosition, TrainOptTemp.Direction);
                    if (TrainOptTemp.IntervalCurrentDis >= 0 && TrainOptTemp.IntervalCurrentDis < TrainOptTemp.OptimalSpeed.Count - 1)
                    {
                        TrainOptTemp.SuggestCarSpeed = (double)TrainOptTemp.OptimalSpeed[TrainOptTemp.IntervalCurrentDis + 1] / 100; // m/s
                        TrainOptTemp.MaxCarSpeed = TrainOptTemp.SuggestCarSpeed + 1.5; // m/s
                        TrainOptTemp.MinCarSpeed = TrainOptTemp.SuggestCarSpeed - 1.5; // m/s
                    }
                    else
                    {
                        TrainOptTemp.SuggestCarSpeed = 0;
                        TrainOptTemp.MaxCarSpeed = 0;
                        TrainOptTemp.MinCarSpeed = 0;
                    }
                    if (TrainOptTemp.MinCarSpeed < 0)
                    {
                        TrainOptTemp.MinCarSpeed = 0;
                    }
                    Log.Info("{0}-{1},loc:{2},end_dis:{3},sug_spd:{4}", TrainOptTemp.CurrentStationCode, TrainOptTemp.NextStationCode, TrainOptTemp.CurrentPosition, TrainOptTemp.OptimalSpeed.Count - TrainOptTemp.IntervalCurrentDis, TrainOptTemp.SuggestCarSpeed);
                    Console.WriteLine("{0}-{1},loc:{2},end_dis:{3},sug_spd:{4}", TrainOptTemp.CurrentStationCode, TrainOptTemp.NextStationCode, TrainOptTemp.CurrentPosition, TrainOptTemp.OptimalSpeed.Count - TrainOptTemp.IntervalCurrentDis, TrainOptTemp.SuggestCarSpeed);

                }
                // 如果停车阶段转区间运行
                if (TrainOptTemp.LeaveFlag == 1 && TrainOptTemp.PlanformCodeList.Count > 0 && TrainOptTemp.NextStationCode != 65535)
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
            GV.GlobalTime.Time = request725.RequestTime;
            GV.GlobalTime.TimeStr = DataQuery.GetTimeStrByDataTime(GV.GlobalTime.Time);
            GV.GlobalTime.TimeStamp = DataQuery.GetTimeStamp(GV.GlobalTime.TimeStr);

            // 同步车辆数据
            List<int> CarCodeList = new List<int>();
            for (int i = 0; i < request725.CarStatusList.Count; i++)
            {
                CarCodeList.Add(int.Parse(request725.CarStatusList[i].CarCode));
            }
            DataQuery.DeleteTrainOperationInfo(CarCodeList);

            // 更新列车运行状态
            int CarCodeTemp = 0;
            int CarIndex = -1;
            for (int i = 0; i < request725.CarStatusList.Count; i++)
            {
                CarCodeTemp = int.Parse(request725.CarStatusList[i].CarCode);
                CarIndex = DataQuery.GetTrainIndexOrCreate(CarCodeTemp);
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
                carGuideDataTemp.InTime = DataQuery.GetDataTimeByTimeStr(GV.trainOperationInfo[i].InTime);
                carGuideDataTemp.OutTime = DataQuery.GetDataTimeByTimeStr(GV.trainOperationInfo[i].OutTime);
                carGuideDataTemp.MaxCarSpeed = GV.trainOperationInfo[i].MaxCarSpeed;
                carGuideDataTemp.MinCarSpeed = GV.trainOperationInfo[i].MinCarSpeed;
                carGuideDataTemp.SuggestCarSpeed = GV.trainOperationInfo[i].SuggestCarSpeed;
                response725.CarGuideDataList.Add(carGuideDataTemp);
            }
            return response725;
        }

        // 测试列车信息更新
        public static void FillTestTrain()
        {
            Response725 response725 = new Response725();
            CarGuideData carGuideData = new CarGuideData();
            response725.CarGuideDataList.Add(carGuideData);
            Request725 request725 = new Request725();
            List<CarStatus> CarStatusList = new List<CarStatus>();
            string StartTime = "2023-11-11 08:00:10.000";
            long StartTimeStamp = DataQuery.GetTimeStamp(StartTime);

            List<TrainTest> TrainTestList = new List<TrainTest>();
            TrainTest trainTestTemp = new TrainTest();
            trainTestTemp.CarCode = 1401;
            trainTestTemp.Dir = 1; // 上行
            trainTestTemp.BeginLocation = 2023;  // 上行初始公里标
            trainTestTemp.Speed = 0;        // 初始速度 cm/s
            TrainTestList.Add(trainTestTemp);

            CarStatus carStatus = new CarStatus();
            carStatus.CarCode = trainTestTemp.CarCode.ToString();
            carStatus.Direction = trainTestTemp.Dir;
            carStatus.CurrentPosition = (int)trainTestTemp.BeginLocation;
            carStatus.CarSpeed = (double)trainTestTemp.Speed / 100;
            carStatus.IsOperatingLine = 2;
            CarStatusList.Add(carStatus);
            request725.CarStatusList = CarStatusList;

            request725.RequestTime = DataQuery.GetDataTimeByTimeStr(DataQuery.GetTimeStr(StartTimeStamp)); // 时间更新
            request725.CarStatusList[0].CarSpeed = 1;
            response725 = SetTrainOperationInfo(request725);

            // 测试循环
            for (int i = 0; i < 10000; i++)
            {
                request725.RequestTime = DataQuery.GetDataTimeByTimeStr(DataQuery.GetTimeStr(StartTimeStamp + i * 200)); // 时间更新
                request725.CarStatusList[0].CarSpeed = response725.CarGuideDataList[0].SuggestCarSpeed;
                TrainTestList[0].RunDis += request725.CarStatusList[0].CarSpeed * 0.2;
                request725.CarStatusList[0].CurrentPosition = DataQuery.GetNewLoc(TrainTestList[0].BeginLocation, (int)TrainTestList[0].RunDis, TrainTestList[0].Dir);
                response725 = SetTrainOperationInfo(request725);
                Thread.Sleep(100); //200ms
            }


        }


    }
}
