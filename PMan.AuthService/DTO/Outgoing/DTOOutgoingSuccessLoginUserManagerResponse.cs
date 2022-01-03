using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMan.AuthService.DTO.Outgoing
{
    public class DTOOutgoingSuccessLoginUserManagerResponse : DTOGenericOutgoingUserManangerResponse
    {

        public override bool isSuccess { get; set; } = true;

        public string Token { get; set; }

        public DateTime? ExpireDate { get; set; }
    }
}
