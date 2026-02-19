using Icom.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Icom.Services.Dtos
{
    public class ServiceDueReceivedHistoryDto
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int ClientId { get; set; }
        public DateTime ServiceDate { get; set; }
        public DateTime ReceiveDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal GrandTotal { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalPaid { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Due { get; set; }
        public bool Default { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
