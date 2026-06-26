using br.com.fiap.cloudgames.Catalog.Application.UseCases.User.RegisterUser;

namespace br.com.fiap.cloudgames.Catalog.Specs.ContextData;

public class UserRegistrationContextData
{
    public RegisterUserRequest Request { get; set; }
    public RegisterUserResponse Response { get; set; }
    public Exception Exception { get; set; }
    public string RegisteredUserID { get; set; }
}