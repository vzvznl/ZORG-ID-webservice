using Newtonsoft.Json;
using Nl.Aet.Cid.Client.Sdk;
using Nl.Aet.Cid.Client.Sdk.Web;
using Org.BouncyCastle.Crypto.Tls;
using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;

namespace Nl.Aet.Cid.Sdk.Rest.Sdk
{
    public class Wrapper : IDisposable
    {

        internal ConcurrentQueue<Message> _messages=new ConcurrentQueue<Message>();

        public Wrapper(IConfiguration configuration)
        {
            Init(configuration);
        }

        private IInstanceManager _instanceManager;

        private void Init(IConfiguration configuration)
        {
            //configure
            ConfigurationSection configurationSection = new ConfigurationSection(configuration);

            X509Certificate2 certificate = OpenCertificate(configurationSection.AuthenticationCertificatePath, configurationSection.AuthenticationCertificatePassword);

            Configuration webConfiguration = new Configuration(configurationSection.ServerUri, certificate, "Nl.Aet.Cid.Client.Sdk.Test.Web", Client.Sdk.LogLevel.Verbose, LogTarget.File, configurationSection.LogPath);
            InstanceManager.Initialize(webConfiguration);
            _instanceManager = InstanceManager.Instance();

            _instanceManager.SessionManager.SessionInitialized += SessionManager_SessionInitialized;
            _instanceManager.SessionManager.SessionOpenCancelled += SessionManager_SessionOpenCancelled;
            _instanceManager.SessionManager.SessionOpened += SessionManager_SessionOpened;
            _instanceManager.SessionManager.SessionClosed += SessionManager_SessionClosed;
            _instanceManager.TransactionManager.TransactionActionCancelled += TransactionManager_TransactionActionCancelled;
            _instanceManager.TransactionManager.TransactionActionFinished += TransactionManager_TransactionActionFinished;
        }

       

        private void TransactionManager_TransactionActionFinished(object sender, TransactionFinishedEventArgs eventArgs)
        {
            Message message = new Message(MessageType.TransactionFinished, JsonConvert.SerializeObject(eventArgs));
            _messages.Enqueue(message);
        }

        private void TransactionManager_TransactionActionCancelled(object sender, TransactionCancelledEventArgs eventArgs)
        {
            Message message = new Message(MessageType.TransactionCancelled, JsonConvert.SerializeObject(eventArgs));
            _messages.Enqueue(message);
        }

        private void SessionManager_SessionOpened(object sender, SessionOpenedEventArgs eventArgs)
        {
            Message message = new Message(MessageType.SessionOpened, JsonConvert.SerializeObject(eventArgs));
            _messages.Enqueue(message);
        }

        private void SessionManager_SessionClosed(object sender, SessionClosedEventArgs eventArgs)
        {
            Message message = new Message(MessageType.SessionClosed, JsonConvert.SerializeObject(eventArgs));
            _messages.Enqueue(message);
        }

        private void SessionManager_SessionOpenCancelled(object sender, SessionOpenCancelledEventArgs eventArgs)
        {
            Message message = new Message(MessageType.SessionOpenedCancelled, JsonConvert.SerializeObject(eventArgs));
            _messages.Enqueue(message);
        }

        private void SessionManager_SessionInitialized(object sender, SessionInitializedEventArgs eventArgs)
        {
            Message message = new Message(MessageType.SessionInitialized, JsonConvert.SerializeObject(eventArgs));
            _messages.Enqueue(message);
        }

        public IInstanceManager Manager { get { return _instanceManager; } }

        /// <summary>
        /// Opens the authentication certificate file and returns it as X509Certificate2, just one certificate is expected in the PFX file.
        /// </summary>
        /// <param name="certificatePath">The certificate path.</param>
        /// <param name="certificatePassword">The certificate input.</param>
        /// <returns></returns>
        private static X509Certificate2 OpenCertificate(string certificatePath, string certificatePassword)
        {
            //nasty hack
            string path = AppContext.BaseDirectory + certificatePath;

            // Create a collection object and populate it using the PFX file
            X509Certificate2Collection certificateCollection = new X509Certificate2Collection();

            certificateCollection.Import(path, certificatePassword, X509KeyStorageFlags.PersistKeySet);
            X509Certificate2 x509Certificate = certificateCollection.Cast<X509Certificate2>().FirstOrDefault(certificate => certificate.HasPrivateKey);
            return x509Certificate;
        }

        internal List<Message> Messages
        {
            get
            {
                List<Message> messages = new List<Message>();

                Message message = null;
                while (_messages.TryDequeue(out message))
                {
                    messages.Add(message);
                }
                return messages;
            }
        }

        public void Dispose()
        {
            _instanceManager?.Dispose();
        }
    }

    public enum MessageType
    { 
        SessionOpenedCancelled=0,
        SessionOpened=1,
        SessionClosed=2,
        SessionInitialized=3,
        TransactionCancelled=4,
        TransactionFinished=5
    }

    internal class Message
    {
        public Message(MessageType type, string payload)
        {
            Type = type;
            Payload = payload;
        }
        
        public MessageType Type { get; set; }
        public string Payload { get; set; }
    }
}