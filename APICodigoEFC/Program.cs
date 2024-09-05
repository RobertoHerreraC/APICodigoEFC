using APICodigoEFC.Models;
using APICodigoEFC.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);




builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<CodigoContext>(
    options =>
    {
        //options.UseSqlServer("Data Source=DESKTOP-BCQFL9J\\SQLEXPRESS;" +
        //    "Initial Catalog=CodigoDB;User ID=userCodigo;Pwd=123456;" +
        //    "TrustServerCertificate=True");

        //options.UseSqlServer("Server=tcp:codigoserverdatabase.database.windows.net,1433;Initial Catalog=codigoDB;Persist Security Info=False;" +
        //    "User ID=usercodigo;Password=Julio2024.;" +
        //    "MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

        options.UseSqlServer("Server=LAPTOP-1BTKKPN9\\SQLEXPRESS; Initial Catalog=DistribuidoDb; Integrated Security=True;trustservercertificate=True");


    });


var app = builder.Build();



// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/security/getMessage", () => "Hello World!").RequireAuthorization();

app.MapPost("/security/createToken",
[AllowAnonymous] (User user) =>
{
    // Validar la existencia del usuario
    var userValidationResult = ValidationHelper.GetRole(user.UserName, user.Password);

    if (userValidationResult.IsValid)
    {

        var issuer = builder.Configuration["Jwt:Issuer"];
        var audience = builder.Configuration["Jwt:Audience"];
        var key = Encoding.ASCII.GetBytes
        (builder.Configuration["Jwt:Key"]);


        var tokenDescriptor = new SecurityTokenDescriptor
        {
            //Lo que va guardar el token
            //Claim: Información Adicional que guarda el token
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, userValidationResult.Role)
             }),

            Expires = DateTime.UtcNow.AddMinutes(5),//Tiempo duración            
            Issuer = issuer,//Quien creo el token
            Audience = audience,//Para quien se creo el token
            SigningCredentials = new SigningCredentials //Tipo de encriptación
            (new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha512Signature)
        };


        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var stringToken = tokenHandler.WriteToken(token);
        return Results.Ok(stringToken);
    }

    return Results.Unauthorized();
});


app.Run();
