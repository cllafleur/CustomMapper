using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.Emit;
using System.Runtime.Versioning;

namespace MapperDslLib.Runtime.Csharp
{
    public class GetInstanceVisitorGenerator<T> : IGetInstanceVisitor<T>
    {
        private string value;
        private MethodInfo method;
        private static Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();

        private string code;

        public GetInstanceVisitorGenerator(string value)
        {
            this.value = value;
            if (methods.ContainsKey(value))
            {
                this.method = methods[value];
            }
            else
            {
                var (className, code) = GenerateCode();
                this.method = CompileCode(className, code);
                this.code = code;
                methods.Add(value, this.method);
            }
        }

        public IEnumerable<object> GetInstance(T obj)
        {
            return (IEnumerable<object>)method.Invoke(null, new object[] { obj });
        }

        public PropertyInfo GetLastPropertyInfo()
        {
            return null;
        }

        private MethodInfo CompileCode(string className, string code)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            string assemblyName = Path.GetRandomFileName();
            List<MetadataReference> references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(List<>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Nullable<>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(T).Assembly.Location),
            };
            if (Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?
    .FrameworkName.StartsWith(".netcore", StringComparison.OrdinalIgnoreCase) == true)
            {
                references.Add(MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location));
            }

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    Assembly assembly = Assembly.Load(ms.ToArray());
                    var type = assembly.GetType(className);
                    return type.GetMethod("Get", BindingFlags.Static | BindingFlags.Public);
                }
            }
            return null;
        }

        private (string className, string code) GenerateCode()
        {
            string className = $"GetInstanceVisitorGenerator_{value.Replace('.', '_')}";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"namespace {typeof(GetInstanceVisitorGenerator<>).Namespace} {{").AppendLine();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;").AppendLine();
            sb.AppendLine($"public class {className} {{");
            sb.AppendLine($"public static IEnumerable<object> Get({typeof(T).FullName.Replace("+", ".")} obj) {{");
            sb.AppendLine($"yield return obj.{value.Replace(".", "?.")};");
            sb.AppendLine("}");
            sb.AppendLine("}}");
            return ($"{typeof(GetInstanceVisitorGenerator<>).Namespace}.{className}", sb.ToString());
        }
    }
}
