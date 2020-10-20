using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedHouseSystem.Models
{
    public class LandType
    {
        public string Name;
        public int Id;
        public uint Price;
        public uint MaxTax, UpTaxPrice;
        public int AllowDay;
        public int TaxDurationForMinutes;
    }
}
