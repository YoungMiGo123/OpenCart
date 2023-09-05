using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenCart.Services.Services.SettingsService;

namespace OpenCart.Api.Controllers
{
    public class SecureOAuthController : ControllerBase
    {
        private readonly ILogger<SecureOAuthController> _logger;
        private readonly IOpenCartServiceSettings _openCartSettings;

        public SecureOAuthController(ILogger<SecureOAuthController> logger, IOpenCartServiceSettings openCartSettings)
        {
            _logger = logger;
            _openCartSettings = openCartSettings;
        }
        [HttpGet]
        [Route("SecureLoginUsingGitHub")]
        public IActionResult SecureLoginUsingGitHub()
        {
            try
            {
                var authenticationProperties = new AuthenticationProperties
                {
                    RedirectUri = _openCartSettings.SecureOAuthSettings.RedirectUrl
                };

                var authenticationSchemes = new[] { "GitHub" };

                return Challenge(authenticationProperties, authenticationSchemes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured at SecureLoginUsingGitHub while attempting to login {ex}");

                return BadRequest("Failed to securely login using github, try again please");
            }
        }

        [HttpGet]
        [Route("SecureGithubCallbackResponse")]
        public IActionResult SecureGithubCallbackResponse()
        {
            return Ok("Success");
        }
    }
}
