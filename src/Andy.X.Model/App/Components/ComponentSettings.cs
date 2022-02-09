using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.App.Components
{
    public class ComponentSettings
    {
        public bool AllowSchemaValidation { get; set; }
        public bool AllowTopicCreation { get; set; }

        public bool EnableAuthorization { get; set; }
        public List<ComponentToken> Tokens { get; set; }

        public ComponentRetention RetentionPolicy { get; set; }


        public ComponentSettings()
        {
            AllowSchemaValidation = false;
            AllowTopicCreation = true;
            EnableAuthorization = false;

            Tokens = new List<ComponentToken>();
            RetentionPolicy = new ComponentRetention();
        }
    }
}
