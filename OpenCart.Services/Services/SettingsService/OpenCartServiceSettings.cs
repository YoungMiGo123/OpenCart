using OpenCart.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCart.Services.Services.SettingsService
{
    public class OpenCartServiceSettings : IOpenCartServiceSettings
    {
        public string ConnectionString { get; set; }
        public SecureOAuthSettings SecureOAuthSettings { get; set; }
        public string SeqUrl { get; set; }
    }
}
