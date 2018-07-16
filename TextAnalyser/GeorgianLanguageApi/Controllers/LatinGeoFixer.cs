using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAggregator;
using GeorgianLanguageUtils;
using Microsoft.AspNetCore.Mvc;

namespace GeorgianLanguageApi.Controllers
{
    [Route("api/[controller]")]
    public class LatinGeoFixer : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{input}")]
        public string FixLatinText(string input)
        {
            return GeorgianLanguageUtils.LatinGeoFixer.TextIsGeorgianWithLatinCharacters(input) ? GeorgianLanguageUtils.LatinGeoFixer.FixLatinCharacters(input) : input;
        }

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
