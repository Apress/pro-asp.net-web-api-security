using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyContacts.Models
{
    public class AuthzCodeRequest
    {
        [Required]
        public string response_type { get; set; }

        [Required]
        public string redirect_uri { get; set; }

        [Required]
        public string client_id { get; set; }

        [Required]
        public string scope { get; set; }
    }
}