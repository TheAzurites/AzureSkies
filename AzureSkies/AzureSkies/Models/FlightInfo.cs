using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSkies.Models
{
    public class FlightInfo
    {
        [Required]
        public int Id { get; set; }

        //public IList<string> GuestPhoneNumber { get; set; }
    }
}
