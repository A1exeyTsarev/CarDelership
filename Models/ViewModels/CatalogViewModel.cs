using Azure;
using CarDelership.Models;

namespace CarDelership.Models.ViewModels
{
    public class CatalogViewModel
    {
        public List<Cars> Cars { get; set; } = new List<Cars>();

        // Поиск и фильтрация
        public string SearchTerm { get; set; } = "";
        public string SortBy { get; set; } = "name_asc";
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public string SelectedBrand { get; set; } = "";
        public int? SelectedYear { get; set; }

        // Списки для фильтров
        public List<string> Brands { get; set; } = new List<string>();
        public List<int> Years { get; set; } = new List<int>();

        // Теги
        public List<Tags> Tags { get; set; } = new List<Tags>();
        public List<int> SelectedTagIds { get; set; } = new List<int>();

        // Пагинация
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 12;
        public int TotalCount { get; set; }
    }
}