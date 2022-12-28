using System.ComponentModel.DataAnnotations.Schema;

namespace CateringWebApplication.Models
{
    [NotMapped]
    public class JoinModel
    {
        public int pid { get; set; }
        public int quantity { get; set; }
        public DateTime date { get; set; }
    }
}
