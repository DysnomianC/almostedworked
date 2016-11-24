using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Almost_worked
{
    public class ExchangeObject
    {
        public static List<String> CURRENCIES = new List<String> {"AUD", "BGN", "BRL", "CAD", "CHF", "CNY", "CZK", "DKK", "GBP",
                                             "HKD", "HRK", "HUF", "IDR", "ILS", "INR", "JPY", "KRW", "MXN", "MYR", "NOK", "NZD",
                                             "PHP", "PLN", "RON", "RUB", "SEK", "SGD", "THB", "TRY", "USD", "ZAR", "EUR"};

        public class Rates
        {
            public double AUD { get; set; }
            public double BGN { get; set; }
            public double BRL { get; set; }
            public double CAD { get; set; }
            public double CHF { get; set; }
            public double CNY { get; set; }
            public double CZK { get; set; }
            public double DKK { get; set; }
            public double GBP { get; set; }
            public double HKD { get; set; }
            public double HRK { get; set; }
            public double HUF { get; set; }
            public double IDR { get; set; }
            public double ILS { get; set; }
            public double INR { get; set; }
            public double JPY { get; set; }
            public double KRW { get; set; }
            public double MXN { get; set; }
            public double MYR { get; set; }
            public double NOK { get; set; }
            public double NZD { get; set; }
            public double PHP { get; set; }
            public double PLN { get; set; }
            public double RON { get; set; }
            public double RUB { get; set; }
            public double SEK { get; set; }
            public double SGD { get; set; }
            public double THB { get; set; }
            public double TRY { get; set; }
            public double USD { get; set; }
            public double ZAR { get; set; }
            public double EUR { get; set; }

            public string getRates(bool full = false)
            {
                if (full)
                {
                    return "AUD = " + AUD + "\n\nBGN = " + BGN + "\n\nBRL = " + BRL + "\n\nCAD = " + CAD + "\n\nCHF = " + CHF + "\n\nCNY = " + CNY + "\n\nCZK = " + CZK +
                        "\n\nDKK = " + DKK + "\n\nGBP = " + GBP + "\n\nEUR = " + EUR + "\n\nHKD = " + HKD + "\n\nHRK = " + HRK + "\n\nHUF = " + HUF + "\n\nIDR = " + IDR +
                        "\n\nILS = " + ILS + "\n\nINR = " + INR + "\n\nJPY = " + JPY + "\n\nKRW = " + KRW + "\n\nMXN = " + MXN + "\n\nMYR = " + MYR + "\n\nNOK = " + NOK +
                        "\n\nNZD = " + NZD + "\n\nPHP = " + PHP + "\n\nPLN = " + PLN + "\n\nRON = " + RON + "\n\nRUB = " + RUB + "\n\nSEK = " + SEK + "\n\nSGD = " + SGD +
                        "\n\nTHB = " + THB + "\n\nTRY = " + TRY + "\n\nUSD = " + USD + "\n\nZAR = " + ZAR;
                } else {
                    //just the top 5 most traded and the NZD
                    return "AUD = " + AUD + "\n\nGBP = " + GBP + "\n\nEUR = " + EUR + "\n\nJPY = " + JPY + "\n\nNZD = " + NZD + "\n\nUSD = " + USD;
                        }
            }
        }

        public class RootObject
        {
            public string @base { get; set; }
            public string date { get; set; }
            public Rates rates { get; set; }
        }
    }
}