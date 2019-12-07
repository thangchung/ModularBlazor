using MainApplication.Serrvices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

namespace MainApplication.Services
{
    public class ModuleManager
    {
        private readonly Dictionary<PluginLoadContext, Assembly> _loadContexts = new Dictionary<PluginLoadContext, Assembly>();

        public event Action<IEnumerable<Assembly>> OnModulesLoaded;

        public bool Loaded { get; private set; }

        public string AppName { get; private set; } = typeof(Program).Assembly.GetName().Name;

        public void LoadModules()
        {
            var modules = new List<string>
            {
                $"{AppName}\\Modules\\Module1.dll",
                $"{AppName}\\Modules\\Module2.dll"
            };

            foreach(var modulePath in modules)
            {
                LoadModule(modulePath, out var pluginLoadContext, out var assembly);
                _loadContexts.Add(pluginLoadContext, assembly);
            }

            OnModulesLoaded?.Invoke(_loadContexts.Values);
            Loaded = true;
        }

        public void UnloadModules()
        {
            foreach(var pluginLoadContext in _loadContexts)
            {
                pluginLoadContext.Key.Unload();
            }

            OnModulesLoaded?.Invoke(null);
            _loadContexts.Clear();
            Loaded = false;
        }

        private void LoadModule(string modulePath, out PluginLoadContext pluginLoadContext, out Assembly assembly)
        {
            string root = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));

            string pluginLocation = Path.GetFullPath(Path.Combine(root, modulePath.Replace('\\', Path.DirectorySeparatorChar)));

            pluginLoadContext = new PluginLoadContext(pluginLocation);
            assembly = pluginLoadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));

            var resources = assembly.GetManifestResourceNames();
            foreach(var resource in resources)
            {
                var fileName = resource.Split(":").Last();
                var dirName = assembly.GetName().Name;
                var stream = assembly.GetManifestResourceStream(resource);
                if(!Directory.Exists($"{root}\\{AppName}\\wwwroot\\{dirName}"))
                {
                    Directory.CreateDirectory($"{root}\\{AppName}\\wwwroot\\{dirName}");
                }
                var file = File.Create($"{root}\\{AppName}\\wwwroot\\{dirName}\\{fileName}");
                file.Write(ReadFully(stream));
                file.Close();
            }
        }

        private byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using MemoryStream ms = new MemoryStream();
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }
    }
}