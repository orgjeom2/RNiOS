using System;
using System.Collections.Generic;
using CarHunters.Core.Settings;
using Grpc.Core;
namespace CarHunters.Core.Common.Services
{
    public class BaseApiService
    {
        protected Channel ConnectionChannel;

        public BaseApiService()
        {
            ConnectionChannel = new Channel($"{SettingUriService.BaseUri}:{SettingUriService.Port}",
                                  new SslCredentials(),
                                 new List<ChannelOption>
            {
                new ChannelOption(ChannelOptions.SslTargetNameOverride, SettingUriService.BaseUri)
            });
        }
    }
}
