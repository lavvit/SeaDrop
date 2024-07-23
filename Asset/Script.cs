using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace SeaDrop
{
    public class DrawScript
    {

    }

    public class Script
    {
        public static void Run(string path)
        {
            var ssr = ScriptSourceResolver.Default
                .WithBaseDirectory(Environment.CurrentDirectory);
            var smr = ScriptMetadataResolver.Default
                .WithBaseDirectory(Environment.CurrentDirectory);
            var so = ScriptOptions.Default
                .WithSourceResolver(ssr)
                .WithMetadataResolver(smr);
            /*var so = ScriptOptions.Default
                .WithFilePath(Path.GetFullPath(path));*/

            var script = CSharpScript.Create(File.ReadAllText(path), so);
            script.RunAsync().Wait();
        }
    }
}
