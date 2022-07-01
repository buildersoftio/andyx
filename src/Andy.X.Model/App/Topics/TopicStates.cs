namespace Buildersoft.Andy.X.Model.App.Topics
{
    public class TopicStates
    {
        public long LatestEntryId { get; set; }
        public long MarkDeleteEntryPosition { get; set; }

        public TopicStates()
        {
            LatestEntryId = 1;
            MarkDeleteEntryPosition = -1;
        }
    }
}
