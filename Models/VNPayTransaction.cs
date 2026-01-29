using Symphony.Portal.Web.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class VNPayTransaction
    {
        public string Id { get; set; }

        public string PaymentId { get; set; }
        public Payment Payment { get; set; }

        public string VnpTxnRef { get; set; }
        public long VnpAmount { get; set; }
        public string VnpOrderInfo { get; set; }
        public string VnpCreateDate { get; set; }

        public string? VnpResponseCode { get; set; }
        public string? VnpTransactionNo { get; set; }
        public string? VnpBankCode { get; set; }
        public string? VnpPayDate { get; set; }

        public VNPayTransactionStatus Status { get; set; }
    }
}
