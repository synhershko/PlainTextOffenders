using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using PlainTextOffenders.Models;
using Raven.Client;

namespace PlainTextOffenders
{
    public class HomeModule : NancyModule
    {
        public HomeModule(IDocumentSession session)
        {
            Get["/"] = _ =>
            {
                return "Homepage";
            };

            Get["/lookup/{q*}"] = _ =>
            {
                string q = _.q;
                if (string.IsNullOrWhiteSpace(q))
                    return "No results found for an empty query";

                // Try loading by URL
                try
                {
                    var potentialUrl = q;
                    if (!potentialUrl.StartsWith("http"))
                        potentialUrl = "http://" + potentialUrl;
                    var uri = new Uri(potentialUrl, UriKind.Absolute);
                    potentialUrl = uri.Host;

                    var offender = session.Load<Offender>(potentialUrl);
                    if (offender != null)
                        return Response.AsJson(offender);
                }
                catch (Exception)
                {
                    // ignored, simply means we are not looking for a site by URL
                }               

                // Lookup by keywords
                var results = session.Advanced.LuceneQuery<Offender, OffendersIndex>()
                    .WhereEquals(x => x.Keywords, q, false)
                    .Take(5)
                    .ToList();

                if (results.Any())
                    return Response.AsJson(results);

                return 404;
            };
        }
    }
}
