using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Nanocode.Net.TCMB
{
    public class TCMBClient
    {
        private string[] Currencies = {
            "ATS",
            "AUD",
            "BEF",
            "BGL",
            "BGN",
            "CAD",
            "CHF",
            "CNY",
            "DEM",
            "DKK",
            "ESP",
            "EUR",
            "FIM",
            "FRF",
            "FYP",
            "GBP",
            "GRD",
            "IEP",
            "ILS",
            "IRR",
            "ITL",
            "JOD",
            "JPY",
            "KWD",
            "LUF",
            "NLG",
            "NOK",
            "PKR",
            "PTE",
            "QAR",
            "ROL",
            "RON",
            "RUB",
            "SAR",
            "SEK",
            "USD",
        };

        public TCMBClient()
        {
            Array.Sort(this.Currencies);
        }

        public TCMBExchangeRates GetExchangeRate(DateTime date, string currency)
        {
            var dict = this.GetExchangeRates(date);
            if (dict.ContainsKey(currency.ToUpper())) return dict[currency.ToUpper()];

            // Return
            return null;
        }

        public SortedDictionary<string, TCMBExchangeRates> GetExchangeRates(DateTime date)
        {
            var dict = new SortedDictionary<string, TCMBExchangeRates>();

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(date.Date == DateTime.Today.Date ? "http://www.tcmb.gov.tr/kurlar/today.xml"
                : "http://www.tcmb.gov.tr/kurlar/" + date.ToString("yyyyMM") + "/" + date.ToString("ddMMyyyy") + ".xml");

                // Tarih ve Bülten No
                var rateDate = date.Date.AddHours(15).AddMinutes(30);
                var rateNum = string.Empty;
                try
                {
                    // var rateTarih = Convert.ToDateTime(xmlDoc.SelectSingleNode("//Tarih_Date").Attributes["Tarih"].Value);
                    rateDate = DateTime.ParseExact(xmlDoc.SelectSingleNode("//Tarih_Date").Attributes["Date"].Value, "d", CultureInfo.InvariantCulture).AddHours(15).AddMinutes(30);
                    rateNum = xmlDoc.SelectSingleNode("//Tarih_Date").Attributes["Bulten_No"].Value;
                }
                catch { }
                foreach (var curr in this.Currencies)
                {
                    try
                    {
                        var rates1 = new TCMBExchangeRates { Date = rateDate, BulletinNumber = rateNum, Currency1 = curr, Currency2 = "TRY", Symbol = curr + "TRY" };
                        var unit_str = xmlDoc.SelectSingleNode("Tarih_Date/Currency[@Kod='" + curr + "']/Unit").InnerXml;
                        if (decimal.TryParse(unit_str, out decimal unit_dec)) rates1.Unit = unit_dec;

                        var val_str_fb = xmlDoc.SelectSingleNode("Tarih_Date/Currency[@Kod='" + curr + "']/ForexBuying").InnerXml;
                        if (decimal.TryParse(val_str_fb, out decimal val_dec_fb)) { rates1.CrossRate = val_dec_fb; rates1.ForexBuying = val_dec_fb; }

                        var val_str_fs = xmlDoc.SelectSingleNode("Tarih_Date/Currency[@Kod='" + curr + "']/ForexSelling").InnerXml;
                        if (decimal.TryParse(val_str_fs, out decimal val_dec_fs)) rates1.ForexSelling = val_dec_fs;

                        var val_str_bb = xmlDoc.SelectSingleNode("Tarih_Date/Currency[@Kod='" + curr + "']/BanknoteBuying").InnerXml;
                        if (decimal.TryParse(val_str_bb, out decimal val_dec_bb)) rates1.BanknoteBuying = val_dec_bb;

                        var val_str_bs = xmlDoc.SelectSingleNode("Tarih_Date/Currency[@Kod='" + curr + "']/BanknoteSelling").InnerXml;
                        if (decimal.TryParse(val_str_bs, out decimal val_dec_bs)) rates1.BanknoteSelling = val_dec_bs;

                        // Add
                        if (rates1.Currency1 != rates1.Currency2) dict.Add(rates1.Symbol, rates1);

                        // Again
                        var rates2 = new TCMBExchangeRates { Date = rateDate, BulletinNumber = rateNum, Currency1 = "USD", Currency2 = curr, Symbol = "USD" + curr, Unit = 1 };
                        var val_str_cr = xmlDoc.SelectSingleNode("Tarih_Date/Currency[@Kod='" + curr + "']/CrossRateUSD").InnerXml;
                        if (val_str_cr.Length > 0)
                        {
                            if (decimal.TryParse(val_str_cr, out decimal val_dec_cr)) rates2.CrossRate = val_dec_cr;
                            if (rates2.Currency1 != rates2.Currency2) dict.Add(rates2.Symbol, rates2);
                        }

                        // Again
                        var rates3 = new TCMBExchangeRates { Date = rateDate, BulletinNumber = rateNum, Currency1 = curr, Currency2 = "USD", Symbol = curr + "USD", Unit = 1 };
                        var val_str_or = xmlDoc.SelectSingleNode("Tarih_Date/Currency[@Kod='" + curr + "']/CrossRateOther").InnerXml;
                        if (val_str_or.Length > 0)
                        {
                            if (decimal.TryParse(val_str_or, out decimal val_dec_or)) rates3.CrossRate = val_dec_or;
                            if (rates3.Currency1 != rates3.Currency2) dict.Add(rates3.Symbol, rates3);
                        }
                    }
                    catch { }
                }
            }
            catch { }

            // Return
            return dict;
        }

    }

}
