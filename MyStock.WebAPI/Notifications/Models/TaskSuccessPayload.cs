using MyStock.Model;
using System.Collections.Generic;

namespace MyStock.WebAPI.Notifications.Models
{
    public class TaskSuccessPayload
    {
        public string Message { get; set; }
        public object Result { get; set; } //�󲿷�ΪList<RealTimeData> 
    }
}