namespace AnimatorNS
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public class Animation
    {
        public Animation()
        {
            this.MinTime = 0f;
            this.MaxTime = 1f;
            this.AnimateOnlyDifferences = true;
        }

        public void Add(Animation a)
        {
            this.SlideCoeff = new PointF(this.SlideCoeff.X + a.SlideCoeff.X, this.SlideCoeff.Y + a.SlideCoeff.Y);
            this.RotateCoeff += a.RotateCoeff;
            this.RotateLimit += a.RotateLimit;
            this.ScaleCoeff = new PointF(this.ScaleCoeff.X + a.ScaleCoeff.X, this.ScaleCoeff.Y + a.ScaleCoeff.Y);
            this.TransparencyCoeff += a.TransparencyCoeff;
            this.LeafCoeff += a.LeafCoeff;
            this.MosaicShift = new PointF(this.MosaicShift.X + a.MosaicShift.X, this.MosaicShift.Y + a.MosaicShift.Y);
            this.MosaicCoeff = new PointF(this.MosaicCoeff.X + a.MosaicCoeff.X, this.MosaicCoeff.Y + a.MosaicCoeff.Y);
            this.MosaicSize += a.MosaicSize;
            this.BlindCoeff = new PointF(this.BlindCoeff.X + a.BlindCoeff.X, this.BlindCoeff.Y + a.BlindCoeff.Y);
            this.TimeCoeff += a.TimeCoeff;
            this.Padding += a.Padding;
        }

        public Animation Clone()
        {
            return (Animation) base.MemberwiseClone();
        }

        public bool AnimateOnlyDifferences { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), TypeConverter(typeof(PointFConverter)), EditorBrowsable(EditorBrowsableState.Advanced)]
        public PointF BlindCoeff { get; set; }

        public static Animation HorizBlind
        {
            get
            {
                return new Animation { BlindCoeff = new PointF(1f, 0f) };
            }
        }

        public static Animation HorizSlide
        {
            get
            {
                return new Animation { SlideCoeff = new PointF(1f, 0f) };
            }
        }

        public static Animation HorizSlideAndRotate
        {
            get
            {
                return new Animation { SlideCoeff = new PointF(1f, 0f), RotateCoeff = 0.3f, RotateLimit = 0.2f, Padding = new System.Windows.Forms.Padding(50, 50, 50, 50) };
            }
        }

        public static Animation Leaf
        {
            get
            {
                return new Animation { LeafCoeff = 1f };
            }
        }

        public float LeafCoeff { get; set; }

        public float MaxTime { get; set; }

        public float MinTime { get; set; }

        public static Animation Mosaic
        {
            get
            {
                return new Animation { MosaicCoeff = new PointF(100f, 100f), MosaicSize = 20, Padding = new System.Windows.Forms.Padding(30, 30, 30, 30) };
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), TypeConverter(typeof(PointFConverter))]
        public PointF MosaicCoeff { get; set; }

        [TypeConverter(typeof(PointFConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), EditorBrowsable(EditorBrowsableState.Advanced)]
        public PointF MosaicShift { get; set; }

        public int MosaicSize { get; set; }

        public System.Windows.Forms.Padding Padding { get; set; }

        public static Animation Particles
        {
            get
            {
                return new Animation { MosaicCoeff = new PointF(200f, 200f), MosaicSize = 1, MosaicShift = new PointF(0f, 0.5f), Padding = new System.Windows.Forms.Padding(100, 50, 100, 150), TimeCoeff = 2f };
            }
        }

        public static Animation Rotate
        {
            get
            {
                return new Animation { RotateCoeff = 1f, TransparencyCoeff = 1f, Padding = new System.Windows.Forms.Padding(50, 50, 50, 50) };
            }
        }

        public float RotateCoeff { get; set; }

        public float RotateLimit { get; set; }

        public static Animation Scale
        {
            get
            {
                return new Animation { ScaleCoeff = new PointF(1f, 1f) };
            }
        }

        public static Animation ScaleAndHorizSlide
        {
            get
            {
                return new Animation { ScaleCoeff = new PointF(1f, 1f), SlideCoeff = new PointF(1f, 0f), Padding = new System.Windows.Forms.Padding(30, 0, 0, 0) };
            }
        }

        public static Animation ScaleAndRotate
        {
            get
            {
                return new Animation { ScaleCoeff = new PointF(1f, 1f), RotateCoeff = 0.5f, RotateLimit = 0.2f, Padding = new System.Windows.Forms.Padding(30, 30, 30, 30) };
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), EditorBrowsable(EditorBrowsableState.Advanced), TypeConverter(typeof(PointFConverter))]
        public PointF ScaleCoeff { get; set; }

        [TypeConverter(typeof(PointFConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), EditorBrowsable(EditorBrowsableState.Advanced)]
        public PointF SlideCoeff { get; set; }

        public float TimeCoeff { get; set; }

        public float TransparencyCoeff { get; set; }

        public static Animation Transparent
        {
            get
            {
                return new Animation { TransparencyCoeff = 1f };
            }
        }

        public static Animation VertBlind
        {
            get
            {
                return new Animation { BlindCoeff = new PointF(0f, 1f) };
            }
        }

        public static Animation VertSlide
        {
            get
            {
                return new Animation { SlideCoeff = new PointF(0f, 1f) };
            }
        }
    }
}

