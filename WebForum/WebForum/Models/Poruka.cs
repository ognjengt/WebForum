using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebForum.Models
{
    public class Poruka
    {
        public string Posiljalac { get; set; }
        public string Primalac { get; set; }
        public string Sadrzaj { get; set; }
        public bool Procitana { get; set; }

    }
}