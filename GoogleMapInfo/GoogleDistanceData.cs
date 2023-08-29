using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleMapInfo
{
    public class GoogleDistanceData
    {
        public string[] destination_address { get; set; }
        public string[] origin_address { get;set; }
        public Row[] rows { get; set; }
        public string status { get; set; }
    }
}
