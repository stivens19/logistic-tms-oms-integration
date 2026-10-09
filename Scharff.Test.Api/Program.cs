using Scharff.Test.Api.Extensions;
using Scharff.Test.Application.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new CustomDateTimeConverter());
    });

builder.Services.AddHttpClient();
builder.Services.AddProjectServices(builder.Configuration);
builder.Services.AddSwaggerConfig();
builder.Services.AddJwtConfig(builder.Configuration);
builder.Services.AddRateLimiterConfig();

var app = builder.Build();


app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Scharff Integration API v1");
    });
}

app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();