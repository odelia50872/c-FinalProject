using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.core.DO;
public enum MailingPreference
{
    None,
    Email,
    Sms,
    Both
}

public class Participant : Person
{
    public Participant() : base() { }
    public bool? IsAttending { get; set; }
    public bool HasPaid { get; set; }
    public decimal AmountContributed { get; set; }
    public MailingPreference MailingPreferences { get; set; } 

}
