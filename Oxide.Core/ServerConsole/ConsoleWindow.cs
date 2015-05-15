﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Oxide.Core.ServerConsole
{
    public class ConsoleWindow
    {
        private const uint ATTACH_PARENT_PROCESS = 0xFFFFFFFF;
        private TextWriter _oldOutput;

        [DllImport("kernel32.dll", CharSet = CharSet.None, ExactSpelling = false, SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", CharSet = CharSet.None, ExactSpelling = false, SetLastError = true)]
        private static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll", CharSet = CharSet.None, ExactSpelling = false, SetLastError = true)]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern bool SetConsoleTitleA(string lpConsoleTitle);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        public static bool Check(bool force = false)
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                    return force || GetConsoleWindow() == IntPtr.Zero;
            }
            return false;
        }

        public void SetTitle(string title)
        {
            if (title != null) SetConsoleTitleA(title);
        }

        public void Initialize()
        {
            if (!Check()) return;
            if (!AttachConsole(ATTACH_PARENT_PROCESS)) AllocConsole();
            _oldOutput = Console.Out;
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput(), Encoding.ASCII) { AutoFlush = true });
        }

        public void Shutdown()
        {
            Console.SetOut(_oldOutput);
            FreeConsole();
        }
    }
}