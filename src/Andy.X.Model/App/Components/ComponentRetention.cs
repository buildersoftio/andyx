namespace Buildersoft.Andy.X.Model.App.Components
{
    public class ComponentRetention
    {
        public string Name { get; set; }
        public long RetentionTimeInMinutes { get; set; }

        public ComponentRetention()
        {
            Name = "default";
            RetentionTimeInMinutes = -1;
        }
    }
}
