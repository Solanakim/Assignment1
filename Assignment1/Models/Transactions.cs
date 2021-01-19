using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomePage.Models
{
    public class Transactions
    {
        public List<Options> options { get; set; }

        public List<Statement> statements { get; set; }
    }
}