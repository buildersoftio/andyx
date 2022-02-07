using Buildersoft.Andy.X.Core.Abstractions.Repositories.Memory;
using Buildersoft.Andy.X.Core.Abstractions.Services.Api;
using Buildersoft.Andy.X.Model.App.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Core.Services.Api
{
    public class ComponentService : IComponentService
    {
        private readonly ILogger<ComponentService> _logger;
        private readonly ITenantRepository _tenantRepository;

        public ComponentService(ILogger<ComponentService> logger, ITenantRepository tenantRepository)
        {
            _logger = logger;
            _tenantRepository = tenantRepository;
        }
        public Component GetComponent(string tenantName, string productName, string componentName)
        {
            try
            {
                return _tenantRepository.GetComponent(tenantName, productName, componentName);
            }
            catch (Exception)
            {
                // TODO Log later
                return null;
            }
        }

        public List<Component> GetComponents(string tenantName, string productName)
        {
            var result = new List<Component>();
            try
            {
                var components = _tenantRepository.GetComponents(tenantName, productName);
                foreach (var component in components)
                {
                    result.Add(new Component() { Id = component.Value.Id, Name = component.Value.Name, Settings = component.Value.Settings });
                }
            }
            catch (Exception)
            {
                // TODO Log later
            }
            return result;
        }
    }
}
