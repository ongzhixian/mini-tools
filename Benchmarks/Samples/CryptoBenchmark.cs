using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace Benchmarks.Samples;

// [SimpleJob(RuntimeMoniker.Net472, baseline: true)]
// [SimpleJob(RuntimeMoniker.NetCoreApp30)]
// [SimpleJob(RuntimeMoniker.CoreRt30)]
// [SimpleJob(RuntimeMoniker.Mono)]
// [RPlotExporter]
[MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter]
[MemoryDiagnoser]
public class BenchmarkHashAlgo
{
    private const int N = 10000;
    private readonly byte[] data;


    private readonly MD5 md5 = MD5.Create();

    private readonly SHA1 sha1 = System.Security.Cryptography.SHA1.Create();

    // private readonly SHA256 sha256 = SHA256.Create();

    // private readonly SHA384 sha384 = System.Security.Cryptography.SHA384.Create();

    // private readonly SHA512 sha512 = System.Security.Cryptography.SHA512.Create();

    public BenchmarkHashAlgo()
    {
        data = new byte[N];
        new Random(42).NextBytes(data);
    }

    [Benchmark]
    public byte[] Md5() => md5.ComputeHash(data);

    [Benchmark]
    public byte[] Sha1() => sha1.ComputeHash(data);

    // [Benchmark]
//     public byte[] Sha256() => sha256.ComputeHash(data);

//     [Benchmark]
//     public byte[] Sha384() => sha384.ComputeHash(data);

//     [Benchmark]
//     public byte[] Sha512() => sha512.ComputeHash(data);

}