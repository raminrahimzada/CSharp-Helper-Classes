namespace AnimatorNS
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public interface IFakeControl
    {
        event EventHandler<PaintEventArgs> FramePainted;

        event EventHandler<PaintEventArgs> FramePainting;

        event EventHandler<TransfromNeededEventArg> TransfromNeeded;

        void InitParent(Control animatedControl, Padding padding);

        Bitmap BgBmp { get; set; }

        Bitmap Frame { get; set; }
    }
}

