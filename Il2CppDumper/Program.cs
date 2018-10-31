using System;
using CommandLine;

namespace Il2CppDumper {
    class Program {
        static void Main(string[] args) {
            Parser.Default.ParseArguments<CliOptions>(args).WithParsed(Run);
        }

        private static void Run(CliOptions options) {
            var config = options.ToConfig();

            IL2CppDump dumper = null;
            var il2cppfile = options.IL2CppPath.OpenRead();

            if (options.MetaFilePath == null)
                dumper = IL2CppDump.FromAPK(il2cppfile, config);
            else
                dumper = new IL2CppDump(il2cppfile, options.MetaFilePath.OpenRead(), config);

            Console.WriteLine("Initializing metadata...");

            if (!dumper.Parse((DumpMode)options.Mode, options.CodeRegistration, options.MetadataRegistration)) {
                Console.WriteLine("Failed to parse");
                return;
            }

            Console.WriteLine("Start dump...");
            dumper.DumpEverything(options.OutputPath.OpenWrite(), options.IDAPath?.OpenWrite());

            if (options.DummyPath != null) {
                Console.WriteLine("Start dump dummy DLL files...");
                dumper.DumpDummyDll(options.DummyPath.FullName);
            }

            Console.WriteLine("Done");
        }
    }
}
