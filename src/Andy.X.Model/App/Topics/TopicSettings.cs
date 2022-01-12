using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Model.App.Topics
{
    public class TopicSettings
    {
        public bool IsPersistent { get; set; }

        public TopicSettings()
        {
            IsPersistent = true;
        }
    }
}
