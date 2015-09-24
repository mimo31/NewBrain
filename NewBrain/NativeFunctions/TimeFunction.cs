using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewBrain.NativeFunctions
{
    public class TimeFunction : Function
    {
        private readonly string[] PossibleQuestions = { "what's the time?", "time", "tell me the time please" , "tell me the time", "time please" }; 

        public override void Ask(MainForm senderForm, string text)
        {
            DateTime time = DateTime.Now;
            senderForm.MainOutputLabel.Text = "It's " + DateTime.Now.TimeOfDay + ".";
            senderForm.Speech.SpeakAsync("It's " + time.Hour.ToString() + " hours, " + time.Minute.ToString() + " minutes and " + time.Second.ToString() + " seconds.");
        }

        public override bool IsAsked(string text)
        {
            string lowerText = text.ToLower();
            if (lowerText[lowerText.Length - 1].Equals('.'))
            {
                lowerText = lowerText.Substring(0, text.Length - 1);
            }
            if (PossibleQuestions.Contains(lowerText))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
