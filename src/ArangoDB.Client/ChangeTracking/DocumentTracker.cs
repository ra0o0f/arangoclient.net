using Newtonsoft.Json.Linq;
using ArangoDB.Client.Serialization;
using System;
using System.Runtime.CompilerServices;

namespace ArangoDB.Client.ChangeTracking
{
    public class DocumentTracker
    {
        private readonly ConditionalWeakTable<object, DocumentContainer> _containerByInstance;
        private readonly ConditionalWeakTable<string, DocumentContainer> _containerById;
        private readonly IArangoDatabase _db;

        public DocumentTracker(IArangoDatabase db)
        {
            _containerById = new ConditionalWeakTable<string, DocumentContainer>();
            _containerByInstance = new ConditionalWeakTable<object, DocumentContainer>();
            _db = db;
        }

        public void StopTrackChanges(object document)
        {
            var container = FindDocumentInfo(document);
            _containerByInstance.Remove(document);
            _containerById.Remove(container.Id);
        }

        public DocumentContainer TrackChanges(object document, JObject jObject)
        {
            var container = CreateContainer(jObject);

            if (container != null)
            {
                _containerById.Add(container.Id, container);
                _containerByInstance.Add(document, container);
            }

            return container;
        }

        public DocumentContainer TrackChanges(object document, IDocumentIdentifierResult identifiers)
        {
            var jObject = JObject.FromObject(document, new DocumentSerializer(_db).CreateJsonSerializer());

            var container = CreateContainer(jObject, identifiers);

            if (container != null)
            {
                _containerById.Add(container.Id, container);
                _containerByInstance.Add(document, container);
            }

            return container;
        }

        public DocumentContainer FindDocumentInfo(string id)
        {
            if (!_containerById.TryGetValue(id, out var container))
            {
                throw new Exception(
                    $"No tracked document found for {id}, change tracking is maybe disabled");
            }

            return container;
        }

        public DocumentContainer FindDocumentInfo(object document)
        {
            if (!_containerByInstance.TryGetValue(document, out var container))
            {
                throw new Exception("No tracked document found");
            }

            return container;
        }

        public JObject GetChanges(object document)
        {
            return GetChanges(document, out _, out _);
        }

        public JObject GetChanges(object document, out DocumentContainer container, out JObject jObject)
        {
            jObject = JObject.FromObject(document, new DocumentSerializer(_db).CreateJsonSerializer());

            JObject changedObject = new JObject();
            if (_containerByInstance.TryGetValue(document, out container))
            {
                CreateChangedDocument(container.Document, jObject, ref changedObject);
            }

            return changedObject;
        }

        DocumentContainer CreateContainer(JObject jObject)
        {
            DocumentContainer container = new DocumentContainer {Id = jObject.Value<string>("_id")};

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

        public void CreateChangedDocument(JObject oldObject, JObject newObject, ref JObject changedObject,
            bool handleInnerObjects = false)
        {
            if (oldObject == null || oldObject.Type == JTokenType.Null)
            {
                changedObject = newObject;
                return;
            }

            foreach (var n in newObject)
            {
                if (!handleInnerObjects && (n.Key == "_id" || n.Key == "_key" || n.Key == "_rev" || n.Key == "_from" ||
                                            n.Key == "_to"))
                    continue;

                JToken newValue = n.Value;
                JToken oldValue = oldObject[n.Key];

                if (newValue.Type == JTokenType.Object)
                {
                    JObject subChangeObject = new JObject();

                    CreateChangedDocument(oldValue as JObject, newValue as JObject, ref subChangeObject,
                        true);
                    if (subChangeObject.Count != 0)
                        changedObject.Add(n.Key, subChangeObject);
                }
                else if (!JToken.DeepEquals(oldValue, newValue))
                {
                    changedObject.Add(n.Key, newValue);
                }
            }
        }
    }
}