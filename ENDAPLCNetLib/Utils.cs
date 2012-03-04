using System;
using System.Collections.Generic;
using System.Text;

namespace ENDA.PLCNetLib
{
    public class Utils
    {
        public static string HexDump(byte[] buf)
        {
            return HexDump(buf, buf.Length);
        }

        public static string HexDump(byte[] buf, long len)
        {
            return HexDump(buf, len, 16);
        }

        public static string HexDump(byte[] buf, long len, int col)
        {
            return HexDump(buf, len, col, "\r\n");
        }

        public static string HexDump(byte[] buf, long len, int col, string delim)
        {
            int rows = (int)Math.Ceiling(len * 1.0 / col);
            string dump = "";
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    if ((r * col + c) < len)
                        dump += buf[r * col + c].ToString("X2") + " ";
                    else
                        dump += "   ";
                }

                for (int c = 0; c < col; c++)
                {
                    if ((r * col + c) < len)
                    {
                        char ch = (char)buf[r * col + c];
                        dump += Char.IsLetterOrDigit(ch) ? ch : '.';
                    }
                    else
                        dump += " ";
                }
                dump += "\r\n";
            }
            return dump;
        }
    }
}
