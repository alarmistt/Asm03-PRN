using Services.Models.SaleReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IOrderService
    {
        Task<List<SalesReportDto>> GetSalesReport(DateTime startDate, DateTime endDate);
        Task CheckAndUpdatePendingOrders();
    }
}
