using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;

namespace Nl.Aet.Cid.Client.Sdk.Sample.Terminal
{
    public static class SecureStringExtensions
    {

        /// <summary>
        /// Converts to unsecure string.
        /// </summary>
        /// <param name="secureString">The secure password.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">secureString</exception>
        public static string ConvertToUnsecureString(this SecureString secureString)
        {
            if (secureString == null)
                throw new ArgumentNullException("secureString");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}
