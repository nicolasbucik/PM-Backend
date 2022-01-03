using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMan.AuthService.DTO.Outgoing
{
    public class DTOOutgoingErrorUserManagerResponse : DTOGenericOutgoingUserManangerResponse
    {
        public override bool isSuccess { get; set; } = false;
        public IEnumerable<string>? Errors { get; set; }

    }
}
