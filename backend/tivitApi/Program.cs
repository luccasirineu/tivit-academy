using Microsoft.EntityFrameworkCore;
using tivitApi.Data;
using tivitApi.Services;
using tivitApi.Exceptions;
using tivitApi.Infra.SQS;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IMatriculaService, MatriculaService>();
builder.Services.AddScoped<ICursoService, CursoService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddScoped<IMateriaService, MateriaService>();
builder.Services.AddScoped<IConteudoService,ConteudoService>();
builder.Services.AddScoped<INotaService, NotaService>();
builder.Services.AddScoped<IAlunoService, AlunoService>();



builder.Services.AddSingleton<SQSProducer>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandler>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllers();

app.Run();


