using GatherUp.core.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GatherUp.core.DO
{
    public class Event:IEntity
    {
        public Event()
        {
            ParticipantIds = new List<int>();
            Vendors = new List<VendorAllocation>();
            PollIds = new List<int>();
        }

        [XmlAttribute]
        public int Id { get; set; } 
        public string Title { get; set; } = string.Empty; 
        public DateTime Date { get; set; } 
        public string Location { get; set; } = string.Empty;
        public List<int> ParticipantIds { get; set; }
        public int EventManagerId { get; set; } 
        public int EventHostId { get; set; } 
        public List<VendorAllocation> Vendors { get; set; } 
        public List<int> PollIds { get; set; }  
    }
}
