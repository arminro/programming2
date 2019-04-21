using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Beadando
{
    
    enum Prioritas
    {
        Normal,Visszatero,Surgos
    }

    class Feladat : IComparable
    {
      

        public Feladat(int jutalom, TimeInterval idotartam, Prioritas prioritas)
        {
            Jutalom = jutalom;
            Idotartam = idotartam;
            Prioritas = prioritas;
            

        }
        public Feladat(string[] input, StreamReader sr)
        {
            int res;
            TimeInterval interval;
            //sajnos a VS 2017 előtti környezetek nem támogatják az alábbi szintaxist, csak a hosszű változat működik
            //Jutalom = (int.TryParse(input[0], out int res) ? res : throw new UserInputException(input[0], null)); //rosszul lett megadva az 1. int, ha hibát dob
            //Idotartam = (TimeInterval.TryParse(input[1], out TimeInterval interval) ? interval : throw new UserInputException(input[1], null)); //rosszul lett megadva a TI
            //Prioritas = Prioritas.Surgos;

            //kiszűrjük a szintaktikai hibákat
            if(int.TryParse(input[0], out res))
            {
                Jutalom = res;
            }
            else
            {
                throw new UserInputException(input[0], sr);
            }

            if(TimeInterval.TryParse(input[1], out interval) && interval.CompareTo(new TimeInterval(0, 0)) > 0)
            {
                Idotartam = interval;
            }
            else
            {
                throw new UserInputException(input[1], sr);
            }
            //az újonnan jött feladatok csak sürgősek lehetnek,
            //de fölösleges lenne csak emiatt új konstruktort írni a Feladatból leszármazó Sürgős Feladatnak
            if (this is SurgosFeladat)
            {
                Prioritas = Prioritas.Surgos; 
            }
            else
            {
                if(int.TryParse(input[2], out res) && res >= 0 && res < 3)
                {
                    this.Prioritas = (Prioritas)res;
                }
                else
                {
                    throw new UserInputException(input[2], sr);
                }
            }
        }

       
        public int Jutalom
        {
            get;
            
        }
        
        public TimeInterval Idotartam
        {
            get;
            set;
            
        }
        public Prioritas Prioritas
        {
            get;
            set;
        }

        public int CompareTo(object obj)
        {
            //prioritási sorrend: sürgős > visszatérő > normál
            //mivel mindig csak 2 feladatot hasonlítunk össze, elég ha jól adjuk meg az enumot és akkor kialakul ez a sorrend
            
            //azért működik, mert az enum igazából egy int
            //föl kellett cserélni a zárójeleket, mert itt minél nagyobb egy elem annál előrébb akarjuk látni (csökkenőbe rendezzük növekvő helyett)
            if (this.Prioritas < (obj as Feladat).Prioritas)
                return 1;
            else if (this.Prioritas > (obj as Feladat).Prioritas)
            {
                
                return -1;
            }
            else
            {
                //ha a felhasználó fordított sorrendben adja meg az újonnan érkező feladatokat, akkor sorrendbe kell állítani azokat
                //ezért a fő CompareTo()-n belül van egy csak az újonnan jöttekre vonatkozó rész
                if (this is SurgosFeladat && obj is SurgosFeladat)
                {
                    if ((this as SurgosFeladat).TimeOfStart.CompareTo((obj as SurgosFeladat).TimeOfStart) < 0)
                        return -1;
                    else if ((this as SurgosFeladat).TimeOfStart.CompareTo((obj as SurgosFeladat).TimeOfStart) > 0)
                        return 1;
                    return 0;
                }
            }   
            return 0;
        }

        //felülírjuk a Feladat osztály ToString() metódusát, hogy egyszerűbben írhassuk ki az adatokat
        public override string ToString()
        {
            return string.Format("Nyereség: {0}, Időtartam: {1}, Prioritás: {2}", Jutalom, Idotartam.ToString(), Prioritas);
        }

        
    }
}
