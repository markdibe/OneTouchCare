using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneToucheCare.Models
{
    public class Authorization
    {
        //private static int AccountType_Admin = 3;
        //private static int AccountType_Doctor = 1;
        //private static int AccountType_Patient = 2;
        private readonly db_oneTouchCareContext _context;

        public Authorization(db_oneTouchCareContext context)
        {
            _context = context;
        }

        public bool IsAuthenticated(string encriptedUserName, string encriptedPassword, int AccountTypeId)
        {
            ApplicationKeys appKeys = new ApplicationKeys();
            string decryptedUserName = new Encription().Decrypt(appKeys.Key_UserName, encriptedUserName);
            string decryptedPassword = new Encription().Decrypt(appKeys.Key_Password, encriptedPassword);
            bool existedUser = _context.Account.Any(x => x.UserName.ToLower().TrimEnd().Equals(decryptedUserName.ToLower().TrimEnd()));
            if (existedUser)
            {
                int? UserAccountTypeId = _context.Account.Where(x => x.UserName.ToLower().TrimEnd().Equals(decryptedUserName.ToLower().TrimEnd())).FirstOrDefault().AccountTypeId;
                if (AccountTypeId == UserAccountTypeId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
