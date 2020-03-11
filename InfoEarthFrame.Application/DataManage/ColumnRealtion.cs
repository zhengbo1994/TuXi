using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Application
{
    public class ColumnRealtion
    {
        public string ColumnName { get; set; }
        public bool IsNumber { get; set; }
        public int Index { get; set; }

        public ColumnRealtion(string name, bool isNumber, int index)
        {
            ColumnName = name;
            IsNumber = isNumber;
            Index = index;
        }
    }
}
