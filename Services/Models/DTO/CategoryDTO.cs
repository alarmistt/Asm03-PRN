using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.DTO
{
    public class CategoryDTO
    {
        [Required, StringLength(40)]
        public string CategoryName { get; set; }

        public string Description { get; set; }

    }
}
