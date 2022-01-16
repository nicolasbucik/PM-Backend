using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMan.Core.DTOModels
{
    public class DTOUserProject
    {
        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string? UserMail { get; set; }

        [Required]
        [Range(2, 3)]
        public int? role { get; set; } = 3;
    }
}
