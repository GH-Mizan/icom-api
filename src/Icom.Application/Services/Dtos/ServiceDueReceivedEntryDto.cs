using Icom.Enums;
using Icom.Sales.Dtos;

namespace Icom.Services.Dtos
{
    public class ServiceDueReceivedEntryDto
    {
        public int ServiceId { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Due { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal PrevTotalPaid { get; set; }

        public ServiceDueReceivedHistoryDto DueReceived { get; set; }
    }
}
