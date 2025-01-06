using kursah_5semestr.Abstractions;
using kursah_5semestr.Services;
using kursah_5semestr;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Text.Json;

const string ISSUER = "my-auth-service";
const string AUDIENCE = "my-application";

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    var enumConverter = new JsonStringEnumConverter(JsonNamingPolicy.CamelCase);
    options.JsonSerializerOptions.Converters.Add(enumConverter);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<ICartItemsService, CartItemsService>();
builder.Services.AddScoped<IOrdersService, OrdersService>();
builder.Services.AddSingleton<IBrokerService, BrokerService>();
builder.Services.AddSingleton<IDataUpdaterService, DataUpdaterService>();

builder.Services.AddAuthentication().AddJwtBearer(jwtOptions =>
{
    var key = File.ReadAllText("./key.dat");

    jwtOptions.Audience = AUDIENCE;
    jwtOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = AUDIENCE,
        ValidIssuer = ISSUER,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key))
    };
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.Services.GetService<IDataUpdaterService>()!.Start();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.Run();
