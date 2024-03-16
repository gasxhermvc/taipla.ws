using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Helpers.DbUtilities
{
    public class TransactionResult
    {
        public bool success { get; set; } = false;

        public Exception exception { get; set; } = null;
    }
}
