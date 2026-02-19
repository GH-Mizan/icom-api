using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Icom.Enums;
using System;

namespace Icom.Entities
{
    public class Client : FullAuditedEntity, IMustHaveTenant
    {
        public DateTime EntryDate { get; set; }
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string WhatsAppNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public ClientType Type { get; set; }
        public string Remarks { get; set; }
        public int TenantId { get; set; }
    }
}
