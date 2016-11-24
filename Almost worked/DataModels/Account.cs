using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Almost_worked
{
    public class Account
    {
        public string id;
        public string name;
        public double intRate;
        public double transactFee;

        public Account(string id, string name, double intRate, double transactFee)
        {
            this.id = id;
            this.name = name;
            this.intRate = intRate;
            this.transactFee = transactFee;
        }

        public override string ToString()
        {
            return $"{name} Account - has an interest rate of {intRate}% and a transaction fee of ${transactFee} per transaction";
        }
    }
}