namespace AnimatorNS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;

    [ProvideProperty("Decoration", typeof(Control))]
    public class Animator : Component, IExtenderProvider
    {
        private AnimatorNS.AnimationType animationType;
        private IContainer components;
        private readonly Dictionary<Control, DecorationControl> DecorationByControls;
        protected List<QueueItem> queue;
        private List<QueueItem> requests;
        private Thread thread;

        public event EventHandler AllAnimationsCompleted;

        public event EventHandler<AnimationCompletedEventArg> AnimationCompleted;

        public event EventHandler<PaintEventArgs> FramePainted;

        public event EventHandler<MouseEventArgs> MouseDown;

        public event EventHandler<NonLinearTransfromNeededEventArg> NonLinearTransfromNeeded;

        public event EventHandler<TransfromNeededEventArg> TransfromNeeded;

        public Animator()
        {
            this.components = null;
            this.queue = new List<QueueItem>();
            this.requests = new List<QueueItem>();
            this.DecorationByControls = new Dictionary<Control, DecorationControl>();
            this.Init();
        }

        public Animator(IContainer container)
        {
            this.components = null;
            this.queue = new List<QueueItem>();
            this.requests = new List<QueueItem>();
            this.DecorationByControls = new Dictionary<Control, DecorationControl>();
            container.Add(this);
            this.Init();
        }

        public void AddToQueue(Control control, AnimateMode mode, bool parallel = true, Animation animation = null, Rectangle clipRectangle = new Rectangle())
        {
            if (animation == null)
            {
                animation = this.DefaultAnimation;
            }
            if (control is IFakeControl)
            {
                control.Visible = false;
            }
            else
            {
                List<QueueItem> list;
                QueueItem item = new QueueItem {
                    animation = animation,
                    control = control,
                    IsActive = parallel,
                    mode = mode,
                    clipRectangle = clipRectangle
                };
                switch (mode)
                {
                    case AnimateMode.Show:
                    {
                        if (!control.Visible)
                        {
                            break;
                        }
                        QueueItem item2 = new QueueItem {
                            control = control,
                            mode = mode
                        };
                        this.OnCompleted(item2);
                        return;
                    }
                    case AnimateMode.Hide:
                    {
                        if (control.Visible)
                        {
                            break;
                        }
                        QueueItem item3 = new QueueItem {
                            control = control,
                            mode = mode
                        };
                        this.OnCompleted(item3);
                        return;
                    }
                }
                lock ((list = this.queue))
                {
                    this.queue.Add(item);
                }
                lock ((list = this.requests))
                {
                    this.requests.Add(item);
                }
            }
        }

        private void Animator_Disposed(object sender, EventArgs e)
        {
            this.ClearQueue();
            this.thread.Abort();
        }

        public void BeginUpdateSync(Control control, bool parallel = false, Animation animation = null, Rectangle clipRectangle = new Rectangle())
        {
            this.AddToQueue(control, AnimateMode.BeginUpdate, parallel, animation, clipRectangle);
            bool flag = false;
            do
            {
                flag = false;
                lock (this.queue)
                {
                    foreach (QueueItem item in this.queue)
                    {
                        if (((item.control == control) && (item.mode == AnimateMode.BeginUpdate)) && (item.controller == null))
                        {
                            flag = true;
                        }
                    }
                }
                if (flag)
                {
                    Application.DoEvents();
                }
            }
            while (flag);
        }

        public bool CanExtend(object extendee)
        {
            return (extendee is Control);
        }

        private void CheckRequests()
        {
            List<QueueItem> list = new List<QueueItem>();
            lock (this.requests)
            {
                Dictionary<Control, QueueItem> dictionary = new Dictionary<Control, QueueItem>();
                foreach (QueueItem item in this.requests)
                {
                    if (item.control != null)
                    {
                        if (dictionary.ContainsKey(item.control))
                        {
                            list.Add(dictionary[item.control]);
                        }
                        dictionary[item.control] = item;
                    }
                    else
                    {
                        list.Add(item);
                    }
                }
                foreach (QueueItem item in dictionary.Values)
                {
                    if (!((item.control == null) || this.IsStateOK(item.control, item.mode)))
                    {
                        this.RepairState(item.control, item.mode);
                    }
                    else
                    {
                        list.Add(item);
                    }
                }
                foreach (QueueItem item in list)
                {
                    this.requests.Remove(item);
                }
            }
        }

        public void ClearQueue()
        {
            List<QueueItem> list = null;
            lock (this.queue)
            {
                list = new List<QueueItem>(this.queue);
                this.queue.Clear();
            }
            using (List<QueueItem>.Enumerator enumerator = list.GetEnumerator())
            {
                MethodInvoker method = null;
                QueueItem item;
                while (enumerator.MoveNext())
                {
                    item = enumerator.Current;
                    if (item.control != null)
                    {
                        if (method == null)
                        {
                            method = delegate {
                                switch (item.mode)
                                {
                                    case AnimateMode.Show:
                                        item.control.Visible = true;
                                        break;

                                    case AnimateMode.Hide:
                                        item.control.Visible = false;
                                        break;
                                }
                            };
                        }
                        item.control.BeginInvoke(method);
                    }
                    AnimationCompletedEventArg e = new AnimationCompletedEventArg {
                        Animation = item.animation,
                        Control = item.control,
                        Mode = item.mode
                    };
                    this.OnAnimationCompleted(e);
                }
            }
            if (list.Count > 0)
            {
                this.OnAllAnimationsCompleted();
            }
        }

        private Controller CreateDoubleBitmap(Control control, AnimateMode mode, Animation animation, Rectangle clipRect)
        {
            Controller controller = new Controller(control, mode, animation, this.TimeStep, clipRect);
            controller.TransfromNeeded += new EventHandler<TransfromNeededEventArg>(this.OnTransformNeeded);
            controller.NonLinearTransfromNeeded += new EventHandler<NonLinearTransfromNeededEventArg>(this.OnNonLinearTransfromNeeded);
            controller.MouseDown += new EventHandler<MouseEventArgs>(this.OnMouseDown);
            controller.DoubleBitmap.Cursor = this.Cursor;
            controller.FramePainted += new EventHandler<PaintEventArgs>(this.OnFramePainted);
            return controller;
        }

        private void DoAnimation(QueueItem item)
        {
            if (Monitor.TryEnter(item))
            {
                try
                {
                    if (item.controller == null)
                    {
                        item.controller = this.CreateDoubleBitmap(item.control, item.mode, item.animation, item.clipRectangle);
                    }
                    if (!item.controller.IsCompleted)
                    {
                        item.controller.BuildNextFrame();
                    }
                }
                catch
                {
                    this.OnCompleted(item);
                }
            }
        }

        public void EndUpdate(Control control)
        {
            lock (this.queue)
            {
                foreach (QueueItem item in this.queue)
                {
                    if ((item.control == control) && (item.mode == AnimateMode.BeginUpdate))
                    {
                        item.controller.EndUpdate();
                        item.mode = AnimateMode.Update;
                    }
                }
            }
        }

        public void EndUpdateSync(Control control)
        {
            this.EndUpdate(control);
            this.WaitAnimation(control);
        }

        public DecorationType GetDecoration(Control control)
        {
            if (this.DecorationByControls.ContainsKey(control))
            {
                return this.DecorationByControls[control].DecorationType;
            }
            return DecorationType.None;
        }

        public void Hide(Control control, bool parallel = false, Animation animation = null)
        {
            this.AddToQueue(control, AnimateMode.Hide, parallel, animation, new Rectangle());
        }

        public void HideSync(Control control, bool parallel = false, Animation animation = null)
        {
            this.Hide(control, parallel, animation);
            this.WaitAnimation(control);
        }

        protected virtual void Init()
        {
            this.DefaultAnimation = new Animation();
            this.MaxAnimationTime = 0x5dc;
            this.TimeStep = 0.02f;
            this.Interval = 10;
            base.Disposed += new EventHandler(this.Animator_Disposed);
            this.thread = new Thread(new ThreadStart(this.Work));
            this.thread.IsBackground = true;
            this.thread.Start();
        }

        private void InitDefaultAnimation(AnimatorNS.AnimationType animationType)
        {
            switch (animationType)
            {
                case AnimatorNS.AnimationType.Rotate:
                    this.DefaultAnimation = Animation.Rotate;
                    break;

                case AnimatorNS.AnimationType.HorizSlide:
                    this.DefaultAnimation = Animation.HorizSlide;
                    break;

                case AnimatorNS.AnimationType.VertSlide:
                    this.DefaultAnimation = Animation.VertSlide;
                    break;

                case AnimatorNS.AnimationType.Scale:
                    this.DefaultAnimation = Animation.Scale;
                    break;

                case AnimatorNS.AnimationType.ScaleAndRotate:
                    this.DefaultAnimation = Animation.ScaleAndRotate;
                    break;

                case AnimatorNS.AnimationType.HorizSlideAndRotate:
                    this.DefaultAnimation = Animation.HorizSlideAndRotate;
                    break;

                case AnimatorNS.AnimationType.ScaleAndHorizSlide:
                    this.DefaultAnimation = Animation.ScaleAndHorizSlide;
                    break;

                case AnimatorNS.AnimationType.Transparent:
                    this.DefaultAnimation = Animation.Transparent;
                    break;

                case AnimatorNS.AnimationType.Leaf:
                    this.DefaultAnimation = Animation.Leaf;
                    break;

                case AnimatorNS.AnimationType.Mosaic:
                    this.DefaultAnimation = Animation.Mosaic;
                    break;

                case AnimatorNS.AnimationType.Particles:
                    this.DefaultAnimation = Animation.Particles;
                    break;

                case AnimatorNS.AnimationType.VertBlind:
                    this.DefaultAnimation = Animation.VertBlind;
                    break;

                case AnimatorNS.AnimationType.HorizBlind:
                    this.DefaultAnimation = Animation.HorizBlind;
                    break;
            }
        }

        private bool IsStateOK(Control control, AnimateMode mode)
        {
            switch (mode)
            {
                case AnimateMode.Show:
                    return control.Visible;

                case AnimateMode.Hide:
                    return !control.Visible;
            }
            return true;
        }

        protected virtual void OnAllAnimationsCompleted()
        {
            if (this.AllAnimationsCompleted != null)
            {
                this.AllAnimationsCompleted(this, EventArgs.Empty);
            }
        }

        protected virtual void OnAnimationCompleted(AnimationCompletedEventArg e)
        {
            if (this.AnimationCompleted != null)
            {
                this.AnimationCompleted(this, e);
            }
        }

        private void OnCompleted(QueueItem item)
        {
            if (item.controller != null)
            {
                item.controller.Dispose();
            }
            lock (this.queue)
            {
                this.queue.Remove(item);
            }
            AnimationCompletedEventArg e = new AnimationCompletedEventArg {
                Animation = item.animation,
                Control = item.control,
                Mode = item.mode
            };
            this.OnAnimationCompleted(e);
        }

        private void OnFramePainted(object sender, PaintEventArgs e)
        {
            if (this.FramePainted != null)
            {
                this.FramePainted(sender, e);
            }
        }

        protected virtual void OnMouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                Controller controller = (Controller) sender;
                Point location = e.Location;
                location.Offset(controller.DoubleBitmap.Left - controller.AnimatedControl.Left, controller.DoubleBitmap.Top - controller.AnimatedControl.Top);
                if (this.MouseDown != null)
                {
                    this.MouseDown(sender, new MouseEventArgs(e.Button, e.Clicks, location.X, location.Y, e.Delta));
                }
            }
            catch
            {
            }
        }

        protected virtual void OnNonLinearTransfromNeeded(object sender, NonLinearTransfromNeededEventArg e)
        {
            if (this.NonLinearTransfromNeeded != null)
            {
                this.NonLinearTransfromNeeded(this, e);
            }
            else
            {
                e.UseDefaultTransform = true;
            }
        }

        protected virtual void OnTransformNeeded(object sender, TransfromNeededEventArg e)
        {
            if (this.TransfromNeeded != null)
            {
                this.TransfromNeeded(this, e);
            }
            else
            {
                e.UseDefaultMatrix = true;
            }
        }
        delegate  void A();
        private void RepairState(Control control, AnimateMode mode)
        {/*
            
            control.BeginInvoke( {
                switch (mode)
                {
                    case AnimateMode.Show:
                        control.Visible = true;
                        break;

                    case AnimateMode.Hide:
                        control.Visible = false;
                        break;
                }
            });
          * */
        }

        public void SetDecoration(Control control, DecorationType decoration)
        {
            DecorationControl control2 = this.DecorationByControls.ContainsKey(control) ? this.DecorationByControls[control] : null;
            if (decoration == DecorationType.None)
            {
                if (control2 != null)
                {
                    control2.Dispose();
                }
                this.DecorationByControls.Remove(control);
            }
            else
            {
                if (control2 == null)
                {
                    control2 = new DecorationControl(decoration, control);
                }
                control2.DecorationType = decoration;
                this.DecorationByControls[control] = control2;
            }
        }

        public void Show(Control control, bool parallel = false, Animation animation = null)
        {
            this.AddToQueue(control, AnimateMode.Show, parallel, animation, new Rectangle());
        }

        public void ShowSync(Control control, bool parallel = false, Animation animation = null)
        {
            this.Show(control, parallel, animation);
            this.WaitAnimation(control);
        }

        public void WaitAllAnimations()
        {
            while (!this.IsCompleted)
            {
                Application.DoEvents();
            }
        }

        public void WaitAnimation(Control animatedControl)
        {
            bool flag;
            bool flag2;
            goto Label_007C;
        Label_006B:
            if (!flag)
            {
                return;
            }
            Application.DoEvents();
        Label_007C:
            flag2 = true;
            flag = false;
            lock (this.queue)
            {
                foreach (QueueItem item in this.queue)
                {
                    if (item.control == animatedControl)
                    {
                        flag = true;
                        goto Label_006B;
                    }
                }
            }
            goto Label_006B;
        }

        private void Work()
        {
            while (true)
            {
                Thread.Sleep(this.Interval);
                try
                {
                    int count = 0;
                    List<QueueItem> list = new List<QueueItem>();
                    List<QueueItem> list2 = new List<QueueItem>();
                    lock (this.queue)
                    {
                        count = this.queue.Count;
                        bool flag = false;
                        foreach (QueueItem item in this.queue)
                        {
                            if (item.IsActive)
                            {
                                flag = true;
                            }
                            if ((item.controller != null) && item.controller.IsCompleted)
                            {
                                list.Add(item);
                            }
                            else if (item.IsActive)
                            {
                                TimeSpan span = (TimeSpan) (DateTime.Now - item.ActivateTime);
                                if (span.TotalMilliseconds > this.MaxAnimationTime)
                                {
                                    list.Add(item);
                                }
                                else
                                {
                                    list2.Add(item);
                                }
                            }
                        }
                        if (!flag)
                        {
                            foreach (QueueItem item in this.queue)
                            {
                                if (!item.IsActive)
                                {
                                    list2.Add(item);
                                    item.IsActive = true;
                                    goto Label_017F;
                                }
                            }
                        }
                    }
                Label_017F:
                    foreach (QueueItem item in list)
                    {
                        this.OnCompleted(item);
                    }
                    using (List<QueueItem>.Enumerator enumerator = list2.GetEnumerator())
                    {
                        MethodInvoker method = null;
                        QueueItem item;
                        while (enumerator.MoveNext())
                        {
                            item = enumerator.Current;
                            try
                            {
                                if (method == null)
                                {
                                    method = () => this.DoAnimation(item);
                                }
                                item.control.BeginInvoke(method);
                            }
                            catch
                            {
                                this.OnCompleted(item);
                            }
                        }
                    }
                    if (count == 0)
                    {
                        if (list.Count > 0)
                        {
                            this.OnAllAnimationsCompleted();
                        }
                        this.CheckRequests();
                    }
                }
                catch
                {
                }
            }
        }

        public AnimatorNS.AnimationType AnimationType
        {
            get
            {
                return this.animationType;
            }
            set
            {
                this.animationType = value;
                this.InitDefaultAnimation(this.animationType);
            }
        }

        [DefaultValue(typeof(System.Windows.Forms.Cursor), "Default")]
        public System.Windows.Forms.Cursor Cursor { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Animation DefaultAnimation { get; set; }

        [DefaultValue(10)]
        public int Interval { get; set; }

        public bool IsCompleted
        {
            get
            {
                lock (this.queue)
                {
                    return (this.queue.Count == 0);
                }
            }
        }

        [DefaultValue(0x5dc)]
        public int MaxAnimationTime { get; set; }

        [DefaultValue((float) 0.02f)]
        public float TimeStep { get; set; }

        protected class QueueItem
        {
            public Animation animation;
            public Rectangle clipRectangle;
            public Control control;
            public Controller controller;
            public bool isActive;
            public AnimateMode mode;

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                if (this.control != null)
                {
                    builder.Append(this.control.GetType().Name + " ");
                }
                builder.Append(this.mode);
                return builder.ToString();
            }

            public DateTime ActivateTime { get; private set; }

            public bool IsActive
            {
                get
                {
                    return this.isActive;
                }
                set
                {
                    if (this.isActive != value)
                    {
                        this.isActive = value;
                        if (value)
                        {
                            this.ActivateTime = DateTime.Now;
                        }
                    }
                }
            }
        }
    }
}

