using Icom.Enums;
using System;

namespace Icom.Clients.Dtos
{
    public class ClientOutputDto
    {
        public int Id { get; set; }
        public DateTime EntryDate { get; set; }
        public string Name { get; set; }
        public string IdentificationName { get; set; }
        public string ContactNumber { get; set; }
        public string WhatsAppNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public ClientType Type { get; set; }
        public string TypeText { get; set; }
        public string Remarks { get; set; }
        public int TenantId { get; set; }
    }
}
