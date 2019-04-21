using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beadando
{
    class UjSurgosFeladatException  : Exception
    {
        public List<SurgosFeladat> Exceptions
        {
            get;

        }
        public UjSurgosFeladatException(List<SurgosFeladat> exceptions) :base("ÚJ SÜRGŐS FELADAT KIVÉTEL")
        {
            this.Exceptions = exceptions;
        }
    }
}
