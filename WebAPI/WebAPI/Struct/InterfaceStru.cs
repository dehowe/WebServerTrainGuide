namespace WebAPI.Struct
{

    //接口7.2.1 请求结构体定义
    public struct Request721
    {
        public string ScheduleID { get; set; }                    // 时刻表编号
        public List<Shift> ShiftList { get; set; }                // 全天班次数据集合
    }

    //接口7.2.1 回复结构体定义
    public struct Response721
    {
        public int ExecutionStatus { get; set; }                 // 执行结果
        public string Result { get; set; }                       // 失败原因
    }


    public class Request7255
    {
        public string RequestTime { get; set; }                      // 请求时间
        public List<CarStatus> CarStatusList { get; set; }           // 车辆相关状态集合
        public List<StationStatus> StationStatusList { get; set; }   // 站台占用状态集合
        public List<SectionStatus> SectionStatusList { get; set; }   // 区段占用状态集合

    }

    //接口7.2.5 请求结构体定义
    public class Request725
    {
        public string RequestTime { get; set; }                      // 请求时间
        public List<CarStatus> CarStatusList { get; set; }           // 车辆相关状态集合
        public List<StationStatus> StationStatusList { get; set; }   // 站台占用状态集合
        public List<SectionStatus> SectionStatusList { get; set; }   // 区段占用状态集合
    }

    //接口7.2.5 请求结构体定义
    public class Response725
    {
        public List<CarGuideData> CarGuideDataList = new List<CarGuideData>(); // 车辆指导数据集合
    }


    //接口7.2.1 内部嵌套结构体定义
    public struct Shift                             // 全天班次数据集合结构体
    {
        public string ShiftCode { get; set; }                     // 班次号
        public int Direction { get; set; }                        // 运行方向	Int	1：上行（逆时针）；2：下行（顺时针）
        public string StartTime { get; set; }                     // 班次计划开始时间
        public string EndTime { get; set; }                       // 班次计划结束时间
        public string CarCode { get; set; }                       // 车辆编号
        public List<ShiftDetail> ShiftDetailList { get; set; }    // 班次明细列表集合
    }

    public struct ShiftDetail                       // 班次明细列表集合结构体
    {
        public string PlatformCode { get; set; }                  // 站台编号
        public string InTime { get; set; }                        // 计划进站时间
        public string OutTime { get; set; }                       // 计划出站时间
    }

    //接口7.2.5 内部嵌套结构体定义
    public class CarStatus                         // 车辆相关状态集合结构体
    {
        public string CarCode { get; set; }                      // 车辆编号
        public string ShiftCode { get; set; }                    // 班次号
        public string ShiftType { get; set; }                    // 班次类型	String	1：常规运营班次；2：救援班次；3：VIP接待班次；4：调车班次
        public int Direction { get; set; }                       // 运行方向	Int	1：上行（逆时针）；2：下行（顺时针）
        public double CarSpeed { get; set; }                     // 车辆速度	Double	单位：米/秒
        public string CurrentSection { get; set; }               // 车辆当前区段编号
        public int CurrentPosition { get; set; }                 // 车辆当前公里标位置
        public string NextSection { get; set; }                  // 车辆拟下一区段编号
        public int NextSectionStatus { get; set; }               // 拟下一区段状态	Int	1：出清；2：占用；3：锁闭；4：封锁
        public int ForwardSignalDistance { get; set; }           // 前方信号机距离	Int	单位：米
        public int ForwardSignalStatus { get; set; }             // 前方信号机状态	Int	1：允许通行；2：禁止通行
        public int ForwardCrossDistance { get; set; }            // 前方路口距离	Int	单位：米
        public int ForwardCrossStatus { get; set; }              // 前方路口状态	Int	1：绿灯（允许通行）；2：红灯（禁止通行）
        public int CrossCountdown { get; set; }                  // 前方路口倒计时	Int	单位：秒
        public string ForwardStation { get; set; }               // 拟前方站台编号	String	
        public int ForwardStationDistance { get; set; }          // 前方站台距离	Int	单位：米
        public int VehicleCommState { get; set; }                // 车辆通讯状态	Int	0：正常；1：故障
        public int IsOperatingLine { get; set; }                 // 车辆是否在正线	Int	1：在场段；2：在正线（车辆在正线期间，进行行车指导，在场段不做处理）
    }

    public class StationStatus                     // 站台占用状态集合结构体
    {
        public string PlatformCode { get; set; }                 // 站台编号
        public int StationOccupyStatus { get; set; }             // 站台占用状态	Int	1：出清；2：占用
        public string StationOccupyCarCode { get; set; }         // 站台占用车辆编号
    }


    public class SectionStatus                     // 区段占用状态集合结构体
    {
        public string SectionCode { get; set; }                  // 区段编号
        public int SectionOccupyStatus { get; set; }             // 区段占用状态  1：出清；2：占用
        public string SectionOccupyCarCode { get; set; }         // 站台占用车辆编号
    }

    public struct CarGuideData                      // 车辆指导数据集合结构体
    {
        public string CarCode { get; set; }                      // 车辆编号
        public string ForwardStopStation { get; set; }           // 拟前方停靠站台编号
        public string InTime { get; set; }                       // 计划进站时间
        public string OutTime { get; set; }                      // 计划出站时间
        public double MaxCarSpeed { get; set; }                  // 车辆最大指导速度 单位：米/秒
        public double MinCarSpeed { get; set; }                  // 车辆最小指导速度 单位：米/秒
        public double SuggestCarSpeed { get; set; }              // 车辆最佳指导速度 单位：米/秒
    }




}