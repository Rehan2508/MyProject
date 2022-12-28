using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace CateringWebApplication.Models
{
    public class ProductSold
    {
        [DisplayName("Product Sold Id")]
        public int id { get; set; }

        [DisplayName("Sale Id")]
        [ForeignKey("sale")]
        public int sid { get; set; }
        public Sale sale { get; set; }

        [DisplayName("Product Id")]
        [ForeignKey("product")]
        public int pid { get; set; }
        public Product product { get; set; }

        [DisplayName("Quantity Sold")]
        public int quantity { get; set; }
    }
}
