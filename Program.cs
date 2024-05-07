using Authentication.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// Adding authorize header in swagger
builder.Services.AddSwaggerGen(options =>
{
    // c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
    options.OperationFilter<CookieHeaderFilter>();//register the cookie entry for testing purpose - check APIs
});

//add dbcontext 
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//add authentication and identity API endpoints 
builder.Services.AddAuthentication();
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<DataContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(options => {
    options
    .AllowAnyOrigin()
    .AllowAnyHeader() 
    .AllowAnyMethod();
    }
);

//map Identity APIs 
app.MapIdentityApi<IdentityUser>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();





















//Package Manager Console
//dotnet ef migrations add Initial
//dotnet ef  database update

//Nuget Packages
//Microsoft.AspNetCore.Identity.EntityFrameworkCore - IdentityDbContext
//Microsoft.EntityFrameworkCore
//Microsoft.EntityFrameworkCore.Design - gives the EF tools to run ef commands
//Microsoft.EntityFrameworkCore.SqlServer - AddDbContext - UseSqlServer()...