﻿

using OpenTK.Core.Platform;
using OpenTK.Platform.Native.Windows;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Text;
using System.Diagnostics;

namespace LocalTestProject
{
    public class Program
    {
        static WindowComponent windowComp = new WindowComponent();
        static WindowHandle WindowHandle;
        static WindowHandle WindowHandle2;

        static IOpenGLComponent glComp = new OpenGLComponent();
        static OpenGLContextHandle WindowContext;
        static OpenGLContextHandle Window2Context;

        static DisplayHandle PrimaryDisplayHandle;

        static IMouseComponent mouseComp = new MouseComponent();
        static ICursorComponent cursorComp = new CursorComponent();
        static IIconComponent iconComp = new IconComponent();

        static IDisplayComponent dispComp = new DisplayComponent();

        static IKeyboardComponent keyboardComp = new KeyboardComponent();

        static IClipboardComponent clipComp = new ClipboardComponent();

        static CursorHandle CursorHandle;
        static CursorHandle ImageCursorHandle;
        static CursorHandle FileCursorHandle;

        static IconHandle IconHandle;
        static IconHandle IconHandle2;

        public static void Main(string[] args)
        {
            windowComp.Initialize(PalComponents.Window);
            glComp.Initialize(PalComponents.OpenGL);

            dispComp.Initialize(PalComponents.Display);

            keyboardComp.Initialize(PalComponents.KeyboardInput);

            iconComp.Initialize(PalComponents.WindowIcon);
            cursorComp.Initialize(PalComponents.MouseCursor);

            clipComp.Initialize(PalComponents.Clipboard);

            Console.WriteLine($"Current Keyboard Layout name: {keyboardComp.GetActiveKeyboardLayout()}");

            Console.WriteLine($"Available Keyboard Layouts:\n  {string.Join("\n  ", keyboardComp.GetAvailableKeyboardLayouts())}");

            {
                PrimaryDisplayHandle = dispComp.CreatePrimary();
                string name = dispComp.GetName(PrimaryDisplayHandle);
                dispComp.GetVideoMode(PrimaryDisplayHandle, out VideoMode videoMode);
                dispComp.GetDisplayScale(PrimaryDisplayHandle, out float scaleX, out float scaleY);
                Console.WriteLine($"Primary monitor name: {name}");
                Console.WriteLine($"  Resoltion: {videoMode.HorizontalResolution}x{videoMode.VerticalResolution}");
                Console.WriteLine($"  Refresh rate: {videoMode.RefreshRate}");
                Console.WriteLine($"  Scale: {videoMode.Scale}");
                Console.WriteLine($"  Dpi: {videoMode.Dpi}");
                Console.WriteLine($"  Scale2: {scaleX}, {scaleY}");

                int modeCount = dispComp.GetSupportedVideoModeCount(PrimaryDisplayHandle);
                Console.WriteLine($"Primary monitor supports {modeCount} video modes.");

                if (dispComp.GetDisplayCount() > 1)
                {
                    var secondaryHandle = dispComp.Create(1);
                    modeCount = dispComp.GetSupportedVideoModeCount(secondaryHandle);
                    Console.WriteLine($"Secondary monitor supports {modeCount} video modes.");
                }
                
                Console.WriteLine();
            }

            Console.WriteLine($"Monitors: {dispComp.GetDisplayCount()}");
            for (int i = 0; i < dispComp.GetDisplayCount(); i++)
            {
                DisplayHandle disp = dispComp.Create(i);

                string name = dispComp.GetName(disp);
                dispComp.GetVideoMode(disp, out VideoMode videoMode);
                dispComp.GetDisplayScale(disp, out float scaleX, out float scaleY);
                Console.WriteLine($"Primary monitor name: {name}");
                Console.WriteLine($"  Resoltion: {videoMode.HorizontalResolution}x{videoMode.VerticalResolution}");
                Console.WriteLine($"  Refresh rate: {videoMode.RefreshRate}");
                Console.WriteLine($"  Scale: {videoMode.Scale}");
                Console.WriteLine($"  Dpi: {videoMode.Dpi}");
                Console.WriteLine($"  Scale2: {scaleX}, {scaleY}");
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();

            OpenGLGraphicsApiHints contextSettings = new OpenGLGraphicsApiHints()
            {
                DoubleBuffer = true,
                sRGBFramebuffer = false,
                Multisamples = 0,
                DepthBits = ContextDepthBits.Depth24,
                StencilBits = ContextStencilBits.Stencil8,
            };

            WindowHandle = windowComp.Create(contextSettings);

            WindowContext = glComp.CreateFromWindow(WindowHandle);

            contextSettings.SharedContext = WindowContext;

            WindowHandle2 = windowComp.Create(contextSettings);

            Window2Context = glComp.CreateFromWindow(WindowHandle2);

            SetWindowSettings(WindowHandle, "Cool test window", 600, 400);
            SetWindowSettings(WindowHandle2, "Cool test window #2", 300, 300);

            windowComp.GetPosition(WindowHandle, out var x, out var y);
            windowComp.GetSize(WindowHandle, out var width, out var height);
            windowComp.GetClientPosition(WindowHandle, out var cx, out var cy);
            windowComp.GetClientSize(WindowHandle, out var cwidth, out var cheight);
            Console.WriteLine($"Window: X: {x}, Y: {y}, Width: {width}, Height: {height}");
            Console.WriteLine($"Client: X: {cx}, Y {cy} Width: {cwidth}, Height: {cheight}");

            windowComp.SetClientSize(WindowHandle, 600, 400);
            windowComp.SetClientPosition(WindowHandle, 100, 100);

            windowComp.SetMinClientSize(WindowHandle, 200, 200);
            windowComp.SetMaxClientSize(WindowHandle, 1600, 900);

            windowComp.GetPosition(WindowHandle, out x, out y);
            windowComp.GetSize(WindowHandle, out width, out height);
            windowComp.GetClientPosition(WindowHandle, out cx, out cy);
            windowComp.GetClientSize(WindowHandle, out cwidth, out cheight);
            Console.WriteLine($"Window: X: {x}, Y: {y}, Width: {width}, Height: {height}");
            Console.WriteLine($"Client: X: {cx}, Y {cy} Width: {cwidth}, Height: {cheight}");

            var mode = windowComp.GetMode(WindowHandle);
            Console.WriteLine($"Mode: {mode}");

            windowComp.SetMode(WindowHandle, WindowMode.Normal);
            windowComp.SetMode(WindowHandle2, WindowMode.Normal);

            mode = windowComp.GetMode(WindowHandle);
            Console.WriteLine($"Mode: {mode}");

            // Subscribe to all events
            EventQueue.EventRaised += EventQueue_EventRaised;

            glComp.SetCurrentContext(WindowContext);

            Win32BindingsContext w32bc = new Win32BindingsContext(glComp, WindowContext);
            GLLoader.LoadBindings(w32bc);

            CursorHandle = cursorComp.Create();
            cursorComp.Load(CursorHandle, SystemCursorType.TextBeam);
            cursorComp.GetSize(CursorHandle, out _, out _);
            windowComp.SetCursor(WindowHandle, CursorHandle);
            windowComp.SetCursor(WindowHandle2, CursorHandle);

            ImageCursorHandle = cursorComp.Create();
            byte[] image = new byte[16 * 16 * 3];
            byte[] mask = new byte[16 * 16 * 1];
            for (int ccx = 0; ccx < 16; ccx++)
            {
                for (int ccy = 0; ccy < 16; ccy++)
                {
                    int index = (ccy * 16 + ccx) * 3;

                    image[index + 0] = (byte)(ccx * 16);
                    image[index + 1] = (byte)(ccx * 16);
                    image[index + 2] = (byte)(ccx * 16);

                    mask[(ccy * 16 + ccx)] = (byte)((ccy % 2 == 0) ? 1 : 0);
                }
            }
            cursorComp.SetHotspot(ImageCursorHandle, 8, 8);
            cursorComp.Load(ImageCursorHandle, 16, 16, image, mask);
            windowComp.SetCursor(WindowHandle, ImageCursorHandle);

            {
                byte[] icon = new byte[16 * 16 * 4];
                for (int ccx = 0; ccx < 16; ccx++)
                {
                    for (int ccy = 0; ccy < 16; ccy++)
                    {
                        int index = (ccy * 16 + ccx) * 4;

                        icon[index + 0] = (byte)(ccx * 16);
                        icon[index + 1] = (byte)(ccx * 16);
                        icon[index + 2] = (byte)(ccx * 16);
                        icon[index + 3] = 255;
                    }
                }

                IconHandle = iconComp.Create();
                iconComp.Load(IconHandle, 16, 16, icon);

                windowComp.SetIcon(WindowHandle, IconHandle);
            }

            {
                IconHandle2 = iconComp.Create();
                iconComp.Load(IconHandle2, "Wikipedia-Flags-UN-United-Nations-Flag.ico");

                windowComp.SetIcon(WindowHandle2, IconHandle2);
            }

            FileCursorHandle = cursorComp.Create();
            cursorComp.Load(FileCursorHandle, "Cute Light Green Normal Select.cur");
            windowComp.SetCursor(WindowHandle, FileCursorHandle);

            {
                cursorComp.GetSize(ImageCursorHandle, out int curW, out int curH);
                Console.WriteLine($"Width: {curW}, Height: {curH}");
            }
            Init();

            windowComp.Loop(WindowHandle, Render);
        }

        public static void SetWindowSettings(WindowHandle handle, string title, int width, int height)
        {
            windowComp.SetTitle(handle, title);

            windowComp.SetSize(handle, width, height);
            //windowComp.SetPosition(handle, 100, 100);

            windowComp.SetBorderStyle(handle, WindowStyle.Borderless);
            WindowStyle border = windowComp.GetBorderStyle(handle);
            Console.WriteLine($"Border: {border}");

            windowComp.SetBorderStyle(handle, WindowStyle.FixedBorder);
            border = windowComp.GetBorderStyle(handle);
            Console.WriteLine($"Border: {border}");

            windowComp.SetBorderStyle(handle, WindowStyle.ResizableBorder);
            border = windowComp.GetBorderStyle(handle);
            Console.WriteLine($"Border: {border}");
        }

        static List<ulong> vks = new List<ulong>();

        static Vector2i MousePos = (0, 0);
        private static void EventQueue_EventRaised(PalHandle? handle, PlatformEventType type, EventArgs args)
        {
            if (type == PlatformEventType.MouseMove)
            {
                MouseMoveEventArgs mouseMoveArgs = (MouseMoveEventArgs)args;

                //Console.WriteLine($"Delta X: {mouseMoveArgs.DeltaX}, DeltaY: {mouseMoveArgs.DeltaY}");

                MousePos = (mouseMoveArgs.DeltaX, mouseMoveArgs.DeltaY);

                if (WindowHandle.UserData is not false)
                {
                    windowComp.ScreenToClient(WindowHandle, MousePos.X, MousePos.Y, out int clientX, out int clientY);
                    windowComp.SetTitle(WindowHandle, $"({clientX},{clientY})");
                }

                return;
            }
            else if (type == PlatformEventType.MouseDown)
            {
                MouseButtonDownEventArgs mouseButtonDownArgs = (MouseButtonDownEventArgs)args;

                Console.WriteLine($"Pressed Mouse Button: {mouseButtonDownArgs.Button}");

                if (mouseButtonDownArgs.Button == MouseButton.Button1)
                {
                    keyboardComp.BeginIme(WindowHandle);

                    keyboardComp.SetImeRectangle(WindowHandle, MousePos.X, MousePos.Y, 0, 0);

                    keyboardComp.EndIme(WindowHandle);
                }

                return;
            }
            else if (type == PlatformEventType.MouseUp)
            {
                MouseButtonUpEventArgs mouseButtonUpArgs = (MouseButtonUpEventArgs)args;

                Console.WriteLine($"Released Mouse Button: {mouseButtonUpArgs.Button}");

                return;
            }
            else if (type == PlatformEventType.Close)
            {
                CloseEventArgs closeArgs = (CloseEventArgs)args;

                closeArgs.Window.UserData = false;

                windowComp.Destroy(closeArgs.Window);

                if (closeArgs.Window == WindowHandle && WindowHandle2.UserData is not false)
                {
                    WindowHandle2.UserData = false;
                    windowComp.Destroy(WindowHandle2);
                }

                return;
            }
            else if (type == PlatformEventType.Focus)
            {
                FocusEventArgs focus = (FocusEventArgs)args;

                if (focus.GotFocus)
                {
                    Console.WriteLine("Got focus");
                }
                else
                {
                    Console.WriteLine("Lost focus");
                }
            }
            else if (type == PlatformEventType.TextInput)
            {
                TextInputEventArgs input = (TextInputEventArgs)args;

                Console.WriteLine($"Input: {input.Text}");

                Console.WriteLine($"Scancodes: {string.Join(", ", vks)}");

                vks.Clear();

                return;
            }
            else if (type == PlatformEventType.MouseEnter)
            {
                MouseEnterEventArgs enter = (MouseEnterEventArgs)args;

                if (enter.Entered)
                {
                    Console.WriteLine($"Mouse entered.");
                }
                else
                {
                    Console.WriteLine("Mouse exited.");
                }
            }
            else if (type == PlatformEventType.FileDrop)
            {
                FileDropEventArgs fileDrop = (FileDropEventArgs)args;

                Console.WriteLine($"Files dropped! Position: {fileDrop.Position}, In Window: {fileDrop.DroppedInWindow}, Paths: {string.Join(", ", fileDrop.FilePaths)}");
            }
            else if (type == PlatformEventType.KeyDown)
            {
                KeyDownEventArgs keyDown = (KeyDownEventArgs)args;

                if (keyDown.WasDown == false)
                    vks.Add(keyDown.VirtualKey);

                if (keyDown.VirtualKey == 'C')
                {
                    clipComp.SetClipboardText("Copy");
                }
                else if (keyDown.VirtualKey == 'V')
                {
                    clipComp.SetClipboardText("Paste");
                }
                else if (keyDown.VirtualKey == 'A')
                {
                    AudioData data = new AudioData();
                    data.SampleRate = 44100;
                    data.Stereo = false;
                    data.Audio = new short[44100 * 2];
                    for (int i = 0; i < data.Audio.Length; i++)
                    {
                        float t = i / (float)data.SampleRate;

                        float sample = MathF.Sin(t * MathHelper.TwoPi * 440);

                        short s = (short)(sample * short.MaxValue);

                        data.Audio[i] = s;
                    }

                    ((ClipboardComponent)clipComp).SetClipboardAudio(data);
                }
                else if (keyDown.VirtualKey == 'B')
                {
                    const int W = 600;
                    const int H = 600;
                    byte[] b = new byte[W * H * 4];
                    for (int xi = 0; xi < W; xi++)
                    {
                        for (int yi = 0; yi < H; yi++)
                        {
                            int index = (yi * W + xi) * 4;

                            static byte ftob(float f) => (byte)(f * 255);

                            float x0 = MathHelper.MapRange(xi, 0f, W, -2f, 0.47f);
                            float y0 = MathHelper.MapRange(yi, 0f, H, -1.12f, 1.12f);

                            float x = 0;
                            float y = 0;
                            const int maxIterations = 1000;
                            int iteration = 0;
                            while (x*x + y*y < 2*2 && iteration < maxIterations)
                            {
                                iteration++;

                                float xTemp = x * x - y * y + x0;
                                y = 2 * x * y + y0;
                                x = xTemp;
                            }

                            // See https://stackoverflow.com/a/22681410/9316430
                            static void SpectralColor(float l, out float r, out float g, out float b) // RGB <0,1> <- lambda l <400,700> [nm]
                            {
                                float t; r = 0.0f; g = 0.0f; b = 0.0f;
                                if ((l >= 400.0) && (l < 410.0)) { t = (l - 400.0f) / (410.0f - 400.0f); r = +(0.33f * t) - (0.20f * t * t); }
                                else if ((l >= 410.0) && (l < 475.0)) { t = (l - 410.0f) / (475.0f - 410.0f); r = 0.14f - (0.13f * t * t); }
                                else if ((l >= 545.0) && (l < 595.0)) { t = (l - 545.0f) / (595.0f - 545.0f); r = +(1.98f * t) - (t * t); }
                                else if ((l >= 595.0) && (l < 650.0)) { t = (l - 595.0f) / (650.0f - 595.0f); r = 0.98f + (0.06f * t) - (0.40f * t * t); }
                                else if ((l >= 650.0) && (l < 700.0)) { t = (l - 650.0f) / (700.0f - 650.0f); r = 0.65f - (0.84f * t) + (0.20f * t * t); }
                                if ((l >= 415.0) && (l < 475.0)) { t = (l - 415.0f) / (475.0f - 415.0f); g = +(0.80f * t * t); }
                                else if ((l >= 475.0) && (l < 590.0)) { t = (l - 475.0f) / (590.0f - 475.0f); g = 0.8f + (0.76f * t) - (0.80f * t * t); }
                                else if ((l >= 585.0) && (l < 639.0)) { t = (l - 585.0f) / (639.0f - 585.0f); g = 0.84f - (0.84f * t); }
                                if ((l >= 400.0) && (l < 475.0)) { t = (l - 400.0f) / (475.0f - 400.0f); b = +(2.20f * t) - (1.50f * t * t); }
                                else if ((l >= 475.0) && (l < 560.0)) { t = (l - 475.0f) / (560.0f - 475.0f); b = 0.7f - (t) + (0.30f * t * t); }
                            }

                            float l = MathHelper.MapRange(iteration, 0, maxIterations, 400.0f, 700.0f);
                            SpectralColor(l, out float red, out float green, out float blue);

                            b[index + 0] = ftob(red);
                            b[index + 1] = ftob(green);
                            b[index + 2] = ftob(blue);
                            b[index + 3] = 255;
                        }
                    }
                    
                    Bitmap bitmap = new Bitmap(W, H, b);

                    ((ClipboardComponent)clipComp).SetClipboardBitmap(bitmap);
                }
                else if (keyDown.VirtualKey == 'P')
                {
                    ClipboardFormat format = clipComp.GetClipboardFormat();
                    Console.WriteLine($"Clipboard format: {format}");
                    switch (format)
                    {
                        case ClipboardFormat.Text:
                            Console.WriteLine($"Current clipboard: '{clipComp.GetClipboardText()}'");
                            break;
                        case ClipboardFormat.HTML:
                            Console.WriteLine($"Current clipboard: '{clipComp.GetClipboardHTML()}',\nunformated: '{clipComp.GetClipboardText()}'");
                            break;
                        case ClipboardFormat.Files:
                            Console.WriteLine($"Current clipboard: '{string.Join(", ", clipComp.GetClipboardFiles()!)}'");
                            break;
                        case ClipboardFormat.Bitmap:
                            {
                                Bitmap? bitmap = clipComp.GetClipboardBitmap();
                                if (bitmap == null)
                                {
                                    Console.WriteLine("Could not get clipboard image!");
                                    break;
                                }
                                Console.WriteLine($"Current clipboard: width: {bitmap.Width}, height: {bitmap.Height}");

                                var tex = GL.GenTexture();

                                GL.ActiveTexture(TextureUnit.Texture0);
                                GL.BindTexture(TextureTarget.Texture2d, tex);

                                GL.TexImage2D(TextureTarget.Texture2d, 0, (int)InternalFormat.Rgba8, bitmap.Width, bitmap.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, bitmap.Data);

                                GL.GenerateMipmap(TextureTarget.Texture2d);

                                GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                                GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                                GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                                GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);

                                GL.BindTexture(TextureTarget.Texture2d, TextureHandle.Zero);

                                if (clipboard_tex != TextureHandle.Zero)
                                {
                                    GL.DeleteTexture(clipboard_tex);
                                }

                                clipboard_tex = tex;

                                break;
                            }
                        case ClipboardFormat.Audio:
                            {
                                var audio = clipComp.GetClipboardAudio()!;

                                int samples = audio.Audio.Length;
                                if (audio.Stereo) samples /= 2;
                                float time = samples * (1f / audio.SampleRate);

                                Console.WriteLine($"Current clipboard: Sample rate: {audio.SampleRate / 1000f}kHz, Stereo: {audio.Stereo}, Length: {time}s");

                                break;
                            }
                        case ClipboardFormat.None:
                        default:
                            break;
                    }
                }
                else if (keyDown.VirtualKey == 'D')
                {
                    DisplayHandle disp = windowComp.GetDisplay(WindowHandle);
                    bool isPrimary = dispComp.IsPrimary(disp);
                    dispComp.GetResolution(disp, out int resX, out int resY);
                    dispComp.GetRefreshRate(disp, out float refreshRate);

                    string name = dispComp.GetName(disp);
                    
                    Console.WriteLine($"Window is on monitor '{name}', primary: {isPrimary}, res: ({resX}x{resY}, refresh rate: {refreshRate:0.})");
                }
                else if (keyDown.VirtualKey == 'S')
                {
                    windowComp.GetClientSize(WindowHandle, out int width, out int height);
                    Console.WriteLine($"Window 1 client size: ({width}, {height})");
                }
            }
        }

        static BufferHandle buffer;

        static VertexArrayHandle vao;
        static VertexArrayHandle vao2;

        static ProgramHandle program;
        static ProgramHandle program2;

        const string vertexShaderSource =
    @"#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aUV;
layout (location = 2) in vec3 aColor;

out vec2 oUV;
out vec3 oColor;

void main()
{
    gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
    oUV = aUV;
    oColor = aColor;
}";

        const string fragmentShaderSource =
    @"#version 330 core
in vec2 oUV;
in vec3 oColor;

out vec4 FragColor;

uniform sampler2D tex;

void main()
{
    FragColor = texture(tex, oUV);
}";

        public static TextureHandle GetCursorImage(CursorHandle handle)
        {
            cursorComp.GetSize(handle, out int width, out int height);
            byte[] data = new byte[width * height * 4];
            // FIXME: Handle proper RGBA format when using AND and XOR masks. Atm it gets a constant alpha = 0.
            cursorComp.GetImage(handle, data);

            for (int i = 0; i < width * height; i++)
            {
                int index = i * 4;

                data[index + 3] = 255;
            }

            TextureHandle tex = GL.GenTexture();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2d, tex);

            GL.TexImage2D(TextureTarget.Texture2d, 0, (int)InternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);

            GL.GenerateMipmap(TextureTarget.Texture2d);

            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.BindTexture(TextureTarget.Texture2d, TextureHandle.Zero);

            return tex;
        }

        public static TextureHandle GetIconImage(IconHandle handle)
        {
            int size = iconComp.GetBitmapSize(handle);
            byte[] data = new byte[size];
            
            // FIXME: Handle proper RGBA format when using AND and XOR masks. Atm it gets a constant alpha = 0.
            iconComp.GetBitmap(handle, data);

            iconComp.GetDimensions(handle, out int width, out int height);

            TextureHandle tex = GL.GenTexture();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2d, tex);

            GL.TexImage2D(TextureTarget.Texture2d, 0, (int)InternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);

            GL.GenerateMipmap(TextureTarget.Texture2d);

            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.BindTexture(TextureTarget.Texture2d, TextureHandle.Zero);

            return tex;
        }

        static TextureHandle cursor_tex;
        static TextureHandle icon_tex;

        static TextureHandle clipboard_tex;

        public static void Init()
        {
            float[] vertices = new float[]
            {
            -1f * 0.5f, -1f * 0.5f, 0f,     0f, 0f,     1f, 0f, 0f,
             1f * 0.5f, -1f * 0.5f, 0f,     1f, 0f,     0f, 1f, 0f,
             1f * 0.5f,  1f * 0.5f, 0f,     1f, 1f,     0f, 0f, 1f,

             1f * 0.5f,  1f * 0.5f, 0f,     1f, 1f,     0f, 0f, 1f,
            -1f * 0.5f,  1f * 0.5f, 0f,     0f, 1f,     0f, 1f, 0f,
            -1f * 0.5f, -1f * 0.5f, 0f,     0f, 0f,     1f, 0f, 0f,
            };

            glComp.SetCurrentContext(WindowContext);

            buffer = CreateBuffer(vertices);
            vao = CreateVAO(buffer);

            CheckError("vao");

            program = CreateShader("", vertexShaderSource, fragmentShaderSource);

            GL.UseProgram(program);

            CheckError("shader");

            cursor_tex = GetCursorImage(ImageCursorHandle);
            icon_tex = GetIconImage(IconHandle2);
            
            CheckError("get cursor tex");

            glComp.SetCurrentContext(Window2Context);

            vao2 = CreateVAO(buffer);
            program2 = CreateShader("", vertexShaderSource, fragmentShaderSource);

            glComp.SetCurrentContext(WindowContext);

            CheckError("getString");

            int encoding = 0;
            GL.GetFramebufferAttachmentParameteri(FramebufferTarget.DrawFramebuffer, (FramebufferAttachment)All.BackLeft, FramebufferAttachmentParameterName.FramebufferAttachmentColorEncoding, ref encoding);

            if ((All)encoding == All.Linear)
            {
                Console.WriteLine("Linear default framebuffer!");
            }
            else if ((All)encoding == All.Srgb)
            {
                Console.WriteLine("sRGB default framebuffer!");
            }
            CheckError("getFramebuffer");

            GL.Disable(EnableCap.FramebufferSrgb);
        }

        public static BufferHandle CreateBuffer(float[] vertices)
        {
            var buffer = GL.GenBuffer();

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, buffer);
            GL.BufferData(BufferTargetARB.ArrayBuffer, vertices, BufferUsageARB.StaticDraw);

            return buffer;
        }

        public static VertexArrayHandle CreateVAO(BufferHandle buffer)
        {
            var vao = GL.GenVertexArray();

            CheckError("buffer");

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTargetARB.ArrayBuffer, buffer);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, sizeof(float) * 8, sizeof(float) * 3);
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, sizeof(float) * 5);
            GL.EnableVertexAttribArray(2);

            return vao;
        }

        public static ProgramHandle CreateShader(string name, string vertexSource, string fragmentSource)
        {
            var vert = GL.CreateShader(ShaderType.VertexShader);
            var frag = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vert, vertexSource);
            GL.ShaderSource(frag, fragmentSource);

            GL.CompileShader(vert);
            GL.CompileShader(frag);

            int success = 0;
            GL.GetShaderi(vert, ShaderParameterName.CompileStatus, ref success);
            if (success == 0)
            {
                GL.GetShaderInfoLog(vert, out string info);
                Console.WriteLine(info);
            }
            GL.GetShaderi(frag, ShaderParameterName.CompileStatus, ref success);
            if (success == 0)
            {
                GL.GetShaderInfoLog(frag, out string info);
                Console.WriteLine(info);
            }

            var program = GL.CreateProgram();

            GL.AttachShader(program, vert);
            GL.AttachShader(program, frag);

            GL.LinkProgram(program);

            GL.GetProgrami(program, ProgramPropertyARB.LinkStatus, ref success);
            if (success == 0)
            {
                GL.GetProgramInfoLog(program, out string info);
                Console.WriteLine(info);
            }

            GL.DetachShader(program, vert);
            GL.DetachShader(program, frag);

            GL.DeleteShader(vert);
            GL.DeleteShader(frag);

            return program;
        }

        static Stopwatch watch = new Stopwatch();
        static float time = 0;
        static int frames = 0;

        static float colorTimer = 0;

        public static bool Render()
        {
            float deltaTime = watch.ElapsedTicks / (float)Stopwatch.Frequency;
            watch.Restart();

            time += deltaTime;
            frames++;

            if (time > 1.5f)
            {
                // FIXME: Only write this out every so often.
                //Console.WriteLine($"Delta time: {(time / frames) * 1000f}ms");
                time = 0;
                frames = 0;
            }

            if (WindowHandle.UserData is not false)
            {
                glComp.SetCurrentContext(WindowContext);

                GL.UseProgram(program);

                colorTimer += deltaTime / 5;
                colorTimer %= 1;
                Color4<Hsva> color = new Color4<Hsva>(colorTimer, 1f, 1f, 1f);

                GL.ClearColor(Color4.ToRgba(color));
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
                windowComp.GetClientSize(WindowHandle, out int width, out int height);
                GL.Viewport(0, 0, width, height);

                CheckError("clear");

                //mouseComp.GetPosition(null, out int x, out int y);
                //windowComp.ScreenToClient(WindowHandle, x, y, out int clientX, out int clientY);
                //windowComp.SetTitle(WindowHandle, $"({clientX},{clientY})");

                GL.ActiveTexture(TextureUnit.Texture0);

                TextureHandle tex = cursor_tex;
                if (clipboard_tex != TextureHandle.Zero) tex = clipboard_tex;

                GL.BindTexture(TextureTarget.Texture2d, tex);

                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                GL.Enable(EnableCap.Blend);

                GL.BindVertexArray(vao);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

                CheckError("draw");

                windowComp.SwapBuffers(WindowHandle);
            }

            if (WindowHandle2.UserData is not false)
            {
                glComp.SetCurrentContext(Window2Context);

                GL.UseProgram(program2);

                GL.ClearColor(new Color4<Rgba>(64 / 255f, 0, 127 / 255f, 255));
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
                windowComp.GetClientSize(WindowHandle2, out int width, out int height);
                GL.Viewport(0, 0, width, height);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2d, icon_tex);

                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                GL.Enable(EnableCap.Blend);

                GL.BindVertexArray(vao2);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                windowComp.SwapBuffers(WindowHandle2);
            }

            return true;
        }

        static void CheckError(string place)
        {
            var error = GL.GetError();
            while (error != ErrorCode.NoError)
            {
                Console.WriteLine($"{place} Error: {error}");
                error = GL.GetError();
            }
        }
    }
}
