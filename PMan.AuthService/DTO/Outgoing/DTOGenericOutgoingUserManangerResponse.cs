using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMan.AuthService.DTO.Outgoing
{
    public class DTOGenericOutgoingUserManangerResponse
    {
        public string Message { get; set; }
        public virtual bool isSuccess { get; set; }
    }
}
