using System;
using System.Text;
using System.Collections.Generic;

namespace CrappyPrizm
{
    public static class Convert
    {
        public static decimal CoinsToAmount(decimal coins) => coins / 100;
        public static decimal AmountToCoins(decimal amount) => amount * 100;
    }
}
