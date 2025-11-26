using Microsoft.EntityFrameworkCore;
using tivitApi.Data;
using tivitApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Injecao de dependencia
builder.Services.AddScoped<IMatriculaService, MatriculaService>();
builder.Services.AddScoped<ICursoService, CursoService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

}

app.UseRouting();
app.UseCors("AllowAll");   
app.UseAuthorization();

app.MapControllers();

app.Run();