using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beadando
{
    public static class Constants
    {
        //azért, hogy több Office osztály esetén ne kelljen fölöslegesen új konstansokat létrehozni
        //hanem mind ezt használja
        //(ebben a feladatban nem valószínű, hogy 1 Office-tól több is lesz, de lehetne)
        static TimeInterval korlat = new TimeInterval(8, 0);
       
        //a 8 órás korlátot jelzi
        public static TimeInterval Korlat
        {
            get { return korlat; }
            set { korlat = value; }
        }

        
        
    }
}
