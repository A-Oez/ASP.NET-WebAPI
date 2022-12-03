var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => //JWT Auth options advanced
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
    {
        Description = "Standard Authorization Header (bearer: token)",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>(); 
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //Auth Schmeme festlegen
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    
    }); 

builder.Services.AddDbContext<TestDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings:Sql").Value);
});

//Add Scoped -> CacheService Interface & ArticleService Interface

builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IOrderService, OrderServices>();
builder.Services.AddScoped<IValidator<Customer>, CustomerValidator>();
builder.Services.AddScoped<IValidator<Article>,ArticleValidator>();
builder.Services.AddScoped<IValidator<Order>, OrderValidator>();
builder.Services.AddScoped<IJWTService, JWTService>();

Log.Logger = new LoggerConfiguration().WriteTo.File(builder.Configuration.GetSection("LogPath:Path").Value, rollingInterval: RollingInterval.Day).CreateLogger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) 
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors(options => 
options.WithOrigins(builder.Configuration.GetSection("Cors:URL").Value)
.AllowAnyMethod()
.AllowAnyHeader()
   );

app.MapControllers();

app.Run();

