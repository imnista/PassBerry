namespace PassBerry.Handler
{
    using PassBerry.Library;
    using PassBerry.Model;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Resources;

    internal class ResourceProcessor
    {
        private static readonly string ResXFilePath = "PassBerry.data";
        private static ResourceProcessor singleton = new ResourceProcessor();

        private ResourceProcessor()
        {
            if (!File.Exists(ResXFilePath))
            {
                using (var writer = new ResXResourceWriter(ResXFilePath))
                {
                    writer.Generate();
                }
            }
        }

        public static ResourceProcessor GetInstance()
        {
            return singleton;
        }

        public List<RecordForDisplay> GetAll()
        {
            var reader = new ResXResourceReader(ResXFilePath);
            var dict = reader.GetEnumerator();
            var result = new List<RecordForDisplay>();
            while (dict.MoveNext())
            {
                string data = dict.Value.ToString();
                if (!string.IsNullOrWhiteSpace(data))
                {
                    var record = DecryptData(data);
                    result.Add(new RecordForDisplay(record));
                }
            }
            return result;
        }

        public RecordForDisplay Save(RecordForDisplay recordForDisplay)
        {
            // Double ID Checker
            //if (recordForDisplay.Id == Guid.Empty) { recordForDisplay.Id = Guid.NewGuid(); }
            var record = recordForDisplay.ToDataModel();
            var data = this.EncryptData(record);
            this.AddOrUpdateResource(record.Id, data);
            return recordForDisplay;
        }

        public void Delete(Guid id)
        {
            this.AddOrUpdateResource(id, null);
        }

        private string EncryptData(Record record)
        {
            // TODO...
            return JsonHelper.Serialize(record);
        }

        private Record DecryptData(string data)
        {
            // TODO...
            return JsonHelper.Deserialize<Record>(data);
        }

        private void AddOrUpdateResource(Guid key, string value)
        {
            this.AddOrUpdateResource(key.ToString(), value);
        }

        private void AddOrUpdateResource(string key, string value)
        {
            var resx = new List<DictionaryEntry>();
            using (var reader = new ResXResourceReader(ResXFilePath))
            {
                resx = reader.Cast<DictionaryEntry>().OrderBy(i => i.Key).ToList();
                var existingResource = resx.Where(r => r.Key.ToString() == key).FirstOrDefault();
                if (existingResource.Key == null && value != null) // NEW!
                {
                    resx.Add(new DictionaryEntry() { Key = key, Value = value });
                }
                else // MODIFIED RESOURCE!
                {
                    if (value == null)
                    {
                        resx.Remove(existingResource);
                    }
                    else
                    {
                        var modifiedResx = new DictionaryEntry { Key = existingResource.Key, Value = value };
                        resx.Remove(existingResource);  // REMOVING RESOURCE!
                        resx.Add(modifiedResx);  // AND THEN ADDING RESOURCE!
                    }
                }
            }
            using (var writer = new ResXResourceWriter(ResXFilePath))
            {
                resx.ForEach(r =>
                {
                    // Again Adding all resource to generate with final items
                    writer.AddResource(r.Key.ToString(), r.Value.ToString());
                });
                writer.Generate();
            }
        }
    }
}