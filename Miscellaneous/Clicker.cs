using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Click_Bot_by_Mathman
{
    class Clicker
    {
        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, uint dx, uint dy, int dwData, int dwExtraInfo);

        public static void SetCursorPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public static void SetCursorPosition(MousePoint point)
        {
            SetCursorPos(point.X, point.Y);
        }

        public static MousePoint GetCursorPosition()
        {
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return currentMousePoint;
        }

        public static void MouseEvent(MouseEventFlags value)
        {
            var position = GetCursorPosition();

            mouse_event
                ((int)value,
                 (uint)position.X,
                 (uint)position.Y,
                 0,
                 0)
                ;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint
        {
            public int X;
            public int Y;

            public MousePoint(int x, int y)
            {
                X = x;
                Y = y;
            }

        }

        void Tikla(uint x, uint y, MouseEventType[] types)
        {
            SetCursorPosition((int)x, (int)y);
            var t = (int)types[0];
            for (var i = 1; i < types.Length; i++)
            {
                t |= (int)types[i];
            }
            mouse_event(t, x, y, 0, 0);
        }

        public enum NumberType
        {
            Millisecond,
            Times
        }

        Stopwatch _clock = Stopwatch.StartNew();

        [Flags]
        public enum MouseEventType
        {
            MouseEventLeftDown = 0x02,
            MouseEventLeftUp = 0x04,
            MouseEventRightDown = 0x08,
            MouseEventRightUp = 0x10
        }

        private int _time;
        private int _clickCount;
        private double interval;

        public double Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        private NumberType _type;
        private int _currClickCount = 0;
        public void SetLast(int number, NumberType type)
        {
            _type = type;
            switch (type)
            {
                case NumberType.Millisecond:
                    _time = number;
                    break;
                case NumberType.Times:
                    _clickCount = number;
                    break;
            }
        }

        private double _lastClickTime;
        private Timer timer;
        private bool stopped;
        private MouseEventType[] eventtypes;

        public MouseEventType[] EventTypes
        {
            get { return eventtypes; }
            set { eventtypes = value; }
        }

        private Point clickPoint;

        public Point ClickPoint
        {
            get { return clickPoint; }
            set { clickPoint = value; }
        }
        public void Start()
        {
            _clock = Stopwatch.StartNew();
            timer = new Timer { Interval = 1 };
            timer.Tick += (a, b) => DoAction(eventtypes, ClickPoint);
            stopped = false;
            timer.Start();
        }

        public void Stop()
        {
            stopped = true;
            _clock.Stop();
            timer.Stop();
        }
        public void DoAction(MouseEventType[] _types_, Point _point_)
        {
            if (stopped)
            {
                return;
            }
            if (_currClickCount > _clickCount)
            {
                return;
            }
            var cari = _clock.Elapsed.TotalSeconds;
            if (!(cari - _lastClickTime > interval)) return;
            _lastClickTime = cari;
            _currClickCount++;
            Tikla((uint)_point_.X, (uint)_point_.Y, _types_);
        }
    }
}
