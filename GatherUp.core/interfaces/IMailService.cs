using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.core.interfaces
{
    public interface IMailService
    {
        void SendEmail(string to, string subject, string body);
    }
}
