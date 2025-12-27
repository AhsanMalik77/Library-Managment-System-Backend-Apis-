using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinalMap.Models
{
    public class BorrowBookRequest
    {
        public string aridno { get; set; }
        public int isbn { get; set; }
        public DateTime returnd { get; set; }
    }
}