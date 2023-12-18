using System.Text;
using System.Diagnostics;
using Nl.Aet.Cid.Client.Sdk.ZorgID.Constants;
using Nl.Aet.Cid.Client.Sdk.Options;
using Nl.Aet.Cid.Client.Sdk.Sample.Terminal;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Transactions;
using System.Security.Cryptography.X509Certificates;

namespace Nl.Aet.Cid.Client.Sdk.Test.Terminal
{

    class Program
    {
        private const string DatetimeFormat1 = "yyyy-MM-ddTHH:mm:ss.fffZ";
        private const string DatetimeFormat2 = "yyyyMMddHHmmss";
        private static bool _active = true;

        enum AttachedMode
        {
            Smartcard = 0,
            SoftCertificate = 1
        }

        static readonly Dictionary<Guid, SecureElement> OpenSessions = new Dictionary<Guid, SecureElement>();
        static ConfigurationSection _configurationSection = null;

        const string SampleText1 = Helper.SampleText1;
        const string SampleText2 = Helper.SampleText2;

        static CertificateKeyUsageFilter _certificateKeyUsageFilter = CertificateKeyUsageFilter.ClientAuthentication;

        static void Main(string[] args)
        {
            Console.WriteLine("--- AET CID SDK REST SAMPLE APPLICATION ---");

            Init();
            Menu();
        }

        private static void Init()
        {
            _configurationSection = new ConfigurationSection();
            Helper.LogPath = _configurationSection.LogPath;
            new Thread(() => Listen()) { IsBackground = true }.Start();
        }


        /// <summary>
        /// 
        /// </summary>
        private static void Menu()
        {
            int n;
            do
            {
                Console.WriteLine();
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine("3 - Open session");
                Console.WriteLine("5 - Sign Hash (sha-1)");
                Console.WriteLine("6 - Sign Hash (sha-256)");
                Console.WriteLine("7 - Sign Token-1 (sha-1)");
                Console.WriteLine("8 - Sign Token-1 (sha-256)");
                Console.WriteLine("9 - Close session");
                Console.WriteLine("11 - Exit the Testing Framework");
                Console.WriteLine("15 - Sign Token-2 (sha-1)");
                Console.WriteLine("16 - Sign Token-2 (sha-256)");
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine("Enter your option: ");

                n = ReadIntInput();

                switch (n)
                {

                    case 3:
                        OpenSession(); //Web
                        break;
                    case 5:
                        SignHashSha1();
                        break;
                    case 6:
                        SignHashSha256();
                        break;
                    case 7:
                        SignToken1Sha1();
                        break;
                    case 8:
                        SignToken1Sha256();
                        break;
                    case 9:
                        CloseSession();
                        break;
                    case 11:
                        break;
                    case 12:
                        break;
                    case 15:
                        SignToken2Sha1();
                        break;
                    case 16:
                        SignToken2Sha256();
                        break;
                    default:
                        Console.WriteLine("Please insert a number (0-16)");
                        break;
                }
            } while (n != 11);//exit
            _active = false;
        }


        private static HttpClient ConstructClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_configurationSection.ServerUri);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }


        private static void Listen()
        {
            while (_active)
            {
                var client = ConstructClient();
                try
                {
                    HttpResponseMessage response = client.GetAsync(_configurationSection.ServerUri + "/v1/events").Result;
                    if (response.IsSuccessStatusCode)
                    {

                        string json = response.Content.ReadAsStringAsync().Result;
                        List<Message> messages = JsonConvert.DeserializeObject<List<Message>>(json);

                        foreach (Message message in messages)
                        {
                            switch (message.Type)
                            {

                                case MessageType.SessionInitialized:
                                    {
                                        SessionInitializedEventArgs args = JsonConvert.DeserializeObject<SessionInitializedEventArgs>(message.Payload);
                                        SessionManager_SessionInitialized(args);
                                    }
                                    break;


                                case MessageType.SessionOpened:
                                    {
                                        JsonSerializerSettings settings = new JsonSerializerSettings();

                                        SessionOpenedEventArgs args = JsonConvert.DeserializeObject<SessionOpenedEventArgs>(message.Payload);
                                        SessionManager_SessionOpened(args);
                                    }
                                    break;

                                case MessageType.SessionClosed:
                                    {
                                        JsonSerializerSettings settings = new JsonSerializerSettings();

                                        SessionClosedEventArgs args = JsonConvert.DeserializeObject<SessionClosedEventArgs>(message.Payload);
                                        SessionManager_SessionClosed(args);
                                    }
                                    break;

                                case MessageType.SessionOpenedCancelled:
                                    {

                                        SessionOpenCancelledEventArgs args = JsonConvert.DeserializeObject<SessionOpenCancelledEventArgs>(message.Payload);
                                        SessionManager_SessionOpenCancelled(args);
                                    }
                                    break;
                                case MessageType.TransactionFinished:
                                    {

                                        TransactionFinishedEventArgs args = JsonConvert.DeserializeObject<TransactionFinishedEventArgs>(message.Payload);
                                        TransactionManager_TransactionActionFinished(args);
                                    }
                                    break;
                                
                                case MessageType.TransactionCancelled:
                                    {

                                        TransactionCancelledEventArgs args = JsonConvert.DeserializeObject<TransactionCancelledEventArgs>(message.Payload);
                                        TransactionManager_TransactionActionCancelled(args);
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"UNEXPECTED HTTP EVENTS: {response.StatusCode}");
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"UNEXPECTED EXCEPTION : exception");
                }

                Thread.Sleep(200);
            }
        }

        private static void OpenSession()
        {
            var client = ConstructClient();
            HttpResponseMessage response = client.PostAsync(_configurationSection.ServerUri + "/v1/sessions/open", null).Result;

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                Identifier identifier = JsonConvert.DeserializeObject<Identifier>(json);

                DisplayCommand(CommandDisplayFormatter.DisplayString("Sessions.Open", "sessionId", identifier.Id));
            }
            else
            {
                Console.WriteLine($"UNEXPECTED HTTP OPEN SESSION: {response.StatusCode}");
            }
        }

        private class Identifier { public Guid Id { get; set; } }

        private static void SignHashSha1()
        {
            Guid sessionId = SelectOpenSession();

            SignDataRequestBag signRequestBag = new SignDataRequestBag();
            signRequestBag.RequestOptions.SignatureOptions.CertificateFilter.CertificateKeyUsageFilter = _certificateKeyUsageFilter;
            SignDataRequest signRequest = new SignDataRequest(HashSha1(SampleText2), Guid.NewGuid().ToString());
            signRequestBag.HashAlgorithm = "sha-1";
            signRequestBag.Add(signRequest);
            signRequestBag.SessionId = sessionId;


            var client = ConstructClient();
            HttpResponseMessage response = client.PostAsJsonAsync(_configurationSection.ServerUri + "/v1/transactions", signRequestBag).Result;

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                Identifier identifier = JsonConvert.DeserializeObject<Identifier>(json);

                DisplayCommand(CommandDisplayFormatter.DisplayString("Transaction.Submit", "transactionId", identifier.Id));
            }
            else
            {
                Console.WriteLine($"UNEXPECTED HTTP OPEN SESSION: {response.StatusCode}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void SignHashSha256()
        {
            Guid sessionId = SelectOpenSession();
            SignDataRequestBag signRequestBag = new SignDataRequestBag();
            SignDataRequest signRequest1 = new SignDataRequest(HashSha256(SampleText1), Guid.NewGuid().ToString());
            signRequestBag.HashAlgorithm = HashAlgorithNames.Sha256;
            signRequestBag.Add(signRequest1);
            signRequestBag.SessionId = sessionId;

            var content = new StringContent(JsonConvert.SerializeObject(signRequestBag), Encoding.UTF8, "application/json");

            HttpClient client = ConstructClient();
            HttpResponseMessage response = client.PostAsync(_configurationSection.ServerUri + "/v1/transactions", content).Result;

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                Identifier identifier = JsonConvert.DeserializeObject<Identifier>(json);
                DisplayCommand(CommandDisplayFormatter.DisplayString("TransactionManager.Sign", "transactionId", identifier.Id));
            }
            else
            {
                Console.WriteLine($"UNEXPECTED HTTP Transaction Sign: {response.StatusCode}");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static void SignToken1Sha1()
        {
            Guid sessionId = SelectOpenSession();

            string data1 = File.ReadAllText("Token1_A.Xml", Encoding.UTF8);
            SignDataRequestBag signRequestBag = new SignDataRequestBag("token-1");
            signRequestBag.RequestOptions.SignatureOptions.CertificateFilter.CertificateKeyUsageFilter = _certificateKeyUsageFilter;
            signRequestBag.HashAlgorithm = "sha-1";
            SignDataRequest signRequest = new SignDataRequest(data1, Guid.NewGuid().ToString());
            signRequestBag.Add(signRequest);
            signRequestBag.SessionId = sessionId;

            var content = new StringContent(JsonConvert.SerializeObject(signRequestBag), Encoding.UTF8, "application/json");

            HttpClient client = ConstructClient();
            HttpResponseMessage response = client.PostAsync(_configurationSection.ServerUri + "/v1/transactions", content).Result;

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                Identifier identifier = JsonConvert.DeserializeObject<Identifier>(json);
                DisplayCommand(CommandDisplayFormatter.DisplayString("TransactionManager.Sign", "transactionId", identifier.Id));
            }
            else
            {
                Console.WriteLine($"UNEXPECTED HTTP Transaction Sign: {response.StatusCode}");
            }
        }


        private static void SignToken1Sha256()
        {
            Guid sessionId = SelectOpenSession();
            string data1 = File.ReadAllText("Token1_A.Xml", Encoding.UTF8);
            SignDataRequestBag signRequestBag = new SignDataRequestBag(ProfileNames.Token1);
            signRequestBag.RequestOptions.SignatureOptions.CertificateFilter.CertificateKeyUsageFilter = _certificateKeyUsageFilter;
            signRequestBag.HashAlgorithm = HashAlgorithNames.Sha256;
            SignDataRequest signRequest = new SignDataRequest(data1, Guid.NewGuid().ToString());
            signRequestBag.Add(signRequest);
            signRequestBag.SessionId = sessionId;

            var content = new StringContent(JsonConvert.SerializeObject(signRequestBag), Encoding.UTF8, "application/json");

            HttpClient client = ConstructClient();
            HttpResponseMessage response = client.PostAsync(_configurationSection.ServerUri + "/v1/transactions", content).Result;

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                Identifier identifier = JsonConvert.DeserializeObject<Identifier>(json);
                DisplayCommand(CommandDisplayFormatter.DisplayString("TransactionManager.Sign", "transactionId", identifier.Id));
            }
            else
            {
                Console.WriteLine($"UNEXPECTED HTTP Transaction Sign: {response.StatusCode}");
            }

        }

        private static void SignToken2Sha1()
        {
            Guid sessionId = SelectOpenSession();


            string data = File.ReadAllText("Token2_A.Xml", Encoding.UTF8);

            //change date time.
            data = data.Replace("{IssueInstant}", DateTime.UtcNow.ToString(DatetimeFormat1));
            data = data.Replace("{NotBefore}", DateTime.UtcNow.Subtract(new TimeSpan(0, 0, 5, 0)).ToString(DatetimeFormat1));
            data = data.Replace("{NotAfter}", DateTime.UtcNow.Add(new TimeSpan(0, 0, 60, 0)).ToString(DatetimeFormat1));
            data = data.Replace("{AuthnInstant}", DateTime.UtcNow.ToString(DatetimeFormat1));
            data = data.Replace("{executionAndDeliveryTime}", DateTime.UtcNow.Add(new TimeSpan(0, 0, 90, 0)).ToString(DatetimeFormat2));
            data = data.Replace("{CreationTime}", DateTime.UtcNow.Add(new TimeSpan(0, 0, 90, 0)).ToString(DatetimeFormat2));

            if (OpenSessions.ContainsKey(sessionId))
            {
                SecureElement secureElement = OpenSessions[sessionId];
                if ((secureElement != null) && (secureElement.AuthenticationCertificate.ParsedFields.Count > 0))
                {
                    //TODO: What about non-repudiation
                    data = data.Replace("{UziRegisterSubscriberNumber}", secureElement.AuthenticationCertificate.ParsedFields["UziRegisterSubscriberNumber"]);
                    data = data.Replace("{UziNumber}", secureElement.AuthenticationCertificate.ParsedFields["UziNumber"]);
                    data = data.Replace("{RoleCode}", secureElement.AuthenticationCertificate.ParsedFields["RoleCode"]);
                    data = data.Replace("{Role}", secureElement.AuthenticationCertificate.ParsedFields["Role"]);
                    data = data.Replace("{PassholderName}", secureElement.AuthenticationCertificate.ParsedFields["PassholderName"]);

                    X509Certificate2 certificate = new X509Certificate2(Convert.FromBase64String(secureElement.AuthenticationCertificate.RawValue));
                    string issuerName = ConvertIssuerName(certificate.IssuerName.Name);

                    data = data.Replace("{X509IssuerName}", issuerName);
                    data = data.Replace("{X509SerialNumber}", certificate.SerialNumberAsDecimal());
                }
            }

            SignDataRequestBag signRequestBag = new SignDataRequestBag("token-2");
            signRequestBag.HashAlgorithm = "sha-1";
            SignDataRequest signRequest1 = new SignDataRequest(data, Guid.NewGuid().ToString());
            signRequestBag.RequestOptions.SignatureOptions.CertificateFilter.CertificateKeyUsageFilter = _certificateKeyUsageFilter;
            signRequestBag.Add(signRequest1);
            signRequestBag.SessionId = sessionId;

            var content = new StringContent(JsonConvert.SerializeObject(signRequestBag), Encoding.UTF8, "application/json");

            HttpClient client = ConstructClient();
            HttpResponseMessage response = client.PostAsync(_configurationSection.ServerUri + "/v1/transactions", content).Result;

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                Identifier identifier = JsonConvert.DeserializeObject<Identifier>(json);
                DisplayCommand(CommandDisplayFormatter.DisplayString("TransactionManager.Sign", "transactionId", identifier.Id));
            }
            else
            {
                Console.WriteLine($"UNEXPECTED HTTP Transaction Sign: {response.StatusCode}");
            }
        }

        private static void SignToken2Sha256()
        {
            Guid sessionId = SelectOpenSession();

            string data = File.ReadAllText("Token2_A.Xml", Encoding.UTF8);

            //change date time.
            data = data.Replace("{IssueInstant}", DateTime.UtcNow.ToString(DatetimeFormat1));
            data = data.Replace("{NotBefore}", DateTime.UtcNow.Subtract(new TimeSpan(0, 0, 5, 0)).ToString(DatetimeFormat1));
            data = data.Replace("{NotAfter}", DateTime.UtcNow.Add(new TimeSpan(0, 0, 60, 0)).ToString(DatetimeFormat1));
            data = data.Replace("{AuthnInstant}", DateTime.UtcNow.ToString(DatetimeFormat1));
            data = data.Replace("{executionAndDeliveryTime}", DateTime.UtcNow.Add(new TimeSpan(0, 0, 90, 0)).ToString(DatetimeFormat2));
            data = data.Replace("{CreationTime}", DateTime.UtcNow.Add(new TimeSpan(0, 0, 90, 0)).ToString(DatetimeFormat2));

            if (OpenSessions.ContainsKey(sessionId))
            {
                SecureElement secureElement = OpenSessions[sessionId];
                if ((secureElement != null) && (secureElement.Certificates[0].ParsedFields.Count > 0))
                {
                    //TODO: What about non-repudiation
                    data = data.Replace("{UziRegisterSubscriberNumber}", secureElement.AuthenticationCertificate.ParsedFields["UziRegisterSubscriberNumber"]);
                    data = data.Replace("{UziNumber}", secureElement.AuthenticationCertificate.ParsedFields["UziNumber"]);
                    data = data.Replace("{RoleCode}", secureElement.AuthenticationCertificate.ParsedFields["RoleCode"]);
                    data = data.Replace("{Role}", secureElement.AuthenticationCertificate.ParsedFields["Role"]);
                    data = data.Replace("{PassholderName}", secureElement.AuthenticationCertificate.ParsedFields["PassholderName"]);

                    X509Certificate2 certificate = new X509Certificate2(Convert.FromBase64String(secureElement.AuthenticationCertificate.RawValue));
                    string issuerName = ConvertIssuerName(certificate.IssuerName.Name);

                    data = data.Replace("{X509IssuerName}", issuerName);
                    data = data.Replace("{X509SerialNumber}", certificate.SerialNumberAsDecimal());
                }
            }

            SignDataRequestBag signRequestBag = new SignDataRequestBag("token-2");
            signRequestBag.HashAlgorithm = "sha-256";
            SignDataRequest signRequest = new SignDataRequest(data, Guid.NewGuid().ToString());
            signRequestBag.RequestOptions.SignatureOptions.CertificateFilter.CertificateKeyUsageFilter = _certificateKeyUsageFilter;
            signRequestBag.Add(signRequest);
            signRequestBag.SessionId = sessionId;

            var content = new StringContent(JsonConvert.SerializeObject(signRequestBag), Encoding.UTF8, "application/json");

            HttpClient client = ConstructClient();
            HttpResponseMessage response = client.PostAsync(_configurationSection.ServerUri + "/v1/transactions", content).Result;

            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                Identifier identifier = JsonConvert.DeserializeObject<Identifier>(json);
                DisplayCommand(CommandDisplayFormatter.DisplayString("TransactionManager.Sign", "transactionId", identifier.Id));
            }
            else
            {
                Console.WriteLine($"UNEXPECTED HTTP Transaction Sign: {response.StatusCode}");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private static void CloseSession()
        {
            Guid sessionId = SelectOpenSession();

            //_sessionManager.Close(sessionId);
            DisplayCommand(CommandDisplayFormatter.DisplayString("SessionManager.Close", "sessionId",sessionId));
            OpenSessions.Remove(sessionId);
        }


        

        #region Event Handlers

        private static void SecureElement_RetrieveSecureElementCancelled(object sender, SecureElementRetrieveCancelledEventArgs args)
        {
            DisplayEvent(args.ToDisplayString());
        }

        private static void SecureElement_RetrievedSecureElement(object sender, SecureElementRetrievedEventArgs args)
        {
            DisplayEvent(args.ToDisplayString());
        }

        private static void _secureElementStore_SecureElementRemoved(object sender, SecureElementRemovedEventArgs args)
        {
            DisplayEvent(args.ToDisplayString());
        }

        private static void _secureElementStore_SecureElementInserted(object sender, SecureElementInsertedEventArgs args)
        {
            DisplayEvent(args.ToDisplayString());
        }

        private static void _secureElementStore_SecureElementReady(object sender, SecureElementReadyEventArgs args)
        {
            DisplayEvent(args.ToDisplayString());
        }

        private static void SessionManager_SessionOpened(SessionOpenedEventArgs args)
        {
            DisplayEvent(args.ToDisplayString());
            try
            {
                OpenSessions.Add(args.SessionId, args.SecureElement);
            }
            catch(Exception exception)
            {
                Debug.WriteLine("Exception SessionManager_SessionOpened: " + exception.ToString());
            }
        }

        private static void SessionManager_SessionOpenCancelled(SessionOpenCancelledEventArgs args)
        {
            DisplayEvent(args.ToDisplayString());
        }

        private static void SessionManager_SessionClosed(SessionClosedEventArgs args)
        {
            DisplayEvent(args.ToDisplayString());
            try
            {
                OpenSessions.Remove(args.SessionId);
            }
            catch
            {
                //do nothing
            }
        }

        private static void SessionManager_SessionInitialized(SessionInitializedEventArgs args)
        {
            DisplayEvent(args.ToDisplayString());
            Console.WriteLine("Session Initialized! Nonce:{0}",args.Nonce.Value);
        }
        
        private static void TransactionManager_TransactionActionCancelled(TransactionCancelledEventArgs args)
        {
            DisplayEvent(args.ToDisplayString());
        }

        private static void TransactionManager_TransactionActionFinished(TransactionFinishedEventArgs args)
        {
            DisplayEvent(args.ToDisplayString());
        }

        

        #endregion

        /// <summary>
        ///  This method is for the user to select a opened session to be used.
        /// </summary>
        /// <returns></returns>
        public static Guid SelectOpenSession()
        {
            Guid selectedSession = Guid.NewGuid();

            for (int i = 0; i < OpenSessions.Count; i++)
            {
                Console.WriteLine(i + " - session number: " + OpenSessions.ElementAt(i).ToString());
            }
            Console.WriteLine("Please insert the index for the matching session, use a non existing index (e.g. 9) for a random session id: ");

            int n = ReadIntInput();
            if (n < OpenSessions.Count)
            {
                selectedSession = OpenSessions.Keys.ElementAt(n);
            }
            return selectedSession;
        }


        private static int ReadIntInput()
        {
            return Helper.ReadIntInput();
        }

        

        private static string HashSha1(string value)
        {
            return Helper.HashSha1(value);
        }

        private static string HashSha256(string value)
        {
            return Helper.HashSha256(value);
        }

        private static void DisplayEvent(string eventDisplayString)
        {
            Helper.DisplayEvent(eventDisplayString);
        }

        private static void DisplayCommand(string commandDisplayString)
        {
            Helper.DisplayCommand(commandDisplayString);
        }

        private static string ConvertIssuerName(string issuerName)
        {
            if (issuerName == string.Empty) return string.Empty;
            List<Tuple<string, string>> subfields = ParseField(issuerName);
            List<Tuple<string, string>> convertedSubfields = new List<Tuple<string, string>>();
            foreach (var subfield in subfields)
            {
                Tuple<string, string> convertedSubfield = subfield;
                if (IsOrganizationName(subfield)) convertedSubfield = ConvertOrganizationName(subfield);
                convertedSubfields.Add(convertedSubfield);
            }
            string result = string.Empty;
            foreach (var convertedSubfield in convertedSubfields)
            {
                if (convertedSubfield != null) result += convertedSubfield.Item1.Trim() + "=" + convertedSubfield.Item2.Trim() + ", ";
            }
            if (result.Length > 0) result = result.Substring(0, result.Length - 2);
            return result;
        }

        private static List<Tuple<string, string>> ParseField(string field)
        {
            string[] subFields = field.Split(',');
            List<Tuple<string, string>> result = subFields.Select(ToKeyValue).ToList();
            return result;
        }

        private static Tuple<string, string> ToKeyValue(string subfield)
        {
            if (subfield == string.Empty) return null;
            string[] keyValue = subfield.Split('=');
            return new Tuple<string, string>(keyValue[0], keyValue[1]);
        }

        private static bool IsOrganizationName(Tuple<string, string> fieldTuple)
        {
            if (fieldTuple == null) return false;
            return fieldTuple.Item1.Contains("2.5.4.97");
        }

        private static Tuple<string, string> ConvertOrganizationName(Tuple<string, string> organizationTuple)
        {
            string key = organizationTuple.Item1.Replace("OID.", string.Empty);

            byte[] bytes = Encoding.UTF8.GetBytes(organizationTuple.Item2);
            string value = "#0C" + bytes.Length.ToString("X2") + BitConverter.ToString(bytes).Replace("-", string.Empty);
            value = value.ToLower();
            return new Tuple<string, string>(key, value);
        }
    }

    public enum MessageType
    {
        SessionOpenedCancelled = 0,
        SessionOpened = 1,
        SessionClosed = 2,
        SessionInitialized = 3,
        TransactionCancelled = 4,
        TransactionFinished = 5
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
