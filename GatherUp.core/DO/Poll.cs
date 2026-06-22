using GatherUp.core.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace GatherUp.core.DO
{
    public  class Poll:IEntity
    {
        public Poll()
        {
            Questions= new List<PollQuestion>();
        }

        [XmlAttribute]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ClosingDate { get; set; }
        public List<PollQuestion> Questions { get; set; } 
    }
}
