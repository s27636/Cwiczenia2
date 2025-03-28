using System;
using System.Collections.Generic;

namespace Cwiczenia2.Inheritance
{
    public class OverfillException : Exception
    {
        public OverfillException(string message) : base(message) { }
    }

    public class Statek
    {
        public string Nazwa { get; set; }
        public int Predkosc { get; set; }
        public int MaxKontenery { get; set; }
        public double MaxWaga { get; set; } // w tonach
        private List<Kontener> kontenery;

        public Statek(string nazwa, int predkosc, int maxKontenery, double maxWaga)
        {
            Nazwa = nazwa;
            Predkosc = predkosc;
            MaxKontenery = maxKontenery;
            MaxWaga = maxWaga;
            kontenery = new List<Kontener>();
        }

        public void DodajKontener(Kontener kontener)
        {
            if (kontenery.Count >= MaxKontenery)
            {
                Console.WriteLine("Max containers reached.");
                return;
            }

            double currentWeight = 0;
            foreach (var k in kontenery)
            {
                currentWeight += k.AktualnyLadunek;
            }
            // zamieniamy kg -> tony dzielac przez 1000
            double newWeightTons = (currentWeight + kontener.AktualnyLadunek) / 1000.0;
            if (newWeightTons > MaxWaga)
            {
                Console.WriteLine("Adding this container exceeds ship's weight limit.");
                return;
            }

            kontenery.Add(kontener);
            Console.WriteLine($"Container {kontener.Numer} added to ship {Nazwa}.");
        }

        public void WypiszInfo()
        {
            Console.WriteLine($"Ship: {Nazwa} (Speed: {Predkosc} knots, Max containers: {MaxKontenery}, Max weight: {MaxWaga} tons)");
            Console.WriteLine("Containers:");
            if (kontenery.Count == 0)
            {
                Console.WriteLine("No containers.");
            }
            else
            {
                foreach (var k in kontenery)
                {
                    k.WypiszInfo();
                    Console.WriteLine("----------------------------------");
                }
            }
        }
    }

    class Program
    {
        static List<Statek> flota = new List<Statek>();
        static List<Kontener> magazynKontenerow = new List<Kontener>();

        static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nProgram container managment");
                Console.WriteLine("1. Add ship");
                Console.WriteLine("2. Remove ship");
                Console.WriteLine("3. Add container to storage");
                Console.WriteLine("4. Load container");
                Console.WriteLine("5. Load container onto ship");
                Console.WriteLine("6. List ships");
                Console.WriteLine("7. List storage containers");
                Console.WriteLine("0. Exit");
                Console.Write("Choose an action: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        DodajStatek();
                        break;
                    case "2":
                        UsunStatek();
                        break;
                    case "3":
                        DodajKontener();
                        break;
                    case "4":
                        ZaladujKontener();
                        break;
                    case "5":
                        ZaladujKontenerNaStatek();
                        break;
                    case "6":
                        WypiszStatki();
                        break;
                    case "7":
                        WypiszMagazynKontenerow();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        static void DodajStatek()
        {
            Console.Write("Ship name: ");
            string nazwa = Console.ReadLine();
            Console.Write("max soeed: ");
            int predkosc = int.Parse(Console.ReadLine());
            Console.Write("Max containers: ");
            int maxKont = int.Parse(Console.ReadLine());
            Console.Write("Max tones: ");
            double maxWaga = double.Parse(Console.ReadLine());

            flota.Add(new Statek(nazwa, predkosc, maxKont, maxWaga));
            Console.WriteLine("Ship added.");
        }

        static void UsunStatek()
        {
            Console.Write("give ship name to remove: ");
            string nazwa = Console.ReadLine();
            var statek = flota.Find(s => s.Nazwa.Equals(nazwa, StringComparison.OrdinalIgnoreCase));
            if (statek != null)
            {
                flota.Remove(statek);
                Console.WriteLine("Ship removed.");
            }
            else
            {
                Console.WriteLine("Ship not found.");
            }
        }

        static void DodajKontener()
        {
            Console.WriteLine("Container type:");
            Console.WriteLine("1. Liquid");
            Console.WriteLine("2. Gas");
            Console.WriteLine("3. Refrigerated");
            string typ = Console.ReadLine();

            Console.Write("Max kilograms: ");
            double maxLad = double.Parse(Console.ReadLine());
            Console.Write("Product type: ");
            string produkt = Console.ReadLine();
            Console.Write("Temperature in celius: ");
            double temp = double.Parse(Console.ReadLine());

            Kontener kontener = null;
            try
            {
                switch (typ)
                {
                    case "1":
                        kontener = new KontenerPlyn(maxLad, produkt, temp);
                        break;
                    case "2":
                        Console.Write("Pressure: ");
                        double cisnienie = double.Parse(Console.ReadLine());
                        kontener = new KontenerGaz(maxLad, produkt, temp, cisnienie);
                        break;
                    case "3":
                        Console.Write("Heigh: ");
                        double wys = double.Parse(Console.ReadLine());
                        Console.Write("Container weight: ");
                        double waga = double.Parse(Console.ReadLine());
                        Console.Write("Depth: ");
                        double gleb = double.Parse(Console.ReadLine());
                        kontener = new KontenerChlodniczy(maxLad, produkt, temp, wys, waga, gleb);
                        break;
                    default:
                        Console.WriteLine("Invalid container type.");
                        return;
                }
                magazynKontenerow.Add(kontener);
                Console.WriteLine($"Container {kontener.Numer} added to storage.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        static void ZaladujKontener()
        {
            Console.Write("Container number: ");
            string numer = Console.ReadLine();
            var kontener = magazynKontenerow.Find(k => k.Numer.Equals(numer, StringComparison.OrdinalIgnoreCase));
            if (kontener == null)
            {
                Console.WriteLine("Container not found.");
                return;
            }

            Console.Write("Load kilogrames: ");
            double masa = double.Parse(Console.ReadLine());
            Console.Write("Dangerous load? y or n: ");
            bool dangerous = Console.ReadLine().ToLower() == "y";

            try
            {
                kontener.Zaladuj(masa, dangerous);
            }
            catch (OverfillException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                if (kontener is IHazardNotifier notifier)
                {
                    notifier.NotifyHazard(kontener.Numer, "Overload attempt.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        static void ZaladujKontenerNaStatek()
        {
            Console.Write("Container number: ");
            string numer = Console.ReadLine();
            var kontener = magazynKontenerow.Find(k => k.Numer.Equals(numer, StringComparison.OrdinalIgnoreCase));
            if (kontener == null)
            {
                Console.WriteLine("Container not found.");
                return;
            }
            Console.Write("Ship name: ");
            string nazwaStatku = Console.ReadLine();
            var statek = flota.Find(s => s.Nazwa.Equals(nazwaStatku, StringComparison.OrdinalIgnoreCase));
            if (statek == null)
            {
                Console.WriteLine("Ship not found.");
                return;
            }
            statek.DodajKontener(kontener);
            magazynKontenerow.Remove(kontener);
        }

        static void WypiszStatki()
        {
            if (flota.Count == 0)
            {
                Console.WriteLine("No ships.");
                return;
            }
            foreach (var s in flota)
            {
                s.WypiszInfo();
                Console.WriteLine("====================");
            }
        }

        static void WypiszMagazynKontenerow()
        {
            if (magazynKontenerow.Count == 0)
            {
                Console.WriteLine("No containers in storage.");
                return;
            }
            foreach (var k in magazynKontenerow)
            {
                k.WypiszInfo();
                Console.WriteLine("-------------------------------------");
            }
        }
    }
}
