﻿using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Il2CppDumper {
    public partial class IL2CppDump {

        public static IL2CppDump FromAPK(Stream zipFileStream, Config config = null, bool closeStream = true) {
            ZipFile zipFile = null;
            try {
                zipFile = new ZipFile(zipFileStream);
                return FromAPK(zipFile, config);
            } finally {
                if (zipFile != null) {
                    zipFile.IsStreamOwner = closeStream;
                    zipFile.Close();
                }
            }
        }

        public static IL2CppDump FromAPK(ZipFile apkFile, Config config = null) {
            Stream metaStream = null, il2cppStream = null;
            foreach (ZipEntry zipEntry in apkFile) {
                if (!zipEntry.IsFile || !zipEntry.CanDecompress)
                    continue;
                switch (Path.GetFileName(zipEntry.Name)) {
                    default: continue;
                    case "global-metadata":
                    case "global-metadata.dat":
                        metaStream = apkFile.GetInputStream(zipEntry);
                        break;
                    case "libil2cpp.so":
                        il2cppStream = apkFile.GetInputStream(zipEntry);
                        break;
                }
                if (il2cppStream != null && metaStream != null) break;
            }
            if (il2cppStream == null || metaStream == null)
                throw new FileNotFoundException("Could not find data for dumping.");
            byte[] copyBuffer = new byte[4096];
            il2cppStream = il2cppStream.Buffer(copyBuffer);
            metaStream = metaStream.Buffer(copyBuffer);
            return new IL2CppDump(il2cppStream, metaStream, config);
        }
    }
}
