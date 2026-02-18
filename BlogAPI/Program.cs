using BlogAPI.Data;
using BlogAPI.Entities;
using BlogAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

/**
 * ========================================================================
 * ========================================================================
 *                              SERVICES AREA
 * ========================================================================
 * ========================================================================
 */

var allowedURLS = builder.Configuration.GetSection("AllowedURLS").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(cors =>
    {
        cors.WithOrigins(allowedURLS!).AllowAnyMethod().AllowAnyHeader();
    });
});
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer("name=DefaultConnection"));

/**
 * ========================================================================
 * ========================================================================
 *                              AUTH & AUTORIZE CONFIG
 * ========================================================================
 * ========================================================================
 */

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<SignInManager<User>>();
builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication().AddJwtBearer(o =>
{
    o.MapInboundClaims = false;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["keyjwt"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("admin", p => p.RequireClaim("admin"));
});




/**
 * ========================================================================
 * ========================================================================
 *                              MIDDLEWARERS AREA
 * ========================================================================
 * ========================================================================
 */

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
    app.UseSwagger();
}
app.UseCors();
app.MapControllers();
app.Run();
    