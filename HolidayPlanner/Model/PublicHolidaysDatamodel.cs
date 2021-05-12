using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HolidayPlanner.Model
{
    public class PublicHolidaysDatamodel
    {
        public string date { get; set; }
        public string localName { get; set; }
        public string name { get; set; }
        public string countryCode { get; set; }
        public bool @fixed { get; set; }
        public bool global { get; set; }
        public string type { get; set; }
    }
}
