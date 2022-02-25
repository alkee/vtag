using vtagSvc;
using vtagSvc.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682



// Add services to the container.
builder.Services
    .AddGrpcReflection() // https://martinbjorkstrom.com/posts/2020-07-08-grpc-reflection-in-net
    .AddGrpc() // IGrpcServiceBuilder 를 return 하므로 chain 의 마지막에 위치
    ;

// singleton for dependency injection
var dbFilePath = Path.Combine(Path.GetDirectoryName(typeof(SqliteNet).Assembly.Location) ?? "", "vtag.db");
builder.Services
    .AddSingleton<SqliteNet>(new SqliteNet(dbFilePath))
    ;


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService(); // [grpcui](https://github.com/fullstorydev/grpcui) 등으로 쉽게 테스트 할 수 있도록
}
app.MapGrpcService<FileSystemService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
