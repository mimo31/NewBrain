using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewBrain.NativeFunctions
{
    public class Countdown
    {
        public DateTime EndTime { get; set; }
        public string CountingTo { get; set; }
        
        public Countdown(DateTime endTime, string countingTo)
        {
            this.EndTime = endTime;
            this.CountingTo = countingTo;
        }

        public TimeSpan GetRemaingTime()
        {
            return this.EndTime - DateTime.Now;
        }
    }
}
