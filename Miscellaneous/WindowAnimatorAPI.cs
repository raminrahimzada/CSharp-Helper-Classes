using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace Matrix
{
    public class WinAPI
    {
        /// <summary>
        /// Açılan pəncərənin soldan sağa doğru istiqamətlədirərək ekrana çıxmasını təmin edir
        /// </summary>
        public const int Soldan_Sağa = 0X1;
        /// <summary>
        /// Animates the window from right to left. 
        /// This flag can be used with roll or slide animation.
        /// </summary>
        public const int Sağdan_Sola = 0X2;
        /// <summary>
        /// Animates the window from top to bottom. 
        /// This flag can be used with roll or slide animation.
        /// </summary>
        public const int Yuxaridan_Aşağıya = 0X4;
        /// <summary>
        /// Animates the window from bottom to top. 
        /// This flag can be used with roll or slide animation.
        /// </summary>
        public const int AW_VER_NEGATIVE = 0X8;
        /// <summary>
        /// Makes the window appear to collapse inward 
        /// if AW_HIDE is used or expand outward if the AW_HIDE is not used.
        /// </summary>
        public const int Aşağıdan_Yuxarıya = 0X10;
        /// <summary>
        /// Hides the window. By default, the window is shown.
        /// </summary>
        public const int AW_HIDE = 0X10000;
        /// <summary>
        /// Activates the window.
        /// </summary>
        public const int AW_ACTIVATE = 0X20000;
        /// <summary>
        /// Uses slide animation. By default, roll animation is used.
        /// </summary>
        public const int AW_SLIDE = 0X40000;
        /// <summary>
        /// Uses a fade effect. 
        /// This flag can be used only if hwnd is a top-level window.
        /// </summary>
        public const int Fade_Effekt = 0X80000;
        /// <summary>
        /// Animates a window.
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int AnimateWindow(IntPtr hwand, int dwTime, int dwFlags);
    } 
}
