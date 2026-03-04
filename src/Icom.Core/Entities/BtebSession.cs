using Abp.Domain.Entities.Auditing;
using System;

namespace Icom.Entities
{
    public class BtebSession : FullAuditedEntity
    {
        public string SessionName { get; set; }
        public string SessionPeriod { get; set; }
        public bool IsExaminationHeld { get; set; }
        public DateTime? ExaminationDate { get; set; }
        public bool IsResultPublished { get; set; }
        public DateTime? ResultPublishedDate { get; set; }
        public bool IsCertificateProvided { get; set; }
        public DateTime? CertificateDate { get; set; }
    }
}
