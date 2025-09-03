using MemberRewardApproval.WebApi.Data;
using MemberRewardApproval.WebApi.Hubs;
using MemberRewardApproval.WebApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var clientUrl = builder.Configuration["Angular:ClientUrl"];

builder.Services.AddDbContext<RewardsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RewardsDb")));
builder.Services.AddScoped<SequenceService>();
builder.Services.AddScoped<RewardService>();
builder.Services.AddScoped<INotificationService, WebNotificationService>();

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

builder.Services.AddSignalR();

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors();

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.MapHub<RewardHub>("/rewardHub");
app.MapHub<RequestHub>("/hubs/request");

app.Run();

