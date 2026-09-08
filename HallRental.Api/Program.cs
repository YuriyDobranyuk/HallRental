using FluentValidation;
using FluentValidation.AspNetCore;
using HallRental.Api.Configuration;
using HallRental.Api.Middlewares;
using HallRental.BL;
using HallRental.BL.DTOs.Hall;

var builder = WebApplication.CreateBuilder(args);

SerilogConfiguration.ConfigureSerilog(builder);

builder.Services.AddControllers();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssembly(typeof(HallRequest).Assembly);
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddApplicationInjection(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

await app.RunAsync();
