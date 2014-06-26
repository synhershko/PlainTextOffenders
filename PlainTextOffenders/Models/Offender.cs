using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlainTextOffenders.Models
{
    public class Offender
    {
        public enum OffenseType
        {
            NewPassword,
            OwnPassword,
        }

        public enum Status
        {
            Published,
            AwaitingModeration,
            Hidden,
        }

        public string Id { get; set; }
        public string Keywords { get; set; }        
        public OffenseType OffenceType { get; set;  }
        public Status CurrentStatus { get; set; }
        public DateTimeOffset StatusDateTime { get; set; }
    }
}
