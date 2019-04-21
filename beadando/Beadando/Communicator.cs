using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beadando
{
    
    class Communicator
    {
        string fileName; //feltesszük, h a file a [elérési út]/bin/debug-ban van

        public Communicator(string fileName)
        {
            this.fileName = fileName;
        }


        /// <summary>
        /// Feltölti a paraméterben kapott Office típusú objektumban található tömböt a megfeleő mennyiségű adattal
        /// </summary>
        /// <param name="o">Egy lepéldányosított objektumra hivatkozik</param>
        public void LoadFile(Office o)
        {
            StreamReader sr = new StreamReader(fileName);
            FileInfo fInfo = new FileInfo(fileName);
            
            if(fInfo.Length < 1)
            {
                /*alapból ezen a szinten a rendszer nem dob hibát ha üres a file,
                 de ha ez ilyen korán kiderül, akkor nem futunk fölösleges köröket itt
                 és 1 szinttel fejjebb le tudjuk kezelni a hibát*/
                
                sr.Close();
                fInfo = null;
                throw new EndOfStreamException("Az Ön által megadott file üres!");
            }

            else
            {
                while (!sr.EndOfStream)
                {
                    string[] read = sr.ReadLine().Split(';');
                    Parse(read, o.NapiFeladatok, sr, o);
                }  
            }
            fInfo = null;
            sr.Close();
            

            //felszabadítjuk az SR-t
            sr = null;


            //ezzel a kis résszel instant tesztelhető, hogy feltöltődött-e az adathalmaz
            Console.WriteLine("-------------------");
            Console.WriteLine("VÉGREHAJTANDÓ FELADATOK");
            Console.WriteLine("-------------------");
            foreach (Feladat f in o.NapiFeladatok)
            {
                Console.WriteLine(f.ToString());
            }

           
      }

       public void Parse(string[] read, List<Feladat> feladatok, StreamReader sr, Office o)
        {
            
            if (read.Length >= 3)
            {
                //mivel a konstruktorból dobunk hibát, így meg kell adni az SR-t is, hogy a hiba bezárhassa a file-t
                Feladat tempFeladat = new Feladat(read, sr);
                o.UjFeladatotHozzaad(tempFeladat, feladatok);
            }
            else
            {
                //a user kihagyta a ;-z
                string temp = "";
                foreach (string s in read)
                {
                    temp += s;
                }


                throw new UserInputException(temp, sr);
            }

        }
    }
        
        
    }

