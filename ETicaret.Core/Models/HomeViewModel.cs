using ETicaret.Core.Entities;

namespace ETicaret.Core.Models
{
    public class HomeViewModel
    {
        public List<Slider> Sliders { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Brand> Brands { get; set; } = new();
        public List<Product> HomeProducts { get; set; } = new();
    }
}
