using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyContacts.Models
{
    public class TokenRequest
    {
        [Required]
        public string redirect_uri { get; set; }

        [Required]
        public string client_id { get; set; }

        [Required]
        public string code { get; set; }

        [Required]
        public string client_secret { get; set; }

        [Required]
        public string grant_type { get; set; }
    }

}