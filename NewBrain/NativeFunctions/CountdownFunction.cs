using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewBrain.NativeFunctions
{
    public class CountdownFunction : Function
    {
        public override void Ask(MainForm senderForm, string text)
        {
            string reducedText = text.ToLower();
            if (reducedText.StartsWith("start countdown to"))
            {
                reducedText = reducedText.Substring(18);
            }
            else if (reducedText.StartsWith("set countdown to"))
            {
                reducedText = reducedText.Substring(16);
            }
            else
            {
                reducedText = reducedText.Substring(12);
            }
            reducedText = reducedText.Trim();
            reducedText = reducedText.Replace(" ", "");
            if (reducedText.All(c => char.IsDigit(c) || c == ':'))
            {
                string[] timeValues = reducedText.Split(':');
                if (timeValues.Length == 2 || timeValues.Length == 3)
                {
                    int seconds;
                    if (timeValues.Length == 3)
                    {
                        seconds = int.Parse(timeValues[2]);
                    }
                    else
                    {
                        seconds = 0;
                    }
                    DateTime now = DateTime.Now;
                    DateTime endTime = new DateTime(now.Year, now.Month, now.Day, int.Parse(timeValues[0]), int.Parse(timeValues[1]), seconds);
                    Countdown countdown = new Countdown(endTime, "something");
                    senderForm.AddCountdown(countdown);
                    senderForm.WriteAndSay("Countdown set.");
                }
                else
                {
                    senderForm.WriteAndSay("You can't specify " + timeValues.Length.ToString() + " time values.");
                }
            }
            else
            {
                senderForm.WriteAndSay("Sorry, but you entered a bad time format.");
            }
        }

        public override bool IsAsked(string text)
        {
            string lowerText = text.ToLower();
            if (lowerText.StartsWith("start countdown to") || lowerText.StartsWith("set countdown to") || lowerText.StartsWith("countdown to"))
            {
                return true;
            }
            return false;
        }
    }
}
