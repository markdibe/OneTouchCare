using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneToucheCare.Models
{
    public class GlobalMethods
    {

        private readonly db_oneTouchCareContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session { get { return _httpContextAccessor.HttpContext.Session; } }

        public GlobalMethods(db_oneTouchCareContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }


        public  int? GetAccountId()
        {
            string userName = new Encription().Decrypt(new ApplicationKeys().Key_UserName, _session.GetString("x").ToString());
            string password = new Encription().Decrypt(new ApplicationKeys().Key_Password, _session.GetString("y").ToString());
            bool IsExisted = _context.Account.Any(x => x.UserName.ToLower().Equals(userName) && x.Password.Equals(password));
            if (!IsExisted)
            {
                return null;
            }
            Account account = _context.Account.Where(x => x.UserName.ToLower().Equals(userName) && x.Password.Equals(password)).FirstOrDefault();
            return account.AccountId;
        }

        public int? GetDoctorId()
        {
            int? AccountId = GetAccountId();
            if (AccountId != null)
            {
                bool DoctorExisted = _context.Doctor.Any(x => x.AccountId == AccountId);
                if (DoctorExisted)
                {
                    return _context.Doctor.Where(x => x.AccountId == AccountId).FirstOrDefault().DoctorId;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
