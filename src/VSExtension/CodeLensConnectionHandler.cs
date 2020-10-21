#nullable enable

namespace Microscope.VSExtension {
    using System;
    using System.Diagnostics;
    using System.IO.Pipes;
    using System.Linq;
    using System.Threading.Tasks;

    using Microscope.CodeAnalysis.Model;
    using Microscope.Shared;

    using Mono.Cecil.Cil;
    using Mono.Collections.Generic;

    using StreamJsonRpc;

    using static Microscope.Shared.Logging;

    using CodeLensConnections = System.Collections.Concurrent.ConcurrentDictionary<System.Guid, CodeLensConnectionHandler>;
    using CodeLensDetails = System.Collections.Concurrent.ConcurrentDictionary<System.Guid, CodeAnalysis.Model.DetailsData>;

    public class CodeLensConnectionHandler : IRemoteVisualStudio, IDisposable {
        private static readonly CodeLensConnections connections = new CodeLensConnections();
        private static readonly CodeLensDetails detailsData = new CodeLensDetails();

        private JsonRpc? rpc;
        private Guid? dataPointId;

        public static async Task AcceptCodeLensConnections() {
            try {
                while (true) {
                    var stream = new NamedPipeServerStream(
                        PipeName.Get(Process.GetCurrentProcess().Id),
                        PipeDirection.InOut,
                        NamedPipeServerStream.MaxAllowedServerInstances,
                        PipeTransmissionMode.Byte,
                        PipeOptions.Asynchronous);
                    await stream.WaitForConnectionAsync().Caf();
                    _ = HandleConnection(stream);
                }
            } catch (Exception ex) {
                LogVS(ex);
                throw;
            }

            static async Task HandleConnection(NamedPipeServerStream stream) {
                try {
                    var handler = new CodeLensConnectionHandler();
                    var rpc = JsonRpc.Attach(stream, handler);
                    handler.rpc = rpc;
                    await rpc.Completion;
                    handler.Dispose();
                    stream.Dispose();
                } catch (Exception ex) {
                    LogVS(ex);
                }
            }
        }

        public void Dispose() {
            if (dataPointId.HasValue) {
                _ = connections.TryRemove(dataPointId.Value, out var _);
                _ = detailsData.TryRemove(dataPointId.Value, out var _);
            }
        }

        // Called from each CodeLensDataPoint via JSON RPC.
        public void RegisterCodeLensDataPoint(Guid id) {
            dataPointId = id;
            connections[id] = this;
        }

        public static void StoreDetailsData(Guid id, DetailsData details) => detailsData[id] = details;

        public static DetailsData GetDetailsData(Guid id) => detailsData[id];

        public static async Task RefreshCodeLensDataPoint(Guid id) {
            if (!connections.TryGetValue(id, out var conn))
                throw new InvalidOperationException($"CodeLens data point {id} was not registered.");

            Debug.Assert(conn.rpc != null);
            await conn.rpc!.InvokeAsync(nameof(IRemoteCodeLens.Refresh)).Caf();
        }

        public static async Task RefreshAllCodeLensDataPoints()
            => await Task.WhenAll(connections.Keys.Select(RefreshCodeLensDataPoint)).Caf();
    }
}
