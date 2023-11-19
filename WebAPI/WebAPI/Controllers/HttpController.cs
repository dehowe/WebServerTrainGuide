using Microsoft.AspNetCore.Mvc;
using WebAPI.Struct;
using Newtonsoft.Json;
using WebAPI.Function;

namespace WebAPI.Controllers
{
    [Route("")]
    [ApiController]
    public class HttpController : Controller
    {
        private static NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        [HttpGet]
        public string Request_1(string RequestID)
        {
            DateTime dateTime = DateTime.Now;
            return JsonConvert.SerializeObject(dateTime);
        }

        // 7.2.1当天计划时刻表下发接口
        [HttpPost("schedule/transmit")] 
        public string Request_721([FromBody] Request721 request721)
        {
            Log.Info("receive request721,plan length:{0}", request721.ShiftList.Count());
            GV.ShiftList = request721.ShiftList;
            //OperationGuide.FillTestTrain();   // 本地测试
            Response721 response721 = new Response721();
            response721.ExecutionStatus = 1;
            response721.Result = "null";
            return JsonConvert.SerializeObject(response721); 
        }

        // 7.2.2当天计划时刻表新增班次接口
        [HttpPost("schedule/shift/add")]
        public string Request_722([FromBody] Request722 request722)
        {
            Log.Info("receive request722");
            Response722 response722 = new Response722();
            return JsonConvert.SerializeObject(response722);
        }

        // 7.2.3当天计划时刻表删除班次接口
        [HttpPost("schedule/shift/delete")]
        public string Request_723([FromBody] Request723 request723)
        {
            Log.Info("receive request723");
            Response723 response723 = new Response723();
            return JsonConvert.SerializeObject(response723);
        }

        // 7.2.4当天计划时刻表修改班次接口
        [HttpPost("schedule/shift/modify")]
        public string Request_724([FromBody] Request724 request724)
        {
            Log.Info("receive request724");
            Response724 response724 = new Response724();
            return JsonConvert.SerializeObject(response724);
        }


        // 7.2.5车辆运行指导接口
        [HttpPost("guide/running")] 
        public string Request_725([FromBody] Request725 request725)
        {
            Log.Info("receive request725");
            Response725 response725 = OperationGuide.SetTrainOperationInfo(request725);
            return JsonConvert.SerializeObject(response725);
        }

        // 7.2.6车辆正线准入验证接口
        [HttpPost("guide/enter-access")]
        public string Request_726([FromBody] Request726 request726)
        {
            Log.Info("receive request726");
            Response726 response726 = AccessVerify.TrainEnterVerify(request726);
            return JsonConvert.SerializeObject(response726);
        }
    }
}
