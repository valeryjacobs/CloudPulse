using MSExpo.Editor.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialRX
{
    class Program
    {
        static void Main(string[] args)
        {
            var x =new  SerialHelper();

            x.Listen();

            Console.ReadLine();
        }
    }
}
