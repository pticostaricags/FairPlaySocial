using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace FairPlaySocial.Services.Generators
{
    [Generator]
    public class ServicesIncrementalGenerator : IIncrementalGenerator
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
                        if (identifierText == "ServiceOfEntity")
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
                        if (identifierText == "ServiceOfEntity")
                        {
                            var argument = singleAttribute.ArgumentList!.Arguments[0];
                            //works only when using nameof()
                            var entityName =
                                ((argument.Expression as InvocationExpressionSyntax)
                                !.ArgumentList!.Arguments[0].Expression as IdentifierNameSyntax)
                                !.Identifier.Text;

                            var primaryKeyTypeArgument = singleAttribute.ArgumentList!.Arguments[1];
                            var primaryKeyTypeMemberAccessExpressionSyntax = primaryKeyTypeArgument.Expression as TypeOfExpressionSyntax;
                            var primartyKeyPredefinedTypeSyntax = primaryKeyTypeMemberAccessExpressionSyntax!.Type as PredefinedTypeSyntax;
                            var primaryKeyTypeValue = primartyKeyPredefinedTypeSyntax!.Keyword.ValueText;

                            StringBuilder stringBuilder = new();
                            stringBuilder.AppendLine("using System.Threading.Tasks;");
                            stringBuilder.AppendLine($"using {assemblyNameFirstPart}.DataAccess.Data;");
                            stringBuilder.AppendLine($"using {assemblyNameFirstPart}.DataAccess.Models;");
                            stringBuilder.AppendLine("using System.Linq;");
                            stringBuilder.AppendLine("using Microsoft.EntityFrameworkCore;");
                            stringBuilder.AppendLine("using Microsoft.Extensions.Logging;");
                            stringBuilder.AppendLine($"namespace {assemblyNameFirstPart}.Services");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine($"public partial class {serviceName}");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine("private readonly FairPlaySocialDatabaseContext _fairplaysocialDatabaseContext;");
                            stringBuilder.AppendLine($"private readonly ILogger<{serviceName}> _logger;");
                            stringBuilder.AppendLine($"public {serviceName}(FairPlaySocialDatabaseContext autogeneratedsystemDatabaseContext, ILogger<{serviceName}> logger)");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine("_fairplaysocialDatabaseContext = autogeneratedsystemDatabaseContext;");
                            stringBuilder.AppendLine($"this._logger = logger;");
                            stringBuilder.AppendLine("}");
                            stringBuilder.AppendLine($"public async Task<{entityName}> Create{entityName}Async({entityName} entity, CancellationToken cancellationToken)");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine($"this._logger.LogInformation(message: \"Start of Create{entityName}Async\");");
                            stringBuilder.AppendLine($"await _fairplaysocialDatabaseContext.{entityName}.AddAsync(entity,cancellationToken);");
                            stringBuilder.AppendLine($"await _fairplaysocialDatabaseContext.SaveChangesAsync(cancellationToken);");
                            stringBuilder.AppendLine($"return entity;");
                            stringBuilder.AppendLine("}");
                            stringBuilder.AppendLine($"public IQueryable<{entityName}> GetAll{entityName}(bool trackEntities, CancellationToken cancellationToken)");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine($"this._logger.LogInformation(message: \"Start of GetAll{entityName}\");");
                            stringBuilder.AppendLine($"if (trackEntities)");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine($"return _fairplaysocialDatabaseContext.{entityName}.AsTracking();");
                            stringBuilder.AppendLine("}");
                            stringBuilder.AppendLine("else");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine($"return _fairplaysocialDatabaseContext.{entityName}.AsNoTracking();");
                            stringBuilder.AppendLine("}");
                            stringBuilder.AppendLine("}");
                            stringBuilder.AppendLine($"public async Task Delete{entityName}Async({primaryKeyTypeValue} {entityName}Id, CancellationToken cancellationToken)");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine($"this._logger.LogInformation(message: \"Start of Delete{entityName}Async\");");
                            stringBuilder.AppendLine($"var entity = await this._fairplaysocialDatabaseContext.{entityName}" +
                                $".Where(p=>p.{entityName}Id == {entityName}Id).SingleOrDefaultAsync(cancellationToken:cancellationToken);");
                            stringBuilder.AppendLine("if (entity is null)");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine($"throw new Exception($\"Unable to find {entityName} with id={{{entityName}Id}}\");");
                            stringBuilder.AppendLine("}");
                            stringBuilder.AppendLine($"_fairplaysocialDatabaseContext.{entityName}.Remove(entity);");
                            stringBuilder.AppendLine($"await _fairplaysocialDatabaseContext.SaveChangesAsync(cancellationToken:cancellationToken);");
                            stringBuilder.AppendLine("}");
                            stringBuilder.AppendLine($"public async Task<{entityName}> Update{entityName}Async({entityName} entity, CancellationToken cancellationToken)");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine($"this._logger.LogInformation(message: \"Start of Update{entityName}Async\");");
                            stringBuilder.AppendLine($"this._fairplaysocialDatabaseContext.{entityName}.Update(entity);");
                            stringBuilder.AppendLine("await this._fairplaysocialDatabaseContext.SaveChangesAsync(cancellationToken);");
                            stringBuilder.AppendLine("return entity;");
                            stringBuilder.AppendLine("}");
                            stringBuilder.AppendLine($"public async Task<{entityName}> Get{entityName}ByIdAsync({primaryKeyTypeValue} {entityName}Id, bool trackEntities, CancellationToken cancellationToken)");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine($"this._logger.LogInformation(message: \"Start of Get{entityName}ByIdAsync\");");
                            stringBuilder.AppendLine($"if (trackEntities)");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine($"return await this._fairplaysocialDatabaseContext.{entityName}.AsTracking().Where(p=>p.{entityName}Id == {entityName}Id).SingleOrDefaultAsync(cancellationToken:cancellationToken);");
                            stringBuilder.AppendLine("}");
                            stringBuilder.AppendLine("else");
                            stringBuilder.AppendLine("{");
                            stringBuilder.AppendLine($"return await this._fairplaysocialDatabaseContext.{entityName}.AsNoTracking().Where(p=>p.{entityName}Id == {entityName}Id).SingleOrDefaultAsync(cancellationToken:cancellationToken);");
                            stringBuilder.AppendLine("}");
                            stringBuilder.AppendLine("}");
                            stringBuilder.AppendLine("}");
                            stringBuilder.AppendLine("}");
                            context.AddSource($"{serviceName}.g.cs",
                        SourceText.From(stringBuilder.ToString(), Encoding.UTF8));
                        }
                    }
                }
            }
        }
    }
}