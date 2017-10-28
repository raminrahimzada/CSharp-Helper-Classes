namespace AnimatorNS
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public class AnimationCompletedEventArg : EventArgs
    {
        public AnimatorNS.Animation Animation { get; set; }

        public System.Windows.Forms.Control Control { get; internal set; }

        public AnimateMode Mode { get; internal set; }
    }
}

