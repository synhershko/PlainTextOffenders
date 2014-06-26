using System;
using System.Linq;
using PlainTextOffenders.Models;
using Raven.Client.Indexes;

namespace PlainTextOffenders
{
    public class OffendersIndex : AbstractIndexCreationTask<Offender>
    {
        public OffendersIndex()
        {
            Map = docs => from doc in docs
                select new
                {
                    doc.CurrentStatus,
                    Keywords = doc.Keywords.Split(new[]{','}, StringSplitOptions.RemoveEmptyEntries),
                    doc.OffenceType,
                    doc.StatusDateTime,
                };

            DisableInMemoryIndexing = true;
        }
    }
}
