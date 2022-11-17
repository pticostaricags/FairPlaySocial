using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace FairPlaySocial.ClientServices.Generators
{
    [Generator]
    public class ClientServicesIncrementalGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
#if DEBUG
            //System.Diagnostics.Debugger.Launch();
#endif
            // Do a simple filter for enums
            IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations =
                context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsSyntaxTargetForGeneration(s), // select enums with attributes
                    transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx)) // sect the enum with the [EnumExtensions] attribute
                .Where(static m => m is not null)!; // filter out attributed enums that we don't care about

            // Combine the selected interfaces with the `Compilation`
            IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)>
                compilationAndClasses
                = context.CompilationProvider.Combine(classDeclarations.Collect());

            // Generate the source using the compilation and classes
            context.RegisterSourceOutput(compilationAndClasses,
                static (spc, source) => Execute(source.Item1, source.Item2, spc));
        }

        private static ClassDeclarationSyntax GetSemanticTargetForGeneration(GeneratorSyntaxContext generatorSyntaxContext)
        {
            var classDeclarationSyntax = generatorSyntaxContext.Node as ClassDeclarationSyntax;
            return classDeclarationSyntax!;
        }

        private static bool IsSyntaxTargetForGeneration(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
            {
                foreach (var singleAttributeList in classDeclarationSyntax.AttributeLists)
                {
                    foreach (var singleAttribute in singleAttributeList.Attributes)
                    {
                        var identifierNameSyntax = (singleAttribute.Name) as IdentifierNameSyntax;
                        string identifierText = identifierNameSyntax!.Identifier.Text;
                        if (identifierText == "ClientServiceOfEntity")
                            return true;
                    }
                }
            }
            return false;
        }

        static void Execute(Compilation compilation,
            ImmutableArray<ClassDeclarationSyntax> classesDeclarationSyntax, SourceProductionContext context)
        {
            string assemblyName = compilation.AssemblyName!;
            string[] splittedAssemblyName = assemblyName.Split('.');
            string assemblyNameFirstPart = splittedAssemblyName[0];
            foreach (var singleClassDeclarationSyntax in classesDeclarationSyntax)
            {
                var serviceName = singleClassDeclarationSyntax.Identifier.Text;
                foreach (var singleAttributeList in singleClassDeclarationSyntax.AttributeLists)
                {
                    foreach (var singleAttribute in singleAttributeList.Attributes)
                    {
                        var identifierNameSyntax = (singleAttribute.Name) as IdentifierNameSyntax;
                        string identifierText = identifierNameSyntax!.Identifier.Text;
                        if (identifierText == "ClientServiceOfEntity")
                        {
                            var entityNameArgument = singleAttribute.ArgumentList!.Arguments[0];
                            var entityNameMemberAccessExpressionSyntax = entityNameArgument.Expression as MemberAccessExpressionSyntax;
                            var entityNameSimpleNameSyntax = entityNameMemberAccessExpressionSyntax!.Name;
                            var entityName = entityNameSimpleNameSyntax.Identifier.Text;

                            var primaryKeyTypeArgument = singleAttribute.ArgumentList!.Arguments[1];
                            var primaryKeyTypeMemberAccessExpressionSyntax = primaryKeyTypeArgument.Expression as TypeOfExpressionSyntax;
                            var primartyKeyPredefinedTypeSyntax = primaryKeyTypeMemberAccessExpressionSyntax!.Type as PredefinedTypeSyntax;
                            var primaryKeyTypeValue = primartyKeyPredefinedTypeSyntax!.Keyword.ValueText;

                            StringBuilder stringBuilder = new();
                            stringBuilder.AppendLine("using System.Threading.Tasks;");
                            stringBuilder.AppendLine("using System.Linq;");
                            stringBuilder.AppendLine("using System.Net.Http.Json;");
                            stringBuilder.AppendLine($"using {assemblyNameFirstPart}.Models.{entityName};");
                            stringBuilder.AppendLine($"namespace {assemblyNameFirstPart}.ClientServices");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine($"public partial class {serviceName}");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine("private readonly HttpClientService _httpClientService;");
                            stringBuilder.AppendLine($"public {serviceName}(HttpClientService httpClientService)");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine($"_httpClientService = httpClientService;");
                            stringBuilder.AppendLine("}");
                            AddCreateMethod(entityName, stringBuilder);
                            AddGetAllMethod(entityName, stringBuilder);
                            AddGetByEntityIdMethod(entityName, primaryKeyTypeValue, stringBuilder);
                            AddDeleteMethod(entityName, stringBuilder);
                            stringBuilder.AppendLine("}");
                            stringBuilder.AppendLine("}");
                            context.AddSource($"{serviceName}.g.cs",
                        SourceText.From(stringBuilder.ToString(), Encoding.UTF8));
                        }
                    }
                }
            }
        }

        private static void AddDeleteMethod(string entityName, StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine($"public async Task Delete{entityName}Async({entityName}Model model," +
                $"CancellationToken cancellationToken)");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"var requestUrl = $\"/api/{entityName}/Delete{entityName}?entityId={{model.{entityName}Id}}\";");
            stringBuilder.AppendLine("var authorizedHttpClient = _httpClientService.CreateAuthorizedClient();");
            stringBuilder.AppendLine("var response = await authorizedHttpClient.DeleteAsync(requestUrl, cancellationToken:cancellationToken);");
            stringBuilder.AppendLine("response.EnsureSuccessStatusCode();");
            stringBuilder.AppendLine("}");
        }

        private static void AddGetByEntityIdMethod(string entityName, string primaryKeyTypeValue, StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine($"public async Task<{entityName}Model> Get{entityName}ById({primaryKeyTypeValue} entityId, CancellationToken cancellationToken)");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("var authorizedHttpClient = _httpClientService.CreateAuthorizedClient();");
            stringBuilder.AppendLine($"var requestUrl = $\"api/{entityName}/Get{entityName}ById?entityId={{entityId}}\";");
            stringBuilder.AppendLine($"var result = await authorizedHttpClient.GetFromJsonAsync<{entityName}Model>(requestUrl, cancellationToken:cancellationToken);");
            stringBuilder.AppendLine("return result;");
            stringBuilder.AppendLine("}");
        }

        private static void AddGetAllMethod(string entityName, StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine($"public async Task<{entityName}Model[]> GetAll{entityName}Async(" +
                $"CancellationToken cancellationToken)");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("var authorizedHttpClient = _httpClientService.CreateAuthorizedClient();");
            stringBuilder.AppendLine($"var requestUrl = \"api/{entityName}/GetAll{entityName}\";");
            stringBuilder.AppendLine($"var result = await authorizedHttpClient.GetFromJsonAsync<{entityName}Model[]>(requestUrl,cancellationToken:cancellationToken);");
            stringBuilder.AppendLine("return result;");
            stringBuilder.AppendLine("}");
        }

        private static void AddCreateMethod(string entityName, StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine($"public async Task<{entityName}Model> Create{entityName}Async(" +
                                            $"Create{entityName}Model model," +
                                            $"CancellationToken cancellationToken)");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("var authorizedHttpClient = _httpClientService.CreateAuthorizedClient();");
            stringBuilder.AppendLine($"var requestUrl = \"api/{entityName}/Create{entityName}\";");
            stringBuilder.AppendLine("var response = await authorizedHttpClient.PostAsJsonAsync(requestUrl,model, cancellationToken:cancellationToken);");
            stringBuilder.AppendLine("response.EnsureSuccessStatusCode();");
            stringBuilder.AppendLine($"var result = await response.Content.ReadFromJsonAsync<{entityName}Model>(cancellationToken:cancellationToken);");
            stringBuilder.AppendLine("return result;");
            stringBuilder.AppendLine("}");
        }
    }
}
