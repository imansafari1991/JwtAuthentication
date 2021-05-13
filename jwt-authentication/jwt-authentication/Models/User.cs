using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace jwt_authentication.Models
{
    public class User
    {
        public string Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(100)]
        public string Password { get; set; }
        [MaxLength(100)]
        public string  Email { get; set; }
        public string Mobile { get; set; }
        public DateTime CreateDate { get; set; }
        
    }
}
