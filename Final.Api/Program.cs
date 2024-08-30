using Final.Api.Middlewares;
using Final.Application.Profiles;
using Final.Data.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(opt =>
    {
        opt.InvalidModelStateResponseFactory = context =>
        {
            // Aggregate all errors per field
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            // Customize the response object
            var responseObj = new
            {
                message = "One or more validation errors occurred.",
                errors = errors
            };

            return new BadRequestObjectResult(responseObj);
        };
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<FinalDbContext>(opt =>
{
    opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(opt =>
{
    opt.AddProfile(new MapperProfile(new HttpContextAccessor()));
});

var app = builder.Build();

//builder.Services.AddIdentity<User, IdentityRole>(options =>
//{
//    options.Password = new()
//    {
//        RequiredLength = 8,
//        RequireUppercase = true,
//        RequireLowercase = true,
//        RequireDigit = true,
//        RequireNonAlphanumeric = true


//    };
//    options.Lockout = new()
//    {
//        MaxFailedAccessAttempts = 5,
//        AllowedForNewUsers = true,
//        DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5)
//    };
//    options.User = new()
//    {
//        //todo:email confirm sondurmusen
//        RequireUniqueEmail = true,
//    };
//    //options.SignIn.RequireConfirmedEmail = true;

//}).AddDefaultTokenProviders().AddEntityFrameworkStores<FinalDbContext>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();
app.Run();
