﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyStock.Data;
using MyStock.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860


namespace MyStock.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        private readonly StockContext _db;

        //默认数据年度
        private const int _defalutYears = 1;

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


        DateTime parstTicks(long ticks)
        {
            DateTime startDate = new DateTime(ticks);
            if (startDate == default)
            {
                startDate = DateTime.Now.AddYears(-_defalutYears);
            }

            return startDate;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<List<DayData>>> Get(string id, [FromQuery] long start)
        {
            DateTime startDate = parstTicks(start);
            var list = await (from i in _db.DayDataSet
                              where i.StockId == id
                              && i.Date >= startDate
                              orderby i.Date ascending
                              select new
                              {
                                  i.Date,
                                  i.Open,
                                  i.High,
                                  i.Low,
                                  i.Close,
                                  i.ZhangDieFu,
                                  i.Amount
                              }
                             ).AsNoTracking()
                           .ToListAsync();

            //最新的一个实时数据
            var realItem = await (
                    from i in _db.RealTimeDataSet
                    where i.StockId == id
                    orderby i.Date descending
                    select i
                ).AsNoTracking().FirstOrDefaultAsync();

            if (realItem != null)
            {
                var date = realItem.Date;
                var dayTime = new DateTime(date.Year, date.Month, date.Day);
                if (list.Any(s => s.Date == dayTime) == false)
                {
                    //历史数据中没有这个数据
                    realItem.Date = dayTime;
                    var item = new
                    {
                        realItem.Date,
                        realItem.Open,
                        realItem.High,
                        realItem.Low,
                        realItem.Close,
                        realItem.ZhangDieFu,
                        realItem.Amount
                    };
                    //在最后添加
                    list.Add(item);
                }
            }

            return Ok(list);
        }

        [HttpGet("GetStock")]
        public async Task<ActionResult<Stock>> GetStock(string id)
        {
            var item = await (from i in _db.StockSet
                              where i.StockId == id
                              select i
                             ).AsNoTracking()
                           .FirstOrDefaultAsync();


            return Ok(item);
        }



        [HttpGet("GetMargin")]
        public async Task<ActionResult<List<Stock>>> GetMargin([FromQuery] long start)
        {

            DateTime startDate = parstTicks(start);

            var item = await (from i in _db.MarginTotal
                              where i.Date >= startDate
                              orderby i.Date ascending
                              select new { i.Date, i.FinValue }
                 ).AsNoTracking().ToListAsync();

            return Ok(item);
        }


        [HttpGet("GetMarketDeal")]
        public async Task<ActionResult<List<MarketDeal>>> GetMarketDeal([FromQuery] long start)

        {

            DateTime startDate = parstTicks(start);
            var item = await (from i in _db.MarketDeal
                              where i.Date >= startDate
                              orderby i.Date ascending
                              select new { i.Date, i.DRZJLR }
               ).AsNoTracking().ToListAsync();


            return Ok(item);
        }


        [HttpGet("GetStaPrice")]
        public async Task<ActionResult<List<StaPrice>>> GetStaPrice([FromQuery] long start)
        {

            DateTime startDate = parstTicks(start);

            var query = from i in _db.StaPrice
                        where i.Date >= startDate
                        orderby i.Date ascending
                        select i;

            var rt = await query.AsNoTracking().ToListAsync();


            return Ok(rt);
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
