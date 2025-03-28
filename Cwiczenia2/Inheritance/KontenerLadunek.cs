using System;
using Cwiczenia2.Inheritance;

namespace Cwiczenia2.Inheritance;

    public static class SerialNumberGenerator
    {
        private static int _counter = 1;
        public static string GenerateSerial(string type)
        {
            return $"KON-{type}-{_counter++}";
        }
    }

    public class KontenerPlyn : Kontener
    {
        public KontenerPlyn(double maksLadownosc, string typProduktu, double temperatura)
            : base(maksLadownosc, typProduktu, temperatura)
        {
            Numer = SerialNumberGenerator.GenerateSerial("L");
        }
    }

    public class KontenerGaz : Kontener, IHazardNotifier
    {
        public double Cisnienie { get; private set; }

        public KontenerGaz(double maksLadownosc, string typProduktu, double temperatura, double cisnienie)
            : base(maksLadownosc, typProduktu, temperatura)
        {
            Cisnienie = cisnienie;
            Numer = SerialNumberGenerator.GenerateSerial("G");
        }

        public override void Oproznij()
        {
            double remain = 0.05 * AktualnyLadunek;
            Console.WriteLine($"Emptying gas container {Numer}. Remains {remain} kg.");
            AktualnyLadunek = remain;
        }

        public void NotifyHazard(string containerNumber, string message)
        {
            Console.WriteLine($"[ALERT][Container {containerNumber}]: {message}");
        }
    }

    public class KontenerChlodniczy : Kontener, IHazardNotifier
    {
        public double Wysokosc { get; private set; }
        public double WagaWlasna { get; private set; }
        public double Glebokosc { get; private set; }

        public KontenerChlodniczy(double maksLadownosc, string typProduktu, double temperatura,
            double wysokosc, double wagaWlasna, double glebokosc)
            : base(maksLadownosc, typProduktu, temperatura)
        {
            Wysokosc = wysokosc;
            WagaWlasna = wagaWlasna;
            Glebokosc = glebokosc;
            Numer = SerialNumberGenerator.GenerateSerial("C");
        }

        public void NotifyHazard(string containerNumber, string message)
        {
            Console.WriteLine($"[ALERT][Container {containerNumber}]: {message}");
        }

        public override void WypiszInfo()
        {
            base.WypiszInfo();
            Console.WriteLine($"Height: {Wysokosc} cm, Own weight: {WagaWlasna} kg, Depth: {Glebokosc} cm");
        }
    }

