using System;
using CarHunters.Core.Common.Enums;

namespace CarHunters.Core.Common.Models
{
    public class SocialData
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public string Photo { get; set; }
        public string Token { get; set; }
        public AuthorizationTypeEnum Source { get; set; }
        public SocialLoginStateEnum LoginState { get; set; }
        public string ErrorString { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
