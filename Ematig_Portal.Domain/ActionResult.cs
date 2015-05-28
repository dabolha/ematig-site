using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ematig_Portal.Domain
{
    public class ActionResult
    {
        public bool Success { get; set; }
        public System.Enum OperationStatus { get; set; }
    }
}
