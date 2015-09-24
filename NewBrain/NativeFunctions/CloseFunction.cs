using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewBrain.NativeFunctions
{
    public class CloseFunction : Function
    {
        private readonly string[] PossibleQuestions = {"close", "close please", "close now"};

        public override void Ask(MainForm senderForm, string text)
        {
            if (!text.ToLower().Equals("close now"))
            {
                senderForm.OnSpeechCompleted += () => Application.Exit();
                senderForm.WriteAndSay("Goodbye!");
            }
            else
            {
                Application.Exit();
            }
        }

        public override bool IsAsked(string text)
        {
            if (this.PossibleQuestions.Contains(text.ToLower()))
            {
                return true;
            }
            return false;
        }
    }
}
