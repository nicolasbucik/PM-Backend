using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PMan.Core.Models
{
    public class ProjectUser
    {
        [Required]
        public int? ProjectId { get; set; }
        [Required]
        public Project? Project { get; set; }


        [Required]
        public string? UserId { get; set; }
        [Required]
        [JsonIgnore]
        public ApplicationUser? User { get; set; }

        [Required]
        [Range(1, 3)]
        public int? Role { get; set; }
    }
}
