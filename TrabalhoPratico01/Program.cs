using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Dados iniciais
var pessoas = new List<string> { "Pessoa 1", "Pessoa 2", "Pessoa 3" };
var carros = new List<string> { "Carro 1", "Carro 2", "Carro 3" };
var animais = new List<string> { "Animal 1", "Animal 2", "Animal 3" };

var cache = new MemoryCache(new MemoryCacheOptions());

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapGet("/pessoas", async (HttpContext context) =>
{
    context.Response.Headers.Add("Content-Type", "application/json");

    if (cache.TryGetValue("pessoas", out List<string> cachedData) && cachedData.SequenceEqual(pessoas))
    {
        context.Response.StatusCode = 304;
        return;
    }

    cache.Set("pessoas", pessoas);
    await context.Response.WriteAsync(JsonConvert.SerializeObject(pessoas));
});

app.MapGet("/carros", async (HttpContext context) =>
{
    context.Response.Headers.Add("Content-Type", "application/json");

    if (cache.TryGetValue("carros", out List<string> cachedData) && cachedData.SequenceEqual(carros))
    {
        context.Response.StatusCode = 304;
        return;
    }

    cache.Set("carros", carros);
    await context.Response.WriteAsync(JsonConvert.SerializeObject(carros));
});

app.MapGet("/animais", async (HttpContext context) =>
{
    context.Response.Headers.Add("Content-Type", "application/json");

    if (cache.TryGetValue("animais", out List<string> cachedData) && cachedData.SequenceEqual(animais))
    {
        context.Response.StatusCode = 304;
        return;
    }

    cache.Set("animais", animais);
    await context.Response.WriteAsync(JsonConvert.SerializeObject(animais));
});

app.MapGet("/{tipo}/{id}", async (HttpContext context, string tipo, int id) =>
{
    List<string> data = null;

    if (tipo == "pessoas")
    {
        data = pessoas;
    }
    else if (tipo == "carros")
    {
        data = carros;
    }
    else if (tipo == "animais")
    {
        data = animais;
    }

    if (data != null && id >= 1 && id <= data.Count)
    {
        context.Response.Headers.Add("Content-Type", "application/json");
        await context.Response.WriteAsync(JsonConvert.SerializeObject(data[id - 1]));
    }
    else
    {
        context.Response.StatusCode = 404;
    }
});

app.Run();