using CarDelership.Models;

namespace CarDelership.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Cars> PopularCars { get; set; } = new List<Cars>();
        public int CartCount { get; set; }
    }
}