using Icom.Enums;
using System;

namespace Icom.ClassSheets.Dto
{
    public class ClassSheetDistributionInputDto
    {
        public int? Id { get; set; }
        public int StudentId { get; set; }
        public DateTime Date { get; set; }
        public ClassSheetType Type { get; set; }
        public bool Distributed { get; set; }
    }
}
