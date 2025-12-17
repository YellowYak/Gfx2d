using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Gfx2d.LevelEditor
{
    public class SdlHost : HwndHost
    {
        private IntPtr hwndHost;
        private const int WS_CHILD = 0x40000000;
        private const int WS_VISIBLE = 0x10000000;
        private const int HOST_ID = 0x00000002;

        private int width;
        private int height;

        public SdlHost(double width, double height)
        {
            this.width = (int)width;
            this.height = (int)height;
        }

        public IntPtr GetHandle() => hwndHost;

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            // Create a child window
            hwndHost = CreateWindowEx(
                0,
                "static",
                "",
                WS_CHILD | WS_VISIBLE,
                0, 0,
                width, height,
                hwndParent.Handle,
                (IntPtr)HOST_ID,
                IntPtr.Zero,
                0
            );

            return new HandleRef(this, hwndHost);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            DestroyWindow(hwnd.Handle);
        }

        #region Win32 API Imports
        [DllImport("user32.dll", EntryPoint = "CreateWindowEx", CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateWindowEx(
            int dwExStyle,
            string lpszClassName,
            string lpszWindowName,
            int style,
            int x, int y,
            int width, int height,
            IntPtr hwndParent,
            IntPtr hMenu,
            IntPtr hInst,
            int lpParam
        );

        [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Unicode)]
        private static extern bool DestroyWindow(IntPtr hwnd);
        #endregion
    }
}