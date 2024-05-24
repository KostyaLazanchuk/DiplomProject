using BusinessLogic.Service;
using DataAccess.Connection;
using DataAccess.Repositories;
using Diplom.Core.Const;
using Neo4j.Driver;

internal class Program
{
    private static void Main(string[] args)
    {
        //var builder = WebApplication.CreateBuilder(args);
        var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<IDriver>(provider =>
        {
            var neo4jService = GraphDatabase.Driver(
                DataConst.ConnectionData.url,
                AuthTokens.Basic(DataConst.ConnectionData.user, DataConst.ConnectionData.password));
            return neo4jService;
        });
        builder.Services.AddSingleton(typeof(CommonRepository));
        builder.Services.AddSingleton(typeof(EdgeRepository));
        builder.Services.AddSingleton(typeof(NodeRepository));
        builder.Services.AddSingleton(typeof(NodeService));
        builder.Services.AddSingleton(typeof(EdgeService));


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
    }
}