using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CityCountry.Models
{
    public class Country
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The name of the country is required")]
        public string Name { get; set; }

        public virtual ICollection<City> Cities { get; set; } = new List<City>();
    }
}