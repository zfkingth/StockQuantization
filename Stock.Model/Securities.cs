using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Stock.Model
{
    public class Securities
    {
        //code,display_name,name,start_date,end_date,type


        public SecuritiesEnum Type { get; set; }
        [MaxLength(15)]
        public string Code { get; set; }

        [MaxLength(20)]
        public string Displayname { get; set; }

        [MaxLength(10)]
        public string Name { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }




    }
}
