using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Beadando
{
    class UserInputException : Exception 
    {
        string problematicPart;
        public UserInputException(string problematicPart, StreamReader sr) : base("Az Ön által megadott input nem megfelelő: " + problematicPart + "\nKérjük, ellenőrizze, hogy az input a megfelelő formátumban van-e: PÉNZÖSSZEG;HH:MM (ami nem lehet 0:00);INT(0-2)")
        {
            this.problematicPart = problematicPart;
            //ha az adatokat a txt-ből olvastuk be, bezárjuk a file-t, mivel a kivétel eldobásakor
            //ez a főprogramban nem történne meg
            if (sr != null)
            {
                sr.Close();
            }        
}
    }
}
