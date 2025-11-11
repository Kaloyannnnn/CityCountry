using System.ComponentModel.DataAnnotations;

namespace CityCountry.Models
{
    public class City
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The name of the city is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public int CountryId { get; set; }

        public virtual Country Country { get; set; }
    }
}