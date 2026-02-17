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

    app.UseCors(policy =>
        policy.SetIsOriginAllowed(origin =>
            new Uri(origin).Host == "localhost")
            .AllowAnyMethod()
            .AllowAnyHeader());
}

app.UseHttpsRedirection();

app.UseStaticFiles();

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
