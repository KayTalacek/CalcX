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
            string currencyISO,
            string currencyShortName,
            string currencyFullName,
            string country,
            string countryISO,
            string ratesValidityDate,
            int currencyUnit,
            double middle,
            double cashBuy,
            double cashSell,
            double noncashBuy,
            double noncashSell)
        {
            CurrencyISO = currencyISO;
            CurrencyShortName = currencyShortName;
            CurrencyFullName = currencyFullName;
            Country = country;
            CountryISO = countryISO;
            RatesValidityDate = ratesValidityDate;
            CurrencyUnit = currencyUnit;
            Middle = middle;
            CashBuy = cashBuy;
            CashSell = cashSell;
            NoncashBuy = noncashBuy;
            NoncashSell = noncashSell;
        }

        public string CurrencyISO { get; init; }
        public string CurrencyShortName { get; }
        public string CurrencyFullName { get; }
        public string Country { get; init; }
        public string CountryISO { get; }
        public string RatesValidityDate { get; }
        public int CurrencyUnit { get; }
        public double Middle { get; }
        public double CashBuy { get; }
        public double CashSell { get; }
        public double NoncashBuy { get; }
        public double NoncashSell { get; }
    }
}
