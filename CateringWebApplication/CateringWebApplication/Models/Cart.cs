using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace CateringWebApplication.Models
{
    public class Cart
    {
        [DisplayName("Cart Id")]
        public int id { get; set; }

        [DisplayName("Purachased Quantity")]
        public int quantity { get; set; }

        [DisplayName("Product Id")]
        [ForeignKey("product")]
        public int pid { get; set; }
        public Product product { get; set; }

        [DisplayName("Total Amount")]
        public double totalAmount { get; set; }

        [DisplayName("User")]
        public string userId { get; set; }
    }
}
