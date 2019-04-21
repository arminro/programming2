using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beadando
{
    delegate void FeladatElkeszult(Feladat f);
    class Office
    {
        
        public Office(string fileName, int hour, int minute)
        {
            napiFeladatok = new List<Feladat>();
            megoldas = new List<Feladat>();
            opt = new List<Feladat>();
           
            StartOfWorkDay = new TimeInterval(hour, minute);

            //erre az osztályra minden esetben szükség lehet, ha új feladatokkal jön meg a főnök
            com = new Communicator(fileName);
            com.LoadFile(this);
            //beállítjuk a sürgős és a visszatérő feladatok egységértékét a prioritás szerint egyel alattuk lévő feldatok számának függvényében
            SetBaseValues();

        }

        TimeInterval StartOfWorkDay
        {
            get;
        }

        public event FeladatElkeszult Ready;
       

        int surgosValue;
        int visszateroValue;

        Communicator com;
        List<Feladat> napiFeladatok;
        public List<Feladat> NapiFeladatok
        {
            get
            {
                return napiFeladatok;
            }
            set
            {
                napiFeladatok = value;
            }
        }

        List<Feladat> opt;
        public List<Feladat> Opt
        {
            get
            {
                return opt;
            }
            set
            {
                opt = value;
            }
        }

        //az éppen aktuális megoldások halmazát sosem akarjuk majd elérni kívülről, mert csak az optimálisra vagyunk kíváncsiak
        List<Feladat> megoldas;

        public void SetBaseValues()
        {
            //egy visszatérő feladat többet ér, mint az összes normál egyszerre
            //mivel 1 normál 1 pontot ér, így ennek többnek kell lennie, mint az összes Normál összege
            //legyen 100-al több, mert az egy szép kerek szám (de 1-el is több lehetne)
            visszateroValue += FindAllElementsOfPriority(Prioritas.Normal, napiFeladatok) + 100;

            //1 Surgos feladat többet ér, mint az összes napi, így neki is számítunk egy értéket
            //mivel 1 visszatérő értéke nem 1, így az összest meg kell szorozni 1 értékével, amit az előbb számítottunk ki
            surgosValue += (FindAllElementsOfPriority(Prioritas.Visszatero, napiFeladatok) * visszateroValue) + 100;


        }






        public void UjFeladatotHozzaad(Feladat f, List<Feladat> feladatok)
        {
            feladatok.Add(f);
        }


        /// <summary>
        /// Visszaadja annak a megoldásnak a referenciáját, amelyik Jósága nagyobb
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="arrayPotential"></param>
        /// <returns></returns>
        public bool Josag(List<Feladat> arrayPotential, List<Feladat> arrayCurrent)
        {
            #region Alapötlet 
            /*Minél több Sürgős feladat és azon belül minél több napi feladat van benne 
             és azokon belül minél nagyobb az össz Jutalom, annál nagyobb a megoldás Jósága. 
             Ezt úgy érjük el, hogy minden egyes Sürgős feladat után a megoldás n pontot kap
             (ahol n értéke nagyobb, mint az összes hierarchikusan alatta lévő napi szintű feladat összértéke),
             a Napi feladatok után k pontot (ahol k az összes Normál prioritású feladatnál nagyobb értéket képvisel), a normálokért viszont csak 1-et
             Tehát a főcél az, hogy S-ből és a N-ból is minél több legyen és
             csak ezen belül nézzük, hogy minél többet kapjunk érte.
             A SetBaseValues() felelős azért, hogy 1 naponta visszatérő elem biztosan magasabb pontot érjen el,
             mint az összes normál együtt; valamint hogy 1 Sürgős eleme is többet érjen az összes előforduló Visszatérőnél.*/
            #endregion

            int valueCurrent = FindAllElementsOfPriority(Prioritas.Surgos, arrayCurrent) * surgosValue;
            valueCurrent += FindAllElementsOfPriority(Prioritas.Visszatero, arrayCurrent) * visszateroValue;
            valueCurrent += FindAllElementsOfPriority(Prioritas.Normal, arrayCurrent);

            int valuePotential = FindAllElementsOfPriority(Prioritas.Surgos, arrayPotential) * surgosValue;
            valuePotential += FindAllElementsOfPriority(Prioritas.Visszatero, arrayPotential) * visszateroValue;
            valuePotential += FindAllElementsOfPriority(Prioritas.Normal, arrayPotential);
            //ha a 2 megoldás nem ugyan annyi pontot ér...
            if (valueCurrent != valuePotential)
            {
                //...akkor pontozásos alapon döntünk
                if (valuePotential > valueCurrent)
                    return true;
                return false;
            }
            else
            {
                //ha ugyan annyit ér (azaz ugyan olyan arányban van bennük sürgős és visszatérő)
                //akkor jönnek szóba az értékek
                foreach (Feladat f in arrayCurrent)
                {
                    valueCurrent += f.Jutalom;
                }
                foreach (Feladat f in arrayPotential)
                {
                    valuePotential += f.Jutalom;
                }
            }

            if (valuePotential > valueCurrent)
                return true;
            return false;

        }
       
        /// <summary>
        /// Összeadja a megoldottnak tekintett feladatok időtartamát és megvizsgálja, hogy belefér-e a 8 órás munkaidőbe az új elem
        /// </summary>
        /// <param name="szint">Ahányadik helyre keressük a megfelelő elemet</param>
        /// <returns></returns>
        public bool Fk(int szint, Feladat f, TimeInterval kuszob)
        {
            //Mivel mindig ugyan azzal a megoldás tömbbel számolunk, nem kell paraméterként megadni
            //ha még nincs megoldás, akkor bármit betehetünk
            if (megoldas.Count == 0)
            {
                return true;
            }



            TimeInterval SZUM = new TimeInterval(0, 0);
            SZUM.AddObjectValue(f.Idotartam);
            int i = 0;
            //ha egy korábbi szinten már választottuk ezt, akkor nem rakhatjuk bele
            if (megoldas.Contains(f))
                return false;
            //az aktuális megoldás elemszámáig megyünk feltéve, hogy a az összeg <= 8 óránál
            while (i < megoldas.Count && (SZUM.CompareTo(kuszob) == -1 || SZUM.CompareTo(kuszob) == 0))
            {
                //alternatív += a SZUM-hoz
                SZUM.AddObjectValue(megoldas[i].Idotartam);
                i++;
            }

            //visszatérésként megvizsgáljuk, hogy a szum értéke <=-e a korláttal,
            //azaz a mostani elemet hozzáadva beleférünk-e a 8 órába
            return (SZUM.CompareTo(kuszob) == -1 || SZUM.CompareTo(kuszob) == 0);
        }



        public int FindAllElementsOfPriority(Prioritas p, List<Feladat> array)
        {
            int count = 0;
            foreach (Feladat f in array)
            {
                if (f.Prioritas == p)
                {
                    count++;
                }
            }
            return count;



        }

        

        /// <summary>
        /// Előállítja a megoldatlan feladatok listáját úgy, hogy az optimális elemeket eltávolítja az eredeti listából
        /// </summary>
        public void Differencia()
        {
            for (int i = 0; i < opt.Count; i++)
            {
                /*Egy LINQ lekérdezéssel eltávolítjuk az összes olyan feladatot, 
                 ami benne van az opt dinamikus tömbben.
                 Természetesn a feladat megoldható lenne úgy is, hogy megkeressük az összes olyan elemet, 
                 amely előfordul az opt-ban, majd kikeresve, hogy ezek hol találhatóak az eredeti feladatokat 
                 tároló tömbben, az értékeket már nem nehéz kitörölni.*/
                napiFeladatok.Remove(napiFeladatok.Find(s => s == opt[i]));
            }
        }

        /// <summary>
        /// Elvégzi a szükséges változtatásokat a maradékelemeken: a Normálokat Sürgőssé teszi, Sürgős vagy Napi esetén kivételt dob
        /// </summary>
        /// <param name="array"></param>
        public void Maradek(List<Feladat> array)
        {

            List<Feladat> exceptions = new List<Feladat>();

            for (int i = 0; i < array.Count; i++)
            {
                if (array[i].Prioritas == Prioritas.Normal)
                {
                    array[i].Prioritas = Prioritas.Surgos;
                }
                else
                {
                    //ha találtunk fontos elemet, akkor azokat elmentjük egy külön listába
                    exceptions.Add(array[i]);
                    //majd az elemet kitöröljük
                    napiFeladatok.Remove(array[i]);
                    i--;
                }
            }
            if (exceptions.Count > 0)
            {
                //az azonnal elvégzendő elemek listájával visszatérünk, hogy a főnök megbízhasson vele mást
                ExceptionBeep();
                throw new NemtuttukmekcsinalniException(exceptions);
            }


        }

        public void BackTrack(int szint, List<Feladat> osszes, List<Feladat> optimal, TimeInterval kuszob)
        {
            #region Alapötlet
            /*Mivel nem tudjuk előre, hogy mennyi megoldás lesz, sem pedig azt, hogy mi számít megoldásnak,
            az előadáson szereplő pseudo-val ellentétben az összes előforduló kombinációt potenciális megoldásnak tekintjük
            függetlenül attól, hogy hány elemű, mennyi az összértéke, vagy mennyi az összidőtartama, az egyedüli kikötés,
            hogy a 8 órát nem haladhatja meg.
            Az algoritmus addig megy, amíg meg nem vizsgálta az összes részerdeményt kezdve az 1 eleműektől a HOSSZ eleműekig (ha ez a helyzet előaáll.)
            Ha az n. szinten belefér egy elem, akkor automatikusan megyünk tovább az n+1. szintre. Minden szinten szint+1 elemű megoldásokra számítunk (mert a 0. szintről indulunk az indexelés miatt)
            így megoldható, hogy az algoritmus szisztematikusan ellenőrizze az összes előforduló lehetőséget.
            Az algoritmus BackTrack jellegét az adja, hogy ha egy elem már nem fér bele egy megoldásba,
            akkor azokat a magasabb elemszámú részmegoldásokat, amiknek a "hibás" elem a gyökere, már nem nézi meg,
            hanem marad a szinten és nézi a következő elemet, ha pedig egyik sem felel meg, akkor visszalép 1 szintet
            és úgy próbálgatja tovább.*/
            #endregion 

            int i = -1;

            /*ahogy növekszik a szintek száma, úgy egyre kevesebb EGYEDI megoldás lehetséges:
             a 0. szinten még annyi 1 elemű, ahány feladatot kaptunk (mert minden feladat önmagában is lehetséges megoldás)
             az utolsó szinten (ha elérjük) pedig csak 1, hiszen az összes lehetséges feladat előfordul bennne min 1 szer
             A -1 kompenzálja, hogy a Lista.Count-adik elemet már nem indexelhetjük*/
            while (i < osszes.Count - szint - 1)
            {
                i++;
                if (Fk(szint, osszes[i + szint], kuszob))
                {
                    //akkor is a következő elemet nézzük, ha szintetléptünk, mert bele tudtuk rakni és akkor is,
                    //ha nem, azaz az i inkrementálódott
                    megoldas.Add(osszes[i + szint]);

                    //ha nem folyamatosan egyre nagyobb elemszámú megoldásokkal dolgoznánk minden egymást követő szinten,
                    //akkor sosem jutnák tovább az 1 elemű részmegoldásoknál
                    if (megoldas.Count == szint + 1)
                    {
                        //ha az aktuális megoldás jobb eredményt ad, mint a jelenéegi optimális, akkor legyen ő az optimális
                        if (Josag(megoldas, optimal))
                        {

                            //azért, hogy ne módosuljon az eredmény, csak az opt, érték szerint adjuk át az elemeket
                            //ehhez előbb kitöröljük a korábbiakat, hogy a LIST.add ne a végére szúrja be (ezt csak akkor végezzük el, ha van benne elem)...
                            if (optimal.Count != 0)
                            {
                                optimal.Clear();
                            }
                            //majd átmásoljuk az összes elemet a megoldásból
                            optimal.AddRange(megoldas);
                        }
                        //ha sikeresen hozzáadtunk 1 elemet, automatikusan megyünk tovább a következő szintre
                        BackTrack(szint + 1, osszes, optimal, kuszob);
                        //ha egy szintet végigpróbálgattunk és visszaugrunk, akkor ki kell törölni az aktuális szinten hozzáadott elemet,
                        //hogy a következő elemet vizsgálhassuk
                        megoldas.Remove(osszes[i + szint]);

                    }
                }

            }

            //a feladatban meg kell oldani, hogy a nagy prioritású feladatok előbb kerüljenek sorra a következő nap
            //mivel bármelyik lehet a következő, a BT végén mindig rendezünk prioritás szerint
            optimal.Sort();
        }

        public void SurgosFeladatokHozzaadasa()
        {
            Console.WriteLine("A szimuláció megkezdése előtt lehetőség van új feladatokat megadni,\nmellyel szimulálható egy-egy új sürgős feladat érkezése.\nA feladatok prioritása kötött, azonban az időtartamon és a feladat teljesítéséért járó fizetségen kívül\nmeg kell adni annak érkezési időpontját is.");
            Console.WriteLine("Kíván új feladatokat hozzáadni? (y/n)");
            char choice = Console.ReadKey().KeyChar;



            if (choice == 'y' || choice == 'Y')
            {
                List<SurgosFeladat> s = new List<SurgosFeladat>();
                Console.WriteLine("\nAdja meg az új feladatokat a következő szintaxist alkalmazva (NYERESÉG;ÓRA:PERC(időtartam);ÓRA:PERC(kezdés ideje))\nHa egy feladat megadásával végzett, üssön ENTER-t. Ha nem kíván több feladatot megadni, gépelje be az 'n' karaktert és üssön ENTER-t.");
                string ujFeladatok = "";
                do
                {
                    ujFeladatok = Console.ReadLine();


                    if (ujFeladatok != "n")
                    {
                        string[] split = ujFeladatok.Split(';');
                        string[] textInput = new string[split.Length - 1];
                        for (int j = 0; j < split.Length - 1; j++)
                        {
                            textInput[j] = split[j];
                        }
                        s.Add(new SurgosFeladat(textInput, split[split.Length - 1]));
                    }
                } while (ujFeladatok != "n");

                
                TimeInterval endOfWorkHours = new TimeInterval(8, 0); //erre azért van szükség, hogy ha nem 8-kor kezdődik a munka, el lehessen tárolni mást is, a munkaidő kezdetét kell itt megadni
                endOfWorkHours.AddObjectValue(Constants.Korlat); //a munkaidő kezdetéhez hozzáadunk 8 órát

                //ha a felhasználó olyan elemet adott meg, ami megelőzi az ÁLLÍTHATÓ munkaidő kezdetét, vagy a 8 órás munkaidő után érkezik, azokat kivesszük
                s.RemoveAll(f => (f.TimeOfStart.CompareTo(StartOfWorkDay) < 0) || f.TimeOfStart.CompareTo(endOfWorkHours) > 0);

                //ha több esemény is érkezik ugyan abban az időben, akkor csak az 1. előfordulást hajtjuk végre
                //egy LINQ paranccsal eltávolítjuk az összes azonos időben érkező feladatot
                //kivéve persze az eseményt magát, mivel nem override-oltuk az Equals-t,
                //objektumként hasonlítja össze az elemeket, azaz referencia szerint

                for (int inc = 0; inc < s.Count; inc++)
                {
                    s.RemoveAll(f => !f.Equals(s[inc]) && f.TimeOfStart.CompareTo(s[inc].TimeOfStart) == 0);
                }
                int i = 0;
                //ha a felhasználó felcseréli az érkező feladatok sorrendjét (például: előbb írja, hogy érkezik egy 12:00-kor, majd beírja, hogy 8:00-kor is érkezett előtte)
                s.Sort();

                //az összes új feladatra meg kell néznünk, hogy hogyan befolyásolják az eredeti listát
                for (int l = 0; l < s.Count; l++)
                {
                    //az összehasonlítás miatt van erre szükség: értéke az az időpont, amikor végeznénk a feladattal, ha pontosan akkor kezdenénk el, amikor megérkezik
                    //erre azért van szükség, hogy kiszűrhessük azokat a feladatokat, amiket a kezdeti időponttól már nem lehetne megoldani
                    TimeInterval comparer = new TimeInterval(0, 0);
                    comparer.AddObjectValue(s[l].Idotartam);
                    comparer.AddObjectValue(s[l].TimeOfStart);

                    //ha a kezdezi időponttól kezdve beleférünk a 8 órába...
                    if (comparer.CompareTo(endOfWorkHours) < 0)
                    {
                        i = 0;
                        TimeInterval helper = new TimeInterval(8, 0);
                        //addig megy, amíg van elem az optimálisban és amíg a segéd értéke meg nem haladja az új elem kezdetét
                        while (i < opt.Count && helper.CompareTo(s[l].TimeOfStart) < 0)
                        {
                            helper.AddObjectValue(opt[i].Idotartam);
                            i++;
                        }


                        //erre a fél óra miatt van szükség
                        TimeInterval temp = new TimeInterval(30); //értéke az a maximális időpont, ameddig elkezdhetjük az új feladatot (kezdeti + fél óra)
                        temp.AddObjectValue(s[l].TimeOfStart);
                        //temp.AddObjectValue(offset);

                        //ha a feladat megérkezésétől számítva fél órán belül el tudjuk kezdni, mert addigra lejár egy feladat
                        if (helper.CompareTo(temp) < 0 || helper.CompareTo(temp) == 0)
                        {
                            //akkor beszúrjuk a megfelelő feladat mögé
                            opt.Insert(i, s[l]);

                            //ezután meg kell nézni, hogy az új esemény utáni feladatokat hogyan lehet a legoptimálisabban elosztani (ha lehet)
                            //ezért, ha maradt még idő, BackTrack-el újraosztjuk az eseményeket a következő módszerrel:
                            helper.AddObjectValue(s[l].Idotartam);

                            //az ideiglenes tömbben null-ra állítjuk a feladatot, remove-olni nem lehet, mert megváltozik az enumeration
                            s[l] = null;

                            //újraosztjuk a feladatokat
                            FeladatokatUjraoszt(helper, endOfWorkHours, i, null);
                        }

                        //ha nem tudjuk fél órán belül elkezdeni, de azért nem mert épp akkor egy feladat folyamatban van...
                        else
                        {
                            /*a Beadandó pdf-jében szereplő 6-os feladat szerint:
                             *"A nap folyamán bármikor jöhet újabb sürgős feladat, amit fél órán belül el kell kezdenünk
                            megvalósítani." Azonban a Beadandó korábbi feladataiból kiderül, hogy minden sürgős feladatot
                            meg kellene oldani, így tehát ha éppen sürgős feladatot hajtunk végre, akkor nem oldható meg a feladat,
                            az új elem pedig végül a kivételek közé kerül majd. Ha nem sürgős...*/

                            //az i most arra az elemre mutat, ami előtt van az a hely, ahová be akarjuk szúrni az új elemet
                            //ha ezen a helyen nem sürgős feladat van...
                            if (opt[i-1].Prioritas != Prioritas.Surgos)
                            {
                                //figyelembe kell venni, hogy az optba beletettük az új elemet...
                                Console.WriteLine("Félbeszkítottuk: {0}, mert sürgős feladat érkezett!", opt[i - 1].ToString());
                                helper.AddObjectValue(s[l].Idotartam);
                                //...de kivettük a régit
                                helper.SubtractObjectValue(opt[i - 1].Idotartam);

                                Feladat csereltFeladat = opt[i - 1];
                                /*a cserélt feladat idejéből ki kell vonnunk azt az időt, amit eltöltöttünk már vele
                                 *Ezt a következőképpen lehet:
                                    ha a helperből kivonjuk a cserélt időtartamát, akkor megkapjuk, hogy a cseréltet mikor kezdtük el
                                    ha az új feladat érkezési idejéből ezt kivonjuk, megkapjuk, hogy mennyi ideje tartott a feladat, mikor az új megjött
                                    ezt kell kivonni a cseréltből
                                    Azaz: ha 8:15-kor kezdtünk el egy nem sürgős feladatot, de 8:20-kor jött egy sürgős,
                                    akkor a már belekezdett feladat 5 perccel rövidebb lesz*/

                                TimeInterval calculation = new TimeInterval(helper - csereltFeladat.Idotartam);
                                csereltFeladat.Idotartam.SubtractObjectValue(new TimeInterval(s[l].TimeOfStart - calculation));

                                //az új feladatot ide szúrjuk majd be, felülírva a korábbi feladatot
                                opt[i - 1] = s[l];
                                s[l] = null;
                                calculation = null;
                                //végül újraosztjuk a feladatokat úgy, hogy a módosított feladatot is átadjuk
                                
                                FeladatokatUjraoszt(helper, endOfWorkHours, i-1, csereltFeladat);
                            }


                        }
                    }

                }

                //mivel null-okkal jelöltük az összes megoldott elemet, ezeket most kivesszük
                s.RemoveAll(feladat => feladat == null);

                //ha van benne még elem, azokat nem tudtuk megoldani, így kivételt generálunk
                if (s.Count > 0)
                {
                    throw new UjSurgosFeladatException(s);
                }

            }
            else if (choice == 'n' || choice == 'N')
            {

                Console.WriteLine("\nA szimuláció változatlanul fog lefutni!");
                return;
            }


        }
        /// <summary>
        /// Újra osztja a feladatokat az optimális megoldást tartalmazó tömb éppen beszúrt elemeiből
        /// </summary>
        /// <param name="helper">Az új elemig eltelt idő</param>
        /// <param name="endOfWorkHours">Ekkora van vége a munkanapnak</param>
        /// <param name="i">Az iterátor, mely az éppen beszúrt elemre mutat</param>
        /// /// <param name="exchangedFeladat">A cserélt feladatra mutat, ha nincs ilyen, akkor null</param>
        void FeladatokatUjraoszt(TimeInterval helper, TimeInterval endOfWorkHours, int i, Feladat exchangedFeladat)
        {
            //tehát ha maradt még idő:
            if (helper.CompareTo(endOfWorkHours) < 0)
            {
                //generálunk két listát
                //az egyik azokat az eseményeket fogja tartalmazni, amik az optimálisban az új feladat után vannak
                List<Feladat> tovabbiFeladatok = new List<Feladat>();
                //ha valamilyen elem HELYÉRE szúrtuk be az újat, akkor ezt az elemet hozzáadjuk a listához
                if(exchangedFeladat != null)
                {
                    tovabbiFeladatok.Add(exchangedFeladat);
                }
                //mivel az i most oda mutat, ahol az új elem van, minden további elemet átmásolunk ide egy új iterátorral
                for (int j = i + 1; j < opt.Count; j++)
                {
                    tovabbiFeladatok.Add(opt[j]);
                }
                //a másik az optimális elosztást, ha van ilyen
                List<Feladat> optimalisUj = new List<Feladat>();
                TimeInterval ujIdobeosztasKuszob = new TimeInterval(endOfWorkHours - helper);
                //majd BT-el megpróbáljuk beosztani őket
                megoldas = new List<Feladat>();
                //ezután kivesszük a most berakott elem után az összeset, hogy az optimálisat adjuk majd vissza
                opt.RemoveRange(i + 1, opt.Count - (i + 1));
                BackTrack(0, tovabbiFeladatok, optimalisUj, ujIdobeosztasKuszob);


                //ha volt optimális megoldás
                if (optimalisUj.Count > 0)
                {
                    //akkor azt hozzáadjuk a valódi optimálishoz
                    opt.AddRange(optimalisUj);
                    //kiszedjük a most berakott elemeket az új elem miatt vizsgálandó elemek között
                    tovabbiFeladatok.RemoveAll(feladat => optimalisUj.Contains(feladat));
                    //majd megnézzük, hogy maradt-e elem
                    if (tovabbiFeladatok.Count > 0)
                    {
                        //ha igen, őket visszatesszük a napi feladatok közé, hogy egy későbbi alkalomkor fel lehessen őket dolgozni
                        napiFeladatok.AddRange(tovabbiFeladatok);
                    }

                }
            }
        }

        public void Simulation()
        {
            
            Console.WriteLine("-----------------");
            Console.WriteLine("SZIMULÁCIÓ");
            Console.WriteLine("-----------------");
            
            int waitForThisAmountOfTime = 0;
            foreach (Feladat f in opt)
            {
                #region Alapötet
                /*A szimuláció időtartaamarányosan fut: 1 óra 1 másodpercnek (1000 milliszekundum) felel meg*/
                #endregion
              
                waitForThisAmountOfTime = f.Idotartam.ConvertToMinutes() * (1000 / 60);
                System.Threading.Thread.Sleep(waitForThisAmountOfTime);
                if (Ready != null)
                {
                    
                    FeladatElkeszultBeep();
                    Ready(f);
                    NapiFeladatok.Remove(f);
                }
                
            }
            //várunk 1 másodpercet, hogy végig lehessen futni a szimuláción
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine();


        }

        public void FeladatElkeszultBeep()
        {
            Console.Beep(2000, 100);
            Console.Beep(5000, 100);
            Console.Beep(5000, 100);
        }
        public void ExceptionBeep()
        {
            Console.Beep(1550, 100);
            Console.Beep(1550, 100);
            Console.Beep(1550, 100);
            Console.Beep(1550, 100);
        }
    }
}
