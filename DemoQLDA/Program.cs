using DemoQLDA.Hubs;
using Libs;
using Libs.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using DemoQLDA.Configurations;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Libs.Repositories;
using Libs.UnitOfWork;
using Libs.Entity;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    //options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

}, ServiceLifetime  .Transient);

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value);
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
    ValidateAudience = false,
    RequireExpirationTime = false,
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero,
};
builder.Services.AddSingleton(tokenValidationParameters);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwt =>
{
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = tokenValidationParameters;
});

builder.Services.AddIdentity<User, IdentityRole>(options =>
                                options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TodoApp", Version = "v1" });
    c.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme.ToLowerInvariant(),
        In = ParameterLocation.Header,
        Name = "Authorization",
        BearerFormat = "JWT",
        Description = "JWT Authorization  header using the Bearer scheme."
    });
    //c.OperationFilter<AuthResponsesOperationFilter>();
});

builder.Services.AddCors(options => {
    options.AddPolicy("Open", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DepartmentPolicy",
        policy => {
            /*policy.RequireClaim("department")*/;
            policy.RequireRole("AppUser");
        }); 
});
builder.Services.AddTransient<ChessService>();
builder.Services.AddTransient<UserService>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");
app.MapHub<ChatHub>("/chatHub");
app.Run();
