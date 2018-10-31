using System;
namespace CarHunters.Core.Units.Network.Services.Abstractions
{
    public interface ITimelessTokenService
    {
        string AuthorizationToken { get; set; }
    }
}
