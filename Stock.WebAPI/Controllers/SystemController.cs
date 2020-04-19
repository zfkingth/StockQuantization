using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyStock.Data;
using MyStock.Model;
using MyStock.WebAPI.ViewModels.Fillers;

namespace MyStock.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class SystemController : ControllerBase
    {
        private readonly ILogger<SystemController> _logger;
        private readonly StockContext _db;

        public SystemController(
            ILogger<SystemController> logger, StockContext db
          )
        {
            _logger = logger;
            _db = db;

        }


        [HttpGet("users")]
        public async Task<ActionResult<List<User>>> GetUsersAsync()
        {
            var list = await (from i in _db.Users
                              select new
                              {
                                  i.Id,
                                  i.Email,
                                  i.RoleName,
                                  i.Username,
                                  i.ExpiredDate
                              })
                             .ToListAsync();


            return Ok(list);
        }



        [HttpGet("taskNum")]
        public ActionResult<int> GetTaskNum()
        {
            var num = BaseDoWorkViewModel.GetTaskNum();

            return Ok(num);
        }

        [HttpGet("statusTable")]
        public async Task<ActionResult<List<StockEvent>>> GetStatusTableAsync()
        {

            var list = await _db.StockEvents.ToListAsync();
            return Ok(list);
        }



        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchAsync(string id, [FromBody]User model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);


            var itemIndb = await _db.Users.FirstOrDefaultAsync(s => s.Id == model.Id);

            itemIndb.RoleName = model.RoleName;
            itemIndb.Username = model.Username;
            itemIndb.Email = model.Email;
            itemIndb.ExpiredDate = model.ExpiredDate;

            await _db.SaveChangesAsync();


            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            var userId = HttpContext.User.Identity.Name;
            if (userId == id) return BadRequest("用户不能删除自己");

            var itemIndb = await _db.Users.FirstOrDefaultAsync(s => s.Id == id);

            _db.Users.Remove(itemIndb);

            await _db.SaveChangesAsync();


            return NoContent();
        }


    }
}
