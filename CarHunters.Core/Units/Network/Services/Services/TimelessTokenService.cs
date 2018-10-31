using System;
using CarHunters.Core.Units.Network.Services.Abstractions;
using CarHunters.Core.PlatformAbstractions;
namespace CarHunters.Core.Units.Network.Services.Services
{
    public class TimelessTokenService : ITimelessTokenService
    {
        private readonly ISettingsBaseService settingsBaseService;

        public TimelessTokenService(ISettingsBaseService settingsBaseService)
        {
            this.settingsBaseService = settingsBaseService;
            _authorizationToken = this.settingsBaseService.Get(string.Empty, nameof(AuthorizationToken));
        }

        private string _authorizationToken;
        public string AuthorizationToken
        {
            get => _authorizationToken;
            set
            {
                _authorizationToken = value;
                this.settingsBaseService.Set(value, nameof(AuthorizationToken));
            }
        }
    }
}
