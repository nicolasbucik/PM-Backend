using PMan.Core.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMan.Core.DTOModels
{
    public class DTOUpdateIssue
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

        [Issue_UpdateEnsureStartTimeFuture]
        public DateTime? StartTime { get; set; }

        [Issue_UpdateEnsureEndTimePastStartTime]
        public DateTime? EndTime { get; set; }

        [Required]
        public bool? IsFinished { get; set; } = false;

        [EmailAddress]
        [StringLength(50)]
        public string? AssignedTo { get; set; }


        public bool ValidateStartTimeFuture()
        {
            if (!StartTime.HasValue) return true;

            return StartTime.Value.Date >= DateTime.UtcNow;

        }

        public bool ValidateEndTimePastStartTime()
        {
            if (!StartTime.HasValue || !EndTime.HasValue) return true;

            return EndTime.Value.Date > StartTime.Value.Date;
        }
    }
}
