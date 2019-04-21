using System;

namespace Beadando 
{
    public class TimeInterval : IComparable
    {
        public TimeInterval(int hour, int minute)
        {
            this.hour = hour;
            this.minute = minute;
        }

        int hour;
        int minute;
        public TimeInterval(int minutes)
        {
            this.hour = minutes / 60;
            this.minute = minutes % 60;
        }

        
        public int Hour
        {
            get
            {
                return hour;
            }
            set
            {
                hour = value;
            }
        }

        public int Minute
        {
            get
            {
                return minute;
            }
            set
            {
                minute = value;
            }
        }

        //arra számítunk, hogy másodpercre pontos FELADAT nem lesz
        /*Ahhoz, hogy össze tudjunk hasonlítani két TI objektumot, át kell váltani mind a kettőt tisztán percbe*/
        public int ConvertToMinutes()
        {
            return hour * 60 + minute;
        }
        public int CompareTo(object obj)
        {
            if (this.ConvertToMinutes() > (obj as TimeInterval).ConvertToMinutes())
                return 1;
            else if (this.ConvertToMinutes() < (obj as TimeInterval).ConvertToMinutes())
                return -1;
            return 0;
        }
        //túlterheljük a + és a - operátorokat, hogy egyszerűbb legyen számításokat végezni a TI objektumokkal
        //ezek az összeadott időket percekben számolják (tehát pl. 1:00 + 1:00 = 120 lesz)
        public static int operator +(TimeInterval t1, TimeInterval t2)
        {
            return t1.ConvertToMinutes() + t2.ConvertToMinutes();
        }


        public static int operator -(TimeInterval t1, TimeInterval t2)
        {
            return t1.ConvertToMinutes() - t2.ConvertToMinutes();
        }

        
        /*időformátumot adunk neki: ha az óra vagy a perc kisebb, mint 10
         akkor hozzácsapunk az elejére egy 0-át*/
        
        public override string ToString()
        {
            string hourString = hour < 10 ? "0" + hour : hour.ToString();
            string minuteString = minute < 10 ? "0" + minute : minute.ToString();
            return string.Format("{0}:{1}", hourString, minuteString);
        }

        
        /// <summary>
        /// Hibás ha: 1) nincs benne ':', 2) a splittelt string[]-ben nincs legalább 2 elem, 3) ha nem lehet int-é kasztolni a 2 elemet
        /// </summary>
        /// <param name="s">Egyszerű string</param>
        /// <param name="t">Egy TI instance mely az OUT miatt inicializálatlan</param>
        /// <returns></returns>
        public static bool TryParse(string s, out TimeInterval t)
        {
            t = null;
            int tempHour;
            int tempMin;
            if (!s.Contains(":"))
            {
                return false;
            }
            string[] split = s.Split(':');
            if (split.Length < 2)
                return false;
            if(int.TryParse(split[0], out tempHour) && int.TryParse(split[1], out tempMin))
            {
                //ha az óra és a perc is megvan és int-é lehet őket konvertálni, akkor beállítjuk a t-t
                t = new TimeInterval(tempHour, tempMin);
                return true;
            }
            //ha a felső nem teljesül, akkor hamissal térünk vissza
            return false;   
                

        }
        //ezzel a 2 metódussal egyszerűen kiváltható a += és  a -= operátor
        //az eredeti TI értéke növekszik
        public void AddObjectValue(TimeInterval t)
        {
            int tempSumMinutes = this.ConvertToMinutes() + t.ConvertToMinutes();
            //egészosztással adjuk hozzá az órát, mod-dal a percet
            this.hour = tempSumMinutes / 60;
            this.minute = tempSumMinutes % 60;
        }

        public void SubtractObjectValue(TimeInterval t)
        {
            int tempSumMinutes = this.ConvertToMinutes() - t.ConvertToMinutes();
            //egészosztással adjuk hozzá az órát, mod-dal a percet
            this.hour = tempSumMinutes / 60;
            this.minute = tempSumMinutes % 60;
        }
        
    }

    
   
}
