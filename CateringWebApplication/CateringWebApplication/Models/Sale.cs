using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace CateringWebApplication.Models
{
    public class Sale
    {
        [DisplayName("Sale Id")]
        public int id { get; set; }

        [DisplayName("Date of Sale")]
        public DateTime date { get; set; }

        [DisplayName("Total Price")]
        public double totalPrice { get; set; }

        [DisplayName("User")]
        public string userId { get; set; }
    }
}
