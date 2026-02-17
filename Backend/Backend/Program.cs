using Backend.Models.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors(policy => policy.WithOrigins("https://localhost:7221").AllowAnyMethod().AllowAnyHeader());

app.UseAuthorization();
app.MapControllers();

CreateDatabase(app.Services);

app.Run();

void CreateDatabase(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var _dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

    _dataContext.Database.EnsureCreated();
    if (_dataContext.Items.Count() < 30)
    {
        var seeder = new Seeder();
        seeder.Seed(_dataContext);
    }
}
