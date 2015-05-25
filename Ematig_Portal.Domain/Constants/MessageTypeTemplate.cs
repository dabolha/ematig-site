using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ematig_Portal.Domain.Enum
{
    public static class MessageTypeTemplate
    {
        public const string ContactRequest = "Nome: {0} {1} <br/>Email: {2} <br/>Telefone: {3} <br/>Assunto: {4} <br/><br/> {5}";
        public const string Custom = "{0}";
    }
}