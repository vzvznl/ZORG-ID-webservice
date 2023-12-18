using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;



namespace Nl.Aet.Cid.Client.Sdk.Sample.Terminal
{
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public static class CommandDisplayFormatter
    {
        const string DateTimeFormat = "o"; //ISO 8601

        public static string DisplayString(string commandName)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("Outgoing ---> {0}",commandName));
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));

            builder.AppendLine("");

            return builder.ToString();
        }

        public static string DisplayString(string commandName, string entityName,Guid entityId)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("Outgoing ---> {0} ",commandName));
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine(string.Format("{0}          {1}",entityName,entityId));

            builder.AppendLine("");

            return builder.ToString();
        }

        public static string DisplayString(string commandName, string entityName, Guid entityId, string pin)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("Outgoing ---> {0} ", commandName));
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine(string.Format("{0}           {1}",entityName,entityId));
            builder.AppendLine(string.Format("Pin:                   {0}",pin));

            builder.AppendLine("");

            return builder.ToString();
        }

        public static string DisplayString(string commandName, string entityName,Guid sessionId,Guid entityId)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("Outgoing ---> {0} ", commandName));
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine(string.Format("{0}           {1}", entityName, entityId));
            builder.AppendLine(string.Format("SessionId:             {0}",sessionId));

            builder.AppendLine("");

            return builder.ToString();
        }

        public static string DisplayString(string commandName, string entityName,Guid sessionId, Guid entityId, SignDataRequestBag signDataRequestBag)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("Outgoing ---> {0} ", commandName));
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine(string.Format("{0}           {1}", entityName, entityId));
            builder.AppendLine(string.Format("SessionId:             {0}", sessionId));
            builder.AppendLine("-sub-  signDataRequests");
            int i=1;
            foreach (SignDataRequest signDataRequest in signDataRequestBag.Content.Values)
            {
                builder.AppendLine(string.Format("-      SignDataRequest {0}",i));
                builder.AppendLine(string.Format("-      Id {0}", signDataRequest.Id));
                builder.AppendLine(string.Format("       ReferenceId:  {0}", signDataRequest.ReferenceId));
                builder.AppendLine(string.Format("       Data:  {0}", signDataRequest.Data));
                i++;
            }                           
            builder.AppendLine("");
            return builder.ToString();
        }

        public static string DisplayString(string commandName, string version)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("Outgoing ---> {0} ", commandName));
            builder.AppendLine(DateTime.UtcNow.ToString(DateTimeFormat));
            builder.AppendLine(string.Format("Version:          {0}",version));

            builder.AppendLine("");

            return builder.ToString();
        }
        
    }
}
