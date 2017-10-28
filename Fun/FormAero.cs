using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Aore_formda
{
    public partial class FormAero : Form
    {
        private const int WM_DWMCOMPOSITIONCHANGED = 0x031E;
        private const int DWM_BB_ENABLE = 0x1;

        [StructLayout(LayoutKind.Sequential)]
        private struct DWM_BLURBEHIND
        {
            public int dwFlags;
            public bool fEnable;
            public IntPtr hRgnBlur;
            public bool fTransitionOnMaximized;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }
        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND blurBehind);

        [DllImport("dwmapi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMargins);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern bool DwmIsCompositionEnabled();

        public Form1()
        {
            InitializeComponent();
            /*this.WindowStyle = System.Windows.WindowStyle.None;
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
            this.Background = Brushes.Transparent;
             
             */
        }
        
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DWMCOMPOSITIONCHANGED)
            {
                this.InitializeGlass(hwnd);
                handled = false;
            }

            return IntPtr.Zero;
        }


        private void InitializeGlass(IntPtr hwnd)
        {
            if (!DwmIsCompositionEnabled())
                return;

            // fill the background with glass
            var margins = new MARGINS();
            margins.cxLeftWidth = margins.cxRightWidth = margins.cyBottomHeight = margins.cyTopHeight = -1;
            DwmExtendFrameIntoClientArea(hwnd, ref margins);

            // initialize blur for the window
            DWM_BLURBEHIND bbh = new DWM_BLURBEHIND();
            bbh.fEnable = true;
            bbh.dwFlags = DWM_BB_ENABLE;
            DwmEnableBlurBehindWindow(hwnd, ref bbh);
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
