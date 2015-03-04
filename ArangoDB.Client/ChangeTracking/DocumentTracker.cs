using ArangoDB.Client.Common.Newtonsoft.Json.Linq;
using ArangoDB.Client.Common.Utility;
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

        ArangoDatabase db;

        public DocumentTracker(ArangoDatabase db)
        {
            this.db = db;
        }

        public void TrackChanges(object document,JObject jObject)
        {
            var container = CreateContainer(jObject);

            if (container != null)
            {
                containerById[container.Id] = container;
                containerByInstance[document] = container;
            }
        }

        public DocumentContainer FindDocumentInfo(string id)
        {
            return containerById[id];
        }

        public DocumentContainer FindDocumentInfo(object document)
        {
            return containerByInstance[document];
        }

        public JObject GetChanges(object document)
        {
            DocumentContainer container = null;
            return GetChanges(document, out container);
        }

        public JObject GetChanges(object document,out DocumentContainer container)
        {
            container = containerByInstance[document];

            var jObject = JObject.FromObject(document,new DocumentSerializer(db).CreateJsonSerializer());

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

        public void CreateChangedDocument(JObject oldObject, JObject newObject, ref JObject changedObject, bool handleInnerObjects = false)
        {
            foreach (var n in newObject)
            {
                if (!handleInnerObjects && (n.Key == "_id" || n.Key == "_key" || n.Key == "_rev" || n.Key == "_from" || n.Key == "_to"))
                    continue;

                JToken newValue = n.Value;
                JToken oldValue = oldObject[n.Key];

                if (newValue.Type == JTokenType.Object)
                {
                    JObject subChangeObject = new JObject();

                    if (oldValue.Type == JTokenType.Null)
                        oldValue = new JObject();

                    CreateChangedDocument(oldValue as JObject, newValue as JObject, ref subChangeObject, handleInnerObjects: true);
                    if (subChangeObject.Count != 0)
                        changedObject.Add(n.Key, subChangeObject);
                }
                else if (newValue == null || !JObject.DeepEquals(oldValue, newValue))
                {
                    changedObject.Add(n.Key, newValue);
                }
            }
        }
    }
}
