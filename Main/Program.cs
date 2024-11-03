using System.Text.Json;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Communication;
using Communication.Implementation;
using Main;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using PresentationServices;
using PresentationServices.Implementation;
using Repositories;
using Repositories.Implementation;
using Services.BusinessLogic;
using Services.BusinessLogic.Implementation;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});


builder.Services.AddDbContext<TaskManagerDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("TaskManagerDbConnection")));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAzureClients(configureClients =>
{
    configureClients.AddServiceBusClient(builder.Configuration.GetConnectionString("TaskManagerServiceBusConnection"))
        .ConfigureOptions(o => o.RetryOptions.MaxRetries = 3);
});

builder.Services.AddScoped<IUserTaskRepository, UserTaskRepository>();
builder.Services.AddScoped<IUserTaskService, UserTaskService>();

builder.Services.AddSingleton<IUserTaskSenderService, UserTaskSenderService>();
builder.Services.AddSingleton<UserTaskReceiverService>();
builder.Services.AddScoped<IUserTaskPresentationService, UserTaskPresentationService>();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();

}).AddApiExplorer(
    options =>
    {
        // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
        // note: the specified format code will format the version as "'v'major[.minor][-status]"
        options.GroupNameFormat = "'v'VVV";

        // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
        // can also be used to control the format of the API version in route templates
        options.SubstituteApiVersionInUrl = true;
    } );

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }


app.UseHttpsRedirection();

app.UseApiExceptionHandler();

app.MapControllers();

await app.Services.GetService<UserTaskReceiverService>()!.Start();

app.Run();

