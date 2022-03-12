using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace vtagSvc.Services
{
    public class FileSystemService
        : FileSystem.FileSystemBase
    {
        private readonly ILogger<FileSystemService> _logger;

        public FileSystemService(ILogger<FileSystemService> logger)
        {
            _logger = logger;
        }

        public override Task<VersionRsp> Version(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new VersionRsp
            {
                Version = GetType().Assembly.GetName().Version?.ToString() ?? "0.0.0.0"
            });
        }
    }
}