using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.core.DO
{
    public record class ReceiptDetails(string ReceiptNumber, decimal Amount, DateTime Date);
    
}
