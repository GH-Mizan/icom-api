using Icom.Enums;
using System;

namespace Icom.ClassSheets.Dto
{
    public class ClassSheetDistributionOutputDto
    {
        public int? Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public DateTime Date { get; set; }
        public ClassSheetType Type { get; set; }
        public string SheetName { get; set; }
        public bool Distributed { get; set; }
        public bool FromRecord { get; set; }
    }
}
