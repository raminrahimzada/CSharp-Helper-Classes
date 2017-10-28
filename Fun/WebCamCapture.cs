using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Kamera
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.Run(new Form1());
        }
    }
    [Designer("Sytem.Windows.Forms.Design.ParentControlDesigner,System.Design", typeof(IDesigner)), ToolboxBitmap(typeof(WebCamCapture), "CAMERA.ICO")]
    public class WebCamCapture : UserControl
    {
        private bool bStopped = true;
        private IContainer components;
        private WebcamEventArgs x = new WebcamEventArgs();
        private ulong m_FrameNumber = 0L;
        private int m_Height = 500;
        private int m_TimeToCapture_milliseconds = 100;
        private int m_Width = 500;
        private int mCapHwnd;
        private Image tempImg;
        private IDataObject tempObj;
        private System.Windows.Forms.Timer timer1;
        public const int WM_CAP_CONNECT = 0x40a;
        public const int WM_CAP_COPY = 0x41e;
        public const int WM_CAP_DISCONNECT = 0x40b;
        public const int WM_CAP_DLG_VIDEOCOMPRESSION = 0x42e;
        public const int WM_CAP_DLG_VIDEODISPLAY = 0x42b;
        public const int WM_CAP_DLG_VIDEOFORMAT = 0x429;
        public const int WM_CAP_DLG_VIDEOSOURCE = 0x42a;
        public const int WM_CAP_GET_FRAME = 0x43c;
        public const int WM_CAP_GET_VIDEOFORMAT = 0x42c;
        public const int WM_CAP_SET_PREVIEW = 0x432;
        public const int WM_CAP_SET_VIDEOFORMAT = 0x42d;
        public const int WM_CAP_START = 0x400;
        public const int WM_USER = 0x400;

        public event WebCamEventHandler ImageCaptured;

        public WebCamCapture()
        {
            this.InitializeComponent();
        }

        #region dllimport metodlari
        [DllImport("user32")]
        public static extern int OpenClipboard(int hWnd);
        [DllImport("user32")]
        public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);
        [DllImport("avicap32.dll")]
        public static extern int capCreateCaptureWindowA(string lpszWindowName, int dwStyle, int X, int Y, int nWidth, int nHeight, int hwndParent, int nID);
        [DllImport("user32")]
        public static extern int CloseClipboard();
        [DllImport("user32")]
        public static extern int EmptyClipboard();
        #endregion
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }


        ~WebCamCapture()
        {
            this.Stop();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // WebCamCapture
            // 
            this.Name = "WebCamCapture";
            this.Size = new Size(500, 500);
            this.Load += new System.EventHandler(this.WebCamCapture_Load);
            this.ResumeLayout(false);

        }


        public void Start(ulong FrameNum)
        {
            try
            {
                this.Stop();
                this.mCapHwnd = capCreateCaptureWindowA("WebCap", 0, 0, 0, this.m_Width, this.m_Height, base.Handle.ToInt32(), 0);
                Application.DoEvents();
                SendMessage(this.mCapHwnd, 0x40a, 0, 0);
                SendMessage(this.mCapHwnd, 0x432, 0, 0);
                this.m_FrameNumber = FrameNum;
                this.timer1.Interval = this.m_TimeToCapture_milliseconds;
                this.bStopped = false;
                this.timer1.Start();
            }
            catch (Exception exception)
            {
                //MessageBox.Show("An error ocurred while starting the video capture. Check that your webcamera is connected properly and turned on.\r\n\n" + exception.Message);
                //this.Stop();
                this.Start(m_FrameNumber);
            }
        }

        public void Stop()
        {
            try
            {
                this.bStopped = true;
                this.timer1.Stop();
                Application.DoEvents();
                SendMessage(this.mCapHwnd, 0x40b, 0, 0);
            }
            catch (Exception)
            {
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                this.timer1.Stop();
                SendMessage(this.mCapHwnd, 0x43c, 0, 0);
                SendMessage(this.mCapHwnd, 0x41e, 0, 0);
                if (this.ImageCaptured != null)
                {
                    this.tempObj = Clipboard.GetDataObject();
                    this.tempImg = (Bitmap)this.tempObj.GetData(DataFormats.Bitmap);
                    this.x.WebCamImage = this.tempImg.GetThumbnailImage(this.m_Width, this.m_Height, null, IntPtr.Zero);
                    this.ImageCaptured(this, this.x);
                }
                Application.DoEvents();
                if (!this.bStopped)
                {
                    this.timer1.Start();
                }
            }
            catch (Exception exception)
            {
                //this.Start(m_FrameNumber);
                //MessageBox.Show("An error ocurred while capturing the video image. The video capture will now be terminated.\r\n\n" + exception.Message);
                //this.Stop();

            }
        }

        public int CaptureHeight
        {
            get
            {
                return this.m_Height;
            }
            set
            {
                this.m_Height = value;
            }
        }

        public int CaptureWidth
        {
            get
            {
                return this.m_Width;
            }
            set
            {
                this.m_Width = value;
            }
        }

        public ulong FrameNumber
        {
            get
            {
                return this.m_FrameNumber;
            }
            set
            {
                this.m_FrameNumber = value;
            }
        }

        public int TimeToCapture_milliseconds
        {
            get
            {
                return this.m_TimeToCapture_milliseconds;
            }
            set
            {
                this.m_TimeToCapture_milliseconds = value;
            }
        }

        public delegate void WebCamEventHandler(object source, WebcamEventArgs e);

        private void WebCamCapture_Load(object sender, EventArgs e)
        {

        }
    }

    public class WebcamEventArgs : EventArgs
    {
        private ulong m_FrameNumber = 0L;
        private Image m_Image;
        public ulong FrameNumber
        {
            get
            {
                return this.m_FrameNumber;
            }
            set
            {
                this.m_FrameNumber = value;
            }
        }
        public Image WebCamImage
        {
            get
            {
                return this.m_Image;
            }
            set
            {
                this.m_Image = value;
            }
        }
    }

    public partial class Form1 : Form
    {
        private Button cmdContinue;
        private Button cmdStart;
        private Button cmdStop;
        Container components = null;
        private Label label1;
        private NumericUpDown numCaptureTime;
        private PictureBox pictureBox1;
        private WebCamCapture UserControl1;
        private WebCamCapture WebCamCapture;

        // Methods
        public Form1()
        {
            this.InitializeComponent();
        }

        private void cmdContinue_Click(object sender, EventArgs e)
        {
            this.WebCamCapture.TimeToCapture_milliseconds = (int)this.numCaptureTime.Value;
            this.WebCamCapture.Start(this.WebCamCapture.FrameNumber);
        }

        private void cmdStart_Click(object sender, EventArgs e)
        {
            this.WebCamCapture.TimeToCapture_milliseconds = (int)this.numCaptureTime.Value;
            this.WebCamCapture.Start(0L);
        }

        private void cmdStop_Click(object sender, EventArgs e)
        {
            this.WebCamCapture.Stop();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Form1_Closing(object sender, CancelEventArgs e)
        {
            this.WebCamCapture.Stop();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WebCamCapture.CaptureHeight = this.pictureBox1.Height;
            this.WebCamCapture.CaptureWidth = this.pictureBox1.Width;
        }

        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cmdStart = new System.Windows.Forms.Button();
            this.cmdStop = new System.Windows.Forms.Button();
            this.cmdContinue = new System.Windows.Forms.Button();
            this.numCaptureTime = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.WebCamCapture = new  WebCamCapture();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCaptureTime)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(6, 6);
            this.pictureBox1.Name = "pictureBox1";
            //this.pictureBox1.Size = new System.Drawing.Size(907, 240);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // cmdStart
            // 
            this.cmdStart.Location = new System.Drawing.Point(23, 25);
            this.cmdStart.Name = "cmdStart";
            this.cmdStart.Size = new System.Drawing.Size(78, 24);
            this.cmdStart.TabIndex = 1;
            this.cmdStart.Text = "Start";
            this.cmdStart.Click += new System.EventHandler(this.cmdStart_Click);
            // 
            // cmdStop
            // 
            this.cmdStop.Location = new System.Drawing.Point(102, 264);
            this.cmdStop.Name = "cmdStop";
            this.cmdStop.Size = new System.Drawing.Size(78, 24);
            this.cmdStop.TabIndex = 2;
            this.cmdStop.Text = "Stop";
            this.cmdStop.Visible = false;
            this.cmdStop.Click += new System.EventHandler(this.cmdStop_Click);
            // 
            // cmdContinue
            // 
            this.cmdContinue.Location = new System.Drawing.Point(198, 264);
            this.cmdContinue.Name = "cmdContinue";
            this.cmdContinue.Size = new System.Drawing.Size(78, 24);
            this.cmdContinue.TabIndex = 3;
            this.cmdContinue.Text = "Continue";
            this.cmdContinue.Visible = false;
            this.cmdContinue.Click += new System.EventHandler(this.cmdContinue_Click);
            // 
            // numCaptureTime
            // 
            this.numCaptureTime.Location = new System.Drawing.Point(162, 306);
            this.numCaptureTime.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numCaptureTime.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numCaptureTime.Name = "numCaptureTime";
            this.numCaptureTime.Size = new System.Drawing.Size(66, 20);
            this.numCaptureTime.TabIndex = 4;
            this.numCaptureTime.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numCaptureTime.Visible = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 312);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 18);
            this.label1.TabIndex = 5;
            this.label1.Text = "Capture Time (Milliseconds)";
            this.label1.Visible = false;
            // 
            // WebCamCapture
            // 
            this.WebCamCapture.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.WebCamCapture.CaptureHeight = 240;
            this.WebCamCapture.CaptureWidth = 320;
            this.WebCamCapture.FrameNumber = ((ulong)(0ul));
            this.WebCamCapture.Location = new System.Drawing.Point(17, 17);
            this.WebCamCapture.Name = "WebCamCapture";
            this.WebCamCapture.Size = new System.Drawing.Size(500, 500);
            this.WebCamCapture.TabIndex = 0;
            this.WebCamCapture.TimeToCapture_milliseconds = 100;
            this.WebCamCapture.Visible = false;
            this.WebCamCapture.ImageCaptured += new  WebCamCapture.WebCamEventHandler(this.WebCamCapture_ImageCaptured);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            ///this.ClientSize = new System.Drawing.Size(955, 357);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numCaptureTime);
            this.Controls.Add(this.cmdContinue);
            this.Controls.Add(this.cmdStop);
            this.Controls.Add(this.cmdStart);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "WebCam Capture";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCaptureTime)).EndInit();
            this.ResumeLayout(false);
            this.Resize += (a, v) => { this.pictureBox1.Size = this.Size; this.Text = this.Size + "-" + this.WebCamCapture.Size; };

        }


        private void WebCamCapture_ImageCaptured(object source, WebcamEventArgs e)
        {
            this.pictureBox1.Image = e.WebCamImage;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
