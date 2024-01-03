using Microsoft.AspNetCore.Mvc;

namespace RemoteSensingApp.Server.Controllers
{
    [ApiController]
    public class ServerController : ControllerBase
    {
        [HttpGet]
        [Route("temperature")]
        public string GetTemperatures()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("humidity")]
        public string GetHumidities()
        {
            throw new NotImplementedException();
        }
    }
}