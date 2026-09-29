using System.Reflection;
var asm = Assembly.LoadFrom("/Users/tuzhechen/Library/Application Support/Steam/steamapps/common/tModLoader/Libraries/FNA/1.0.0/FNA.dll");
var lzxType = asm.GetTypes().First(t => t.Name.Contains("Lzx") && t.GetMethods(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance).Any(m => m.Name == "Decompress"));
string outDir = args[0];
foreach (var path in args.Skip(1))
{
    using var fs = File.OpenRead(path);
    var br = new BinaryReader(fs);
    br.ReadBytes(3); br.ReadByte(); br.ReadByte(); byte flags = br.ReadByte();
    int fileSize = br.ReadInt32();
    Stream data;
    if ((flags & 0x80) != 0)
    {
        int outSize = br.ReadInt32();
        int compressed = fileSize - 14;
        var ms = new MemoryStream(outSize);
        var dec = Activator.CreateInstance(lzxType, BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance, null, new object[] { 16 }, null);
        var m = lzxType.GetMethod("Decompress", BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance);
        long start = fs.Position, pos = start;
        while (pos - start < compressed)
        {
            int hi = fs.ReadByte(), lo = fs.ReadByte();
            int block = (hi << 8) | lo, frame = 0x8000;
            if (hi == 0xFF) { hi = lo; lo = fs.ReadByte(); frame = (hi << 8) | lo; hi = fs.ReadByte(); lo = fs.ReadByte(); block = (hi << 8) | lo; pos += 5; } else pos += 2;
            if (block == 0 || frame == 0) break;
            m.Invoke(dec, new object[] { fs, block, ms, frame });
            pos += block;
            fs.Seek(pos, SeekOrigin.Begin);
        }
        ms.Position = 0; data = ms;
    }
    else data = fs;
    var r = new BinaryReader(data);
    int Read7() { int v = 0, s = 0; byte b; do { b = r.ReadByte(); v |= (b & 0x7F) << s; s += 7; } while ((b & 0x80) != 0); return v; }
    int readers = Read7();
    for (int i = 0; i < readers; i++) { int len = Read7(); r.ReadBytes(len); r.ReadInt32(); }
    Read7(); Read7();
    int fmt = r.ReadInt32(), w = r.ReadInt32(), h = r.ReadInt32(); r.ReadInt32();
    int size = r.ReadInt32();
    byte[] px = r.ReadBytes(size);
    // Write as raw RGBA with a tiny header; Python turns it into PNG.
    var name = Path.GetFileNameWithoutExtension(path);
    using var o = File.Create(Path.Combine(outDir, name + ".rgba"));
    var bw = new BinaryWriter(o); bw.Write(w); bw.Write(h); bw.Write(fmt); bw.Write(px);
    Console.WriteLine($"{name} {w}x{h} fmt {fmt}");
}
