using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMan.Core.Models
{
    public class ApplicationUser : IdentityUser
    {

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }
        public bool IsAdmin { get; set; } = false;
        public ICollection<Project>? Projects { get; set; }
        public List<ProjectUser>? ProjectUsers { get; set; }
    }
}
