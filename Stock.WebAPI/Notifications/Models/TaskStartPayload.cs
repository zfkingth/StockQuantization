
using Stock.WebAPI.ViewModels;

namespace Stock.WebAPI.Notifications.Models
{
    public class TaskStartPayload
    {
        public StockTaskStatus  Status{ get; set; }
    }
}