using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Core.Utility;
using OpenTK.Mathematics;

#nullable enable

namespace OpenTK.Core.Platform
{
    /// <summary>
    /// A set of platform abstraction layer components.
    /// </summary>
    public class ComponentSet : IClipboardComponent,
                                ICursorComponent,
                                IDisplayComponent,
                                IIconComponent,
                                IKeyboardComponent,
                                IMouseComponent,
                                IOpenGLComponent,
                                ISurfaceComponent,
                                IWindowComponent
    {
        // This class is a bunch of boilerplate some of which has been generated by the IDE.
        private IClipboardComponent? _clipboardComponent;
        private ICursorComponent? _cursorComponent;
        private IDisplayComponent? _displayComponent;
        private IIconComponent? _iconComponent;
        private IKeyboardComponent? _keyboardComponent;
        private IMouseComponent? _mouseComponent;
        private IOpenGLComponent? _openGLComponent;
        private ISurfaceComponent? _surfaceComponent;
        private IWindowComponent? _windowComponent;

        /// <summary>
        /// Indicated whether the component set has been initialized.
        /// </summary>
        /// <remarks>
        /// An initialized component set cannot be modified.
        /// </remarks>
        public bool Initialized { get; private set; } = false;

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                HashSet<string> names = new HashSet<string>();
                if (_cursorComponent is not null)
                {
                    names.Add(_cursorComponent.Name);
                }

                if (_displayComponent is not null)
                {
                    names.Add(_displayComponent.Name);
                }

                if (_iconComponent is not null)
                {
                    names.Add(_iconComponent.Name);
                }

                if (_keyboardComponent is not null)
                {
                    names.Add(_keyboardComponent.Name);
                }

                if (_mouseComponent is not null)
                {
                    names.Add(_mouseComponent.Name);
                }

                if (_openGLComponent is not null)
                {
                    names.Add(_openGLComponent.Name);
                }

                if (_surfaceComponent is not null)
                {
                    names.Add(_surfaceComponent.Name);
                }

                if (_windowComponent is not null)
                {
                    names.Add(_windowComponent.Name);
                }

                return $"Component Set [{string.Join(", ", names)}]";
            }
        }

        /// <inheritdoc/>
        public PalComponents Provides => (_cursorComponent is not null ? PalComponents.MouseCursor : 0) |
                                         (_displayComponent is not null ? PalComponents.Display : 0) |
                                         (_iconComponent is not null ? PalComponents.WindowIcon : 0) |
                                         (_keyboardComponent is not null ? PalComponents.KeyboardInput : 0) |
                                         (_mouseComponent is not null ? PalComponents.MouseCursor : 0) |
                                         (_openGLComponent is not null ? PalComponents.OpenGL : 0) |
                                         (_surfaceComponent is not null ? PalComponents.Surface : 0) |
                                         (_windowComponent is not null ? PalComponents.Window : 0);

        private ILogger? _logger;

        /// <inheritdoc/>
        public ILogger? Logger
        {
            get => _logger;
            set
            {
                _logger = value;

#pragma warning disable SA1503 // Braces should not be omitted

                if (_clipboardComponent != null) _clipboardComponent.Logger = _logger;

                if (_cursorComponent != null) _cursorComponent.Logger = _logger;

                if (_displayComponent != null) _displayComponent.Logger = _logger;

                if (_iconComponent != null) _iconComponent.Logger = _logger;

                if (_keyboardComponent != null) _keyboardComponent.Logger = _logger;

                if (_mouseComponent != null) _mouseComponent.Logger = _logger;

                if (_openGLComponent != null) _openGLComponent.Logger = _logger;

                if (_surfaceComponent != null) _surfaceComponent.Logger = _logger;

                if (_windowComponent != null) _windowComponent.Logger = _logger;

#pragma warning restore SA1503 // Braces should not be omitted
            }
        }

        /// <summary>
        /// Get or set which components are in the set.
        /// </summary>
        /// <param name="which">The component group.</param>
        /// <exception cref="NotImplementedException">Not implemented, yet.</exception>
        /// <exception cref="ArgumentException">The given <paramref name="which"/> enum should only contain bit set for get.</exception>
        /// <exception cref="PalException">Raised when the set is modified after initialization.</exception>
        public IPalComponent? this[PalComponents which]
        {
            get => which switch
            {
                PalComponents.ControllerInput => throw new NotImplementedException(),
                PalComponents.Display => _displayComponent,
                PalComponents.KeyboardInput => _keyboardComponent,
                PalComponents.MiceInput => _mouseComponent,
                PalComponents.MouseCursor => _cursorComponent,
                PalComponents.Surface => _surfaceComponent,
                PalComponents.Vulkan => throw new NotImplementedException(),
                PalComponents.Window => _windowComponent,
                PalComponents.WindowIcon => _iconComponent,
                PalComponents.OpenGL => _openGLComponent,
                PalComponents.Clipboard => _clipboardComponent,
                _ => throw new ArgumentException("Components are a bitfield or out of range.", nameof(which))
            };
            set
            {
                if (Initialized)
                {
                    throw new PalException(this, "Cannot change set after components are initialized.");
                }

                if ((which & PalComponents.Display) != 0)
                {
                    _displayComponent = value as IDisplayComponent;
                }
                if ((which & PalComponents.KeyboardInput) != 0)
                {
                    _keyboardComponent = value as IKeyboardComponent;
                }
                if ((which & PalComponents.MiceInput) != 0)
                {
                    _mouseComponent = value as IMouseComponent;
                }
                if ((which & PalComponents.MouseCursor) != 0)
                {
                    _cursorComponent = value as ICursorComponent;
                }
                if ((which & PalComponents.Surface) != 0)
                {
                    _surfaceComponent = value as ISurfaceComponent;
                }
                if ((which & PalComponents.Window) != 0)
                {
                    _windowComponent = value as IWindowComponent;
                }
                if ((which & PalComponents.WindowIcon) != 0)
                {
                    _iconComponent = value as IIconComponent;
                }
                if ((which & PalComponents.OpenGL) != 0)
                {
                    _openGLComponent = value as IOpenGLComponent;
                }
                if ((which & PalComponents.Clipboard) != 0)
                {
                    _clipboardComponent = value as IClipboardComponent;
                }
            }
        }

        /// <inheritdoc/>
        void IPalComponent.Initialize(PalComponents which)
        {
            if ((which & ~Provides) != 0)
            {
                throw new PalException(this, $"Platform does not support requested features.")
                {
                    Data =
                    {
                        ["Requested"] = which,
                        ["Supported"] = Provides
                    }
                };
            }

            _cursorComponent?.Initialize(which & PalComponents.MouseCursor);
            _displayComponent?.Initialize(which & PalComponents.Display);
            _iconComponent?.Initialize(which & PalComponents.WindowIcon);
            _keyboardComponent?.Initialize(which & PalComponents.KeyboardInput);
            _mouseComponent?.Initialize(which & PalComponents.MiceInput);
            _surfaceComponent?.Initialize(which & PalComponents.Surface);
            _windowComponent?.Initialize(which & PalComponents.Window);
            _openGLComponent?.Initialize(which & PalComponents.OpenGL);
            _clipboardComponent?.Initialize(which & PalComponents.Clipboard);

            Initialized = true;
        }

        IReadOnlyList<ClipboardFormat> IClipboardComponent.SupportedFormats => _clipboardComponent!.SupportedFormats;

        ClipboardFormat IClipboardComponent.GetClipboardFormat()
        {
            return _clipboardComponent!.GetClipboardFormat();
        }

        void IClipboardComponent.SetClipboardText(string text)
        {
            _clipboardComponent!.SetClipboardText(text);
        }

        string? IClipboardComponent.GetClipboardText()
        {
            return _clipboardComponent!.GetClipboardText();
        }

        AudioData? IClipboardComponent.GetClipboardAudio()
        {
            return _clipboardComponent!.GetClipboardAudio();
        }

        Bitmap? IClipboardComponent.GetClipboardBitmap()
        {
            return _clipboardComponent!.GetClipboardBitmap();
        }

        string? IClipboardComponent.GetClipboardHTML()
        {
            return _clipboardComponent!.GetClipboardHTML();
        }

        List<string>? IClipboardComponent.GetClipboardFiles()
        {
            return _clipboardComponent!.GetClipboardFiles();
        }

        /// <inheritdoc/>
        bool ICursorComponent.CanLoadFromFile => _cursorComponent!.CanLoadFromFile;

        /// <inheritdoc/>
        bool ICursorComponent.CanLoadSystemCursor => _cursorComponent!.CanLoadSystemCursor;

        /// <inheritdoc/>
        bool ICursorComponent.CanScaleCursor => _cursorComponent!.CanScaleCursor;

        /// <inheritdoc/>
        bool ICursorComponent.CanSupportAnimatedCursor => _cursorComponent!.CanSupportAnimatedCursor;

        /// <inheritdoc/>
        bool IIconComponent.CanLoadFile => _iconComponent!.CanLoadFile;

        /// <inheritdoc/>
        bool IIconComponent.CanLoadSystemIcon => _iconComponent!.CanLoadSystemIcon;

        /// <inheritdoc/>
        bool IIconComponent.HasMipmaps => _iconComponent!.HasMipmaps;

        /// <inheritdoc/>
        bool IWindowComponent.CanSetIcon => _windowComponent!.CanSetIcon;

        /// <inheritdoc/>
        bool IWindowComponent.CanGetDisplay => _windowComponent!.CanGetDisplay;

        /// <inheritdoc/>
        bool IWindowComponent.CanSetCursor => _windowComponent!.CanSetCursor;

        /// <inheritdoc/>
        IReadOnlyList<PlatformEventType> IWindowComponent.SupportedEvents => _windowComponent!.SupportedEvents;

        /// <inheritdoc/>
        IReadOnlyList<WindowStyle> IWindowComponent.SupportedStyles => _windowComponent!.SupportedStyles;

        /// <inheritdoc/>
        IReadOnlyList<WindowMode> IWindowComponent.SupportedModes => _windowComponent!.SupportedModes;

        /// <inheritdoc/>
        WindowHandle IWindowComponent.Create(GraphicsApiHints hints)
        {
            return _windowComponent!.Create(hints);
        }

        /// <inheritdoc/>
        void IWindowComponent.Destroy(WindowHandle handle)
        {
            _windowComponent!.Destroy(handle);
        }

        /// <inheritdoc/>
        string IWindowComponent.GetTitle(WindowHandle handle)
        {
            return _windowComponent!.GetTitle(handle);
        }

        /// <inheritdoc/>
        void IWindowComponent.SetTitle(WindowHandle handle, string title)
        {
            _windowComponent!.SetTitle(handle, title);
        }

        /// <inheritdoc/>
        IconHandle IWindowComponent.GetIcon(WindowHandle handle)
        {
            return _windowComponent!.GetIcon(handle);
        }

        /// <inheritdoc/>
        void IWindowComponent.SetIcon(WindowHandle handle, IconHandle icon)
        {
            _windowComponent!.SetIcon(handle, icon);
        }

        /// <inheritdoc/>
        void IWindowComponent.GetPosition(WindowHandle handle, out int x, out int y)
        {
            _windowComponent!.GetPosition(handle, out x, out y);
        }

        /// <inheritdoc/>
        void IWindowComponent.SetPosition(WindowHandle handle, int x, int y)
        {
            _windowComponent!.SetPosition(handle, x, y);
        }

        /// <inheritdoc/>
        void IWindowComponent.GetSize(WindowHandle handle, out int width, out int height)
        {
            _windowComponent!.GetSize(handle, out width, out height);
        }

        /// <inheritdoc/>
        void IWindowComponent.SetSize(WindowHandle handle, int width, int height)
        {
            _windowComponent!.SetSize(handle, width, height);
        }

        /// <inheritdoc/>
        void IWindowComponent.GetClientPosition(WindowHandle handle, out int x, out int y)
        {
            _windowComponent!.GetClientPosition(handle, out x, out y);
        }

        /// <inheritdoc/>
        void IWindowComponent.SetClientPosition(WindowHandle handle, int x, int y)
        {
            _windowComponent!.SetClientPosition(handle, x, y);
        }

        /// <inheritdoc/>
        void IWindowComponent.GetClientSize(WindowHandle handle, out int width, out int height)
        {
            _windowComponent!.GetClientSize(handle, out width, out height);
        }

        /// <inheritdoc/>
        void IWindowComponent.SetClientSize(WindowHandle handle, int width, int height)
        {
            _windowComponent!.SetClientSize(handle, width, height);
        }

        /// <inheritdoc/>
        void IWindowComponent.GetMaxClientSize(WindowHandle handle, out int? width, out int? height)
        {
            _windowComponent!.GetMaxClientSize(handle, out width, out height);
        }

        /// <inheritdoc/>
        void IWindowComponent.SetMaxClientSize(WindowHandle handle, int? width, int? height)
        {
            _windowComponent!.SetMaxClientSize(handle, width, height);
        }

        /// <inheritdoc/>
        void IWindowComponent.GetMinClientSize(WindowHandle handle, out int? width, out int? height)
        {
            _windowComponent!.GetMinClientSize(handle, out width, out height);
        }

        /// <inheritdoc/>
        void IWindowComponent.SetMinClientSize(WindowHandle handle, int? width, int? height)
        {
            _windowComponent!.SetMinClientSize(handle, width, height);
        }

        /// <inheritdoc/>
        DisplayHandle IWindowComponent.GetDisplay(WindowHandle handle)
        {
            return _windowComponent!.GetDisplay(handle);
        }

        /// <inheritdoc/>
        WindowMode IWindowComponent.GetMode(WindowHandle handle)
        {
            return _windowComponent!.GetMode(handle);
        }

        /// <inheritdoc/>
        void IWindowComponent.SetMode(WindowHandle handle, WindowMode mode)
        {
            _windowComponent!.SetMode(handle, mode);
        }

        /// <inheritdoc/>
        WindowStyle IWindowComponent.GetBorderStyle(WindowHandle handle)
        {
            return _windowComponent!.GetBorderStyle(handle);
        }

        /// <inheritdoc/>
        void IWindowComponent.SetBorderStyle(WindowHandle handle, WindowStyle style)
        {
            _windowComponent!.SetBorderStyle(handle, style);
        }

        /// <inheritdoc/>
        void IWindowComponent.SetAlwaysOnTop(WindowHandle handle, bool floating)
        {
            _windowComponent!.SetAlwaysOnTop(handle, floating);
        }

        /// <inheritdoc/>
        bool IWindowComponent.IsAlwaysOnTop(WindowHandle handle)
        {
            return _windowComponent!.IsAlwaysOnTop(handle);
        }

        /// <inheritdoc/>
        void IWindowComponent.SetCursor(WindowHandle handle, CursorHandle cursor)
        {
            _windowComponent!.SetCursor(handle, cursor);
        }

        /// <inheritdoc/>
        void IWindowComponent.FocusWindow(WindowHandle handle)
        {
            _windowComponent!.FocusWindow(handle);
        }

        /// <inheritdoc/>
        void IWindowComponent.RequestAttention(WindowHandle handle)
        {
            _windowComponent!.RequestAttention(handle);
        }

        /// <inheritdoc/>
        void IWindowComponent.ScreenToClient(WindowHandle handle, int x, int y, out int clientX, out int clientY)
        {
            _windowComponent!.ScreenToClient(handle, x, y, out clientX, out clientY);
        }

        /// <inheritdoc/>
        void IWindowComponent.ClientToScreen(WindowHandle handle, int clientX, int clientY, out int x, out int y)
        {
            _windowComponent!.ClientToScreen(handle, clientX, clientY, out x, out y);
        }

        void IWindowComponent.SwapBuffers(WindowHandle handle)
        {
            _windowComponent!.SwapBuffers(handle);
        }

        /// <inheritdoc/>
        IEventQueue<PlatformEventType, WindowEventArgs> IWindowComponent.GetEventQueue(WindowHandle handle)
        {
            return _windowComponent!.GetEventQueue(handle);
        }

        /// <inheritdoc/>
        IconHandle IIconComponent.Create()
        {
            return _iconComponent!.Create();
        }

        /// <inheritdoc/>
        void ISurfaceComponent.Destroy(SurfaceHandle handle)
        {
            _surfaceComponent!.Destroy(handle);
        }

        /// <inheritdoc/>
        SurfaceType ISurfaceComponent.GetType(SurfaceHandle handle)
        {
            return _surfaceComponent!.GetType(handle);
        }

        /// <inheritdoc/>
        DisplayHandle ISurfaceComponent.GetDisplay(SurfaceHandle handle)
        {
            return _surfaceComponent!.GetDisplay(handle);
        }

        /// <inheritdoc/>
        void ISurfaceComponent.SetDisplay(SurfaceHandle handle, DisplayHandle display)
        {
            _surfaceComponent!.SetDisplay(handle, display);
        }

        /// <inheritdoc/>
        void ISurfaceComponent.GetClientSize(SurfaceHandle handle, out int width, out int height)
        {
            _surfaceComponent!.GetClientSize(handle, out width, out height);
        }

        /// <inheritdoc/>
        IEventQueue<PlatformEventType, WindowEventArgs> ISurfaceComponent.GetEventQueue(SurfaceHandle handle)
        {
            return _surfaceComponent!.GetEventQueue(handle);
        }

        /// <inheritdoc/>
        void IIconComponent.Destroy(IconHandle handle)
        {
            _iconComponent!.Destroy(handle);
        }

        /// <inheritdoc/>
        void IIconComponent.GenerateMipmaps(IconHandle handle)
        {
            _iconComponent!.GenerateMipmaps(handle);
        }

        /// <inheritdoc/>
        void IIconComponent.GetDimensions(IconHandle handle, out int width, out int height)
        {
            _iconComponent!.GetDimensions(handle, out width, out height);
        }

        /// <inheritdoc/>
        void IIconComponent.GetDimensions(IconHandle handle, int level, out int width, out int height)
        {
            _iconComponent!.GetDimensions(handle, level, out width, out height);
        }

        /// <inheritdoc/>
        void IIconComponent.GetBitmap(IconHandle handle, Span<byte> data)
        {
            _iconComponent!.GetBitmap(handle, data);
        }

        /// <inheritdoc/>
        void IIconComponent.GetBitmap(IconHandle handle, int level, Span<byte> data)
        {
            _iconComponent!.GetBitmap(handle, level, data);
        }

        /// <inheritdoc/>
        int IIconComponent.GetBitmapSize(IconHandle handle)
        {
            return _iconComponent!.GetBitmapSize(handle);
        }

        /// <inheritdoc/>
        int IIconComponent.GetBitmapSize(IconHandle handle, int level)
        {
            return _iconComponent!.GetBitmapSize(handle, level);
        }

        /// <inheritdoc/>
        void IIconComponent.Load(IconHandle handle, int width, int height, ReadOnlySpan<byte> data)
        {
            _iconComponent!.Load(handle, width, height, data);
        }

        /// <inheritdoc/>
        void IIconComponent.Load(IconHandle handle, int width, int height, ReadOnlySpan<byte> data, int level)
        {
            _iconComponent!.Load(handle, width, height, data, level);
        }

        /// <inheritdoc/>
        void IIconComponent.Load(IconHandle handle, string file)
        {
            _iconComponent!.Load(handle, file);
        }

        /// <inheritdoc/>
        void IIconComponent.Load(IconHandle handle, Stream stream)
        {
            _iconComponent!.Load(handle, stream);
        }

        /// <inheritdoc/>
        void IIconComponent.Load(IconHandle handle, SystemIconType name)
        {
            _iconComponent!.Load(handle, name);
        }

        /// <inheritdoc/>
        CursorHandle ICursorComponent.Create()
        {
            return _cursorComponent!.Create();
        }

        /// <inheritdoc/>
        void ICursorComponent.Destroy(CursorHandle handle)
        {
            _cursorComponent!.Destroy(handle);
        }

        /// <inheritdoc/>
        void ICursorComponent.GetSize(CursorHandle handle, out int width, out int height)
        {
            _cursorComponent!.GetSize(handle, out width, out height);
        }

        /// <inheritdoc/>
        void ICursorComponent.GetHotspot(CursorHandle handle, out int x, out int y)
        {
            _cursorComponent!.GetHotspot(handle, out x, out y);
        }

        /// <inheritdoc/>
        void ICursorComponent.GetImage(CursorHandle handle, Span<byte> image)
        {
            _cursorComponent!.GetImage(handle, image);
        }

        /// <inheritdoc/>
        void ICursorComponent.GetScale(CursorHandle handle, out float horizontal, out float vertical)
        {
            _cursorComponent!.GetScale(handle, out horizontal, out vertical);
        }

        /// <inheritdoc/>
        void ICursorComponent.Load(CursorHandle handle, SystemCursorType systemCursor)
        {
            _cursorComponent!.Load(handle, systemCursor);
        }

        /// <inheritdoc/>
        void ICursorComponent.Load(CursorHandle handle, int width, int height, ReadOnlySpan<byte> image)
        {
            _cursorComponent!.Load(handle, width, height, image);
        }

        /// <inheritdoc/>
        void ICursorComponent.Load(CursorHandle handle, int width, int height, ReadOnlySpan<byte> colorData, ReadOnlySpan<byte> maskData)
        {
            _cursorComponent!.Load(handle, width, height, colorData, maskData);
        }

        /// <inheritdoc/>
        void ICursorComponent.Load(CursorHandle handle, string file)
        {
            _cursorComponent!.Load(handle, file);
        }

        /// <inheritdoc/>
        void ICursorComponent.Load(CursorHandle handle, Stream stream)
        {
            _cursorComponent!.Load(handle, stream);
        }

        /// <inheritdoc/>
        void ICursorComponent.SetHotspot(CursorHandle handle, int x, int y)
        {
            _cursorComponent!.SetHotspot(handle, x, y);
        }

        /// <inheritdoc/>
        void ICursorComponent.SetScale(CursorHandle handle, float horizontal, float vertical)
        {
            _cursorComponent!.SetScale(handle, horizontal, vertical);
        }

        /// <inheritdoc/>
        bool IDisplayComponent.CanSetVideoMode => _displayComponent!.CanSetVideoMode;

        /// <inheritdoc/>
        bool IDisplayComponent.CanGetVirtualPosition => _displayComponent!.CanGetVirtualPosition;

        /// <inheritdoc/>
        int IDisplayComponent.GetDisplayCount()
        {
            return _displayComponent!.GetDisplayCount();
        }

        /// <inheritdoc/>
        bool IMouseComponent.IsMultiMouse => _mouseComponent!.IsMultiMouse;

        /// <inheritdoc/>
        int IMouseComponent.GetMouseCount()
        {
            return _mouseComponent!.GetMouseCount();
        }

        /// <inheritdoc/>
        MouseHandle IMouseComponent.Create(int index)
        {
            return _mouseComponent!.Create(index);
        }

        /// <inheritdoc/>
        void IMouseComponent.Destroy(MouseHandle handle)
        {
            _mouseComponent!.Destroy(handle);
        }

        /// <inheritdoc/>
        void IMouseComponent.GetPosition(MouseHandle handle, out int x, out int y)
        {
            _mouseComponent!.GetPosition(handle, out x, out y);
        }

        /// <inheritdoc/>
        void IMouseComponent.SetPosition(MouseHandle handle, int x, int y)
        {
            _mouseComponent!.SetPosition(handle, x, y);
        }

        /// <inheritdoc/>
        DisplayHandle IDisplayComponent.Create(int index)
        {
            return _displayComponent!.Create(index);
        }

        /// <inheritdoc/>
        DisplayHandle IDisplayComponent.CreatePrimary()
        {
            return _displayComponent!.CreatePrimary();
        }

        /// <inheritdoc/>
        void IDisplayComponent.Destroy(DisplayHandle handle)
        {
            _displayComponent!.Destroy(handle);
        }

        /// <inheritdoc/>
        bool IDisplayComponent.IsPrimary(DisplayHandle handle)
        {
            return _displayComponent!.IsPrimary(handle);
        }

        /// <inheritdoc/>
        string IDisplayComponent.GetName(DisplayHandle handle)
        {
            return _displayComponent!.GetName(handle);
        }

        /// <inheritdoc/>
        void IDisplayComponent.GetVideoMode(DisplayHandle handle, out VideoMode mode)
        {
            _displayComponent!.GetVideoMode(handle, out mode);
        }

        /// <inheritdoc/>
        void IDisplayComponent.SetVideoMode(DisplayHandle handle, in VideoMode mode)
        {
            _displayComponent!.SetVideoMode(handle, in mode);
        }

        /// <inheritdoc/>
        int IDisplayComponent.GetSupportedVideoModeCount(DisplayHandle handle)
        {
            return _displayComponent!.GetSupportedVideoModeCount(handle);
        }

        /// <inheritdoc/>
        void IDisplayComponent.GetSupportedVideoModes(DisplayHandle handle, Span<VideoMode> modes)
        {
            _displayComponent!.GetSupportedVideoModes(handle, modes);
        }

        /// <inheritdoc/>
        void IDisplayComponent.GetVirtualPosition(DisplayHandle handle, out int x, out int y)
        {
            _displayComponent!.GetVirtualPosition(handle, out x, out y);
        }

        /// <inheritdoc/>
        void IDisplayComponent.GetResolution(DisplayHandle handle, out int width, out int height)
        {
            _displayComponent!.GetResolution(handle, out width, out height);
        }

        /// <inheritdoc/>
        void IDisplayComponent.GetWorkArea(DisplayHandle handle, out Box2i area)
        {
            _displayComponent!.GetWorkArea(handle, out area);
        }

        /// <inheritdoc/>
        void IDisplayComponent.GetRefreshRate(DisplayHandle handle, out float refreshRate)
        {
            _displayComponent!.GetRefreshRate(handle, out refreshRate);
        }

        /// <inheritdoc/>
        void IDisplayComponent.GetDisplayScale(DisplayHandle handle, out float scaleX, out float scaleY)
        {
            _displayComponent!.GetDisplayScale(handle, out scaleX, out scaleY);
        }

        /// <inheritdoc/>
        bool IKeyboardComponent.SupportsLayouts => _keyboardComponent!.SupportsLayouts;

        /// <inheritdoc/>
        bool IKeyboardComponent.SupportsIme => _keyboardComponent!.SupportsIme;

        /// <inheritdoc/>
        string IKeyboardComponent.GetActiveKeyboardLayout(WindowHandle handle)
        {
            return _keyboardComponent!.GetActiveKeyboardLayout(handle);
        }

        /// <inheritdoc/>
        string[] IKeyboardComponent.GetAvailableKeyboardLayouts()
        {
            return _keyboardComponent!.GetAvailableKeyboardLayouts();
        }

        /// <inheritdoc/>
        void IKeyboardComponent.BeginIme(WindowHandle window)
        {
            _keyboardComponent!.BeginIme(window);
        }

        /// <inheritdoc/>
        void IKeyboardComponent.SetImeRectangle(WindowHandle window, int x, int y, int width, int height)
        {
            _keyboardComponent!.SetImeRectangle(window, x, y, width, height);
        }

        /// <inheritdoc/>
        void IKeyboardComponent.EndIme(WindowHandle window)
        {
            _keyboardComponent!.EndIme(window);
        }

        /// <inheritdoc/>
        bool IOpenGLComponent.CanShareContexts => _openGLComponent!.CanShareContexts;

        /// <inheritdoc/>
        bool IOpenGLComponent.CanCreateFromWindow => _openGLComponent!.CanCreateFromWindow;

        /// <inheritdoc/>
        bool IOpenGLComponent.CanCreateFromSurface => _openGLComponent!.CanCreateFromSurface;

        /// <inheritdoc/>
        OpenGLContextHandle IOpenGLComponent.CreateFromSurface()
        {
            return _openGLComponent!.CreateFromSurface();
        }

        /// <inheritdoc/>
        OpenGLContextHandle IOpenGLComponent.CreateFromWindow(WindowHandle handle)
        {
            return _openGLComponent!.CreateFromWindow(handle);
        }

        /// <inheritdoc/>
        void IOpenGLComponent.DestroyContext(OpenGLContextHandle handle)
        {
            _openGLComponent!.DestroyContext(handle);
        }

        /// <inheritdoc/>
        IntPtr IOpenGLComponent.GetProcedureAddress(OpenGLContextHandle handle, string procedureName)
        {
            return _openGLComponent!.GetProcedureAddress(handle, procedureName);
        }

        /// <inheritdoc/>
        OpenGLContextHandle? IOpenGLComponent.GetCurrentContext()
        {
            return _openGLComponent!.GetCurrentContext();
        }

        /// <inheritdoc/>
        bool IOpenGLComponent.SetCurrentContext(OpenGLContextHandle? handle)
        {
            return _openGLComponent!.SetCurrentContext(handle);
        }

        /// <inheritdoc/>
        OpenGLContextHandle? IOpenGLComponent.GetSharedContext(OpenGLContextHandle handle)
        {
            return _openGLComponent!.GetSharedContext(handle);
        }

        /// <inheritdoc/>
        void IOpenGLComponent.SetSwapInterval(int interval)
        {
            _openGLComponent!.SetSwapInterval(interval);
        }

        /// <inheritdoc/>
        int IOpenGLComponent.GetSwapInterval()
        {
            return _openGLComponent!.GetSwapInterval();
        }

        /// <inheritdoc/>
        SurfaceHandle ISurfaceComponent.Create()
        {
            return _surfaceComponent!.Create();
        }
    }
}
