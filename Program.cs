using Microsoft.Data.Sqlite;
using Dapper;
using StorageManagement.API.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5027");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger to use JWT authentication
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "StorageManagement.API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});


builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);

// Register repositories
var connectionString = "Data Source=storage.db";
builder.Services.AddScoped<IUserRepository>(provider => 
    new UserRepository(connectionString));
builder.Services.AddScoped<IStorageItemRepository>(provider => 
    new StorageItemRepository(connectionString));
builder.Services.AddScoped<ISupplierRepository>(provider => 
    new SupplierRepository(connectionString));
builder.Services.AddScoped<ISuperitemRepository>(provider => 
    new SuperitemRepository(connectionString));
builder.Services.AddScoped<IOutgoingOrderRepository>(provider => 
    new OutgoingOrderRepository(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseStaticFiles(); // Enable static file serving

// Add authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Initialize database
InitializeDatabase(connectionString);

app.Run();

void InitializeDatabase(string connectionString)
{
    using var connection = new SqliteConnection(connectionString);
    connection.Open();
    
    // Create Users table with IsAdmin column
    connection.Execute(@"
        CREATE TABLE IF NOT EXISTS Users (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            FirstName TEXT NOT NULL,
            LastName TEXT NOT NULL,
            Email TEXT UNIQUE NOT NULL,
            Username TEXT UNIQUE NOT NULL,
            Password TEXT NOT NULL,
            IsAdmin INTEGER NOT NULL DEFAULT 0,
            CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
        )");

    // Add IsAdmin column if it doesn't exist (for migrations)
    try {
        connection.Execute("ALTER TABLE Users ADD COLUMN IsAdmin INTEGER NOT NULL DEFAULT 0");
    } catch { /* Ignore if already exists */ }
    
    // Create Suppliers table first (referenced by StorageItems)
    connection.Execute(@"
        CREATE TABLE IF NOT EXISTS Suppliers (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT,
            Afm TEXT,
            PhoneNumber TEXT,
            Address TEXT,
            Email TEXT,
            CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
        )");

    // Create StorageItems table with Supplier relationship
    connection.Execute(@"
        CREATE TABLE IF NOT EXISTS StorageItems (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Quantity INTEGER NOT NULL DEFAULT 0,
            Location TEXT NOT NULL,
            Price REAL DEFAULT 0,
            SupplierId INTEGER,
            PhotoPath TEXT,
            CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
            UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id)
        )");

    // Add PhotoPath column if it doesn't exist (for migrations)
    var tableInfo = connection.Query("PRAGMA table_info(StorageItems)").ToList();
    var columnExists = tableInfo.Any(c => c.name == "PhotoPath");
    if (!columnExists)
    {
        connection.Execute("ALTER TABLE StorageItems ADD COLUMN PhotoPath TEXT");
    }

    // Create Superitems table
    connection.Execute(@"
        CREATE TABLE IF NOT EXISTS Superitems (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Location TEXT NOT NULL,
            Quantity INTEGER NOT NULL DEFAULT 0
        )
    ");

    // Create SuperitemStorageItems join table with Quantity
    connection.Execute(@"
        CREATE TABLE IF NOT EXISTS SuperitemStorageItems (
            SuperitemId INTEGER NOT NULL,
            StorageItemId INTEGER NOT NULL,
            Quantity INTEGER NOT NULL DEFAULT 1,
            PRIMARY KEY (SuperitemId, StorageItemId),
            FOREIGN KEY (SuperitemId) REFERENCES Superitems(Id) ON DELETE CASCADE,
            FOREIGN KEY (StorageItemId) REFERENCES StorageItems(Id) ON DELETE CASCADE
        )
    ");

    // Create OutgoingOrders table
    connection.Execute(@"
        CREATE TABLE IF NOT EXISTS OutgoingOrders (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            OrderDate DATETIME DEFAULT CURRENT_TIMESTAMP,
            Recipient TEXT NOT NULL,
            SerialNumber TEXT NOT NULL UNIQUE,
            Status TEXT NOT NULL DEFAULT 'Open'
        )
    ");

    // Create OutgoingOrderItems join table
    connection.Execute(@"
        CREATE TABLE IF NOT EXISTS OutgoingOrderItems (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            OutgoingOrderId INTEGER NOT NULL,
            StorageItemId INTEGER,
            Quantity INTEGER NOT NULL DEFAULT 1,
            CustomItemDescription TEXT,
            CHECK ((StorageItemId IS NOT NULL AND CustomItemDescription IS NULL) OR (StorageItemId IS NULL AND CustomItemDescription IS NOT NULL)),
            FOREIGN KEY (OutgoingOrderId) REFERENCES OutgoingOrders(Id) ON DELETE CASCADE,
            FOREIGN KEY (StorageItemId) REFERENCES StorageItems(Id) ON DELETE CASCADE
        )
    ");
}