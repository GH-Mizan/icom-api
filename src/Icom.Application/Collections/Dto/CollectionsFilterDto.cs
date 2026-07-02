using Icom.Common;
using Icom.Enums;
using System;

namespace Icom.Collections.Dto
{
    public class CollectionsFilterDto : FilterBaseDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public CollectionType? Type { get; set; }
    }
}
