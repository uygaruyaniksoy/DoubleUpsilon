
using System;

namespace DoubleUpsilon {
    public static class StatisticsUtilities {
        public static bool Percent(double percent) {
            var randomValue = new Random().Next(0, 100);
            return randomValue < percent;
        }
        
    }
}