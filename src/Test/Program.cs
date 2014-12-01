using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSharp;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Window.Open("");
            Draw.FillBox(0, 0, 200, 200, System.Drawing.Color.Red);
            Draw.FillBox(25, 25, 200, 200, System.Drawing.Color.Green);
            Draw.FillBox(50, 50, 200, 200, System.Drawing.Color.Black);
            Draw.FillBox(75, 75, 200, 200, System.Drawing.Color.Blue);
            Draw.FillBox(100, 100, 200, 200, System.Drawing.Color.Yellow);
        }
    }
}
