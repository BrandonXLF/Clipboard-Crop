using System;
using System.Windows;

namespace ClipboardCrop {
    public struct SavedState {
        public double Left;
        public double Top;
        public double Width;
        public double Height;
        public bool Maximized;
    }

    public class WindowStateManager {
        private readonly double DEFAULT_WIDTH;
        private readonly double DEFAULT_HEIGHT;

        public WindowStateManager(double defaultWidth, double DefaultHeight) {
            DEFAULT_WIDTH = defaultWidth;
            DEFAULT_HEIGHT = DefaultHeight;
        }

        private static bool StateWithinDisplay(SavedState state) {
            return state.Left >= SystemParameters.VirtualScreenLeft &&
                state.Top >= SystemParameters.VirtualScreenTop &&
                state.Left + state.Width <= SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth &&
                state.Top + state.Height <= SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight;
        }

        public static void Save(Window win) {
            bool maximized = win.WindowState == WindowState.Maximized;
            Rect currentBounds;

            if (maximized) {
                currentBounds = win.RestoreBounds;
            } else {
                currentBounds = new Rect(win.Left, win.Top, win.Width, win.Height);
            }

            Properties.Settings.Default.WinLeft = currentBounds.Left;
            Properties.Settings.Default.WinTop = currentBounds.Top;
            Properties.Settings.Default.WinWidth = currentBounds.Width;
            Properties.Settings.Default.WinHeight = currentBounds.Height;
            Properties.Settings.Default.WinMaximized = maximized;
            Properties.Settings.Default.Save();
        }

        private void NormalizeState(ref SavedState state) {
            if (state.Width == -1) {
                state.Width = Math.Min(SystemParameters.PrimaryScreenWidth, DEFAULT_WIDTH);
            }

            if (state.Height == -1) {
                state.Height = Math.Min(SystemParameters.PrimaryScreenHeight, DEFAULT_HEIGHT);
            }

            if (state.Left == -1) {
                state.Left = SystemParameters.PrimaryScreenWidth / 2 - (state.Width / 2);
            }

            if (state.Top == -1) {
                state.Top = SystemParameters.PrimaryScreenHeight / 2 - (state.Height / 2);
            }
        }

        private SavedState GetState() {
            SavedState state = new() {
                Left = Properties.Settings.Default.WinLeft,
                Top = Properties.Settings.Default.WinTop,
                Height = Properties.Settings.Default.WinHeight,
                Width = Properties.Settings.Default.WinWidth,
                Maximized = Properties.Settings.Default.WinMaximized
            };

            NormalizeState(ref state);

            return state;
        }

        public void Restore(Window win) {
            SavedState state = GetState();

            if (!StateWithinDisplay(state)) {
                // Try to centre on primary display while perserving width and height
                state.Left = -1;
                state.Top = -1;

                NormalizeState(ref state);
            }

            if (!StateWithinDisplay(state)) {
                // Reset everything
                state.Left = -1;
                state.Top = -1;
                state.Width = -1;
                state.Height = -1;

                NormalizeState(ref state);
            }

            win.Left = state.Left;
            win.Top = state.Top;
            win.Width = state.Width;
            win.Height = state.Height;
            win.WindowState = state.Maximized ? WindowState.Maximized : WindowState.Normal;
        }

        public void Connect(Window win) {
            if (win.IsInitialized) {
                Restore(win);
            } else {
                win.SourceInitialized += new EventHandler((_, _) => Restore(win));
            }

            win.LocationChanged += new EventHandler((_, _) => Save(win));
        }
    }
}
