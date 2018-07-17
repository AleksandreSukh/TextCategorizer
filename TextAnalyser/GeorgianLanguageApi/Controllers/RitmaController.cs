using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using GeorgianLanguageCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GeorgianLanguageApi.Controllers
{
    [Route("api/[controller]")]
    public class RitmaController : Controller
    {
        private readonly IWordDataRepository _wordDataRepository;
        //private readonly ApiConfig _config;
        public RitmaController(
            //IOptions<ApiConfig> config, 
            IWordDataRepository wordDataRepository)
        {
            _wordDataRepository = wordDataRepository;
            //_config = config.Value;
        }

        [HttpGet("{requestword}")]
        public IActionResult GetWords(string requestword)
        {
            if (!requestword.IsGeorgianWord())
                return BadRequest($"Word {requestword} is not a valid Georgian word");

            var topn = 20;//_config.EvalTopNSimiilarities;

            var rep = GetTopNRitmas(requestword,  topn);
            if (rep == null)
            {
                return NotFound();
            }
            return Ok(rep);
        }

        private string[] GetTopNRitmas(string target, int topn)
        {
            var words = _wordDataRepository.GetAllWords();
            var vss = new ConcurrentDictionary<string, double>();
            Parallel.ForEach(words, b => vss.GetOrAdd(b, s => GeoWordMatcher.EvaluateRhymeSimilarity(target, s, true)));
            return vss.OrderByDescending(w => w.Value).Take(topn).Select(w => w.Key).ToArray();
        }
    }
}