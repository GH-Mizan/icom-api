using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icom.Pricelists.Dtos
{
    public class ProductQuantityInfoDto
    {
        public int AvailableQuantity { get; set; }
        public bool HasSerial { get; set; }
        public List<string> Serials { get; set; }
    }
}
