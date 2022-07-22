using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SendMailAPI.Models
{
    public class Message
    {
        public string CCO { get; set; } = string.Empty;
        public string TO { get; set; } = string.Empty;        
        public string senderEmail { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string subject { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public string adjuntoURL { get; set; } = string.Empty;
        public string hostEmail { get; set; } = string.Empty;
        public short port { get; set; } 
        public short tls { get; set; }
        public bool isBodyHTML { get; set; }
    }
    public class MessageCode
    {
        public short code { get; set; }
        public string description { get; set; } = string.Empty;
    }
}