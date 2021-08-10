using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSkies.Models
{
    public class Data
    {
        public int id { get; set; }
        [Required]
        public string flight_date { get; set; }
        [Required]
        public string flight_status { get; set; }
        [Required]
        public Flight Flight { get; set; }
        public Departure Departure { get; set; }
    }
}
