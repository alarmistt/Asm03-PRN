using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.DTO
{
    public class ProductDTO
    {
        [Key]
        public int ProductId { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [Required, StringLength(40)]
        public string ProductName { get; set; }

        [Required, StringLength(20)]
        public string Weight { get; set; }

        [Column(TypeName = "money"), Required, Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int UnitsInStock { get; set; }

        public Category Category { get; set; }
    }
}
