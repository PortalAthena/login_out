using Login.Data;
using Login.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// ---------------------
// 🔐 REGISTRO DE SERVIÇOS (ANTES DO BUILD)
// ---------------------

builder.Services.AddControllersWithViews();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "PortalAthenaAuth";
        // options.Cookie.Domain = ".meusistema.com"; // usar só quando configurar hosts
        options.LoginPath = "/Home/Login";
        options.AccessDeniedPath = "/Home/AcessoNegado";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        //Mudar para Always quando for configurar o ambiente de produção com HTTPS. O SameAsRequest é recomendado para desenvolvimento local, onde pode ser necessário usar HTTP. Em produção, é importante garantir que os cookies sejam transmitidos apenas por conexões seguras (HTTPS) para proteger as informações de autenticação.
        //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });



builder.Services.AddAuthorization();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\ProjetoPortaLAthena\ChavesCompartilhadas"))
    .SetApplicationName("MeuSistemaSSO");

// 🔵 REGISTRO DO DB CONTEXT DO BANCO DE USUARIOS
builder.Services.AddDbContext<UsuarioDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("AdministracaoConnection")
    )
);
// 🔵 REGISTRO DO DB CONTEXT DO BANCO DE APARELHOS PUBLICOS
builder.Services.AddDbContext<AparelhosPublicosDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("AparelhosPublicosConnection")
    )
);


// ---------------------
// REGISTRO DE LISTAS
// ---------------------
builder.Services.AddScoped<ListaService>();

// ---------------------
// REGISTRO DE Cruds
// ---------------------
builder.Services.AddScoped<CrudService>();


// ---------------------
// 🚀 BUILD
// ---------------------

var app = builder.Build();

// ---------------------
// 🔄 PIPELINE
// ---------------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();   // ⚠️ Sempre antes
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";
    await next();
});

app.Run();
