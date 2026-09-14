using CredentialManagement;

namespace FGC_Stat_Analyzer_wpf.Services
{
    public class KeyManager
    {
        private const string Target = "PotMonster.ApiKey";

        public void SaveKey(string apiKey)
        {
            var credential = new Credential
            {
                Target = Target,
                Password = apiKey,
                PersistanceType = PersistanceType.LocalComputer,
                Type = CredentialType.Generic
            };

            if (!credential.Save())
                throw new InvalidOperationException("Failed to save API key.");
        }

        public string? GetKey()
        {
            var credential = new Credential
            {
                Target = Target,
                Type = CredentialType.Generic
            };

            if (credential.Load())
            {
                return credential.Password;
            } 
            else
            {
                return null;
            }   
        }

        public void DeleteKey()
        {
            var credential = new Credential
            {
                Target = Target,
                Type = CredentialType.Generic
            };

            credential.Delete();
        }
    }
}
