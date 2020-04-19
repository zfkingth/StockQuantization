
using MyStock.WebAPI.ViewModels;

namespace MyStock.WebAPI.Notifications.Models
{
    public class TaskStartPayload
    {
        public StockTaskStatus  Status{ get; set; }
    }
}