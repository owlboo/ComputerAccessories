using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerAccessoriesV2.Helpers
{
    public class AccountHelpers
    {
        public static string GenerateCodeConfirm()
        {
            Random rand = new Random();
            return rand.Next(100000, 999999).ToString();
        }
    }
}
