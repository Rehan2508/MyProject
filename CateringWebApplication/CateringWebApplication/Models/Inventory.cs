using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace CateringWebApplication.Models
{
    public class Inventory
    {
        [DisplayName("Inventory Id")]
        public int id { get; set; }

        [DisplayName("Stock Available")]
        public int quantity { get; set; }

        [DisplayName("Product Id")]
        [ForeignKey("product")]
        public int pid { get; set; }
        public Product product { get; set; }

        [DisplayName("Category Id")]
        [ForeignKey("category")]
        public int cid { get; set; }
        public Category category { get; set; }
    }
}
