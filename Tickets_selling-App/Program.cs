using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
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
    options.UseSqlServer(builder.Configuration.GetConnectionString("TicketSelling_Conection")));

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
    //User roles 
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));

    //Creator Roles
    options.AddPolicy("EveryRole", policy => policy.RequireAssertion(context =>
        context.User.IsInRole("Creator") ||
        context.User.IsInRole("Moderator") ||
        context.User.IsInRole("CreatorAdmin")
    ));
    options.AddPolicy("CreatorAdminOnly", policy => policy.RequireRole("CreatorAdmin"));
    options.AddPolicy("ModeratorOnly", policy => policy.RequireRole("Moderator"));
    options.AddPolicy("CreatorOnly", policy => policy.RequireRole("Creator"));
});


// Add application services
builder.Services.AddScoped<Admin_Interface, AdminService>();
builder.Services.AddScoped<User_Interface, UserServicre>();
builder.Services.AddScoped<Gmail_Interface, GmailService>();
builder.Services.AddScoped<NotauthorisedInterface,NotauthorisedServices>();
builder.Services.AddScoped<CreatorInterface,CreatorService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowAnyOrigin();
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

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Photos")),
    RequestPath = "/Photos"
});

// Apply CORS policies
app.UseCors("AllowAngularLocalhost");
app.UseCors(options =>
{
    options.WithOrigins("http://localhost:7081")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin();
});

// Use Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Run the application
app.Run();
