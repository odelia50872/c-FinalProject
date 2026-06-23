using GatherUp.core.interfaces;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace GatherUp.core.DO
{
    public class Event : IEntity
    {
        public Event()
        {
            ParticipantIds = new List<int>();
            ParticipantData = new List<EventParticipantData>();
            Vendors = new List<VendorAllocation>();
            PollIds = new List<int>();
        }

        private int _id;
        [XmlAttribute]
        public int Id { get => _id; set { if (_id == 0) _id = value; } }

        [Required(ErrorMessage = "Event title is required")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event date is required")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Event location is required")]
        public string Location { get; set; } = string.Empty;

        public List<int> ParticipantIds { get; set; }
        public List<EventParticipantData> ParticipantData { get; set; }
        public int EventManagerId { get; set; }
        public int EventHostId { get; set; }
        public List<VendorAllocation> Vendors { get; set; }
        public List<int> PollIds { get; set; }
    }
}
