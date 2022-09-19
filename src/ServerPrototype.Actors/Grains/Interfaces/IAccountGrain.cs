using Orleans;
using ServerPrototype.Actors.Grains.Messages.Requests;
using ServerPrototype.Common;

namespace ServerPrototype.Actors.Grains.Interfaces
{
    public interface IAccountGrain : IGrainWithStringKey
    {
        Task<ApiResult<LoginResponse>> Login(LoginRequest request);
    }
}