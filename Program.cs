using BookStore_API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//{
//    optionsBuilder.UseNpgsql(@"Host=localhost;Port=5433;Username=postgres;Password=(Direct0r1);Database=EFTest");
//}

// Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();


// ADDing CORS
builder.Services.AddCors(o => {
    o.AddPolicy("CorsPolicy",
        builder => builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});


// ADDED Swagger
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Book Store API",
        Version = "v1",
        Description = "This is an educational API for a Book Store"
    });

    //var x_file = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var x_path = Path.Combine(AppContext.BaseDirectory, x_file);
    //c.IncludeXmlComments(x_path);
});

// ADDing for NLog
// This is Dependency Injection
builder.Services.AddSingleton<ILoggerService, LoggerService>();

// Keep below for Razor Pages
// builder.Services.AddRazorPages();


// Note: AddControllers HAS TO BE Last of the Services!!!!
// Adding for WebApi Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Add Swagger 
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Book Store API");
    c.RoutePrefix = "";
});

app.UseHttpsRedirection();

// Static files not needed for WebApi's
// app.UseStaticFiles();


app.UseCors("CorsPolicy");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Not needed for WebApis
// app.MapRazorPages();

// Adding for WebApi
app.MapControllers();

app.Run();

