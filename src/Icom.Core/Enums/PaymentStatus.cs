using System.ComponentModel;

namespace Icom.Enums
{
    public enum PaymentStatus
    {
        Paid = 1,
        [Description("P. Paid")]
        Partialpaid,
        Due
    }
}
