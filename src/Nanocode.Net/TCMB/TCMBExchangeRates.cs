using System;
using System.Collections.Generic;
using System.Text;

namespace Nanocode.Net.TCMB
{
    public class TCMBExchangeRates
    {
        // Tarih
        public DateTime Date { get; set; }
        public string BulletinNumber { get; set; }

        // Parite
        public string Currency1 { get; set; }
        public string Currency2 { get; set; }
        public string Symbol { get; set; }

        // Kaç Birim
        public decimal? Unit { get; set; }

        // Çapraz Kur
        public decimal? CrossRate { get; set; }

        // Döviz Alış-Bid
        public decimal? ForexBuying { get; set; }
        // Döviz Satış-Ask
        public decimal? ForexSelling { get; set; }

        // Efektif Alış-Bid
        public decimal? BanknoteBuying { get; set; }
        // Efektif Satış-Ask
        public decimal? BanknoteSelling { get; set; }
    }
}
