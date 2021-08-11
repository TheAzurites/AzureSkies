using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSkies.Models
{
        public class SMSRoot
        {
            public Data data { get; set; }
        }
        public class Data
        {
            public string From { get; set; }
            public string Message { get; set; }
            public DateTime ReceivedTimestamp { get; set; }
            public string validationCode { get; set; }
            public string validationUrl { get; set; }
        }

}
