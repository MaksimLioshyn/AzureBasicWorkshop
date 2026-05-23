using WebAppGateway.Messaging;
using WebAppGateway.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MessagingOptions>(builder.Configuration.GetSection(MessagingOptions.SectionName));

builder.Services.AddSingleton<IRetryPolicyExecutor, RetryPolicyExecutor>();
builder.Services.AddSingleton<IServiceBusPublisher, ServiceBusPublisher>();
builder.Services.AddSingleton<IEventHubPublisher, EventHubPublisher>();
builder.Services.AddSingleton<IEventGridPublisher, EventGridPublisher>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
