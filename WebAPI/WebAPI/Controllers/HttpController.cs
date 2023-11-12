using Microsoft.AspNetCore.Mvc;
using WebAPI.Struct;
using Newtonsoft.Json;
using WebAPI.Common;



namespace WebAPI.Controllers
{
    [Route("web")]
    [ApiController]
    public class HttpController : Controller
    {
        [HttpGet]
        public string Request_1(string RequestID)
        {
            if (RequestID == "123456")
            {
                return "yes";
            }
            return "no";
        }

        // 7.2.1当天计划时刻表下发接口
        [HttpPost("721")] 
        public string Request_721([FromBody] Request721 request721)
        {
            GV.ShiftList = request721.ShiftList;
            Response721 response721 = new Response721();
            response721.ExecutionStatus = 1;
            response721.Result = "null";
            return JsonConvert.SerializeObject(response721); 
        }

        // 7.2.5车辆运行指导接口
        [HttpPost("725")] 
        public string Request_725([FromBody] Request725 request725)
        {
            Response725 response725 = Common.Common.SetTrainOperationInfo(request725);
            return JsonConvert.SerializeObject(response725);
        }

    }
}
