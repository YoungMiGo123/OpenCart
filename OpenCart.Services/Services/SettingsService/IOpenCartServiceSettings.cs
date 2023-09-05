using OpenCart.Models.Models;

namespace OpenCart.Services.Services.SettingsService
{
    public interface IOpenCartServiceSettings
    {
        string ConnectionString { get; set; }
        SecureOAuthSettings SecureOAuthSettings { get; set; }
        string SeqUrl { get; set; }
    }
}
