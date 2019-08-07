using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbiGamesBackupToolX.Utils
{
    class EncodingConvert
    {
        public static string GetUtf8ByUnicodeString(string str)
        {
            byte[] srcbytes = Encoding.Unicode.GetBytes(str);
            byte[] newbytes =  Encoding.Convert(Encoding.Unicode, Encoding.UTF8, srcbytes);
            return Encoding.UTF8.GetString(newbytes);
        }
    }
}
