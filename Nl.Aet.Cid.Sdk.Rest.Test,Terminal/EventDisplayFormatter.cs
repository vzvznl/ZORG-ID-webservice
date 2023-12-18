using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nl.Aet.Cid.Client.Sdk.ZorgID.Constants;
using static Nl.Aet.Cid.Client.Sdk.Sample.Terminal.JsonExtension;

namespace Nl.Aet.Cid.Client.Sdk.Sample.Terminal
{
    internal static class TypeExtensions
    {
        const string DateTimeFormat = "o"; //ISO 8601
        const string LogInfo = "INFO";
        const string LogEvent = "Event";
        const string LogCommand = "Command";
        const string LogPipe = "|";

        public static string ToJsonEventString(this EventArgs args, string eventName)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.Append(LogPipe);
            builder.Append(LogInfo);
            builder.Append(LogPipe);
            builder.Append(LogEvent);
            builder.Append(LogPipe);
            builder.Append(eventName);
            builder.Append(LogPipe);
            builder.Append(args.ToJson());
            builder.AppendLine("");
            return builder.ToString();
        }

        public static string ToJsonCommandString(this JsonCommandLog args, string commandName)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.Append(LogPipe);
            builder.Append(LogInfo);
            builder.Append(LogPipe);
            builder.Append(LogCommand);
            builder.Append(LogPipe);
            builder.Append(commandName);
            builder.Append(LogPipe);
            builder.Append(args.ToJson());
            builder.AppendLine("");
            return builder.ToString();
        }

        public static string ToDisplayString(this SecureElementRetrieveCancelledEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Incomming ---> SecureElementRetrieveCancelledEventArgs ");
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine(string.Format("SessionId:             {0}",args.SessionId));
            builder.AppendLine(string.Format("Reason:                {0}", args.Reason));
            builder.AppendLine("");

            return builder.ToString();
        }

        public static string ToDisplayString(this DataEntryRetrieveCancelledEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Incomming ---> DataEntryRetrieveCancelled ");
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine(string.Format("SessionId:             {0}", args.SessionId));
            builder.AppendLine(string.Format("Reason:                {0}", args.Reason));
            builder.AppendLine("");

            return builder.ToString();
        }

        public static string ToDisplayString(this DataEntryRetrievedEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Incomming ---> DataEntryRetrieved ");
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine(string.Format("SessionId:             {0}", args.SessionId));
            builder.AppendLine(string.Format($"SCAN-TOKEN: {args.SignedBag.FirstOrDefault().Value.Data.Replace("\n", "").Replace("\r", "")}"));

            foreach (var attribute in args.Attributes)
            {
                builder.AppendLine(string.Format($"-sub-  WID attribute ({attribute.Key} | {attribute.Value} )"));
            }
            return builder.ToString();
        }

        public static string ToDisplayString(this DataEntriesRetrievedEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Incomming ---> DataEntriesRetrieved ");
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine(string.Format("SessionId:             {0}", args.SessionId));
            foreach (var entry in args.DataEntries)
            {
                builder.AppendLine(string.Format($"-sub-  entry ({entry.Key} | {entry.Value}"));
            }
            builder.AppendLine("");
            return builder.ToString();
        }

        public static string ToDisplayString(this DataEntriesRetrieveCancelledEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Incomming ---> DataEntriesRetrieveCancelled ");
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine(string.Format("SessionId:             {0}", args.SessionId));
            builder.AppendLine(string.Format("Reason:                {0}", args.Reason));
            builder.AppendLine("");

            return builder.ToString();
        }

        public static string ToDisplayString(this SecureElementRetrievedEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Incomming ---> SecureElementRetrievedEventArgs ");
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine(string.Format("SessionId:         {0}", args.SessionId));

            if (args.SecureElement == null) return builder.ToString();

            SecureElement secureElement = args.SecureElement;
            string label = string.Empty;
            if (args.SecureElement.MetaData.ContainsKey(SecureElementMetaDataKeys.Label))
                label = args.SecureElement.MetaData[SecureElementMetaDataKeys.Label];
            builder.AppendLine(string.Format("-sub-  SecureElement ({0})", label));
            builder.AppendLine(string.Format("       SecureElement.AuthenticationCertificate:         {0}", secureElement.AuthenticationCertificate.RawValue.Replace("\n", "").Replace("\r", "")));
            builder.AppendLine(string.Format("       Has warnings:                                    {0}", secureElement.AuthenticationCertificate.HasWarnings));
            builder.AppendLine(GetCertificateWarningDisplayString(secureElement.AuthenticationCertificate.Warnings));
            builder.Append(GetCertificateFieldsDisplayString(secureElement.AuthenticationCertificate));
            builder.AppendLine("");
            return builder.ToString();
        }

        public static string ToDisplayString(this SecureElementInsertedEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Incomming ---> SecureElementInsertedEventArgs ");
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine("");
            return builder.ToString();
        }

        public static string ToDisplayString(this SecureElementReadyEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Incomming ---> SecureElementReadyEventArgs ");
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine("");
            return builder.ToString();
        }

        public static string ToDisplayString(this SecureElementRemovedEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Incomming ---> SecureElementRemovedEventArgs ");
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine("");
            return builder.ToString();
        }

        public static string ToDisplayString(this SessionOpenedEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Incomming ---> SessionOpenedEventArgs ");
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine(string.Format("SessionId:         {0}", args.SessionId));
            if (args.SecureElement == null) return builder.ToString();

            SecureElement secureElement = args.SecureElement;
            string label = string.Empty;
            if (args.SecureElement.MetaData.ContainsKey(SecureElementMetaDataKeys.Label))
                label = args.SecureElement.MetaData[SecureElementMetaDataKeys.Label];
            //builder.AppendLine(string.Format("-sub-  SecureElement ({0})", label));
            //builder.AppendLine(string.Format("       SecureElement.AuthenticationCertificate:         {0}", secureElement.AuthenticationCertificate.RawValue.Replace("\n", "").Replace("\r", "")));
            //builder.AppendLine(string.Format("       Has warnings:                                    {0}", secureElement.AuthenticationCertificate.HasWarnings));
            //builder.AppendLine(GetCertificateWarningDisplayString(secureElement.AuthenticationCertificate.Warnings));
            builder.Append(GetCertificateFieldsDisplayString(secureElement.Certificates[0]));
            builder.AppendLine("");

            if (secureElement.Attributes?.Count > 0)
            {
                builder.AppendLine(string.Format("Attribute count:         {0}", secureElement.Attributes.Count));
                foreach (var attribute in secureElement.Attributes)
                {
                    builder.AppendLine(string.Format($"-sub-  Attribute ({attribute.URA} {attribute.Type} )"));
                    builder.AppendLine(string.Format($"- Values:"));
                    foreach (var val in attribute.Values)
                    builder.AppendLine(string.Format($"-    Value: {val.Key}:{val.Value}"));
                }
            }
            return builder.ToString();
        }

        public static string ToDisplayString(this SessionOpenCancelledEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Incomming ---> SessionOpenCancelledEventArgs ");
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine(string.Format("SessionId:         {0}", args.SessionId));
            builder.AppendLine(string.Format("Reason:                {0}", args.Reason));
            builder.AppendLine("");
            return builder.ToString();
        }

        public static string ToDisplayString(this SessionClosedEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Incomming ---> SessionClosedEventArgs ");
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine(string.Format("SessionId:             {0}", args.SessionId));
            builder.AppendLine(string.Format("Reason:                {0}", args.Reason));
            builder.AppendLine("");
            return builder.ToString();
        }

        public static string ToDisplayString(this SessionInitializedEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Incomming ---> SessionInitializedEventArgs ");
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine(string.Format("SessionId:             {0}", args.SessionId));
            builder.AppendLine(string.Format("Nonce:                 {0}", args.Nonce.Value));
            builder.AppendLine("");

            return builder.ToString();
        }
        
        public static string ToDisplayString(this TransactionFinishedEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Incomming ---> TransactionFinishedEventArgs ");
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine(string.Format("SessionId:             {0}", args.SessionId));
            builder.AppendLine(string.Format("TransactionId:         {0}", args.TransactionId));
            builder.AppendLine(string.Format("       Has warnings:                                    {0}", args.HasWarnings));
            builder.AppendLine(GetCertificateWarningDisplayString(args.CertificateWarnings));
            if (args.SignedDataBag == null) return builder.ToString();
            if (args.SignedDataBag.Values.FirstOrDefault() == null) return builder.ToString();

            int i = 1;
            foreach (SignedData signedData in args.SignedDataBag.Values)
            {
                builder.AppendLine(string.Format("-sub-  SignedData element: {0}",i));
                builder.AppendLine(string.Format("       SignedData.Id:                     {0}", signedData.Id));
                builder.AppendLine(string.Format("       SignedBagEntry.SignDataRequestId:  {0}", signedData.SignDataRequestId));
                builder.AppendLine(string.Format("       SignedBagEntry.ReferenceId:        {0}", signedData.ReferenceId));
                builder.AppendLine(string.Format("       SignedBagEntry.Data:               {0}", signedData.Data.Replace("\n", "").Replace("\r", "")));
                i++;
            }
            return builder.ToString();
        }
    
        public static string ToDisplayString(this TransactionCancelledEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Incomming ---> TransactionCancelledEventArgs ");
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine(string.Format("SessionId:             {0}", args.SessionId));
            builder.AppendLine(string.Format("TransactionId:         {0}", args.TransactionId));
            builder.AppendLine(string.Format("Reason:                {0}", args.Reason));
            builder.AppendLine("");
            return builder.ToString();
        }

        public static string ToDisplayString(this ValidationFinishedEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Incomming ---> ValidationFinishedEventArgs ");
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine(string.Format("InvocationId:         {0}", args.InvocationId));
            builder.AppendLine(string.Format("       Has warnings:                                    {0}", args.HasWarnings));
            builder.AppendLine(GetCertificateWarningDisplayString(args.CertificateWarnings));
            return builder.ToString();
        }

        public static string ToDisplayString(this ValidationCancelledEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Incomming ---> ValidationCancelledEventArgs ");
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine(string.Format("InvocationId:         {0}", args.InvocationId));
            builder.AppendLine(string.Format("Reason:                {0}", args.Reason));
            builder.AppendLine("");
            return builder.ToString();
        }

        public static string ToDisplayString(this DiagnoseInformationRetrievedEventArgs args)
        {
            var builder = new StringBuilder();
            builder.Append("Incomming ---> DiagnoseInformationRetrievedEventArgs ");
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine(args.DiagnoseInformation.ToJson()); JsonExtension.ToJson(args.DiagnoseInformation);
            builder.AppendLine("");
            return builder.ToString();
        }
        

        private static string GetCertificateWarningDisplayString(List<CertificateWarning> certificateWarnings)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine();
            foreach (CertificateWarning warning in certificateWarnings)
                builder.AppendLine(warning.ToString());

            return builder.ToString();
        }

        private static string GetCertificateFieldsDisplayString(Certificate certificate)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine();
            //foreach (string key in certificate.ParsedFields.Keys)
            //    builder.AppendLine(string.Format("{0}: {1}",key, certificate.ParsedFields[key]));

            return builder.ToString();
        }
    }
}
