using Grpc.Core;

namespace ShopFlow.ProductService.UnitTests;

public static class ServerCallContextMock
{
    public static ServerCallContext Create()
    {
        return TestServerCallContext.Create(
            method: "test",
            host: "localhost",
            deadline: DateTime.UtcNow.AddMinutes(1),
            requestHeaders: new Metadata(),
            cancellationToken: CancellationToken.None,
            peer: "127.0.0.1",
            authContext: null,
            contextPropagationToken: null,
            writeHeadersFunc: _ => Task.CompletedTask,
            writeOptionsGetter: () => new WriteOptions(),
            writeOptionsSetter: _ => { }
        );
    }

    private sealed class TestServerCallContext : ServerCallContext
    {
        public static TestServerCallContext Create(
            string method,
            string host,
            DateTime deadline,
            Metadata requestHeaders,
            CancellationToken cancellationToken,
            string peer,
            AuthContext? authContext,
            ContextPropagationToken? contextPropagationToken,
            Func<Metadata, Task> writeHeadersFunc,
            Func<WriteOptions?> writeOptionsGetter,
            Action<WriteOptions?> writeOptionsSetter)
        {
            return new TestServerCallContext(
                method,
                host,
                deadline,
                requestHeaders,
                cancellationToken,
                peer,
                authContext,
                contextPropagationToken,
                writeHeadersFunc,
                writeOptionsGetter,
                writeOptionsSetter);
        }

        private TestServerCallContext(
            string method,
            string host,
            DateTime deadline,
            Metadata requestHeaders,
            CancellationToken cancellationToken,
            string peer,
            AuthContext? authContext,
            ContextPropagationToken? contextPropagationToken,
            Func<Metadata, Task> writeHeadersFunc,
            Func<WriteOptions?> writeOptionsGetter,
            Action<WriteOptions?> writeOptionsSetter)
        {
            MethodCore = method;
            HostCore = host;
            DeadlineCore = deadline;
            RequestHeadersCore = requestHeaders;
            CancellationTokenCore = cancellationToken;
            PeerCore = peer;
            AuthContextCore = authContext ?? new AuthContext(string.Empty, new Dictionary<string, List<AuthProperty>>());
            ContextPropagationTokenCore = contextPropagationToken;
            WriteHeadersFunc = writeHeadersFunc;
            WriteOptionsGetter = writeOptionsGetter;
            WriteOptionsSetter = writeOptionsSetter;
        }

        protected override string MethodCore { get; }
        protected override string HostCore { get; }
        protected override string PeerCore { get; }
        protected override DateTime DeadlineCore { get; }
        protected override Metadata RequestHeadersCore { get; }
        protected override CancellationToken CancellationTokenCore { get; }
        protected override Metadata ResponseTrailersCore { get; } = new();
        protected override Status StatusCore { get; set; }
        protected override WriteOptions? WriteOptionsCore
        {
            get => WriteOptionsGetter();
            set => WriteOptionsSetter(value);
        }

        protected override AuthContext AuthContextCore { get; }

        protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions? options)
            => ContextPropagationTokenCore!;

        private ContextPropagationToken? ContextPropagationTokenCore { get; }

        private Func<Metadata, Task> WriteHeadersFunc { get; }
        private Func<WriteOptions?> WriteOptionsGetter { get; }
        private Action<WriteOptions?> WriteOptionsSetter { get; }

        protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders)
            => WriteHeadersFunc(responseHeaders);
    }
}