namespace AnimatorNS
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public class TransfromNeededEventArg : EventArgs
    {
        public TransfromNeededEventArg()
        {
            this.Matrix = new System.Drawing.Drawing2D.Matrix(1f, 0f, 0f, 1f, 0f, 0f);
        }

        public AnimatorNS.Animation Animation { get; set; }

        public Rectangle ClientRectangle { get; internal set; }

        public Rectangle ClipRectangle { get; internal set; }

        public System.Windows.Forms.Control Control { get; internal set; }

        public float CurrentTime { get; internal set; }

        public System.Drawing.Drawing2D.Matrix Matrix { get; set; }

        public AnimateMode Mode { get; internal set; }

        public bool UseDefaultMatrix { get; set; }
    }
}

