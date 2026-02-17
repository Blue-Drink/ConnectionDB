using Backend.Models.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors(policy => policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost").AllowAnyMethod().AllowAnyHeader());

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

CreateDatabase(app.Services);

app.Run();

void CreateDatabase(IServiceProvider services)
{
    using IServiceScope scope = services.CreateScope();
    using DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

    dataContext.Database.EnsureCreated();
    if (dataContext.Items.Count() < 30)
    {
        Seeder seeder = new();
        seeder.Seed(dataContext);
    }
}
