using Backend.Models.Database;

var builder = WebApplication.CreateBuilder(args);

Directory.SetCurrentDirectory(AppContext.BaseDirectory);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<DataContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

CreateDatabase(app.Services);

app.Run();

void CreateDatabase(IServiceProvider services)
{
    using IServiceScope scope = services.CreateScope();
    using DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

    if (dataContext.Database.EnsureCreated())
    {
        Seeder seeder = new();
        seeder.Seed(dataContext);
    }
}
