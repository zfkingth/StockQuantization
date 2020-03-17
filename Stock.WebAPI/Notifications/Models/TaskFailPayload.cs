using System.Collections.Generic;
using Stock.Model;

namespace Stock.WebAPI.Notifications.Models
{
    public class TaskFailPayload
    {
        public string Message { get; set; }
        public List<TempPrice> ResultList { get; set; }
    }
}