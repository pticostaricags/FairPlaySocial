using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace FairPlaySocial.Server.Generators
{
    [Generator]
    public class ControllersIncrementalGenerator : IIncrementalGenerator
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
                        if (identifierText == "ControllerOfEntity")
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
                var controllerName = singleClassDeclarationSyntax.Identifier.Text;
                foreach (var singleAttributeList in singleClassDeclarationSyntax.AttributeLists)
                {
                    foreach (var singleAttribute in singleAttributeList.Attributes)
                    {
                        var identifierNameSyntax = (singleAttribute.Name) as IdentifierNameSyntax;
                        string identifierText = identifierNameSyntax!.Identifier.Text;
                        if (identifierText == "ControllerOfEntity")
                        {
                            var entityNameArgument = singleAttribute.ArgumentList!.Arguments[0];
                            //works only when using nameof()
                            var entityName =
                                ((entityNameArgument.Expression as InvocationExpressionSyntax)
                                !.ArgumentList!.Arguments[0].Expression as IdentifierNameSyntax)
                                !.Identifier.Text;

                            var primaryKeyTypeArgument = singleAttribute.ArgumentList!.Arguments[1];
                            var primaryKeyTypeMemberAccessExpressionSyntax = primaryKeyTypeArgument.Expression as TypeOfExpressionSyntax;
                            var primartyKeyPredefinedTypeSyntax = primaryKeyTypeMemberAccessExpressionSyntax!.Type as PredefinedTypeSyntax;
                            var primaryKeyTypeValue = primartyKeyPredefinedTypeSyntax!.Keyword.ValueText;

                            StringBuilder stringBuilder = new();
                            stringBuilder.AppendLine("using System.Threading.Tasks;");
                            stringBuilder.AppendLine("using Microsoft.AspNetCore.Mvc;");
                            stringBuilder.AppendLine($"using {assemblyNameFirstPart}.Services;");
                            stringBuilder.AppendLine($"using {assemblyNameFirstPart}.DataAccess.Data;");
                            stringBuilder.AppendLine($"using {assemblyNameFirstPart}.DataAccess.Models;");
                            stringBuilder.AppendLine($"using {assemblyNameFirstPart}.Models.{entityName};");
                            stringBuilder.AppendLine($"using AutoMapper;");
                            stringBuilder.AppendLine("using Microsoft.EntityFrameworkCore;");
                            stringBuilder.AppendLine($"namespace {assemblyNameFirstPart}.Server.Controllers");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine($"public partial class {controllerName}");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine($"private {entityName}Service {entityName}Service {{ get; set; }}");
                            stringBuilder.AppendLine("private IMapper mapper { get; set; }");
                            stringBuilder.AppendLine($"public {controllerName}(" +
                                $"{entityName}Service {entityName}Service, " +
                                $"IMapper mapper)");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine($"this.{entityName}Service = {entityName}Service;");
                            stringBuilder.AppendLine("this.mapper = mapper;");
                            stringBuilder.AppendLine("}");
                            stringBuilder.AppendLine("[HttpPost(\"[action]\")]");
                            stringBuilder.AppendLine($"public async Task<{entityName}Model> Create{entityName}(Create{entityName}Model create{entityName}Model, CancellationToken cancellationToken)");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine($"var mappedEntity = mapper.Map<Create{entityName}Model,{entityName}>(create{entityName}Model);");
                            stringBuilder.AppendLine($"var resultEntity = await {entityName}Service.Create{entityName}Async(mappedEntity, cancellationToken);");
                            stringBuilder.AppendLine($"var resultModel = mapper.Map<{entityName},{entityName}Model>(resultEntity);");
                            stringBuilder.AppendLine($"return resultModel;");
                            stringBuilder.AppendLine("}");
                            stringBuilder.AppendLine("[HttpGet(\"[action]\")]");
                            stringBuilder.AppendLine($"public async Task<{entityName}Model[]> GetAll{entityName}(CancellationToken cancellationToken)");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine($"var query = {entityName}Service.GetAll{entityName}(trackEntities:false,cancellationToken:cancellationToken);");
                            stringBuilder.AppendLine($"var mappedQuery = query.Select(p=>mapper.Map<{entityName},{entityName}Model>(p));");
                            stringBuilder.AppendLine("var result = await mappedQuery.ToArrayAsync();");
                            stringBuilder.AppendLine("return result;");
                            stringBuilder.AppendLine("}");
                            stringBuilder.AppendLine("[HttpDelete(\"[action]\")]");
                            stringBuilder.AppendLine($"public async Task<IActionResult> Delete{entityName}({primaryKeyTypeValue} entityId, CancellationToken cancellationToken)");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine($"await {entityName}Service.Delete{entityName}Async(entityId, cancellationToken);");
                            stringBuilder.AppendLine("return Ok();");
                            stringBuilder.AppendLine("}");
                            stringBuilder.AppendLine("[HttpPut(\"[action]\")]");
                            stringBuilder.AppendLine($"public async Task<{entityName}Model> Update{entityName}({entityName}Model {entityName}Model, CancellationToken cancellationToken)");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine($"var {entityName}Entity = mapper.Map<{entityName}Model, {entityName}>({entityName}Model);");
                            stringBuilder.AppendLine($"var updated{entityName}Entity = await {entityName}Service.Update{entityName}Async({entityName}Entity, cancellationToken);");
                            stringBuilder.AppendLine($"var updated{entityName}Model = mapper.Map<{entityName}, {entityName}Model>(updated{entityName}Entity);");
                            stringBuilder.AppendLine($"return updated{entityName}Model;");
                            stringBuilder.AppendLine("}");
                            stringBuilder.AppendLine("[HttpGet(\"[action]\")]");
                            stringBuilder.AppendLine($"public async Task<{entityName}Model> Get{entityName}ById([FromQuery]{primaryKeyTypeValue} entityId, CancellationToken cancellationToken)");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine($"var entity = await {entityName}Service.Get{entityName}ByIdAsync({entityName}Id:entityId, trackEntities:false, cancellationToken);");
                            stringBuilder.AppendLine($"var result = mapper.Map<{entityName}, {entityName}Model>(entity);");
                            stringBuilder.AppendLine("return result;");
                            stringBuilder.AppendLine("}");
                            stringBuilder.AppendLine("}");
                            stringBuilder.AppendLine("}");
                            context.AddSource($"{controllerName}.g.cs",
                        SourceText.From(stringBuilder.ToString(), Encoding.UTF8));
                        }
                    }
                }
            }
        }
    }
}