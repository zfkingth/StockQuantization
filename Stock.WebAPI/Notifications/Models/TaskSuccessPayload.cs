using MyStock.Model;
using System.Collections.Generic;

namespace MyStock.WebAPI.Notifications.Models
{
    public class TaskSuccessPayload
    {
        public string Message { get; set; }
        public List<RealTimeData> ResultList { get; set; }
    }
}