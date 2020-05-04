using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OneToucheCare.Models;
using Microsoft.AspNetCore.Session;


namespace OneToucheCare.Controllers
{
    public class AccountController : Controller
    {
        //readonly dbContext
        private readonly db_oneTouchCareContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        //private ISession Session { get { return _httpContextAccessor.HttpContext.Session; } }
        //injection of dbContext inside the constructor

        public AccountController(db_oneTouchCareContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        //account type if doctor , patient , admin
        private static int AccountType_Doctor = 1;
        private static int AccountType_Admin = 3;
        private static int AccountType_Patient = 2;


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            Account account = new Account();
            return View(account);
        }


        [HttpPost]
        public IActionResult Login([Bind("UserName", "Password")] Account account)
        {
            Encription encription = new Encription();
            bool existed = _context.Account.Any(x => x.UserName.TrimEnd().ToLower().Equals(account.UserName.TrimEnd().ToLower()) && x.Password.Equals(account.Password));
            if (existed)
            {
                Account selectedAccount = _context.Account.Where(x => x.UserName.TrimEnd().ToLower().Equals(account.UserName.TrimEnd().ToLower()) && x.Password.Equals(account.Password)).FirstOrDefault();
                int? AccountTypeId = selectedAccount.AccountTypeId;
                ApplicationKeys appKeys = new ApplicationKeys();
                string UserNameEncription = encription.Encrypt(appKeys.Key_UserName, selectedAccount.UserName);
                string PasswordEncription = encription.Encrypt(appKeys.Key_Password, selectedAccount.Password);

                //HttpContext.Session.SetString("x", UserNameEncription);
                //HttpContext.Session.SetString("y", PasswordEncription);
                _session.SetString("x", UserNameEncription);
                _session.SetString("y", PasswordEncription);

                if (AccountTypeId == AccountType_Doctor)
                {
                    ViewData["Layout"] = "_DoctorLayout";
                    //ViewData["ContactLayout"] = "~/Views/Shared/_DoctorLayout.cshtml";
                    return RedirectToAction("DoctorDefault", "Home", new { id = UserNameEncription, y = PasswordEncription });

                }
                else if (AccountTypeId == AccountType_Admin)
                {
                    ViewData["Layout"] = "_AdminLayout";
                    //ViewData["ContactLayout"] = "~/Views/Shared/_AdminLayout.cshtml";
                    return RedirectToAction("AdminDefault", "Home", new { id = UserNameEncription, y = PasswordEncription });
                }
                else if (AccountTypeId == AccountType_Patient)
                {
                    ViewData["Layout"] = "_PatientLayout";
                    //ViewData["ContactLayout"] = "~/Views/Shared/_PatientLayout.cshtml";
                    return RedirectToAction("PatientDefault", "Home", new { id = UserNameEncription, y = PasswordEncription });
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public IActionResult Create()
        {
            SelectList AccountTypeList = new SelectList(_context.AccountType.ToList(), "AccountTypeId", "AccountTypeName");
            ViewData["AccountTypeId"] = AccountTypeList;
            Account account = new Account();
            return View(account);
        }

        [HttpPost]
        public IActionResult Create([Bind("UserName", "Password", "AccountTypeId", "RecoveryQuestion", "RecoveryAnswer")] Account account)
        {
            string userName = new Encription().Decrypt(new ApplicationKeys().Key_UserName, _session.GetString("x").ToString());
            string password = new Encription().Decrypt(new ApplicationKeys().Key_Password, _session.GetString("y").ToString());

            if (ModelState.IsValid)
            {
                bool existedBefore = _context.Account.Any(x => x.UserName.ToLower().TrimEnd().Equals(account.UserName.ToLower().TrimEnd()));
                if (!existedBefore)
                {
                    _context.Account.Add(account);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }


        public IActionResult ListOfAccounts()
        {
            SelectList ListOfAccountTypes = new SelectList(_context.AccountType.ToList(), "AccountTypeId", "AccountTypeName");
            ViewData["AccountTypeId"] = ListOfAccountTypes;
            return View();
        }

        [HttpPost]
        public JsonResult GetListOfAccounts([FromRoute] int id)
        {
            //int.TryParse(AccountTypeId, out int id);
            var list = _context.Account.Where(x => x.AccountTypeId == id).OrderByDescending(x => x.AccountId).ToList();
            return Json(list);
        }


        public IActionResult Edit(int id)
        {
            SelectList ListOfAccountTypes = new SelectList(_context.AccountType.ToList(), "AccountTypeId", "AccountTypeName");
            ViewData["AccountTypeId"] = ListOfAccountTypes;
            if (id > 0)
            {
                bool accountExisted = _context.Account.Any(x => x.AccountId == id);
                if (accountExisted)
                {
                    Account account = _context.Account.Where(x => x.AccountId == id).FirstOrDefault();
                    return View(account);
                }
                else
                {
                    return RedirectToAction("ListOfAccounts");
                }
            }
            else
            {
                return RedirectToAction("ListOfAccounts");
            }
        }

        [HttpPost]
        public IActionResult Edit(Account account)
        {
            int? accountId = account.AccountId;
            if (accountId != null && accountId > 0)
            {
                Account oldAccount = _context.Account.Where(x => x.AccountId == accountId).FirstOrDefault();
                oldAccount.AccountTypeId = account.AccountTypeId;
                oldAccount.Password = account.Password;
                oldAccount.RecoveryAnswer = account.RecoveryAnswer;
                oldAccount.RecoveryQuestion = account.RecoveryQuestion;
                oldAccount.UserName = account.UserName;
                _context.SaveChanges();
            }
            return RedirectToAction("ListOfAccounts");
        }


        public IActionResult Delete(int id)
        {
            //SelectList ListOfAccountTypes = new SelectList(_context.AccountType.ToList(), "AccountTypeId", "AccountTypeName");
            //ViewData["AccountTypeId"] = ListOfAccountTypes;
            bool accountExisted = _context.Account.Any(x => x.AccountId == id);
            if (accountExisted)
            {
                Account account = _context.Account.Where(x => x.AccountId == id).FirstOrDefault();
                return View(account);
            }
            else
            {
                return RedirectToAction("ListOfAccounts");
            }
        }

        [HttpPost]
        public IActionResult BlockAccount([Bind("AccountId")]int AccountId)
        {
            bool existed = _context.Account.Any(x => x.AccountId == AccountId);
            if (existed)
            {
                Account account = _context.Account.Where(x => x.AccountId == AccountId).FirstOrDefault();
                account.IsBlocked = true;
                _context.SaveChanges();
            }
            return RedirectToAction("ListOfAccounts");
        }

        [HttpPost]
        public void Unblock([FromRoute] int Id)
        {
            bool existed = _context.Account.Any(x => x.AccountId == Id);
            if (existed)
            {
                Account account = _context.Account.Where(x => x.AccountId == Id).FirstOrDefault();
                account.IsBlocked = false;
                _context.SaveChanges();
            }
        }




        public IActionResult EditAccount(int id)
        {

            Account account = _context.Account.Where(x => x.AccountId == id).FirstOrDefault();
            return View(account);
        }

        [HttpPost]
        public IActionResult EditAccount(Account account)
        {
            int accountId = account.AccountId;
            Account oldAccount = _context.Account.Where(x => x.AccountId == accountId).FirstOrDefault();
            oldAccount.AccountTypeId = account.AccountTypeId;
            oldAccount.Password = account.Password;
            oldAccount.RecoveryAnswer = account.RecoveryAnswer;
            oldAccount.RecoveryQuestion = account.RecoveryQuestion;
            oldAccount.UserName = account.UserName;
            _context.SaveChanges();
            return RedirectToAction("DoctorDetails", "Doctors");
        }


        

        
    }
}