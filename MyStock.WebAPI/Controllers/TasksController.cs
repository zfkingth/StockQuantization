﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using BackgroundTasksSample.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MyStock.Data;
using MyStock.Model;

namespace MyStock.WebAPI.Controllers
{



    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class TasksController : ControllerBase
    {
        BackServiceUtil _util;
        StockContext _db;
        readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        TimedService _timedService;

        public TasksController(BackServiceUtil util, StockContext db, TimedService timedService,
        Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _util = util;
            _db = db;
            _configuration = configuration;
            _timedService = timedService;

        }


        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "task count: " };
        }

        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        public class ArgAriseSystemEvent
        {

            public string EventName { get; set; }
        }


        [HttpPost("ariseSystemEvent")]
        public async Task<ActionResult> ariseSystemEventAsync([FromBody] ArgAriseSystemEvent model)
        {
            switch (model.EventName)
            {
                case SystemEvents.CalcLimitNum: _util.EnqueCalcLimitNum(); break;
                case SystemEvents.PullStockIndex1d: _util.EnquePullDayDataTask(); break;
                case SystemEvents.PullStockF10: _util.EnquePullF10Task(); break;
                case SystemEvents.PullMarginData: _util.EnquePullMarginData(); break;
                case SystemEvents.PullMarketDealData: _util.EnquePullMarketDealData(); break;
                case SystemEvents.PullRealTime:
                    return await handlePullRealtime();

                case SystemEvents.PullAllStockNames:
                    _util.EnquePullAllStockNamesTask();
                    break;
                case SystemEvents.PullIndex30m: _util.EnquePullIndex30mData(); break;
                case SystemEvents.PullHuShenTongInTradeTime: _util.EnquepullHuShenTongTask(); break;
                case SystemEvents.CalcRealTimeLimitNum: _util.EnqueCalcRealTimeLimitNum(); break;
            }
            return NoContent();
        }

        private async Task<ActionResult> handlePullRealtime()
        {
            var item = await _db.StockEvents.FirstOrDefaultAsync(s => s.EventName == SystemEvents.PullRealTime);

            if (item.LastAriseEndDate == null)
            {
                _util.EnquepullRealTimeDataTask();
                return NoContent();
            }

            if (item.Status == EventStatusEnum.Idle)
            {

                var interval = DateTime.Now.Subtract(item.LastAriseEndDate.Value).TotalSeconds;

                int seconds = _configuration.GetValue<int>("MinimumIntervalForRealTime");

                if (interval > seconds)
                {
                    _timedService.ResetTimerForPullRealTimeData();
                    _util.EnquepullRealTimeDataTask();
                    return NoContent();
                }
                else
                {
                    string mes = $"两次获取实时数据的时间太近，小于{seconds}秒，请稍后尝试。";
                    return BadRequest(mes);
                }
            }

            else
            {
                string mes = $"后台任务正在运行，不能授受此命令。";
                return BadRequest(mes);
            }
        }

        [HttpPost("clearDate")]
        public async Task<ActionResult> clearDateAsync([FromBody] string eventName)
        {
            var item = await _db.StockEvents.FirstOrDefaultAsync(s => string.Equals(s.EventName, eventName, StringComparison.OrdinalIgnoreCase));

            if (item == null)
            {
                return BadRequest($"找不到事件：{eventName}");
            }
            else
            {
                item.Status = EventStatusEnum.Idle;
                await _db.SaveChangesAsync();
                return NoContent();
            }



        }

        [HttpPost("EraseRealTimeData")]
        public ActionResult EraseRealTimeDataAsync()
        {

            _util.EnqueEraseRealTimeDataTask();
            return NoContent();
        }



        [HttpPost]
        public void Post([FromBody] string value)
        {


        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
