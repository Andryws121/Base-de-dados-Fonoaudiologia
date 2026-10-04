using Microsoft.AspNetCore.Identity; // NOVO: Necessário para o Identity
using Microsoft.EntityFrameworkCore;
using Base_de_dados_Fonoaudiologia.Models;
using Base_de_dados_Fonoaudiologia.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços do MVC
builder.Services.AddControllersWithViews();

// Regista os serviços de IA para serem usados em qualquer parte do sistema
builder.Services.AddHttpClient<GeminiService>();
builder.Services.AddHttpClient<GroqService>();

// CONFIGURAÇÃO DO BANCO DE DADOS
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// NOVO: Configuração do motor do Identity (Autenticação) ligando ao AppDbContext
builder.Services.AddDefaultIdentity<Usuario>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<AppDbContext>();

var app = builder.Build();

// Configuração de rotas e ambiente
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// NOVO: Ativa o sistema de Autenticação (tem de ficar estritamente ANTES da Autorização)
app.UseAuthentication(); 
app.UseAuthorization();

// NOVO: Mapeia as rotas das telas de Login/Registro nativas do Identity
app.MapRazorPages(); 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();