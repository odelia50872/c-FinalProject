using System.Xml.Serialization;

namespace GatherUp.core.DO
{
    public class EventParticipantData
    {
        [XmlAttribute]
        public int ParticipantId { get; set; }
        public bool? IsAttending { get; set; }
        public bool HasPaid { get; set; }
        public decimal AmountContributed { get; set; }
    }
}
