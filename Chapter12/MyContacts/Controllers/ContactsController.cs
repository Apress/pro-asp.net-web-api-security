using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Permissions;
using System.Web.Http;
using MyContacts.Models;

namespace MyContacts.Controllers
{
    public class ContactsController : ApiController
    {
        [ClaimsPrincipalPermission(SecurityAction.Demand, Operation = "Get", Resource = "Contacts")]
        public IEnumerable<MailingContact> Get()
        {
            return Contact.GenerateContacts()
                            .Where(c => c.Owner == User.Identity.Name)
                            .Select(c => new MailingContact()
                            {
                                Name = c.Name,
                                Email = c.Email
                            }).ToList();
        }
    }

    public class MailingContact
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}