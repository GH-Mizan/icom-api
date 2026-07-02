using System.ComponentModel;

namespace Icom.Enums
{
    public enum ClassSheetType
    {
        [Description("Microsoft Word")]
        MS_Word = 1,
        [Description("Microsoft Excel")]
        MS_Excel,
        [Description("Microsoft Power Point")]
        MS_PowerPoint,
        [Description("Microsoft Access")]
        MS_Access,
        [Description("Practice")]
        Practice,
        [Description("Joint Letters")]
        JointLetters,
        [Description("Suggestions")]
        Suggestions,
        [Description("Prev Questions")]
        PrevQuestions
    }
}
