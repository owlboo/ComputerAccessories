using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.Helpers
{
    public static class AccountHelpers
    {
        public static string GenerateCodeConfirm()
        {
            Random rand = new Random();
            return rand.Next(100000, 999999).ToString();
        }
        public static string Guild(int length)
        {
            string str = "ABCDEFGHIKLMNOPQWRZVXJ1234567890";
            string result = "";
            Random rand = new Random();
            while (result.Length <= length)
            {
                result += str[rand.Next(0, 31)];
            }
            return result;
        }
    }
}
