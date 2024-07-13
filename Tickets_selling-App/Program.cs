using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Tickets_selling_App;
using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Services;
using Hangfire;
using Hangfire.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<Tkt_Dbcontext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TicketSelling_Conection")));

builder.Services.AddAuthentication(x =>
{
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x=>
{

});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<Admin_Interface, AdminService>();
builder.Services.AddScoped<User_Interface, UserServicre>();
builder.Services.AddScoped<Gmail_Interface, GmailService>();
builder.Services.AddScoped<Login_Registration_Interface, Login_Registration_Service>();
builder.Services.AddScoped<BackgroundServices>();
// Add services to the container.
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("TicketSelling_Conection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(1),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(1),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer();

builder.Services.AddScoped<GmailService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularLocalhost",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200/")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowAnyOrigin();
        });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
});
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHangfireDashboard();

// Schedule the recurring job
using (var scope = app.Services.CreateScope())
{
    var backgroundServices = scope.ServiceProvider.GetRequiredService<BackgroundServices>();
    RecurringJob.AddOrUpdate(() => backgroundServices.DeleteExpiredEmailValidations(), Cron.Minutely);
}


app.UseCors("AllowLocalhost");
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Photos")),
    RequestPath = "/Photos"
});


app.UseCors(options =>
  options.WithOrigins("http://localhost:7081")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowAnyOrigin()
    );
builder.Services.AddCors();
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
