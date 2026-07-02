using Icom.Enums;
using System;

namespace Icom.Collections.Dto
{
    public class SalesAndServiceCollectionDto
    {
        public DateTime Date { get; set; }
        public CollectionType Type { get; set; }
        public decimal Amount { get; set; }
    }
}
