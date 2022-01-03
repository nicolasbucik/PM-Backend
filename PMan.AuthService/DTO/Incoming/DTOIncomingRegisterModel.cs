using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMan.AuthService.DTO.Incoming
{
    public class DTOIncomingRegisterModel
    {
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [StringLength(100)]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string? LastName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string? Password { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string? ConfirmPassword { get; set; }

    }
}
