using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewBrain.NativeFunctions
{
    public class LockFunction : Function
    {
        private readonly string[] PossibleQuestions = { "lock", "lock please" };

        public override void Ask(MainForm senderForm, string text)
        {
            senderForm.Locked = true;
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
