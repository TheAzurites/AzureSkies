using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSkies.Models
{
    public class FlightInfo
    {
        public int id { get; set; }
        [Required]
        public Data Data { get; set; }

        //public IList<string> GuestPhoneNumber { get; set; }
    }
}
