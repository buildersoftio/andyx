using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.App.Tenants
{
    public class TenantSettings
    {
        public bool AllowProductCreation { get; set; }
        public string DigitalSignature { get; set; }
        public bool EnableEncryption { get; set; }
        public bool EnableGeoReplication { get; set; }
        public TenantLogging Logging { get; set; }

        public bool EnableAuthorization { get; set; }
        public List<TenantToken> Tokens { get; set; }

        // Split tenants by certificates will not be possible with version two
        public string CertificatePath { get; set; }

        public TenantSettings()
        {
            AllowProductCreation = true;
            EnableEncryption = false;

            EnableAuthorization = false;
            Tokens = new List<TenantToken>();

            EnableGeoReplication = false;
            Logging = TenantLogging.ERROR_ONLY;
        }
    }
}
