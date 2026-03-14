using HDA.Infrastructure.Data;
using HDA.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<HdaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Blazor ────────────────────────────────────────────────────────────────────
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

// ── HTTP ──────────────────────────────────────────────────────────────────────
builder.Services.AddHttpClient<IOpenDotaService, OpenDotaService>();

// ── App Services ──────────────────────────────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<IProPlayerService, ProPlayerService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IOpenDotaService, OpenDotaService>();
builder.Services.AddSingleton<AppState>();

var app = builder.Build();

// ── Middleware ────────────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// ── Seed DB ───────────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<HdaDbContext>();
    await DbSeeder.SeedAsync(ctx);
}

app.Run();
