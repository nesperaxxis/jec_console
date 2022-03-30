using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAIDII_SAP_CONSOLE.Models
{
    public class EDGERESULT
    {
        public string dateTime { get; set; }
        public string bandwidth { get; set; }
        public string dataTransferred { get; set; }
    }
    public class SESSION
    {
        public string sessionToken { get; set; }
        public string svcGroupName { get; set; }
        public string svcGroupIdentifier { get; set; }
    }  

}
