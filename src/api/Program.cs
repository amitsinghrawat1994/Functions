using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// builder.Services.AddAuthentication(option =>
// {
//     option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// }).AddJwtBearer(option =>
// {
//     option.Authority = config["Auth0:Domain"];
//     option.Audience = config["Auth0:Audience"];
//     option.TokenValidationParameters = new TokenValidationParameters
//     {
//         NameClaimType = "name"
//     };

//     option.RequireHttpsMetadata = false;
// });

// Configure Auth0 authentication
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
//         options.Audience = builder.Configuration["Auth0:Audience"];
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ClockSkew = TimeSpan.Zero // Default: 5 min
//         };
//     });

// Configure JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.Zero, // Default: 5 min
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });

// Configure Authorization policies
builder.Services.AddAuthorization(options =>
{
    // Default policy requires authentication
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    // Admin policy
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("https://schemas.auth0.com/roles", "admin"));

    // Specific role requirements
    options.AddPolicy("HasRole", policy =>
        policy.RequireClaim("https://schemas.auth0.com/roles"));
});

// builder.Services.AddAuthentication();
// builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddCors(o => o.AddPolicy("AllowLocalAngular", policy =>
    policy.WithOrigins("http://localhost:4200")
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowLocalAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();
