using System;
using System.Runtime.InteropServices;

namespace OpenTK.Platform.Native.X11
{
    /// <summary>
    /// Wrapper for the native library libX11.so.
    /// </summary>
    public static class LibX11
    {
        private const string X11 = "X11";

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern XDisplayPtr XOpenDisplay([MarshalAs(UnmanagedType.LPStr)]string? name);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern int XDefaultScreen(XDisplayPtr display);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong XBlackPixel(XDisplayPtr display, int screenNumber);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong XWhitePixel(XDisplayPtr display, int screenNumber);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern XWindow XCreateSimpleWindow(
            XDisplayPtr display,
            XWindow parent,
            int x,
            int y,
            uint width,
            uint height,
            ulong border,
            ulong background);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern XWindow XCreateWindow(
            XDisplayPtr display,
            XWindow parent,
            int x,
            int y,
            uint width,
            uint height,
            uint border,
            int depth,
            uint @class,
            ref XVisual visual,
            XWindowAttributeValueMask valueMask,
            ref XSetWindowAttributes attributes);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern int XSelectInput(XDisplayPtr display, XWindow xWindow, XEventMask events);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern XWindow XDefaultRootWindow(XDisplayPtr display);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern void XSetStandardProperties(
            XDisplayPtr display,
            XWindow window,
            [MarshalAs(UnmanagedType.LPStr)]string windowName,
            [MarshalAs(UnmanagedType.LPStr)]string iconName,
            XPixMap iconPixmap,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[]? argv,
            int argc,
            ref XSizeHints hints);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern XGC XCreateGC(XDisplayPtr display, XDrawable drawable, long valueMask,
            IntPtr values);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern int XSetBackground(XDisplayPtr display, XGC gc, ulong background);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern int XSetForeground(XDisplayPtr display, XGC gc, ulong background);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern int XClearWindow(XDisplayPtr display, XWindow window);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern int XMapRaised(XDisplayPtr display, XWindow window);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern int XFreeGC(XDisplayPtr display, XGC gc);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern int XDestroyWindow(XDisplayPtr display, XWindow window);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern int XCloseDisplay(XDisplayPtr display);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern int XNextEvent(XDisplayPtr display, out XEvent @event);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern int XFree(IntPtr pointer);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern XColorMap XCreateColormap(
            XDisplayPtr display,
            XWindow window,
            ref XVisual visual,
            int alloc);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern int XFreeColormap(XDisplayPtr display, XColorMap colormap);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern int XStoreName(XDisplayPtr display, XWindow window, [MarshalAs(UnmanagedType.LPStr)]string name);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern int XGetWindowAttributes(
            XDisplayPtr display,
            XWindow window,
            out XWindowAttributes attributes);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern int XTranslateCoordinates(
            XDisplayPtr display,
            XWindow source,
            XWindow destination,
            int sourceX,
            int sourceY,
            out int destinationX,
            out int destinationY,
            out XWindow child);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern int XSendEvent(
            XDisplayPtr display,
            XWindow window,
            int propagate,
            XEventMask eventMask,
            in XEvent ea);


        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern int XEventsQueued(XDisplayPtr display, XEventsQueuedMode mode);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int XFetchName(XDisplayPtr display, XWindow window, out byte* name);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern XAtom XInternAtom(XDisplayPtr display, string atomName, bool onlyIfExists);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern int XInternAtoms(
            XDisplayPtr display,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 3)]
            string[] names,
            int count,
            [MarshalAs(UnmanagedType.I4)] bool onlyIfExists,
            [Out, MarshalAs(UnmanagedType.LPArray)] XAtom[] atoms);


        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr XGetAtomName(XDisplayPtr display, XAtom atom);

        [DllImport(X11, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int XGetWindowProperty(
            XDisplayPtr display,
            XWindow window,
            XAtom property,
            long offset,
            long length,
            bool delete,
            XAtom requestType,
            out XAtom actualType,
            out int actualFormat,
            out long numberOfItems,
            out long remainingBytes,
            out IntPtr contents
        );
    }
}
