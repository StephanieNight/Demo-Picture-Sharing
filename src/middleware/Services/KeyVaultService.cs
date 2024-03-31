using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace picture_sharing.Services
{
    public class KeyVaultService
    {
        private readonly SecretClient _client;

        public KeyVaultService(string vaultUri)
        {
            var credential = new DefaultAzureCredential();
            _client = new SecretClient(new Uri(vaultUri), credential);
        }

        public string GetSecret(string secretName)
        {
            try
            {
                var secret = _client.GetSecret(secretName);
                return secret.Value.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching secret: {ex.Message}");
                throw;
            }
        }
    }
}

