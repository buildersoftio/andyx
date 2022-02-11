﻿using Buildersoft.Andy.X.Model.App.Components;
using System.Collections.Generic;

namespace Buildersoft.Andy.X.Model.Storages.Requests.Components
{
    public class CreateComponentTokenDetails
    {
        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }

        public ComponentToken Token { get; set; }
        public List<string> StoragesAlreadySent { get; set; }

        public CreateComponentTokenDetails()
        {
            Token = new ComponentToken();
            StoragesAlreadySent = new List<string>();
        }
    }
}