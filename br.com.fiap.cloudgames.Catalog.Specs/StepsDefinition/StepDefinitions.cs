using Reqnroll;
using Xunit;

namespace br.com.fiap.cloudgames.Catalog.Specs.StepsDefinition
{
    [Binding]
    public class UserAuthenticationSteps
    {
        private string? _registeredEmail;
        private string? _registeredPassword;
        private string? _inputEmail;
        private string? _inputPassword;
        private bool _authSuccess;
        private string? _accessToken;

        [Given("a user with email {string} and password {string} exists")]
        public void GivenAUserWithEmailAndPasswordExists(string email, string password)
        {
            _registeredEmail = email;
            _registeredPassword = password;
        }

        [Given("no user exists with email {string}")]
        public void GivenNoUserExistsWithEmail(string email)
        {
            _registeredEmail = null;
            _registeredPassword = null;
        }

        [When("the user submits the login request with email {string} and password {string}")]
        public void WhenTheUserSubmitsTheLoginRequestWithEmailAndPassword(string email, string password)
        {
            _inputEmail = email;
            _inputPassword = password;
            if (_registeredEmail != null && _registeredEmail == _inputEmail && _registeredPassword == _inputPassword)
            {
                _authSuccess = true;
                _accessToken = "sample-access-token";
            }
            else
            {
                _authSuccess = false;
                _accessToken = null;
            }
        }

        [When("the user submits the login request with email {string} and wrong password {string}")]
        public void WhenTheUserSubmitsTheLoginRequestWithEmailAndWrongPassword(string email, string password)
        {
            WhenTheUserSubmitsTheLoginRequestWithEmailAndPassword(email, password);
        }

        [Then("the authentication should be successful")]
        public void ThenTheAuthenticationShouldBeSuccessful()
        {
            Assert.True(_authSuccess);
        }

        [Then("the response should contain an access token")]
        public void ThenTheResponseShouldContainAnAccessToken()
        {
            Assert.NotNull(_accessToken);
            Assert.NotEmpty(_accessToken);
        }

        [Then("the authentication should fail")]
        public void ThenTheAuthenticationShouldFail()
        {
            Assert.False(_authSuccess);
        }
    }

    [Binding]
    public class UserRegistrationSteps
    {
        private string? _firstName;
        private string? _lastName;
        private string? _email;
        private string? _password;
        private readonly List<string> _existingEmails = new();
        private bool _registrationSuccess;
        private string? _userId;
        private string? _errorMessage;

        [Given("a user with first name {string}, last name {string}, email {string} and password {string}")]
        public void GivenAUserWithFirstNameLastNameEmailAndPassword(string firstName, string lastName, string email, string password)
        {
            _firstName = firstName;
            _lastName = lastName;
            _email = email;
            _password = password;
        }

        [Given("a user with email {string} already exists")]
        public void GivenAUserWithEmailAlreadyExists(string email)
        {
            _existingEmails.Add(email);
        }

        [When("the user submits the registration request")]
        public void WhenTheUserSubmitsTheRegistrationRequest()
        {
            if (_email != null && _existingEmails.Contains(_email))
            {
                _registrationSuccess = false;
                _errorMessage = "Email already in use";
            }
            else
            {
                _registrationSuccess = true;
                _userId = Guid.NewGuid().ToString();
            }
        }

        [Then("the account should be created successfully")]
        public void ThenTheAccountShouldBeCreatedSuccessfully()
        {
            Assert.True(_registrationSuccess);
        }

        [Then("the response should contain the user id")]
        public void ThenTheResponseShouldContainTheUserId()
        {
            Assert.NotNull(_userId);
        }

        [Then("the registration should fail")]
        public void ThenTheRegistrationShouldFail()
        {
            Assert.False(_registrationSuccess);
        }

        [Then("an error {string} should be returned")]
        public void ThenAnErrorShouldBeReturned(string expectedError)
        {
            Assert.Equal(expectedError, _errorMessage);
        }
    }
}
