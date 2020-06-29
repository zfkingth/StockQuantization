using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyStock.WebAPI.ViewModels;
using MyStock.WebAPI.ViewModels.Searcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyStock.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SearchController : ControllerBase
    {

        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        protected readonly IConfiguration _configuration;
        public SearchController(
            ILogger<SearchController> logger,
             IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
        }


        /// <summary>
        /// 向上跳空
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("UpwardGap")]
        public ActionResult UpwardGap([FromBody] ArgUpwardGap model)
        {
            var userId = HttpContext.User.Identity.Name;
            var flag = HttpContext.User.IsInRole("admin");
            var searcher = new UpwardGapSearcher(_serviceScopeFactory, userId, _configuration,
                _logger, model);
            //开启新的线程来执行任务
            Task.Run(async () => await searcher.Search());
            return NoContent();
        }

        /// <summary>
        /// 平台突破
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("ColseBreak")]
        public ActionResult ColseBreak([FromBody] ArgCloseBreak model)
        {
            var userId = HttpContext.User.Identity.Name;
            var searcher = new CloseBreakSearcher(_serviceScopeFactory, userId, _configuration,
                _logger, model);
            //开启新的线程来执行任务
            Task.Run(async () => await searcher.Search());
            return NoContent();
        }

        /// <summary>
        /// 平台接近
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("CloseApproach")]
        public ActionResult CloseApproach([FromBody] ArgApproach model)
        {
            var userId = HttpContext.User.Identity.Name;
            var searcher = new CloseApproachSearcher(_serviceScopeFactory, userId, _configuration,
                _logger, model);
            //开启新的线程来执行任务
            Task.Run(async () => await searcher.Search());
            return NoContent();
        }


        /// <summary>
        /// 均线突破
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost("UpMA")]
        public ActionResult UpMA([FromBody] ArgUpMA model)
        {
            var userId = HttpContext.User.Identity.Name;
            var searcher = new UpMASearcher(_serviceScopeFactory, userId, _configuration,
                _logger, model);
            //开启新的线程来执行任务
            Task.Run(async () => await searcher.Search());
            return NoContent();
        }

        /// <summary>
        /// 均线突破
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost("UpMATwice")]
        public ActionResult UpMATwice([FromBody] ArgUpMATwice model)
        {
            var userId = HttpContext.User.Identity.Name;
            var searcher = new UpMATwiceSearcher(_serviceScopeFactory, userId, _configuration,
                _logger, model);
            //开启新的线程来执行任务
            Task.Run(async () => await searcher.Search());
            return NoContent();
        }


        [HttpPost("STAArise")]
        public ActionResult STAArise([FromBody] ArgSTA model)
        {
            var userId = HttpContext.User.Identity.Name;
            var searcher = new STAAriseSearcher(_serviceScopeFactory, userId, _configuration,
                _logger, model);
            //开启新的线程来执行任务
            Task.Run(async () => await searcher.Search());
            return NoContent();
        }



        /// <summary>
        /// macd指标diff>0,dea>0
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("UpMACD")]
        public ActionResult UpMACD([FromBody] ArgUpMACD model)
        {
            var userId = HttpContext.User.Identity.Name;
            var searcher = new UpMACDSearcher(_serviceScopeFactory, userId, _configuration,
                _logger, model);
            //开启新的线程来执行任务
            Task.Run(async () => await searcher.Search());
            return NoContent();
        }


        /// <summary>
        /// 流通市场筛选
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost("CirculatedMarket")]
        public ActionResult CirculatedMarket([FromBody] ArgCirculatedMarket model)
        {
            var userId = HttpContext.User.Identity.Name;
            var searcher = new CirculatedMarketSearcher(_serviceScopeFactory, userId, _configuration,
                _logger, model);
            //开启新的线程来执行任务
            Task.Run(async () => await searcher.Search());
            return NoContent();
        }

        /// <summary>
        /// 换手率筛选
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost("TurnOverRate")]
        public ActionResult TurnOverRate([FromBody] ArgTurnOverRate model)
        {
            var userId = HttpContext.User.Identity.Name;
            var searcher = new TurnOverRateSearcher(_serviceScopeFactory, userId, _configuration,
                _logger, model);
            //开启新的线程来执行任务
            Task.Run(async () => await searcher.Search());
            return NoContent();
        }


        /// <summary>
        /// 限定涨跌幅
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost("ExceptZhangFu")]
        public ActionResult ExceptZhangFu([FromBody] ArgExceptZhangFu model)
        {
            var userId = HttpContext.User.Identity.Name;
            var searcher = new ExceptZhangFuSearcher(_serviceScopeFactory, userId, _configuration,
                _logger, model);
            //开启新的线程来执行任务
            Task.Run(async () => await searcher.Search());
            return NoContent();
        }


        /// <summary>
        /// 连续上涨
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost("NRiseOpen")]
        public ActionResult NRiseOpen([FromBody] ArgNRiseOpen model)
        {
            var userId = HttpContext.User.Identity.Name;
            var searcher = new NRiseOpenSearcher(_serviceScopeFactory, userId, _configuration,
                _logger, model);
            //开启新的线程来执行任务
            Task.Run(async () => await searcher.Search());
            return NoContent();
        }

        /// <summary>
        /// 连续缩量
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost("VolumeDecrease")]
        public ActionResult VolumeDecrease([FromBody] ArgVolumeDecrease model)
        {
            var userId = HttpContext.User.Identity.Name;
            var searcher = new VolumeDecreaseSearcher(_serviceScopeFactory, userId, _configuration,
                _logger, model);
            //开启新的线程来执行任务
            Task.Run(async () => await searcher.Search());
            return NoContent();
        }


        /// <summary>
        /// 成交放量
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost("VolumeBreak")]
        public ActionResult VolumeBreak([FromBody] ArgVolumeBreak model)
        {
            var userId = HttpContext.User.Identity.Name;
            var searcher = new VolumeBreakSearcher(_serviceScopeFactory, userId, _configuration,
                _logger, model);
            //开启新的线程来执行任务
            Task.Run(async () => await searcher.Search());
            return NoContent();
        }


        // GET: api/Search
        [HttpGet]
        public IActionResult Get()
        {
            var arg = new ArgUpMA();
            arg.StockIdList = new List<string>() { "aaaa", "bbbb" };

            return Ok(arg);
        }

        // GET: api/Search/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Search
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Search/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

}
