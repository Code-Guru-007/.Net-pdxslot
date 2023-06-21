using AutoWrapper;
using Azure.Storage.Blobs;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PdxSlots.API.Common;
using PdxSlots.API.Common.Firebase;
using PdxSlots.API.Data;
using PdxSlots.API.Services;
using PdxSlots.API.Services.Interfaces;
using PdxSlots.IGPClient.Services;
using SendGrid.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
    builder =>
    {
        builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    }));

// Add services to the container.

builder.Services.AddControllers()
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

builder.Services.AddDbContext<PdxSlotsContext>(
       options => options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));

builder.Services.AddDetection();

var firebaseProjectId = builder.Configuration["FirebaseAuth:ProjectId"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://securetoken.google.com/{firebaseProjectId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{firebaseProjectId}",
            ValidateAudience = true,
            ValidAudience = firebaseProjectId,
            ValidateLifetime = true
        };
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin",
        policy => policy.RequireClaim(Constants.Firebase_Claims_IsAdmin, "true"));
});


// Automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Services
builder.Services.AddScoped<IGamesService, GameService>();
builder.Services.AddScoped<IRoundService, RoundService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFirebaseClient, FirebaseClient>();
builder.Services.AddScoped<IGameMathsService, GameMathsService>();
builder.Services.AddScoped<IOperatorService, OperatorService>();
builder.Services.AddScoped<IAzureService, AzureService>();
builder.Services.AddScoped<IIgcUserGafService, IgcUserGafService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IZipFileUploadService, ZipFileUploadService>();
builder.Services.AddScoped<IPeriodicJobService, PeriodicJobService>();

// external services
builder.Services.AddScoped<IGCPClientService, GCPClientService>();

builder.Services.AddScoped<BlobServiceClient>(x =>
{
    return new BlobServiceClient(builder.Configuration["AzureBlob:ConnectionString"]);
});

var sendGridApiKey = builder.Configuration["SendGrid:ApiKey"];

builder.Services.AddSendGrid(options =>
{
    options.ApiKey = sendGridApiKey;
});

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<PdxSlots.API.Common.Configurations.GameConfiguration>(builder.Configuration.GetSection("GameConfiguration"));
builder.Services.Configure<PdxSlots.API.Common.Configurations.PdxConfiguration>(builder.Configuration.GetSection("PdxConfiguration"));

// Configure Firebase
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.GetApplicationDefault(),
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseDetection();

app.UseApiResponseAndExceptionWrapper(new AutoWrapperOptions { IsDebug = app.Environment.IsDevelopment(), ShowStatusCode = true });

app.UseAuthentication();

app.UseCors(cors => cors.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseAuthorization();

app.MapControllers();

app.Run();
