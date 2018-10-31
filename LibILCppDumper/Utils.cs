using System;
using System.IO;
using ICSharpCode.SharpZipLib.Core;

namespace Il2CppDumper {
    public static class Utils {
        public static MemoryStream Buffer(this Stream source, byte[] copyBuffer = null) {
            MemoryStream dest;
            try {
                dest = new MemoryStream((int)source.Length);
            } catch(NotSupportedException) {
                dest = new MemoryStream();
            }
            StreamUtils.Copy(source, dest, copyBuffer ?? new byte[4096]);
            dest.Seek(0, SeekOrigin.Begin);
            return dest;
        }
    }
}
