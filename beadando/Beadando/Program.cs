using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Windows.Input;


namespace Beadando
{
    class Program
    {
        
        static void Main(string[] args)
        {

            Office of;
            TimeInterval startOfTheTask = new TimeInterval(8, 0);
            TimeInterval endOfTheTask = new TimeInterval(8, 0);
            int napokSzama = 0;


            void FeladatElkeszult(Feladat f)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                endOfTheTask.AddObjectValue(f.Idotartam);
                Console.WriteLine("{0}-{1}|A kovetkező feladat végrehajtásra került: {2}", startOfTheTask.ToString(), endOfTheTask.ToString(), f.ToString());
                Console.ResetColor();
                startOfTheTask.AddObjectValue(f.Idotartam);
            }
            //azért választottam a névtelen metódus + lambda kifejezéses elrendezést,
            //mert csak itt fogjuk használni az esemény eltüzelésekor lefutó metódust
            //az itt szereplő időpontok csak abban segítenek, hogy belefértünk-e a 8 órába, új feladat beszúrásakor elképzelhető, hogy pontatlanul működik


            //a külső try blokkon belül is található try-catch, hogy bizonyos hibák után a program tovább fusson
            try
            {

                of = new Office("input.txt", 8, 0);
                of.Ready += FeladatElkeszult;

                while (of.NapiFeladatok.Count > 0)
                {
                    of.Opt.Clear();
                    of.BackTrack(0, of.NapiFeladatok, of.Opt, Constants.Korlat);
                    Console.WriteLine("\n-------------------------");
                    Console.WriteLine("TERVEZETT IDŐBEOSZTÁS");
                    Console.WriteLine("-------------------------");
                    Console.WriteLine("\n{0}. Nap", ++napokSzama);
                    Console.WriteLine("-------------");
                   
                    Console.WriteLine("Optimális:");
                 
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    foreach (Feladat f in of.Opt)
                    {
                        Console.WriteLine(f.ToString());
                    }
                    Console.ResetColor();
                    if (of.NapiFeladatok.Count > 0)
                    {
                        of.Differencia(); 
                        Console.WriteLine("{0}", of.NapiFeladatok.Count > 0 ?  "\nNem tudtuk beosztani a következőket:" : "\nAz összes feladatot sikeresen beosztottuk!");
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        foreach (Feladat f in of.NapiFeladatok)
                        {
                            Console.WriteLine(f.ToString());
                        }
                        Console.WriteLine();
                        Console.ResetColor();
                    }


                    try
                    {
                        of.SurgosFeladatokHozzaadasa();
                    }
                    catch (UjSurgosFeladatException u)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        of.ExceptionBeep();
                        Console.WriteLine(u.Message);
                        if (u.Exceptions.Count > 1)
                        {
                            Console.WriteLine("A következő új feladatokat nem tudtuk beosztani: ");
                            foreach (Feladat s in u.Exceptions)
                            {
                                Console.WriteLine(s.ToString());
                            }
                            

                        }
                        else
                        {
                            Console.WriteLine("A következő új feladatot nem tudtuk beosztani: ");
                            Console.WriteLine(u.Exceptions[0].ToString());
                        }
                        Console.ResetColor();
                        Console.WriteLine();
                    }
                    of.Simulation();
                    startOfTheTask = new TimeInterval(8, 0);
                    endOfTheTask = new TimeInterval(8, 0);
                    

                    if (of.NapiFeladatok.Count > 0)
                    {


                        try
                        {
                            of.Maradek(of.NapiFeladatok);
                        }
                        catch (NemtuttukmekcsinalniException n)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(n.Message);
                            if (n.Exceptions.Count == 1)
                            {
                                Console.WriteLine("A következő feladatot nem sikerült végrehajtani: {0}", n.Exceptions[0].ToString());
                            }
                            else
                            {
                                Console.WriteLine("A következő feladatokat nem sikerült végrehajtani:");
                                foreach (Feladat f in n.Exceptions)
                                {
                                    Console.WriteLine(f.ToString());
                                }
                            }
                            Console.ResetColor();
                            Console.WriteLine();
                        }


                        //a cikluson belül is elfogyhat a feladat
                        if (of.NapiFeladatok.Count > 0)
                        {
                            Console.WriteLine("Átírás után:");
                            foreach (Feladat f in of.NapiFeladatok)
                            {
                                Console.WriteLine(f.ToString());
                            } 
                        }
                    }
                   
                }
               
            }
            
            
            catch (FileNotFoundException n)
            {
                //a hiba önmagában is ki tudja írni, hogy mi történt, de így is lehetséges egyedi üzenetet megjeleníteni
                Console.WriteLine("Az Ön által megadott file nem létezik: {0}", n.FileName);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine("---------------------------");
                Console.WriteLine("---------------------------");
                Console.WriteLine("A PROGRAM FUTÁSA VÉGET ÉRT!");
                Console.WriteLine("---------------------------");
                Console.WriteLine("---------------------------");
            }
           

            



            Console.ReadLine();

        }
        

        

        
        }
    }

