using Newtonsoft.Json.Linq;
using ArangoDB.Client.Data;
using ArangoDB.Client.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.ChangeTracking
{
    public class DocumentTracker
    {
        private readonly Dictionary<object, DocumentContainer> containerByInstance = new Dictionary<object, DocumentContainer>();
        private readonly Dictionary<string, DocumentContainer> containerById = new Dictionary<string, DocumentContainer>();

        IArangoDatabase db;

        public DocumentTracker(IArangoDatabase db)
        {
            this.db = db;
        }

        public void StopTrackChanges(object document)
        {
            var container = FindDocumentInfo(document);
            containerByInstance.Remove(document);
            containerById.Remove(container.Id);
        }

        public DocumentContainer TrackChanges(object document,JObject jObject)
        {
            var container = CreateContainer(jObject);

            if (container != null)
            {
                containerById[container.Id] = container;
                containerByInstance[document] = container;
            }

            return container;
        }

        public DocumentContainer TrackChanges(object document, IDocumentIdentifierResult identifiers)
        {
            var jObject = JObject.FromObject(document, new DocumentSerializer(db).CreateJsonSerializer());

            var container = CreateContainer(jObject, identifiers);

            if (container != null)
            {
                containerById[container.Id] = container;
                containerByInstance[document] = container;
            }

            return container;
        }

        public DocumentContainer FindDocumentInfo(string id)
        {
            try
            {
                return containerById[id];
            }
            catch (KeyNotFoundException e)
            {
                throw new Exception(string.Format("No tracked document found for {0}, change tracking is maybe disabled", id), e);
            }
        }

        public DocumentContainer FindDocumentInfo(object document)
        {
            try
            {
                return containerByInstance[document];
            }
            catch(KeyNotFoundException e)
            {
                throw new Exception("No tracked document found", e);
            }
        }

        public JObject GetChanges(object document)
        {
            DocumentContainer container = null;
            JObject jObject = null;
            return GetChanges(document, out container, out jObject);
        }

        public JObject GetChanges(object document,out DocumentContainer container,out JObject jObject)
        {
            container = containerByInstance[document];

            jObject = JObject.FromObject(document,new DocumentSerializer(db).CreateJsonSerializer());

            JObject changedObject = new JObject();
            CreateChangedDocument(container.Document, jObject, ref changedObject);

            return changedObject;
        }

        DocumentContainer CreateContainer(JObject jObject)
        {
            DocumentContainer container = new DocumentContainer();

            container.Id = jObject.Value<string>("_id");
            if (container.Id == null)
                return null;

            container.Key = jObject.Value<string>("_key");
            if (container.Key == null)
                return null;

            container.Rev = jObject.Value<string>("_rev");
            if (container.Rev == null)
                return null;

            container.From = jObject.Value<string>("_from");
            container.To = jObject.Value<string>("_to");

            container.Document = jObject;

            return container;
        }

        DocumentContainer CreateContainer(JObject jObject, IDocumentIdentifierResult identifiers)
        {
            DocumentContainer container = new DocumentContainer();

            if (string.IsNullOrEmpty(identifiers.Id))
                return null;

            container.Id = identifiers.Id;
            container.Key = identifiers.Key;
            container.Rev = identifiers.Rev;

            container.Document = jObject;

            return container;
        }

        public void CreateChangedDocument(JObject oldObject, JObject newObject, ref JObject changedObject, bool handleInnerObjects = false)
        {
            if (oldObject == null || oldObject.Type == JTokenType.Null)
            {
                changedObject = newObject;
                return;
            }

            foreach (var n in newObject)
            {
                if (!handleInnerObjects && (n.Key == "_id" || n.Key == "_key" || n.Key == "_rev" || n.Key == "_from" || n.Key == "_to"))
                    continue;                    

                JToken newValue = n.Value;
                JToken oldValue = oldObject[n.Key];
                
                if (newValue.Type == JTokenType.Object)
                {
                    JObject subChangeObject = new JObject();

                    CreateChangedDocument(oldValue as JObject, newValue as JObject, ref subChangeObject, handleInnerObjects: true);
                    if (subChangeObject.Count != 0)
                        changedObject.Add(n.Key, subChangeObject);
                }
                else if (!JObject.DeepEquals(oldValue, newValue))
                {
                    changedObject.Add(n.Key, newValue);
                }
            }
        }
    }
}
