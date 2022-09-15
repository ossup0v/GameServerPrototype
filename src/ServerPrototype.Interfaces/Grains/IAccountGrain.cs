using Orleans;
using ServerPrototype.Interfaces.Messages.Requests;

namespace ServerPrototype.Interfaces.Grains
{
    public interface IAccountGrain : IGrainWithStringKey
    {
        Task<ApiResult<LoginResponse>> Login(LoginRequest request);
    }
}