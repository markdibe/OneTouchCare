using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OneToucheCare.Models
{
    public partial class Account
    {
        public Account()
        {
            Doctor = new HashSet<Doctor>();
            Patient = new HashSet<Patient>();
        }

        public int AccountId { get; set; }
        [Display(Name ="User name or Email")]
        public string UserName { get; set; }
        [Display(Name ="Password")]
        public string Password { get; set; }
        [Display(Name ="Recovery question")]
        public string RecoveryQuestion { get; set; }
        [Display(Name ="Recovery answer")]
        public string RecoveryAnswer { get; set; }
        [Display(Name ="Account Type")]
        public int? AccountTypeId { get; set; }

        public bool? IsBlocked { get; set; }

        public AccountType AccountType { get; set; }
        public ICollection<Doctor> Doctor { get; set; }
        public ICollection<Patient> Patient { get; set; }
    }
}
