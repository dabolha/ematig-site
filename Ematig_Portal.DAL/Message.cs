//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ematig_Portal.Domain
{
    using System;
    using System.Collections.Generic;
    
    public partial class Message : DomainObject
    {
        public long Id { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public short MessageTypeId { get; set; }
        public System.DateTime CreationDate { get; set; }
        public Nullable<System.DateTime> SentDate { get; set; }
    
        public virtual MessageType MessageType { get; set; }
    }
}
