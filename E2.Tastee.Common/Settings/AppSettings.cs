using System.Collections;
using System.Collections.Generic;
using E2.Tastee.Common.Dtos;

namespace E2.Tastee.Common
{
    /// <summary>
    /// Default template generated application settings file - see AppSettingsFactory for details
    /// </summary>
    public class AppSettings
    {
        public string ConnectionString { get; set; }

        [Default("300")]
        public int DefaultDatabaseCommandTimeoutSeconds { get; set; }

        public string MigrationsConnectionString { get; set; }
        public IList<string> ValidCorsOrigins { get; set; }
        public string AppURL { get; set; }
        public string AzureBlobAccount { get; set; }
        public string AzureBlobURL { get; set; }
        public string AppRoot { get; set; }
        [Default("Tastee")]
        public string AppName { get; set; }
        [Default("20")]
        public int DefaultPageSizeInRows { get; set; }
        public string ReportTemplatePath { get; set; }

        public string SharedApiKey { get; set; }
        public string JwtSecret { get; set; }
        public string JwtValidIssuer { get; set; }
        public string JwtValidAudience { get; set; }

        public string MailFromAddress { get; set; }
        public string MailFromName { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }

        public bool SmtpUseSsl { get; set; }
        public bool SmtpRequiresAuthentication { get; set; }
        //public string TestRecipientList { get; set; }
        public string LogoPath { get; set; }
        public string LogoMimeType { get; set; }
        public bool EnableCache { get; set; }
        public string StorageConnectionString { get; set; }
        public string DocumentStorageContainerName { get; set; }
        public string TwilioAccountSid { get; set; }
        public string TwilioAuthToken { get; set; }
        
        public static class Cache
        {
            public static Dictionary<TypeOfCache, CacheConfigDto> Config = new Dictionary<TypeOfCache, CacheConfigDto>()
            {
                {TypeOfCache.BlobContent, new CacheConfigDto() { Key = "BLOB_CONTENT_{0}", ExpiryMinutes = 120 }}
            };
        }
    }
}