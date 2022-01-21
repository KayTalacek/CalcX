using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcX
{
    public class Currency
    {
        public Currency(
            string kodMeny,
            string menaNazev,
            string mnozstvi,
            string kurz,
            string zeme)
        {
            Kod = kodMeny;
            Mena = menaNazev;
            Mnozstvi = mnozstvi;
            Kurz = kurz;
            Zeme = zeme;
        }

        public string Kod { get; }
        public string Mena { get; }
        public string Mnozstvi { get; }
        public string Kurz { get; }
        public string Zeme { get; }
    }
}
