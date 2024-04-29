using ApplicationDev.Common.Database.DatabaseContext;
using ApplicationDev.Common.Middleware.Errors;
using ApplicationDev.Modules.User.Repos;
using ApplicationDev.Modules.User.Services;
using ApplicationDev.Modules.Authentication.Services;
using ApplicationDev.Common.Middleware.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MyAppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AppConnectionString")));

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => //swaggerGen method takes a configuration action that makes it possible to customize the behavior of the swagger generator
{
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		In = ParameterLocation.Header,
		Description = "Please enter token",
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		BearerFormat = "JWT",
		Scheme = "bearer"
	});

	//Swagger document for Admin APIs
	c.SwaggerDoc("admin", new OpenApiInfo { Title = "Admin API", Version = "v1" });

	// Swagger document for User APIs
	c.SwaggerDoc("user", new OpenApiInfo { Title = "User API", Version = "v1" });


	// Decides which controller action (Api Endpoints) should be included in the swagger documentation
	c.DocInclusionPredicate((docName, apiDesc) =>
	{
		//Check if the api has a group name and the group name matches the name of the swagger document ignoring case
		if (apiDesc.GroupName != null && apiDesc.GroupName.Equals(docName, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		return false;
	});
});


// Users Injectable
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserService>();

//Auth Injectable
builder.Services.AddScoped<AuthenticationService>();

var app = builder.Build();
app.UseMiddleware<ErrorFilter>();
app.UseMiddleware<ResponseInterceptor>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/admin/swagger.json", "Admin API");
		c.SwaggerEndpoint("/swagger/user/swagger.json", "User API");
	});
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();