using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewBrain
{
    public abstract class Function
    {
        public abstract bool IsAsked(string text);

        public abstract void Ask(MainForm senderForm, string text);
    }
}
