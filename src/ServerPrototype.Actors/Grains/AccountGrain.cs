using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using ServerPrototype.Interfaces;
using ServerPrototype.Interfaces.Grains;
using ServerPrototype.Interfaces.Messages.Requests;

namespace ServerPrototype.Actors.Grains
{
    public class AccountGrain : Grain<AccountGrain.AccountState>, IAccountGrain
    {
        public class AccountState
        {
            public string UserId { get; set; }

            public DateTime LastLogin { get; set; }

            public DateTime Created { get; set; }

            public string Id { get; set; }
        }

        private readonly ILogger _log;

        public AccountGrain(ILogger<AccountGrain> log)
        {
            _log = log;
        }

        public override Task OnActivateAsync()
        {
            State.Id = GrainReference.GetPrimaryKeyString();

            return base.OnActivateAsync();
        }

        public async Task<ApiResult<LoginResponse>> Login(LoginRequest request)
        {
            _log.LogInformation($"Login called. device_id: {GrainReference.GetPrimaryKeyString()}. UserId: `{State.UserId}`");

            if (string.IsNullOrEmpty(State.UserId))
            {
                State.UserId = Guid.NewGuid().ToString();
                State.Created = DateTime.UtcNow;
            }

            State.LastLogin = DateTime.UtcNow;

            await WriteStateAsync();

            return new ApiResult<LoginResponse>(LoginResponse.Create(State.UserId));
        }
    }
}
