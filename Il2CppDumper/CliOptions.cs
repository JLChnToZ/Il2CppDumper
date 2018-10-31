using CommandLine;
using System.IO;

namespace Il2CppDumper {
    class CliOptions {
        [Value(0, MetaName = "binary path", Required = true, HelpText = "libil2cpp.so or APK file path")]
        public FileInfo IL2CppPath { get; set; }

        [Value(1, MetaName = "output file path", Required = true, HelpText = "C# dump output path")]
        public FileInfo OutputPath { get; set; }

        [Option('g', "global-metadata-path", HelpText = "global-metadata.dat path. Required if provided .so file in binary path.")]
        public FileInfo MetaFilePath { get; set; }

        [Option('i', "ida-path", HelpText = "Python IDA output path")]
        public FileInfo IDAPath { get; set; }

        [Option('d', "dumy-output", HelpText = "Dummy DLLs output path")]
        public DirectoryInfo DummyPath { get; set; }

        [Option('m', "mode", Default = (int)DumpMode.AutoPlus, HelpText = "0 = Manual, 1 = Auto, 2 = Auto(Advanced), 3 = Auto(Plus), 4 = Auto(Symbol)")]
        public int Mode { get; set; }

        [Option('r', "code-registration", HelpText = "Offset of code registration. Require in manual mode")]
        public ulong CodeRegistration { get; set; }

        [Option('e', "metadata-registration", HelpText = "Offset of code registration. Require in manual mode")]
        public ulong MetadataRegistration { get; set; }

        [Option('M', "no-dump-method", HelpText = "Disable dump methods")]
        public bool NoDumpMethod { get; set; }

        [Option('F', "no-dump-field", HelpText = "Disable dump fields")]
        public bool NoDumpField { get; set; }

        [Option('p', "dump-property", HelpText = "Enable dump property methods")]
        public bool DumpProperty { get; set; }

        [Option('A', "no-dump-attrbute", HelpText = "Disable dump attributes")]
        public bool NoDumpAttribute { get; set; }

        [Option('O', "no-dump-field-offset", HelpText = "Disable dump field offsets")]
        public bool NoDumpFieldOffset { get; set; }

        [Option('N', "no-make-function", HelpText = "Disable make function in IDAs")]
        public bool NoMakeFunction { get; set; }
        
        [Option('V', "force-il2cpp-version", HelpText = "Enforce the parser to use specified IL2Cpp version to parse the code")]
        public int? ForceVersion { get; set; }

        public Config ToConfig() {
            return new Config {
                DumpField = !NoDumpField,
                DumpMethod = !NoDumpMethod,
                DumpProperty = DumpProperty,
                DumpAttribute = !NoDumpAttribute,
                DumpFieldOffset = !NoDumpFieldOffset,
                MakeFunction = !NoMakeFunction,
                ForceIl2CppVersion = ForceVersion.HasValue,
                ForceVersion = ForceVersion.GetValueOrDefault(16),
                DummyDll = DummyPath != null
            };
        }
    }
}
