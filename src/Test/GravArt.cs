using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSharp;

namespace Test
{
    class GravArt :TuringDotNet
    {
        static void Instructions()
        {
            setscreen("text,screen,noecho,nocursor");
            locate(1, 30);
            put("GRAVITY BOREALIS");
            put(skip);
            color(2);
            put("In Plunox VI the universe is a box bounded by negative gravity,");
            put("so you can never go near the edge.");
            put(skip);
            put("But the two planets of Plunox VI have ordinary gravity, of the");
            put("kind officially approved by Mr. Newton.");
            put(skip);
            put("Gravity Borealis, a visual phenomenal, shows the tug of the");
            put("various gravity fields on your space craft.  Specifically, the");
            put("blue line indicates the gravitational pull from the blue planet,");
            put("the purple line the pull from the purple planet and the white");
            put("lines the push from the walls.");
            put(skip);
            put("Set the initial velocity of your space craft using the arrow");
            put("keys, then use the space bar to launch yourself and observe the...");
            colour(14);
            putPart(skip, "                     ");

            // Put multi - colour string out
            var str = "Gravity Borealis";
            for (int i = 1; i <= 16; i++) {
                colour(i % 5 + 11);
                putPart(str[i]);
            }

            // Wait for a keystroke
            Input.Pause();

            cls();
        }
        static void Run()
        {

        }
    }
}
