using _02_BusinessLogic.Clases;
using _02_BusinessLogic.Interfaces;

var builder = WebApplication.CreateBuilder(args);
// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin() // Permitir cualquier origen
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


builder.Services.AddScoped<IAtencionUrgenciaBL, AtencionUrgenciaBL>();








builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseAuthentication();
    app.UseAuthorization();
}

app.UseSwagger();
app.UseSwaggerUI();

// Habilitar CORS
app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();



app.MapControllers();

app.Run();