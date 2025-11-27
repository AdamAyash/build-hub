using Scalar.AspNetCore;
using BuildHub.Common.Logger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

//Initialization
Logger.Initialize();
Logger.LogInformation("Application Starting Up");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference("/api-docs", options =>

	{
		options.Title = "BuildHub API";
		options.Theme = ScalarTheme.Saturn;
		options.HideClientButton = true;
	});
}
app.MapGet("/", () => Results.Redirect("api-docs")).ExcludeFromDescription();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
