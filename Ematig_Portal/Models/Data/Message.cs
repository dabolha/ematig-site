using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ematig_Portal.Models
{
    public class Message
    {
        public long Id { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? SentDate { get; set; }
    }
}