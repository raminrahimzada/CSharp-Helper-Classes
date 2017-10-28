namespace AnimatorNS
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class TabControlEx : TabControl
    {
        private Animator animator;
        private IContainer components = null;

        public TabControlEx()
        {
            this.InitializeComponent();
            this.animator = new Animator();
            this.animator.AnimationType = AnimationType.VertSlide;
            this.animator.DefaultAnimation.TimeCoeff = 1f;
            this.animator.DefaultAnimation.AnimateOnlyDifferences = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
        }

        protected override void OnSelecting(TabControlCancelEventArgs e)
        {
            MethodInvoker method = null;
            try
            {
                base.OnSelecting(e);
                this.animator.BeginUpdateSync(this, false, null, new Rectangle(0, base.ItemSize.Height + 3, base.Width, (base.Height - base.ItemSize.Height) - 3));
                if (method == null)
                {
                    method = () => this.animator.EndUpdate(this);
                }
                base.BeginInvoke(method);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public AnimatorNS.Animation Animation
        {
            get
            {
                return this.animator.DefaultAnimation;
            }
            set
            {
                this.animator.DefaultAnimation = value;
            }
        }
    }
}

