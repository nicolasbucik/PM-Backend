using PMan.Core.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMan.Core.DTOModels
{
    public class DTOIssue
    {
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }

        [Range(1, 3)]
        public int? Priority { get; set; } = 2;

        [Range(1, 3)]
        [Required]
        public int? Type { get; set; }

        [Issue_EnsureStartTimeFuture]
        public DateTime? StartTime { get; set; }

        [Issue_EnsureStartTimeFuture]
        public DateTime? EndTime { get; set; }

        [Required]
        public bool? IsFinished { get; set; } = false;

        public string? AssignedTo { get; set; }
         
        [Required]
        public int? ProjectId { get; set; }
    }
}
