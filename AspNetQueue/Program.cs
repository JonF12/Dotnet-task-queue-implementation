using AspNetQueue.Services;
using AspNetQueue.Services.JobQueue;
using AspNetQueue.Services.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<QueuedHostedService>();
builder.Services.AddSingleton<IJobQueue, JobQueue>();
builder.Services.AddScoped<FailingJob>();
builder.Services.AddScoped<SuccessfulJob>();
builder.Services.AddScoped<LongRunningJob>();
builder.Services.AddScoped<IGenericService, GenericService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
