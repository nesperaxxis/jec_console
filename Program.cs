using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAIDII_SAP_CONSOLE
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Class.clsStart cls = new Class.clsStart();
                Console.WriteLine("Getting Started!");
                cls.GetStarted();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Main Exception Error : " + ex.Message);
            }
        }
    }
}
