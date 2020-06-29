using System.Collections.Generic;
using MyStock.Model;

namespace MyStock.WebAPI.Notifications.Models
{
    public class TaskFailPayload
    {
        public string Message { get; set; }
        public object Result { get; set; }
    }
}