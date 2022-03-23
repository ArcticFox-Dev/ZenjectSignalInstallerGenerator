using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Pixions.SignalsGenerator.Utils;

namespace ArcticFox.Zenject.SignalsGenerator
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // Register a factory that can create our custom syntax receiver
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // the generator infrastructure will create a receiver and populate it
            // we can retrieve the populated instance via the context
            var syntaxReceiver = (SyntaxReceiver)context.SyntaxReceiver;
            if (syntaxReceiver is null) return;

            var signals = new Dictionary<string, Signal>();

            try
            {
                foreach (var entityFullType in syntaxReceiver.SignalsToInstall)
                {
                    var typeSymbol = context.Compilation.GetTypeByMetadataName(entityFullType);
                    if (typeSymbol is null || signals.ContainsKey(typeSymbol.Name)) continue;
                    signals.Add(typeSymbol.Name, new Signal(entityFullType, typeSymbol.Name));
                }
            }
            finally
            {
                if (signals.Count > 0)
                {
                    var signalsBuilder = new StringBuilder();
                    foreach (var signal in signals)
                    {
                        signalsBuilder.AppendLine($"Container.DeclareSignal<{signal.Value.FullQualifiedName}>();");
                    }

                    var generatorStats = $@"
using Zenject;

namespace Generated
{{
    public static class GeneratedSignalInstaller
    {{
            public static void InstallSignals(DiContainer Container)
            {{
{signalsBuilder.ToString()}
            }}
    }}
}}
";
                    FormattedFileWriter.WriteSourceFile(context, generatorStats, $"GeneratedSignalInstaller");
                }
            }

            syntaxReceiver.SignalsToInstall.Clear();
        }

        
    }
}
