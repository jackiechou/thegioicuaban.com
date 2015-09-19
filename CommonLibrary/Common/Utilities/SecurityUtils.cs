using System.Runtime.InteropServices;
using System.Security.Principal;
using System;


namespace CommonLibrary.Common.Utilities
{
    /// <summary>
    /// A set of utilities functions related to security.
    /// </summary>
    public static class SecurityUtils
    {
        const int LOGON32_LOGON_INTERACTIVE = 2;
        const int LOGON32_LOGON_NETWORK = 3;
        const int LOGON32_LOGON_BATCH = 4;
        const int LOGON32_LOGON_SERVICE = 5;
        const int LOGON32_LOGON_UNLOCK = 7;
        const int LOGON32_LOGON_NETWORK_CLEARTEXT = 8;
        const int LOGON32_LOGON_NEW_CREDENTIALS = 9;
        const int LOGON32_PROVIDER_DEFAULT = 0;

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern int LogonUser(
            string lpszUsername,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            out IntPtr phToken
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int CloseHandle(IntPtr hObject);

        /// <summary>
        /// Logs on a user and changes the current process impersonation to that user.
        /// 
        /// IMPORTANT: Returns a WindowsImpersonationContext and you have to either
        /// dispose this instance or call RevertImpersonation with it.
        /// </summary>
        /// <remarks>
        /// Requires Full Trust permissions in ASP.NET Web Applications.
        /// </remarks>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        /// <returns>WindowsImpersonation Context. Call RevertImpersonation() to clear the impersonation or Dispose() instance.</returns>
        public static WindowsImpersonationContext ImpersonateUser(string username, string password, string domain)
        {
            IntPtr token = IntPtr.Zero;
            try
            {
                int TResult = LogonUser(username, domain, password,
                                        LOGON32_LOGON_NETWORK, LOGON32_PROVIDER_DEFAULT,
                                        out token);

                WindowsImpersonationContext context = null;
                context = WindowsIdentity.Impersonate(token);
                CloseHandle(token);

                return context;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (token != IntPtr.Zero)
                    CloseHandle(token);
            }
        }

        /// <summary>
        /// Releases an impersonation context and releases associated resources
        /// </summary>
        /// <param name="context">WindowsImpersonation context created with ImpersonateUser</param>
        public static void RevertImpersonation(WindowsImpersonationContext context)
        {
            context.Undo();
            context.Dispose();
        }
    }
}
