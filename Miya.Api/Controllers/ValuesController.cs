using System.Collections.Generic;
using System.Linq;
using Miya.Core.Extensions;
using Miya.Api.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Wangkanai.Detection;

namespace Miya.Api.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IDeviceResolver _deviceResolver;
        public ValuesController(IDistributedCache distributedCache,
                                IDeviceResolver deviceResolver
                                )
        {
            _distributedCache = distributedCache;
            _deviceResolver = deviceResolver;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var headerList = HttpContext.Request.Headers.ToList();
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        //[HmacFilter]
        [ServiceFilter(typeof(HmacFilterAttribute))]
        [ServiceFilter(typeof(HmacTokenControllerAttribute))]
        public string Get(int id)
        {
            
            var user = HttpContext.Items["testUser"];
            var user2 = HttpContext.GetMiyaUser();
            var userName = user2.Email;
            var publicKey = HttpContext.GetPublicKey();
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
