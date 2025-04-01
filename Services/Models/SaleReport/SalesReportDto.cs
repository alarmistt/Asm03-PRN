using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.SaleReport
{
    public class SalesReportDto
    {
        public string ProductName { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalSales { get; set; }
    }
}
