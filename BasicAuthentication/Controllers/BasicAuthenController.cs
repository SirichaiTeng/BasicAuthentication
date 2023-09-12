using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BasicAuthentication.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BasicAuthenController : ControllerBase
    {
        IConfiguration _config;

        public BasicAuthenController(IConfiguration config)
        {
            _config = config;
        }


        [HttpGet]
        [Route("getSampleList")]
        public IActionResult getSampleList()
        {
            string name = "รายชื่อ";


            return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(name));
        }
    }
}
