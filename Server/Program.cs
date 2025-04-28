using Microsoft.EntityFrameworkCore;
using DuolingoClassLibrary.Data;
using DuolingoClassLibrary.Repositories.Repos;
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
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<FriendsRepository>();
builder.Services.AddScoped<IFriendsRepository, DuolingoClassLibrary.Repositories.Proxies.FriendsRepositoryProxy>();
builder.Services.AddScoped<HashtagRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
