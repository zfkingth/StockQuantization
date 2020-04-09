using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stock.Data;
using Stock.JQData;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860


namespace Stock.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        private readonly StockContext _db;

        public ValuesController(
            ILogger<ValuesController> logger, StockContext db
          )
        {
            _logger = logger;
            _db = db;

        }

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<List<Stock.Model.Price>>> Get(string id)
        {
            var list = await (from i in _db.PriceSet
                              where i.Code == id && i.Unit == Model.UnitEnum.Unit1d
                              select i
                             ).AsNoTracking()
                           .ToListAsync();


            return Ok(list);
        }

        [HttpGet("GetStock")]
        public async Task<ActionResult<Stock.Model.Securities>> GetStock(string id)
        {
            var item = await (from i in _db.SecuritiesSet
                              where i.Code == id
                              select i
                             ).AsNoTracking()
                           .FirstOrDefaultAsync();


            return Ok(item);
        }



        [HttpGet("GetMargin")]
        public async Task<ActionResult<List<Stock.Model.Price>>> GetMargin()
        {
            //var item = await (from i in _db.MarginTotal
            //                  group new { i.Date, i.FinValue } by i.Date into gp
            //                  orderby gp.Key ascending
            //                  select gp
            //                 ).ToListAsync();
            var item = await (from i in _db.MarginTotal
                              select new { i.Date, i.FinValue }
            ).AsNoTracking().ToListAsync();

            return Ok(item);
        }


        [HttpGet("GetMarketDeal")]
        public async Task<ActionResult<List<Stock.Model.MarketDeal>>> GetMarketDeal()
        {
            //var item = await (from i in _db.MarginTotal
            //                  group new { i.Date, i.FinValue } by i.Date into gp
            //                  orderby gp.Key ascending
            //                  select gp
            //                 ).ToListAsync();
            var item = await (from i in _db.MarketDeal
                              where Constants.LinkIds.Contains(i.LinkId)
                              select new { Date = i.Day, i.LinkId, i.BuyAmount, i.SellAmount }
                        ).AsNoTracking().ToListAsync();


            return Ok(item);
        }



        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
