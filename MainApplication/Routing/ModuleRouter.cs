using MainApplication.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace MainApplication.Routing
{
    public class ModuleRouter : Router, IComponent, IHandleAfterRender, IDisposable
    {
        [Inject] public ModuleManager ModuleManager { get; set; }

        public new void Attach(RenderHandle renderHandle)
        {
            base.Attach(renderHandle);
            ModuleManager.OnModulesLoaded += OnModulesLoaded;
        }

        public new void Dispose()
        {
            base.Dispose();
            ModuleManager.OnModulesLoaded -= OnModulesLoaded;
        }

        public new Task SetParametersAsync(ParameterView parameters)
        {
            return base.SetParametersAsync(parameters);
        }

        private void OnModulesLoaded(IEnumerable<Assembly> modules)
        {
            var dict = new Dictionary<string, object>();
            if (modules != null)
            {
                dict.Add("AdditionalAssemblies", modules);
            }

            var pv = ParameterView.FromDictionary(dict);
            SetParametersAsync(pv);
        }
    }
}