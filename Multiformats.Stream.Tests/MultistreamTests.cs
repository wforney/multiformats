using BinaryEncoding;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Xunit.Sdk;

namespace Multiformats.Stream.Tests;

public class MultistreamTests
{
    [Fact]
    public Task Async_TestInvalidProtocol()
    {
        return UsePipeWithMuxerAsync(async (a, b, mux) =>
        {
            _ = mux.NegotiateAsync(a, CancellationToken.None);

            var ms = Multistream.Create(b, "/THIS_IS_WRONG");
            Assert.NotNull(ms);

            await Assert.ThrowsAsync<Exception>(() => ms.WriteAsync([], 0, 0));
        });
    }

    [Fact]
    public Task Async_TestLazyConns()
    {
        return UsePipeWithMuxerAsync(async (a, b, mux) =>
        {
            _ = mux.AddHandler(new TestHandler("/a", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/b", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/c", (p,s)=>true));

            var la = Task.Run(() => Multistream.CreateSelect(a, "/c"));
            var lb = Task.Run(() => Multistream.CreateSelect(b, "/c"));

            var result = await Task.WhenAll(la, lb);
            Assert.All(result, r => Assert.NotNull(r));
            await VerifyPipeAsync(result[0], result[1]);
        });
    }

    [Fact]
    public Task Async_TestProtocolNegotation()
    {
        return UsePipeWithMuxerAsync(async (a, b, mux) =>
        {
            _ = mux.AddHandler(new TestHandler("/a", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/b", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/c", (p,s)=>true));

            var negotiator = mux.NegotiateAsync(a, CancellationToken.None);

            await MultistreamMuxer.SelectProtoOrFailAsync("/a", b, CancellationToken.None);

            var selected = await negotiator;
            Assert.NotNull(selected);
            Assert.NotNull(selected.Protocol);
            Assert.Equal("/a", selected.Protocol);
        }, verify: true);
    }

    [Fact]
    public Task Async_TestSelectFails()
    {
        return UsePipeWithMuxerAsync(async (a, b, mux) =>
        {
            _ = mux.AddHandler(new TestHandler("/a", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/b", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/c", (p,s)=>true));

            _ = mux.NegotiateAsync(a, CancellationToken.None);

            await Assert.ThrowsAsync<NotSupportedException>(() => MultistreamMuxer.SelectOneOfAsync(["/d", "/e"], b, CancellationToken.None));
        });
    }

    [Fact]
    public Task Async_TestSelectOne()
    {
        return UsePipeWithMuxerAsync(async (a, b, mux) =>
        {
            _ = mux.AddHandler(new TestHandler("/a", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/b", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/c", (p,s)=>true));

            var negotiator = mux.NegotiateAsync(a, CancellationToken.None);

            await MultistreamMuxer.SelectOneOfAsync(["/d", "/e", "/c"], b, CancellationToken.None);

            var finished = await Task.WhenAny(negotiator, Task.Delay(500));
            Assert.Equal(negotiator, finished);
            var selected = negotiator.Result;
            Assert.NotNull(selected);
            Assert.NotNull(selected.Protocol);
            Assert.Equal("/c", selected.Protocol);
        }, verify: true);
    }

    [Fact]
    public Task Async_TestSelectOneAndWrite()
    {
        return UsePipeWithMuxerAsync(async (a, b, mux) =>
        {
            _ = mux.AddHandler(new TestHandler("/a", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/b", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/c", (p,s)=>true));

            var task = Task.Run(async () =>
            {
                var selected = await mux.NegotiateAsync(a, CancellationToken.None);
                Assert.NotNull(selected);
                Assert.NotNull(selected.Protocol);
                Assert.Equal("/c", selected.Protocol);
            });

            var sel = await MultistreamMuxer.SelectOneOfAsync(["/d", "/e", "/c"], b, CancellationToken.None);
            Assert.NotNull(sel);
            Assert.Equal("/c", sel);
            Assert.True(task.Wait(500));
        }, verify: true);
    }

    [Fact]
    public void TestAddHandlerOverride()
    {
        UsePipeWithMuxer((a, b, mux) =>
        {
            _ = mux.AddHandler("/foo", (p, s) => throw new XunitException("should not get executed"));
            _ = mux.AddHandler("/foo", (p, s) => true);

            var task = Task.Run(() => MultistreamMuxer.SelectProtoOrFail("/foo", a));

            Assert.True(mux.Handle(b));
            Assert.True(task.Wait(500));
        }, verify: true);
    }

    [Fact]
    public Task TestAddSyncAndAsyncHandlers()
    {
        return UsePipeWithMuxerAsync(async (a, b, mux) =>
        {
            _ = mux.AddHandler("/foo", asyncHandle: (p, s, c) => Task.FromResult(true));

            var selectTask = MultistreamMuxer.SelectProtoOrFailAsync("/foo", a, CancellationToken.None);

            var result = await mux.HandleAsync(b, CancellationToken.None);
            await selectTask;
            Assert.True(result);
        }, verify: true);
    }

    [Fact]
    public void TestHandleFunc()
    {
        UsePipeWithMuxer((a, b, mux) =>
        {
            _ = mux.AddHandler(new TestHandler("/a", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/b", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/c", (p, s) =>
            {
                Assert.Equal("/c", p);
                return true;
            }));

            var task = Task.Run(() => MultistreamMuxer.SelectProtoOrFail("/c", a));

            Assert.True(mux.Handle(b));
            Assert.True(task.Wait(500));
        }, verify: true);
    }

    [Fact]
    public void TestInvalidProtocol()
    {
        UsePipeWithMuxer((a, b, mux) =>
        {
            _ = Task.Run(() => mux.Negotiate(a));

            var ms = Multistream.Create(b, "/THIS_IS_WRONG");
            Assert.NotNull(ms);

            Assert.Throws<AggregateException>(() => ms.Write([], 0, 0));
        });
    }

    [Fact]
    public void TestLazyAndMux()
    {
        UsePipeWithMuxer((a, b, mux) =>
        {
            _ = mux.AddHandler(new TestHandler("/a", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/b", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/c", (p,s)=>true));

            var task = Task.Run(() =>
            {
                var selected = mux.Negotiate(a);
                Assert.NotNull(selected);
                Assert.NotNull(selected.Protocol);
                Assert.Equal("/c", selected.Protocol);

                var msg = new byte[5];
                var bytesRead = a.Read(msg, 0, msg.Length);
                Assert.Equal(bytesRead, msg.Length);
            });

            var lb = Multistream.CreateSelect(b, "/c");
            Assert.NotNull(lb);
            var outmsg = Encoding.UTF8.GetBytes("hello");
            lb.Write(outmsg, 0, outmsg.Length);

            Assert.True(task.Wait(500));

            VerifyPipe(a, lb);
        });
    }

    [Fact]
    public void TestLazyAndMuxWrite()
    {
        UsePipeWithMuxer((a, b, mux) =>
        {
            _ = mux.AddHandler("/a", (p,s)=>true);
            _ = mux.AddHandler("/b", (p,s)=>true);
            _ = mux.AddHandler("/c", (p,s)=>true);

            var doneTask = Task.Run(() =>
            {
                var selected = mux.Negotiate(a);
                Assert.NotNull(selected);
                Assert.NotNull(selected.Protocol);
                Assert.Equal("/c", selected.Protocol);

                var msg = Encoding.UTF8.GetBytes("hello");
                a.Write(msg, 0, msg.Length);
            });

            var lb = Multistream.CreateSelect(b, "/c");
            Assert.NotNull(lb);
            var msgin = new byte[5];
            var received = lb.Read(msgin, 0, msgin.Length);
            Assert.Equal(received, msgin.Length);
            Assert.Equal("hello", Encoding.UTF8.GetString(msgin));

            Assert.True(doneTask.Wait(500));

            VerifyPipe(a, lb);
        });
    }

    [Fact]
    public void TestLazyConns()
    {
        UsePipeWithMuxer((a, b, mux) =>
        {
            _ = mux.AddHandler(new TestHandler("/a", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/b", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/c", (p,s)=>true));

            var la = Task.Run(() => Multistream.CreateSelect(a, "/c"));
            var lb = Task.Run(() => Multistream.CreateSelect(b, "/c"));

            _ = Task.WhenAll(la, lb).ContinueWith(t =>
            {
                Assert.All(t.Result, r => Assert.NotNull(r));
                VerifyPipe(t.Result[0], t.Result[1]);
            });
        });
    }

    [Fact]
    public void TestLs()
    {
        SubTestLs();
        SubTestLs("a");
        SubTestLs("a", "b", "c", "d", "e");
        SubTestLs("", "a");
    }

    [Fact]
    public void TestProtocolNegotation()
    {
        UsePipeWithMuxer((a, b, mux) =>
        {
            _ = mux.AddHandler(new TestHandler("/a", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/b", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/c", (p,s)=>true));

            var negotiator = Task.Run(() => mux.Negotiate(a));

            MultistreamMuxer.SelectProtoOrFail("/a", b);

            var result = Task.WhenAny(negotiator, Task.Delay(500)).Result;
            Assert.True(result == negotiator);
            var selected = negotiator.Result;
            Assert.NotNull(selected);
            Assert.NotNull(selected.Protocol);
            Assert.Equal("/a", selected.Protocol);
        }, verify: true);
    }

    private static readonly string[] actual = ["/a", "/c"];
    private static readonly string[] actualArray = ["/a", "/b", "/c"];

    [Fact]
    public void TestRemoveProtocol()
    {
        var mux = new MultistreamMuxer();
        _ = mux.AddHandler(new TestHandler("/a", (p,s)=>true));
        _ = mux.AddHandler(new TestHandler("/b", (p,s)=>true));
        _ = mux.AddHandler(new TestHandler("/c", (p,s)=>true));

        var protos = mux.Protocols.ToList();
        protos.Sort();
        Assert.Equal(protos, actualArray);

        mux.RemoveHandler("/b");

        protos = [.. mux.Protocols];
        protos.Sort();
        Assert.Equal(protos, actual);
    }

    [Fact]
    public void TestSelectFails()
    {
        UsePipeWithMuxer((a, b, mux) =>
        {
            _ = mux.AddHandler(new TestHandler("/a", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/b", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/c", (p,s)=>true));

            _ = Task.Run(() => mux.Negotiate(a));

            _ = Assert.Throws<NotSupportedException>(() => MultistreamMuxer.SelectOneOf(["/d", "/e"], b));
        });
    }

    [Fact]
    public void TestSelectOne()
    {
        UsePipeWithMuxer((a, b, mux) =>
        {
            _ = mux.AddHandler(new TestHandler("/a", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/b", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/c", (p,s)=>true));

            var negotiator = Task.Run(() => mux.Negotiate(a));

            _ = MultistreamMuxer.SelectOneOf(["/d", "/e", "/c"], b);

            var result = Task.WhenAny(negotiator, Task.Delay(500)).Result;
            Assert.True(result == negotiator);
            var selected = negotiator.Result;
            Assert.NotNull(selected);
            Assert.NotNull(selected.Protocol);
            Assert.Equal("/c", selected.Protocol);
        }, verify: true);
    }

    [Fact]
    public void TestSelectOneAndWrite()
    {
        UsePipeWithMuxer((a, b, mux) =>
        {
            _ = mux.AddHandler(new TestHandler("/a", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/b", (p,s)=>true));
            _ = mux.AddHandler(new TestHandler("/c", (p,s)=>true));

            var task = Task.Run(() =>
            {
                var selected = mux.Negotiate(a);
                Assert.NotNull(selected);
                Assert.NotNull(selected.Protocol);
                Assert.Equal("/c", selected.Protocol);
            });

            var sel = MultistreamMuxer.SelectOneOf(["/d", "/e", "/c"], b);
            Assert.NotNull(sel);
            Assert.Equal("/c", sel);
            Assert.True(task.Wait(500));
        }, verify: true);
    }

    private static async Task RunWithConnectedNetworkStreamsAsync(Func<NetworkStream, NetworkStream, Task> func)
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        try
        {
            listener.Start(1);
            var clientEndPoint = (IPEndPoint)listener.LocalEndpoint;

            using var client = new TcpClient(clientEndPoint.AddressFamily);
            var remoteTask = listener.AcceptTcpClientAsync();
            var clientConnectTask = client.ConnectAsync(clientEndPoint.Address, clientEndPoint.Port);

            await Task.WhenAll(remoteTask, clientConnectTask);

            using var remote = remoteTask.Result;
            using var serverStream = new NetworkStream(remote.Client, true);
            using var clientStream = new NetworkStream(client.Client, true);
            await func(serverStream, clientStream);
        }
        finally
        {
            listener.Stop();
        }
    }

    private static void SubTestLs(params string[] protos)
    {
        var mr = new MultistreamMuxer();
        var mset = new HashSet<string>(protos);
        foreach (var proto in protos)
        {
            mr.AddHandler(proto, (p,s)=>true);
        }

        using var buf = new MemoryStream();
        mr.Ls(buf);
        buf.Seek(0, SeekOrigin.Begin);

        var count = Binary.Varint.Read(buf, out ulong n);
        Assert.Equal((int)n, buf.Length - count);

        Binary.Varint.Read(buf, out ulong nitems);
        Assert.Equal((int)nitems, protos.Length);

        for (var i = 0; i < (int)nitems; i++)
        {
            var token = MultistreamMuxer.ReadNextToken(buf);
            Assert.False(string.IsNullOrEmpty(token));
            Assert.Contains(token, mset);
        }

        Assert.Equal(buf.Position, buf.Length);
    }

    private static void UsePipe(Action<System.IO.Stream, System.IO.Stream> action, int timeout = 1000, bool verify = false)
    {
        RunWithConnectedNetworkStreamsAsync((a, b) =>
        {
            action(a, b);
            if (verify)
            {
                VerifyPipe(a, b);
            }
            return Task.CompletedTask;
        }).Wait();
    }

    private static async Task UsePipeAsync(Func<System.IO.Stream, System.IO.Stream, Task> action, int timeout = 1000, bool verify = false)
    {
        await RunWithConnectedNetworkStreamsAsync(async (a, b) =>
        {
            await action(a, b);
            if (verify)
            {
                await VerifyPipeAsync(a, b);
            }
        });
    }

    private static void UsePipeWithMuxer(Action<System.IO.Stream, System.IO.Stream, MultistreamMuxer> action,
        int timeout = 1000, bool verify = false) => UsePipe((a, b) => action(a, b, new MultistreamMuxer()), timeout, verify);

    private static Task UsePipeWithMuxerAsync(Func<System.IO.Stream, System.IO.Stream, MultistreamMuxer, Task> action,
        int timeout = 1000, bool verify = false) => UsePipeAsync((a, b) => action(a, b, new MultistreamMuxer()), timeout, verify);

    private static void VerifyPipe(System.IO.Stream a, System.IO.Stream b)
    {
        var mes = new byte[1024];
        new Random().NextBytes(mes);
        Task.Run(() =>
        {
            a.Write(mes, 0, mes.Length);
            b.Write(mes, 0, mes.Length);
        }).Wait(500);

        var buf = new byte[mes.Length];
        var n = a.Read(buf, 0, buf.Length);
        Assert.Equal(n, buf.Length);
        Assert.Equal(buf, mes);

        n = b.Read(buf, 0, buf.Length);
        Assert.Equal(n, buf.Length);
        Assert.Equal(buf, mes);
    }

    private static async Task VerifyPipeAsync(System.IO.Stream a, System.IO.Stream b)
    {
        var mes = new byte[1024];
        new Random().NextBytes(mes);
        mes[0] = 0x01;

        var aw = a.WriteAsync(mes, 0, mes.Length);
        var bw = b.WriteAsync(mes, 0, mes.Length);

        await Task.WhenAll(aw, bw).ConfigureAwait(false);

        var buf = new byte[mes.Length];
        var n = await a.ReadAsync(buf).ConfigureAwait(false);
        Assert.Equal(n, buf.Length);
        Assert.Equal(buf, mes);

        n = await b.ReadAsync(buf).ConfigureAwait(false);
        Assert.Equal(n, buf.Length);
        Assert.Equal(buf, mes);
    }

    private class TestHandler(string protocol, Func<string, System.IO.Stream, bool> action) : IMultistreamHandler
    {
        private readonly Func<string, System.IO.Stream, bool> _action = action ?? throw new ArgumentNullException(nameof(action));

        public string Protocol { get; } = protocol ?? throw new ArgumentNullException(nameof(protocol));

        public bool Handle(string protocol, System.IO.Stream stream)
        {
            ArgumentNullException.ThrowIfNull(protocol);
            ArgumentNullException.ThrowIfNull(stream);
            return _action(protocol, stream);
        }

        public Task<bool> HandleAsync(string protocol, System.IO.Stream stream, CancellationToken cancellationToken) =>
            Task.Run(() => Handle(protocol, stream), cancellationToken);
    }
}
