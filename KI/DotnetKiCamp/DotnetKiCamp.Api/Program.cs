using DotnetKiCamp;

[module:Dapper.DapperAot]

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.AddCors();
builder.Services.AddOpenAIClient(builder.Configuration.GetSection("OpenAI"));
builder.Services.AddSqlConnection(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddTransient<IStreamProcessor, StreamProcessor>();
builder.Services.AddSingleton<IChatManager, ChatManager>();
builder.Services.AddTransient<IAiFunctions, AiFunctions>();
builder.Services.ConfigureJsonOptions([
    ChatCompletionsApiSerializerContext.Default,
]);

var app = builder.Build();
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.MapGet("/", () => "Hello World!");
app.MapChatCompletions();

app.Run();
