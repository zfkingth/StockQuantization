using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStock.WebAPI.ViewModels
{
    public class StockTaskStatus
    {

        public Guid Id { get; set; }


        public DateTime StartTime { get; set; }

    }
}
