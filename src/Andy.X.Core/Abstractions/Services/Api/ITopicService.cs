using Buildersoft.Andy.X.Model.App.Topics;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Abstractions.Services.Api
{
    public interface ITopicService
    {
        Topic GetTopic(string tenantName, string productName, string componentName, string topicName);
        List<Topic> GetTopics(string tenantName, string productName, string componentName);
        bool AddTopic(string tenantName, string productName, string componentName, string topicName);
    }
}
