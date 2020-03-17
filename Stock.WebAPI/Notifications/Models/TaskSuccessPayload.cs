using Stock.Model;
using System.Collections.Generic;

namespace Stock.WebAPI.Notifications.Models
{
    public class TaskSuccessPayload
    {
        public string Message { get; set; }
        public List<TempPrice> ResultList { get; set; }
    }
}