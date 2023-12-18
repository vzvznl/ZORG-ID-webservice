using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Nl.Aet.Cid.Client.Sdk.Sample.Terminal
{
    public static class CertificateExtensions
    {
        public static string SerialNumberAsDecimal(this X509Certificate2 value)
        {
            BigInteger serialNumberAsDecimal = 0;
            BigInteger.TryParse(value?.SerialNumber, NumberStyles.HexNumber, null, out serialNumberAsDecimal);

            return serialNumberAsDecimal.ToString();
        }
    }
}
