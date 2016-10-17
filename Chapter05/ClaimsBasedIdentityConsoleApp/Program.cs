using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Linq;
using System.Security.Claims;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClaimsBasedIdentityConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "badri"),
                new Claim(ClaimTypes.Email, "badri@nowhere.com"),
                new Claim(ClaimTypes.Role, "Hunters"),
                new Claim(ClaimTypes.Role, "FireWeaponLicensees")
            };

            var id = new ClaimsIdentity(claims, "Dummy"); // Non-empty string is needed as authentication type
            var principal = new ClaimsPrincipal(new[] { id });
            Thread.CurrentPrincipal = principal;

            Shoot(); // Call the method that needs authorization
        }

        // RBAC
        [PrincipalPermission(SecurityAction.Demand, Role = "Hunters")]
        
        // CBAC
        [ClaimsPrincipalPermission(SecurityAction.Demand, Operation = "Shoot", Resource = "Gun")]
        private static void Shoot()
        {
            new PrincipalPermission(null, "FireWeaponLicensees").Demand();
            Console.WriteLine(Thread.CurrentPrincipal.IsInRole("Hunters"));
            Console.WriteLine("Bam Bam");
        }

    }
}
