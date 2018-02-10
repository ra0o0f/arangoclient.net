using System.IO;
using Xunit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ArangoDB.Client.Serialization;

namespace ArangoDB.Client.Test.Serialization {
	public class PropertyNames {

		private IArangoDatabase _db;
		private ObjectToSerialize _toTest;
		private DocumentSerializer _docSerializer;
		private JsonSerializer _jsonSerializer;

		public PropertyNames(){
			_toTest = new ObjectToSerialize();
			_db = new ArangoDatabase();
			_docSerializer = new DocumentSerializer(_db);
			_jsonSerializer = _docSerializer.CreateJsonSerializer();
		}
		
		[Fact]
		public void UseUnderlyingNames()
		{

			_db.SharedSetting.Serialization.UseUnderlyingPropertyName = true;

			string testData = SerializeTestObject();
			JObject jo = JObject.Parse(testData);

			Assert.NotNull(jo["TestProp02"]);
		}

		[Fact]
		public void UseJsonPropertyNames()
		{
			_db.SharedSetting.Serialization.UseUnderlyingPropertyName = false;

			string testData = SerializeTestObject();
			JObject jo = JObject.Parse(testData);

			Assert.NotNull(jo["MyTestProp"]);
		}

		private string SerializeTestObject(){
			string retVal = null;

			using (MemoryStream ms = new MemoryStream(1024)) {
				using (StreamWriter sw = new StreamWriter(ms)) {
					using (JsonWriter jw = new JsonTextWriter(sw)) {
						_jsonSerializer.Serialize(jw, _toTest);
						sw.Flush();
						ms.Seek(0, SeekOrigin.Begin);

						using (StreamReader sr = new StreamReader(ms)) {
							retVal = sr.ReadToEnd();
						}
					}
				}
			}
			return retVal;
		}

		private class ObjectToSerialize {
			public bool TestProp01 { get; set; }

			[JsonProperty(PropertyName = "MyTestProp")]
			public bool TestProp02 { get; set; }
		}
	}
}
