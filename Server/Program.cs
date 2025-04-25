using Microsoft.EntityFrameworkCore;
using DuolingoClassLibrary.Data;
using DuolingoClassLibrary.Repositories.Repos;
using DuolingoClassLibrary.Repositories.Proxies;
using DuolingoClassLibrary.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<CategoryRepository>();
builder.Services.AddScoped<PostRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepositoryProxi>();
builder.Services.AddScoped<IPostRepository, PostRepositoryProxi>();
builder.Services.AddScoped<IHashtagRepository, HashtagRepositoryProxi>();
builder.Services.AddScoped<ICommentRepository, CommentRepositoryProxi>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", 
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
