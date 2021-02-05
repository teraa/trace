using System.Collections.Generic;

namespace TwitchLogger.Data.Models
{
    public class MessageSource : EntityBase<short>
    {
        public MessageSource()
        {
            Messages = new HashSet<Message>();
        }

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public virtual ICollection<Message> Messages { get; set; }
    }
}
