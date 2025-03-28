
using System;
using Cwiczenia2.Inheritance;

namespace Cwiczenia2.Inheritance;

    public abstract class Kontener
    {
        public string Numer { get; protected set; }
        public double MaksymalnaLadownosc { get; protected set; }
        public double AktualnyLadunek { get; protected set; }
        public string TypProduktu { get; protected set; }
        public double Temperatura { get; protected set; }

        public Kontener(double maksLadownosc, string typProduktu, double temperatura)
        {
            MaksymalnaLadownosc = maksLadownosc;
            TypProduktu = typProduktu;
            Temperatura = temperatura;
            AktualnyLadunek = 0;
        }

        public virtual void Zaladuj(double masa, bool dangerous)
        {
            double maxFill = dangerous ? 0.5 * MaksymalnaLadownosc : 0.9 * MaksymalnaLadownosc;
            if (AktualnyLadunek + masa > maxFill)
            {
                throw new Exception($"Load {masa} kg exceeds limit ({maxFill} kg) for container {Numer}.");
            }
            AktualnyLadunek += masa;
            Console.WriteLine($"Container {Numer} loaded {masa} kg. Current load: {AktualnyLadunek} kg.");
        }

        public virtual void Oproznij()
        {
            Console.WriteLine($"Emptying container {Numer}. Load was: {AktualnyLadunek} kg.");
            AktualnyLadunek = 0;
        }

        public virtual void WypiszInfo()
        {
            Console.WriteLine($"Container: {Numer}");
            Console.WriteLine($"Product type: {TypProduktu}, Temp: {Temperatura} C");
            Console.WriteLine($"Capacity: {MaksymalnaLadownosc} kg, Load: {AktualnyLadunek} kg");
        }
    }

