using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatherUp.core.interfaces;

namespace GatherUp.core.DO
{
    public class Event:IEntity
    {
        public int Id { get; set; } 
        public string Title { get; set; } = string.Empty; 
        public DateTime Date { get; set; } 
        public string Location { get; set; } = string.Empty;
        public List<int> ParticipantIds { get; set; } = new List<int>();
        public int EventManagerId { get; set; } 
        public int EventHostId { get; set; } 
        public List<VendorAllocation> Vendors { get; set; } = new List<VendorAllocation>(); 
        public List<int> PollIds { get; set; } = new List<int>(); 
    }
}
