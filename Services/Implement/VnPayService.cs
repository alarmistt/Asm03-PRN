using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly string _tmnCode;
        private readonly string _hashSecret;
        private readonly string _vnpUrl;
        private readonly string _version;
        private readonly string _command;
        private readonly string _currCode;
        private readonly string _orderType;
        private readonly string _locale;
        private readonly string _returnUrl;

        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
            _tmnCode = _configuration["VnPayConfig:TmnCode"];
            _hashSecret = _configuration["VnPayConfig:HashSecret"];
            _vnpUrl = _configuration["VnPayConfig:VnpUrl"];
            _version = _configuration["VnPayConfig:Version"];
            _command = _configuration["VnPayConfig:Command"];
            _currCode = _configuration["VnPayConfig:Currency"];
            _orderType = _configuration["VnPayConfig:OrderType"];
            _locale = _configuration["VnPayConfig:Locale"];
            _returnUrl = _configuration["VnPayConfig:ReturnUrl"];
        }
        public string CreatePaymentUrl(HttpContext httpContext, int orderId, decimal total)
        {
            Console.WriteLine("Context: " + httpContext);
            Console.WriteLine("OrderId: " + orderId);
            Console.WriteLine("Total: " + total.ToString("G29"));
            var vnpParams = new Dictionary<string, string>
        {
            { "vnp_Version", _version },
            { "vnp_Command", _command },
            { "vnp_TmnCode", _tmnCode },
            { "vnp_CurrCode", _currCode },
            { "vnp_OrderType", _orderType },
            { "vnp_Amount", (total * 100L).ToString("G29") },
            { "vnp_OrderInfo", orderId.ToString() },
            { "vnp_Locale", _locale },
            { "vnp_ReturnUrl", _returnUrl },
            { "vnp_IpAddr", GetClientIp(httpContext) }
        };

            var now = DateTime.Now;
            vnpParams["vnp_CreateDate"] = now.ToString("yyyyMMddHHmmss");
            vnpParams["vnp_ExpireDate"] = now.AddMinutes(10).ToString("yyyyMMddHHmmss");
            vnpParams["vnp_TxnRef"] = GenerateTransactionCode();

            string hashData = BuildQueryString(vnpParams, false);
            string secureHash = HmacSHA512(_hashSecret, hashData);
            vnpParams["vnp_SecureHash"] = secureHash;

            return _vnpUrl + "?" + BuildQueryString(vnpParams, true);
        }
        private string GetClientIp(HttpContext httpContext)
        {
            string ip = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip) || ip.Equals("unknown", StringComparison.OrdinalIgnoreCase))
            {
                ip = httpContext.Connection.RemoteIpAddress?.ToString();
            }
            return !string.IsNullOrEmpty(ip) ? ip : "127.0.0.1";
        }

        private string GenerateTransactionCode()
        {
            var random = new Random();
            return DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + random.Next(0, 999).ToString("D3");
        }

        private static string BuildQueryString(Dictionary<string, string> paramsDict, bool encode)
        {
            var sortedParams = paramsDict
                .Where(p => !string.IsNullOrEmpty(p.Value))
                .OrderBy(p => p.Key)
                .Select(p => $"{(encode ? WebUtility.UrlEncode(p.Key) : p.Key)}={WebUtility.UrlEncode(p.Value)}");

            return string.Join("&", sortedParams);
        }

        private static string HmacSHA512(string key, string data)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
