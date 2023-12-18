using System;
using System.Configuration;
using Nl.Aet.Cid.Client.Sdk.Options;

namespace Nl.Aet.Cid.Client.Sdk.Sample.Terminal
{
    public class ConfigurationSection
    {
        public ConfigurationSection()
        {
            ServerUri = ConfigurationManager.AppSettings["ServerUri"];
            LogPath = ConfigurationManager.AppSettings["LogPath"];
            KeyUsageFilter  = (CertificateKeyUsageFilter)int.Parse(ConfigurationManager.AppSettings["KeyUsageFilter"]);
        }

        /// <summary>
        /// Gets the server URI 
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

        /// <summary>
        /// Gets or sets the group identifier.
        /// </summary>
        /// <value>
        /// The group identifier.
        /// </value>
        public Guid GroupId { get; set; }

        /// <summary>
        /// Gets the key usage filter.
        /// </summary>
        /// <value>
        /// The key usage filter.
        /// </value>
        public CertificateKeyUsageFilter KeyUsageFilter { get; }
    }
}
