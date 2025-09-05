using MemberRewardApproval.WebApi.Data;
using MemberRewardApproval.WebApi.Hubs;
using MemberRewardApproval.WebApi.Options;
using MemberRewardApproval.WebApi.Services;
using MemberRewardApproval.WebApi.Services.Bots;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

var clientUrl = builder.Configuration["Angular:ClientUrl"];

// --- DbContext ---
builder.Services.AddDbContext<RewardsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RewardsDb")));

// --- Application services ---
builder.Services.AddScoped<SequenceService>();
builder.Services.AddScoped<RewardService>();
builder.Services.AddScoped<ConversationReferenceService>();

// --- Options ---
builder.Services.Configure<AzureAdOptions>(builder.Configuration.GetSection("AzureAd"));
builder.Services.Configure<BotOptions>(builder.Configuration.GetSection("Bot"));

// --- Bot-related services ---
builder.Services.AddSingleton<GraphUserService>();

builder.Services.AddSingleton<CloudAdapter>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var botConfiguration = config.GetSection("Bot");
    return new CloudAdapter(new ConfigurationBotFrameworkAuthentication(botConfiguration));
});
builder.Services.AddScoped<IBot, RewardBot>();
builder.Services.AddScoped<INotificationService, BotNotificationService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
    {
        builder.Configuration.Bind("AzureAuth", options);
        options.Events = new JwtBearerEvents();
    }, options => { builder.Configuration.Bind("AzureAuth", options); });


// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(clientUrl)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// --- SignalR ---
builder.Services.AddSignalR();

builder.Services.AddControllers();
// API Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}
app.UseCors();

app.UseStaticFiles();

app.UseRouting();
// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapHub<RewardHub>("/rewardHub");
app.MapHub<RequestHub>("/hubs/request");

app.Run();

