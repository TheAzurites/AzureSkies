using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSkies.DTO
{
    public class NewSMSDataDTO
    {
        public string messageId { get; set; }
        public string from { get; set; }
        public string message { get; set; }
        public string receivedTimestamp { get; set; }
    }
}
