using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace CateringWebApplication.Models
{
    public class Product
    {
        [DisplayName("Product Id")]
        public int id { get; set; }

        [DisplayName("Product Name")]
        public string name { get; set; }

        [DisplayName("Product Description")]
        public string description { get; set; }

        public string imagePath { get; set; }
        [NotMapped]
        public IFormFile formFile { get; set; }

        [DisplayName("Product Selling Price")]
        public double price { get; set; }

        [DisplayName("Current Discount")]
        public double discount { get; set; }

        [DisplayName("Available Quantity")]
        [NotMapped]
        public int quantity { get; set; }

        [ForeignKey("category")]
        [DisplayName("Category Id")]
        public int categoryId { get; set; }

        [DisplayName("Category")]
        public Category category { get; set; }

        [NotMapped]
        public bool check { get; set; } = false;
    }
}
