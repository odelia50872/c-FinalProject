using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.core.DO
{
    public  class PollQuestion
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new List<string>();
        public Dictionary<int, string> ParticipantResponses { get; set; } = new Dictionary<int, string>();
    }
}
