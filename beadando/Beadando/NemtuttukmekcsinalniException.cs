using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Beadando
{
   
    class NemtuttukmekcsinalniException : Exception
    {
        
        public List<Feladat> Exceptions
        {
            get;
            
        }
        public NemtuttukmekcsinalniException(List<Feladat> exceptions) :base("FONTOS FELADAT KIVÉTEL")
        {
            this.Exceptions = exceptions;
        }
       
    }
}
