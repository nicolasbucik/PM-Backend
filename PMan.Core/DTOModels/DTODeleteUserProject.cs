using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMan.Core.DTOModels
{
    public class DTODeleteUserProject
    {
        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string? UserMail { get; set; }
    }
}
