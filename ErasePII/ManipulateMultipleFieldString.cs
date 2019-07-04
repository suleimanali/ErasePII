using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErasePII
{
    internal static class ManipulateMultipleFieldString
    {
        public static string GetStringFromMultiFieldArray(string inputString)
        => string.Join(string.Empty, inputString.Trim().Split('\''));
    }
}
