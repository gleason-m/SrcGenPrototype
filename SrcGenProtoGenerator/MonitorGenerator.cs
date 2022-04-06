using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json;
using SrcGenProtoLib;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SrcGenProtoGenerator
{
    [Generator]
    public class MonitorGenerator : ISourceGenerator
    {
        private static readonly IDeserializer yamlDeserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

        public void Execute(GeneratorExecutionContext context)
        {
            var xmlFiles = context.AdditionalFiles.Where(at => at.Path.EndsWith(".xml"));
            foreach (var xmlFile in xmlFiles)
            {
                ProcessXmlFile(xmlFile, context);
            }

            var yamlFiles = context.AdditionalFiles.Where(at => at.Path.EndsWith(".yaml"));

            var sb = new StringBuilder($@"using SrcGenProtoLib;

namespace Monitors
{{
    using System;
    using System.Collections.Generic;

    public class MonitorConfigProvider
    {{
        public static List<MonitorConfig> monitorConfigs = new List<MonitorConfig>({yamlFiles.Count()});

        public static List<MonitorConfig> LoadConfigs()
        {{");

            foreach (var yamlFile in yamlFiles)
            {
                var className = ProcessYamlFile(yamlFile, context);
                sb.AppendLine($"            monitorConfigs.Add({className}.LoadConfig());");
            }

            sb.Append($@"        return monitorConfigs;
        }}
    }}
}}
");

            context.AddSource("MonitorConfigProvider.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }

        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif
        }

        private void ProcessXmlFile(AdditionalText xmlFile, GeneratorExecutionContext context)
        {
            // try and load the settings file
            XmlDocument xmlDoc = new XmlDocument();
            var text = xmlFile.GetText(context.CancellationToken).ToString();
            try
            {
                xmlDoc.LoadXml(text);
            }
            catch
            {
                return;
            }

            // create a class in the XmlSetting class that represnts this entry, and a static field that contains a singleton instance.
            var monitorName = xmlDoc.DocumentElement.GetAttribute("name");
            var speakElement = (XmlElement)xmlDoc.DocumentElement.ChildNodes[0];
            var says = speakElement.InnerText;

            var sb = new StringBuilder($@"using SrcGenProtoLib;

namespace Monitors
{{
    using System;
    using System.Xml;

    public class {monitorName} : Monitor
    {{
        public override void Speak()
        {{
            Console.WriteLine(""{says}"");
        }}
    }}
}}
");

            context.AddSource($"{monitorName}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }

        private string ProcessYamlFile(AdditionalText yamlFile, GeneratorExecutionContext context)
        {
            var text = yamlFile.GetText(context.CancellationToken).ToString();
            var config = yamlDeserializer.Deserialize<MonitorConfig>(text);
            var className = config.MonitorName;

            var sb = new StringBuilder($@"using SrcGenProtoLib;

namespace Monitors
{{
    using System;
    using Newtonsoft.Json;

    public class {className} : MonitorConfig
    {{
        private static string ConfigAsJson = {JsonConvert.SerializeObject(JsonConvert.SerializeObject(config))};

        public static MonitorConfig LoadConfig()
            => JsonConvert.DeserializeObject<MonitorConfig>(ConfigAsJson);
    }}
}}
");

            context.AddSource($"{className}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
            return className;
        }
    }
}