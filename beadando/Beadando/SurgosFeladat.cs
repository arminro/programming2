using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beadando
{
    class SurgosFeladat : Feladat
    {
        public SurgosFeladat(string[] input, string timeOfStart) : base(input, null)
        {
            TimeInterval interval;
            if(TimeInterval.TryParse(timeOfStart, out interval))
            {
                this.TimeOfStart = interval;
            }
            else
            {
                //mivel ebben az esetben nem file-ból olvassuk be az adatot, nem kell bezármi semmit, így null-t adunk át az exception-nek hiba esetén
                throw new UserInputException(input[1], null);
            }
        }

        public TimeInterval TimeOfStart
        {
            get;
            set;
        }

        public override string ToString()
        {
            //ismét felülírjuk, hogy könnyebb legyen megkülönböztetni a txt-ben megadott feladatoktól
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            return base.ToString() + "\nmely a következő időpontban érkezett: " + TimeOfStart.ToString(); 
        }

        
    }
}
