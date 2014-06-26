using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Raven.Client;
using Raven.Client.Document;

namespace PlainTextOffenders
{
    public class Bootstraper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            
            var docStore = new DocumentStore { ConnectionStringName = "RavenDB" };
   
            docStore.Initialize();
            container.Register<IDocumentStore>(docStore, "DocStore");
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            var docStore = container.Resolve<IDocumentStore>("DocStore");
            Raven.Client.Indexes.IndexCreation.CreateIndexes(typeof(Bootstraper).Assembly, docStore);
        }

        protected override void ConfigureRequestContainer(global::Nancy.TinyIoc.TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            var docStore = container.Resolve<IDocumentStore>("DocStore");
            var session = docStore.OpenSession();
            container.Register<IDocumentSession>(session);
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);
            
            pipelines.AfterRequest.AddItemToEndOfPipeline(nancyContext =>
            {
                // session disposal
                using (var session = container.Resolve<IDocumentSession>()) {}
            });
        }
    }
}
