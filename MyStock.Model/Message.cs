using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Linq;


namespace MyStock.Model
{
    public partial class Message
    {
        public Message()
        {

        }


        [Key]
        public DateTime MesTime { get; set; }

        [MaxLength(2048)]
        [Required]
        public string Text { get; set; }


    }
}
