namespace AnimatorNS
{
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public class NonLinearTransfromNeededEventArg : EventArgs
    {
        public AnimatorNS.Animation Animation { get; set; }

        public Rectangle ClientRectangle { get; internal set; }

        public System.Windows.Forms.Control Control { get; internal set; }

        public float CurrentTime { get; internal set; }

        public AnimateMode Mode { get; internal set; }

        public byte[] Pixels { get; internal set; }

        public Rectangle SourceClientRectangle { get; internal set; }

        public byte[] SourcePixels { get; internal set; }

        public int SourceStride { get; set; }

        public int Stride { get; internal set; }

        public bool UseDefaultTransform { get; set; }
    }
}

