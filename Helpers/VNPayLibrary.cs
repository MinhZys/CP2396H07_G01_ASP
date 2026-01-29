using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Symphony.Portal.Web.Helpers
{
    public class VNPayLibrary
    {
        private SortedList<string, string> _requestData =
            new SortedList<string, string>(StringComparer.Ordinal);

        private SortedList<string, string> _responseData =
            new SortedList<string, string>(StringComparer.Ordinal);

        // ================= REQUEST =================
        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public string CreateRequestUrl(string baseUrl, string hashSecret)
        {
            var data = new StringBuilder();

            foreach (var kv in _requestData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(
                        WebUtility.UrlEncode(kv.Key) + "=" +
                        WebUtility.UrlEncode(kv.Value) + "&"
                    );
                }
            }

            var queryString = data.ToString();

            if (queryString.EndsWith("&"))
            {
                queryString = queryString.Remove(queryString.Length - 1);
            }

            var secureHash = HmacSHA512(hashSecret, queryString);

            return $"{baseUrl}?{queryString}&vnp_SecureHash={secureHash}";
        }

        // ================= RESPONSE =================
        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        private string GetResponseData()
        {
            if (_responseData.ContainsKey("vnp_SecureHash"))
                _responseData.Remove("vnp_SecureHash");

            if (_responseData.ContainsKey("vnp_SecureHashType"))
                _responseData.Remove("vnp_SecureHashType");

            var data = new StringBuilder();

            foreach (var kv in _responseData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(
                        WebUtility.UrlEncode(kv.Key) + "=" +
                        WebUtility.UrlEncode(kv.Value) + "&"
                    );
                }
            }

            if (data.Length > 0)
                data.Remove(data.Length - 1, 1);

            return data.ToString();
        }

        public bool ValidateSignature(string inputHash, string secretKey)
        {
            var rawData = GetResponseData();
            var myHash = HmacSHA512(secretKey, rawData);
            return myHash.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private static string HmacSHA512(string key, string data)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
