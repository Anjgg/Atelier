using Atelier.Api._Data;
using Atelier.Api._DTOs;
using Atelier.Api._Entities;
using Atelier.Api._Extensions;
using Atelier.Api._Middleware;
using Atelier.Api._Options;
using Atelier.Api._Swagger;
using Atelier.Api.Calculator;
using Atelier.Api.Helpers;
using Atelier.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDatabase()
    .AddApplicationServices()
    .AddControllersWithConfig()
    .AddSwaggerWithJwt()
    .AddJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.InitializeDatabase();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

