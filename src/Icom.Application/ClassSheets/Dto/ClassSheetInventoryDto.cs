using Icom.Enums;

namespace Icom.ClassSheets.Dto
{
    public class ClassSheetInventoryDto
    {
        public int Id { get; set; }
        public ClassSheetType Type { get; set; }
        public string SheetName { get; set; }
        public int Quantity { get; set; }
    }
}
