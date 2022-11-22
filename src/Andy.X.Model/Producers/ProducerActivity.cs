namespace Buildersoft.Andy.X.Model.Producers
{
    public class ProducerActivity
    {
        public string Key { get; set; }
        public string Tenant { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
        public int InstancesCount { get; set; }
    }
}
