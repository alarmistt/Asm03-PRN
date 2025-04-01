using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext httpContext, int orderId, decimal total);
    }
}
