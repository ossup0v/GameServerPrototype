using Orleans.Concurrency;

namespace ServerPrototype.Actors.Grains.Messages.Requests
{
    [Immutable]
    public sealed class LoginRequest
    {
        public static LoginRequest Instance = new LoginRequest();
    }


    [Immutable]
    public class LoginResponse
    {
        private LoginResponse(string userId) { UserId = userId; }
        public static LoginResponse Create(string userId) => new LoginResponse(userId);

        public string UserId { get; set; }
    }
}
