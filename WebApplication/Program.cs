﻿using BusinessLogic.Interface;
using BusinessLogic.Service;
using DataAccess.Connection;
using DataAccess.Repositories;
using Diplom.Core.Const;
using Diplom.Core.Features.NodeFeatures.Command;
using Diplom.Core.Features.NodeFeatures.Handler;
using Diplom.Core.Features.Validation;
using Diplom.Core.Models;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add Neo4j driver
        builder.Services.AddSingleton<IDriver>(provider =>
        {
            var neo4jService = GraphDatabase.Driver(
                DataConst.ConnectionData.url,
                AuthTokens.Basic(DataConst.ConnectionData.user, DataConst.ConnectionData.password));
            return neo4jService;
        });

        // Register repositories
        builder.Services.AddSingleton<CommonRepository>();
        builder.Services.AddSingleton<EdgeRepository>();
        builder.Services.AddSingleton<NodeRepository>();

        // Register services
        builder.Services.AddSingleton<INodeService, NodeService>();
        builder.Services.AddSingleton<IEdgeService, EdgeService>();
        builder.Services.AddSingleton<ICommonService, CommonService>();

        // Add FluentValidation
        builder.Services.AddTransient<IValidator<Node>, NodeValidator>();

        // Add MediatR
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

        // Add handler for validation
        //builder.Services.AddTransient<IRequestHandler<ValidateNodeCommand, Unit>, ValidateNodeHandler>();

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