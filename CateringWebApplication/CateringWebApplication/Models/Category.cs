using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace CateringWebApplication.Models
{
    public class Category
    {
        [DisplayName("Category Id")]
        public int id { get; set; }

        [DisplayName("Category Name")]
        public string name { get; set; }

        [DisplayName("Category Description")]
        public string description { get; set; }

        public string imagePath { get; set; }

        [NotMapped]
        public IFormFile formFile { get; set; }
    }
}
