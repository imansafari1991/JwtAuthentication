using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace jwt_authentication.ViewModels
{
    public class SignUpViewModels
    {
        public string UserName { get; set; }
        
        public string FirstName { get; set; }
       
        public string LastName { get; set; }
      
        public string Password { get; set; }
        [Compare(nameof(Password),ErrorMessage = "Password not matched")]
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        
    }

    public class LoginViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponseViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; } 
        public string LastName { get; set; }
        [JsonIgnore]
        public string Password { get; set; }

        public string token { get; set; }
        
    }
}
