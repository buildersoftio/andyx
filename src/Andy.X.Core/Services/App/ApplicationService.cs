using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Core.Services.App
{
    public class ApplicationService
    {
        public ApplicationService(ILogger<ApplicationService> logger)
        {
            logger.LogInformation("Buildersoft");
            logger.LogInformation("Buildersoft Andy X");
            logger.LogInformation("ANDYX#READY");
        }
    }
}
