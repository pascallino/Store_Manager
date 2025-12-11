using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
namespace StoreManager
{
   

    public class RawPrinterHelper
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pDataType;
        }

        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterW",
            SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern bool OpenPrinter(string src, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter",
            SetLastError = true, ExactSpelling = true)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterW",
            SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern bool StartDocPrinter(
            IntPtr hPrinter, Int32 level, [In] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter",
            SetLastError = true, ExactSpelling = true)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter",
            SetLastError = true, ExactSpelling = true)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter",
            SetLastError = true, ExactSpelling = true)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter",
            SetLastError = true, ExactSpelling = true)]
        public static extern bool WritePrinter(
            IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        public static bool SendStringToPrinter(string printerName, string text)
        {
            IntPtr pBytes = Marshal.StringToCoTaskMemAnsi(text);
            bool result = SendBytesToPrinter(printerName, pBytes, text.Length);
            Marshal.FreeCoTaskMem(pBytes);
            return result;
        }

        public static bool SendBytesToPrinter(string printerName, IntPtr pBytes, int count)
        {
            IntPtr hPrinter;
            DOCINFOA di = new DOCINFOA();
            di.pDocName = "Receipt";
            di.pDataType = "RAW";

            if (!OpenPrinter(printerName.Normalize(), out hPrinter, IntPtr.Zero))
                return false;

            bool success = false;
            if (StartDocPrinter(hPrinter, 1, di))
            {
                if (StartPagePrinter(hPrinter))
                {
                    Int32 written;
                    success = WritePrinter(hPrinter, pBytes, count, out written);
                    EndPagePrinter(hPrinter);
                }
                EndDocPrinter(hPrinter);
            }

            ClosePrinter(hPrinter);
            return success;
        }
    }

}
