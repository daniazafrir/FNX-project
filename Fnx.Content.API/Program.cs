using Fnx.Content.Models;
using Fnx.Content.Services.Auth;
using Fnx.Content.Services.BL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IContentBL, ContentBL>();
builder.Services.AddHttpClient<IContentBL, ContentBL>();
builder.Services.AddSwaggerGen();

//Jwt configuration starts here
var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = jwtIssuer,
         ValidAudience = jwtIssuer,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
     };
 });
//Jwt configuration ends here

builder.Services.AddSwaggerGen(config =>
{
    //config.SwaggerDoc("v1", info);
});

builder.Services.AddCors(policyBuilder =>
    policyBuilder.AddDefaultPolicy(policy =>
        policy.WithOrigins("*").AllowAnyHeader().AllowAnyHeader())
);
var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGet("/api/search",
    async (IContentBL bl, [FromQuery] string? searchKey, [FromQuery] int? take, [FromQuery] int? skip) =>
    {
        var res = await bl.GetGitItems(searchKey, take, skip);
        return res;
    }).WithMetadata(new SwaggerOperationAttribute(summary: "Summary", description: "Descritption Test"))
    .Produces<List<GitRepoItem>>()
    .WithName("GetGitItems")
    .RequireAuthorization();

app.MapGet("/api/auth",
     () =>
    {       
        var jwt = new JwtService(jwtKey, jwtIssuer);        
        var token = jwt.GenerateToken();
        return Results.Ok(new { Token = token });
    })   
    .WithName("GenerateToken");

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Fnx.Content.API v1");
    });
}

app.Run();
