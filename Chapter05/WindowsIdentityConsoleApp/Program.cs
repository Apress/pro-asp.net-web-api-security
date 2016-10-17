using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace WindowsIdentityConsoleApp
{
    class Program
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
            int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);


        static void Main(string[] args)
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            Console.WriteLine(id.Name);
            Console.WriteLine(id.User);

            foreach (var group in id.Groups)
                Console.WriteLine(group.Value);

            foreach (var group in id.Groups.Translate(typeof(NTAccount)))
                Console.WriteLine(group);

            WindowsPrincipal principal = new WindowsPrincipal(id);
            Console.WriteLine(principal.IsInRole("Builtin\\Users"));

            // Create WindowsIdentity using token
            string userName = "102628";
            string password = "Wolfpack@4";
            string domain = "CTS";

            IntPtr token = IntPtr.Zero;

            try
            {
                if (LogonUser(userName, domain, password, 3, 0, ref token))
                {
                    using (var idBasedonToken = new WindowsIdentity(token))
                    {
                        // We now have the WindowsIdentity for username here!
                    }
                }
            }
            finally
            {
                if (token != IntPtr.Zero)
                    CloseHandle(token);
            }

            // Create WindowsIdentity using UPN
            var idUpn = new WindowsIdentity("Myself@MyDomain.com");
            var principalUpn = new WindowsPrincipal(idUpn);
            bool isInRole = principalUpn.IsInRole("MyDomain\\SomeGroup");

            // Impersonation
            Console.WriteLine("Before: " + WindowsIdentity.GetCurrent().Name);

            using (WindowsIdentity identity = new WindowsIdentity(token)) // LogonUser() token
            {
                using (WindowsImpersonationContext impersonatedUser = identity.Impersonate())
                {
                    // WindowsIdentity.GetCurrent().Name will be that of impersonated identity
                    Console.WriteLine("After: " + WindowsIdentity.GetCurrent().Name);

                    impersonatedUser.Undo(); // Undo the impersonation, once done
                }
            }
        }
    }
}
