namespace WebAPI.Struct
{
    public class TrainOperationInfo        // 列车运行信息
    {
        public int CarCode;                       // 车辆编号
        // 输入数据
        public string ShiftCode;                     // 班次号
        public int ShiftType;                        // 班次类型		1：常规运营班次；2：救援班次；3：VIP接待班次；4：调车班次
        public int Direction;                        // 运行方向	Int	1：上行（逆时针）；2：下行（顺时针）
        public double CarSpeed;                      // 车辆速度	Double	单位：米/秒
        public double CarSpeedLast;                  // 上周期车辆速度	Double	单位：米/秒
        public string CurrentSection;                // 车辆当前区段编号
        public int CurrentPosition;                  // 车辆当前公里标位置
        public string NextSection;                   // 车辆拟下一区段编号
        public int NextSectionStatus;                // 拟下一区段状态	Int	1：出清；2：占用；3：锁闭；4：封锁
        public int ForwardSignalDistance;            // 前方信号机距离	Int	单位：米
        public int ForwardSignalStatus;              // 前方信号机状态	Int	1：允许通行；2：禁止通行
        public int ForwardCrossDistance;             // 前方路口距离	Int	单位：米
        public int ForwardCrossStatus;               // 前方路口状态	Int	1：绿灯（允许通行）；2：红灯（禁止通行）
        public int CrossCountdown;                   // 前方路口倒计时	Int	单位：秒
        public string ForwardStation;                // 拟前方站台编号	String	
        public int ForwardStationDistance;           // 前方站台距离	Int	单位：米
        public int VehicleCommState;                 // 车辆通讯状态	Int	0：正常；1：故障
        public int IsOperatingLine;                  // 车辆是否在正线	Int	1：在场段；2：在正线（车辆在正线期间，进行行车指导，在
        // 输出数据 
        public string ForwardStopStation;            // 拟前方停靠站台编号
        public string InTime;                        // 计划进站时间
        public string OutTime;                       // 计划出站时间
        public double MaxCarSpeed;                   // 车辆最大指导速度 单位：米/秒
        public double MinCarSpeed;                   // 车辆最小指导速度 单位：米/秒
        public double SuggestCarSpeed;               // 车辆最佳指导速度 单位：米/秒
        // 其他自更新
        public int DriveStageFlag;                   // 驾驶阶段标识 1：停车阶段 2：区间运行
        public int DriveStageLastFlag;               // 上周期驾驶阶段标识 1：停车阶段 2：区间运行
        public int LeaveFlag;                        // 指导速度有效标识 0：无效 1：有效
        public int OfflineSpeedOptFlag;              // 优化曲线标识 0：未优化 1：正在优化 2：已优化
        public int CurrentStationCode=65535;         // 当前站Code   
        public int NextStationCode=65535;            // 目的站Code
        public int IntervalBeginLoc;                 // 区间开始公里标
        public int IntervalCurrentDis;               // 区间当前位移 m
        public long ArriveTimeStamp;                 // 到站时间戳
        public long StopCutDowm = 65535;             // 停站倒计时 ms
        public float TargetOperationTime;            // 目标运行时分
        public UInt32 IntervalLength;                // 区间长度 cm
        public List<float> Gradient = new List<float>();       // 区间坡度 按照1m间隔的离散数据 千分之一 
        public UInt32[,] Limit;                                // 限速 限速转换点-限速值
        public List<int> OptimalLoc = new List<int>();         // 优化速度
        public List<int> OptimalSpeed = new List<int>();       // 优化速度
        public List<int> LevelFlag = new List<int>();          // 优化工况 
        public int SpeedOptStationCode = 0;                    // 优化曲线的目的站
        // 运行计划
        public Shift Shift = new Shift();
        public List<int> PlanformCodeList = new List<int>();
        public List<TimeAndStamp> InTimeList = new List<TimeAndStamp>();
        public List<TimeAndStamp> OutTimeList = new List<TimeAndStamp>();
        public List<int> StopTimeList = new List<int>();


    }

    public struct TimeAndStamp      // 时间和时间戳
    {
        public DateTime Time;
        public string TimeStr;
        public long TimeStamp;
    }

    public class TrainTest         // 测试列车结构体
    {
        public int CarCode;
        public int BeginLocation;
        public double RunDis;
        public int Speed;  // cm/s
        public int Dir;
    }


    public struct StationConfig             // 站台配置信息
    {
        public int StationID;                   // 站台ID
        public int StationCode;                 // 站台Code
        public string StationName;              // 站台名称
        public string StationType;              // 站台类型 站台-正线 站台-侧线 避让点-侧线 避让点-正线
        public string SectionID;                 // 所属区段ID
        public int StationLength;               // 站台长度
        public int StationLocDown;              // 站台下行起点公里标 m
        public int StationLocUp;                // 站台上行起点公里标 m
        public int StopPointLocDown;            // 下行停车点所在公里标 m
        public int StopPointLocUp;              // 上行停车点所在公里标 m
    }

    public struct TrainConfig               // 列车配置信息
    {
        public int TrainID;                     // 列车ID
        public int TrainCode;                   // 列车Code
        public string TrainName;                // 列车名称
        public int TrainLength;                 // 列车长度 m
        public int TractionAcc;                 // 牵引加速度 cm/s/s
        public int BrakeAcc;                    // 常用制动减速度 cm/s/s
    }

    public struct SectionConfig             // 联锁区段配置信息      
    {
        public int SectionID;                   // 区段ID
        public string SectionName;              // 区段名称
        public int SectionBeginLoc;             // 区段始端公里标 m
        public int SectionEndLoc;               // 区段末端公里标 m
    }

    public struct LimitConfig           // 线路限速区段信息
    {
        public int LimitID;                 // 线路限速区段信息
        public string LimitName;            // 交点号
        public int LimitBeginLoc;           // 始端公里标 m
        public int LimitEndLoc;             // 末端公里标 m
        public int LimitValue;              // 曲线限速 km/h
    }

    public class InterlockStatus        // 联锁状态
    {
        public List<SectionOccupyStatus> SectionOccupyList = new List<SectionOccupyStatus>();

    }

    public class SectionOccupyStatus          // 联锁区段占用信息
    {
        public int SectionID;                   // 区段ID
        public string SectionName;              // 区段名称
        public int OccupyStatus;                // 占用状态 0：空闲 1：计划占用 2：正在占用
    }



}