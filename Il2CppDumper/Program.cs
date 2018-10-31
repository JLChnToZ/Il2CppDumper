using System;
using System.IO;
using System.Windows.Forms;
using System.Web.Script.Serialization;

namespace Il2CppDumper {
    class Program {
        [STAThread]
        static void Main(string[] args) {
            var config = File.Exists("config.json") ?
                new JavaScriptSerializer().Deserialize<Config>(File.ReadAllText("config.json")) :
                new Config();
            var ofd = new OpenFileDialog();
            var sfd = new SaveFileDialog();
            var sfd2 = new FolderBrowserDialog();
            Stream outStream = null, scriptStream = null;

            ofd.Filter = "ELF file or Mach-O file|*.*|APK File (*.apk;.zip)|*.apk;*.zip";
            if (ofd.ShowDialog() != DialogResult.OK) return;
            IL2CppDump dumper = null;
            var il2cppfile = File.OpenRead(ofd.FileName);

            if(ofd.FilterIndex == 2) {
                dumper = IL2CppDump.FromAPK(il2cppfile, config);
            } else {
                ofd.Filter = "global-metadata|global-metadata.dat";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                dumper = new IL2CppDump(il2cppfile, File.OpenRead(ofd.FileName), config);
            }

            Console.WriteLine("Initializing metadata...");
            Console.WriteLine("Select Mode: 1.Manual 2.Auto 3.Auto(Advanced) 4.Auto(Plus) 5.Auto(Symbol)");
            DumpMode mode = (DumpMode)(int.Parse(Console.ReadKey(true).KeyChar.ToString()) - 1);
            ulong codeRegistration = 0L, metadataRegistration = 0L;
            if(mode == DumpMode.Manual) {
                Console.Write("Input CodeRegistration: ");
                codeRegistration = Convert.ToUInt64(Console.ReadLine(), 16);
                Console.Write("Input MetadataRegistration: ");
                metadataRegistration = Convert.ToUInt64(Console.ReadLine(), 16);
            }
            if (!dumper.Parse(mode, codeRegistration, metadataRegistration)) {
                Console.WriteLine("Failed to parse");
                return;
            }
            sfd.Filter = "C-Sharp (*.cs)|*.cs|All Files (*.*)|*.*";
            if (sfd.ShowDialog() != DialogResult.OK) return;
            outStream = File.OpenWrite(sfd.FileName);
            sfd.Filter = "IDL Script (*.py)|*.py|All Files (*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
                scriptStream = File.OpenWrite(sfd.FileName);
            dumper.DumpEverything(outStream, scriptStream);

            if(sfd2.ShowDialog() == DialogResult.OK)
                dumper.DumpDummyDll(sfd2.SelectedPath);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
