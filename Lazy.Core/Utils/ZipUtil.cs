using ICSharpCode.SharpZipLib.Zip;

namespace Lazy.Core.Utils;

public static class ZipUtil
{
    /// <summary>
    /// 压缩字节信息
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static byte[] Compress(string fileName, byte[] bytes)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (var zip = new ZipOutputStream(ms))
            {
                var entity = new ZipEntry(fileName);
                zip.PutNextEntry(entity);
                zip.Write(bytes, 0, bytes.Length);
                zip.CloseEntry();
                zip.Finish();
                zip.Close();

                return ms.ToArray();
            }
        }
    }

    /// <summary>
    /// 压缩单个文件
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static byte[] CompressFile(string filePath)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (ZipOutputStream zip = new ZipOutputStream(ms))
            {
                zip.SetLevel(9);
                ZipEntry entry = new ZipEntry(Path.GetFileName(filePath));
                entry.DateTime = DateTime.Now;
                zip.PutNextEntry(entry);

                using (FileStream fs = File.OpenRead(filePath))
                {
                    var buffer = new byte[fs.Length];
                    fs.ReadExactly(buffer);
                    zip.Write(buffer, 0, buffer.Length);
                }
                zip.Finish();
                zip.Close();

                return ms.ToArray();
            }
        }
    }

    /// <summary>
    /// 压缩目录
    /// </summary>
    /// <param name="inputFolderPath"></param>
    /// <returns></returns>
    public static byte[] CompressFiles(string inputFolderPath)
    {
        var files = Directory.GetFiles(inputFolderPath);
        byte[] obuffer;

        using (MemoryStream ms = new MemoryStream())
        {
            using (var zip = new ZipOutputStream(ms))
            {
                zip.SetLevel(9);
                foreach (string file in files)
                {
                    var entry = new ZipEntry(Path.GetFileName(file));
                    zip.PutNextEntry(entry);

                    using (var fs = File.OpenRead(file))
                    {
                        obuffer = new byte[fs.Length];
                        fs.ReadExactly(obuffer);
                        zip.Write(obuffer, 0, obuffer.Length);
                    }
                }

                zip.Finish();
                zip.Close();

                return ms.ToArray();
            }
        }
    }
}
