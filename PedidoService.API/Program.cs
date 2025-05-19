using PedidoService.API.Configuration;
using PedidoService.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<RabbitMQConfig>(
    builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<IMessageBusService, RabbitMQService>();
builder.Services.AddScoped<IPedidoService, PedidoService.API.Services.PedidoService>();
builder.Services.AddScoped<IPagamentoService, PagamentoService>();
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