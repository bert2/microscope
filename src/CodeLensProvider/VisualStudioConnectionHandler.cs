#nullable enable

namespace Microscope.CodeLensProvider {
    using System;
    using System.IO.Pipes;
    using System.Threading.Tasks;

    using Microscope.Shared;

    using StreamJsonRpc;

    public class VisualStudioConnectionHandler : IRemoteCodeLens, IDisposable {
        private readonly NamedPipeClientStream stream = new NamedPipeClientStream(
            serverName: ".",
            Constants.MicroscopePipe,
            PipeDirection.InOut,
            PipeOptions.Asynchronous);
        private readonly CodeLensDataPoint owner;
        private JsonRpc? rpc;

        public async static Task<VisualStudioConnectionHandler> Create(CodeLensDataPoint owner) {
            var handler = new VisualStudioConnectionHandler(owner);
            await handler.Connect().Caf();
            return handler;
        }

        public VisualStudioConnectionHandler(CodeLensDataPoint owner) => this.owner = owner;

        public void Dispose() => stream.Dispose();

        public async Task Connect() {
            await stream.ConnectAsync().Caf();
            rpc = JsonRpc.Attach(stream, this);
            await rpc.InvokeAsync(nameof(IRemoteVisualStudio.RegisterCodeLensDataPoint), owner.id).Caf();
        }

        public void Refresh() => owner.Refresh();
    }
}
