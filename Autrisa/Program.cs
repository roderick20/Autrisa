//using Autrisa.Helper;
using Autrisa.Models;
using Autrisa;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

var builder = WebApplication.CreateBuilder(args);

#if (DEBUG)
builder.Services.AddControllersWithViews(options =>
{{
    options.Filters.Add(typeof(AuthenticationFilter));
}}).AddRazorRuntimeCompilation();
#else
builder.Services.AddControllersWithViews(options =>
{{
    options.Filters.Add(typeof(AuthenticationFilter));
}});
#endif



builder.Services.AddSession(s => s.IdleTimeout = TimeSpan.FromMinutes(30));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IPrincipal>(provider => provider.GetService<IHttpContextAccessor>()?.HttpContext?.User);
builder.Services.AddHttpClient();

var connection = builder.Configuration.GetConnectionString("DB");
builder.Services.AddDbContext<EFContext>(options => options.UseSqlServer(connection));


var app = builder.Build();
if (!app.Environment.IsDevelopment())
{{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(name: "areas", pattern: "{area:exists}/{controller=App}/{action=Index}/{id?}");
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


//Scaffold-DbContext "server=3.131.217.111;Database=Autrisa_394;user id=user_Autrisa_394;password=!g78Z8-H;Encrypt=false" "Microsoft.EntityFrameworkCore.SqlServer" -OutputDir Models -Context EFContext -force