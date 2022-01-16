using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PMan.Core.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public List<Issue>? Issues { get; set; }
        [Required]

        [JsonIgnore]
        public ICollection<ApplicationUser>? Users { get; set; }

        [Required]
        public List<ProjectUser>? ProjectUsers { get; set; }


    }
}
