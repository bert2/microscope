#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.Diagnostics;
    using System.IO.Pipes;
    using System.Threading.Tasks;

    using Microscope.Shared;

    using StreamJsonRpc;

    using static Microscope.Shared.Logging;

    using CodeLensConnections = System.Collections.Concurrent.ConcurrentDictionary<System.Guid, CodeLensConnectionHandler>;

    public class CodeLensConnectionHandler : IRemoteVisualStudio, IDisposable {
        private static readonly CodeLensConnections connections = new CodeLensConnections();

        private JsonRpc? rpc;
        private Guid? dataPointId;

        public static async Task AcceptCodeLensConnections() {
            try {
                while (true) {
                    var stream = new NamedPipeServerStream(
                        Constants.MicroscopePipe,
                        PipeDirection.InOut,
                        NamedPipeServerStream.MaxAllowedServerInstances,
                        PipeTransmissionMode.Byte,
                        PipeOptions.Asynchronous);
                    await stream.WaitForConnectionAsync().Caf();
                    _ = HandleConnection(stream);
                }
            } catch (Exception ex) {
                Log(ex);
                throw;
            }

            static async Task HandleConnection(NamedPipeServerStream stream) {
                try {
                    var handler = new CodeLensConnectionHandler();
                    var rpc = JsonRpc.Attach(stream, handler);
                    handler.rpc = rpc;
                    await rpc.Completion;
                    handler.Dispose();
                } catch (Exception ex) {
                    Log(ex);
                }
            }
        }

        public void RegisterCodeLensDataPoint(Guid id) {
            dataPointId = id;
            connections[id] = this;
            Log($"CodeLens data point {id} was registered.");
        }

        public static async Task RefreshCodeLensDataPoint(Guid id) {
            if (!connections.TryGetValue(id, out var conn))
                throw new InvalidOperationException($"CodeLens data point {id} was not registered.");

            Debug.Assert(conn.rpc != null);
            await conn.rpc!.InvokeAsync(nameof(IRemoteCodeLens.Refresh)).Caf();
        }

        public void Dispose() {
            if (dataPointId.HasValue)
                _ = connections.TryRemove(dataPointId.Value, out var _);
            Log($"CodeLens data point {dataPointId} was disposed.");
        }
    }
}
