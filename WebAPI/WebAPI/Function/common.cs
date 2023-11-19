using WebAPI.Struct;
using WebAPI.SpeedPlan;
using System.Runtime.InteropServices;


namespace WebAPI.Function
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
        public static InterlockStatus InterlockInfo = new InterlockStatus();            // 全线联锁实时占用状态
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
        private static NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();
        [DllImport("libQuadProg.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern int test();
        public static void ProgramInit()  // 程序初始化
        {
            // 基础数据初始化
            DataInit();
            // 线路联锁占用初始化
            InterlockInit();  
            // 二次规划求解器测试 linux
            //int result = test();
            //Log.Info("QuadProg load sucess,test result:{0}", result);

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
            catch (Exception e)
            {
                result = 0; // 异常
                Log.Error("101 Base data init error");
                Console.WriteLine(e.Message);
            }
            Log.Info("101 data init sucess");
            return result;
        }

        // 线路联锁占用初始化
        public static void InterlockInit()  
        {
            // 联锁区段初始化
            GV.InterlockInfo.SectionOccupyList.Clear();
            for (int i = 0; i < GV.sectionConfig.Count; i++)
            {
                SectionOccupyStatus sectionOccupyTemp = new SectionOccupyStatus();
                sectionOccupyTemp.SectionID = GV.sectionConfig[i].SectionID;
                sectionOccupyTemp.SectionName = GV.sectionConfig[i].SectionName;
                sectionOccupyTemp.OccupyStatus = 0; // 占用状态空闲
                GV.InterlockInfo.SectionOccupyList.Add(sectionOccupyTemp);
            }

        }
    }








}
