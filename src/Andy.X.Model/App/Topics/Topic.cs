using System;

namespace Buildersoft.Andy.X.Model.App.Topics
{
    public class Topic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public TopicSettings TopicSettings { get; set; }

        public Schema Schema { get; set; }
        public Topic()
        {
            Schema = new Schema();
            TopicSettings = new TopicSettings();
        }
    }

    public class Schema
    {
        public string Name { get; set; }
        public bool IsSchemaValidated { get; set; }
        public string SchemaRaw { get; set; }
        public int Version { get; set; }

        public Schema()
        {
            Version = 0;
            IsSchemaValidated = false;
        }
    }
}
