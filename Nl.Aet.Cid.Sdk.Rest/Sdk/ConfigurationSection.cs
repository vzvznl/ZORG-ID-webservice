using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Nl.Aet.Cid.Sdk.Rest.Sdk
{

    public class ConfigurationSection
    {
        public ConfigurationSection(IConfiguration configuration)
        {
            AuthenticationCertificatePath = configuration.GetValue<string>("SdkSettings:ClientCertificatePath", "text/plain");
            AuthenticationCertificatePassword = configuration.GetValue<string>("SdkSettings:ClientCertificatePassword", "text/plain");

            ServerUri = configuration.GetValue<string>("SdkSettings:ServerUri", "text/plain"); ;
            LogPath = configuration.GetValue<string>("SdkSettings:LogPath", "text/plain"); ;
        }

        /// <summary>
        /// Gets the client certificate thumprint.
        /// </summary>
        /// <value>
        /// The client certificate thumprint.
        /// </value>
        public string AuthenticationCertificatePassword { get; private set; }

        /// <summary>
        /// Gets the client certificate thumprint.
        /// </summary>
        /// <value>
        /// The client certificate thumprint.
        /// </value>
        public string AuthenticationCertificatePath { get; private set; }

        /// <summary>
        /// Gets the file store path.
        /// </summary>
        /// <value>
        /// The file store path.
        /// </value>
        public string ServerUri { get; private set; }

        /// <summary>
        /// Gets the log path.
        /// </summary>
        /// <value>
        /// The log path.
        /// </value>
        public string LogPath { get; private set; }

    }
}

