using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Tickets_selling_App;
using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddDbContext<Tkt_Dbcontext>(options =>
    options.UseSqlServer(Configuration.GetConnectionString("TicketSelling_Conection")));

// Configure Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AppSettings:Token"])),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    options.AddPolicy("EveryRole", policy => policy.RequireAssertion(context =>
        context.User.IsInRole("Creator") ||
        context.User.IsInRole("Moderator") ||
        context.User.IsInRole("User")
    ));
    options.AddPolicy("ModeratorOnly", policy => policy.RequireRole("Moderator"));
    options.AddPolicy("CreatorOnly", policy => policy.RequireRole("Creator"));
});

// Add application services
builder.Services.AddScoped<AdminInterface, AdminService>();
builder.Services.AddScoped<UserInterface, UserServicre>();
builder.Services.AddScoped<GmailInterface, GmailService>();
builder.Services.AddScoped<NotauthorisedInterface, NotauthorisedServices>();
builder.Services.AddScoped<CreatorInterface, CreatorService>();

// Configure CORS to allow requests from any origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
});

// Add controllers
builder.Services.AddControllers();

// Build the application
var app = builder.Build();

// Apply CORS policy before any other middleware
app.UseCors("AllowAllOrigins");

app.UseRouting();

// Enable Swagger UI in development
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1");
    });
}

// Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Run the application
app.Run();

