using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Net;

namespace Nl.Aet.Cid.Client.Sdk.Sample.Terminal
{
    internal static class JsonExtension
    {
        public static string SESSION_INITIALIZED = "sessionInitialized";
        public static string SESSION_OPENED = "sessionOpened";
        public static string SESSION_OPEN_CANCELLED = "sessionOpenCancelled";
        public static string SESSION_CLOSED = "sessionClosed";
        public static string TRANSACTION_FINISHED = "transactionFinished";
        public static string TRANSACTION_CANCELLED = "transactionCancelled";
        public static string SECURE_ELEMENT_RETRIEVED = "secureElementRetrieved";
        public static string SECURE_ELEMENT_CANCELLED = "secureElementCancelled";
        public static string SECURE_ELEMENT_INSERTED = "secureElementInserted";
        public static string SECURE_ELEMENT_READY = "secureElementReady";
        public static string SECURE_ELEMENT_REMOVED = "secureElementRemoved";
        public static string SECURE_ELEMENT_RETRIEVED_COMMAND = "SecureElementStore.Retrieve";
        public static string SESSION_OPENED_DETACHED_COMMAND = "SessionManager.OpenDetached";
        public static string SESSION_OPENED_ATTACHED_COMMAND = "SessionManager.OpenAttached";
        public static string TRANSACTION_SIGN_COMMAND = "TransactionManager.Sign";
        public static string SESSION_CLOSE_COMMAND = "SessionManager.Close";
        public static string DIAGNOSE_RETRIEVED = "diagnoseInformationRetrieved";
        public static string DATA_RETRIEVE_CANCELLED = "dataEntryRetrieveCancelled";
        public static string DATAS_RETRIEVE_CANCELLED = "dataEntriesRetrieveCancelled";
        public static string DATAS_RETRIEVED = "dataEntriesRetrieved";
        public static string DATA_RETRIEVED = "dataEntryRetrieved";

        private static JsonSerializerSettings settings;
        static JsonExtension()
        {
            settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
            settings.Converters.Add(new IPAddressConverter());
        }

        /// <summary>
        /// Turns an object into an json string. The object must have a default constructor.
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="toSerialize">The object to serialize.</param>
        /// <returns>An json string.</returns>
        public static string ToJson<T>(this T toSerialize)
        {           
            return JsonConvert.SerializeObject(toSerialize, settings);
        }

        public class JsonCommandLog
        {
            public JsonCommandLog() { }

            public static JsonCommandLog CreateLogEntry(Guid sessionId)
            {
                return new JsonCommandLog(sessionId);
            }

            public static JsonCommandLog CreateLogEntry(Guid sessionId, Guid transactionId, SignDataRequest signDataRequest)
            {
                return new JsonCommandLog(sessionId, transactionId, signDataRequest);
            }

            public static JsonCommandLog CreateLogEntry(Guid sessionId, Guid transactionId, SignDataRequestBag signDataRequestBag)
            {
                return new JsonCommandLog(sessionId, transactionId, signDataRequestBag);
            }

            private JsonCommandLog(Guid sessionid)
            {
                SessionId = sessionid;
            }

            private JsonCommandLog(Guid sessionId, Guid transactionId, SignDataRequest signDataRequest)
            {
                TransactionId = transactionId;
                SessionId = sessionId;
                SignDataRequest = signDataRequest;
            }

            private JsonCommandLog(Guid sessionId, Guid transactionId, SignDataRequestBag signDataRequestBag)
            {
                TransactionId = transactionId;
                SessionId = sessionId;
                SignDataRequestBag = signDataRequestBag;
            }     

            public Guid TransactionId { get; set; }
            public Guid SessionId { get; set; }
            public SignDataRequest SignDataRequest { get; set; }
            public SignDataRequestBag SignDataRequestBag { get; set; }

        }
    }

    internal class IPAddressConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IPAddress));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return IPAddress.Parse((string)reader.Value);
        }
    }
}
