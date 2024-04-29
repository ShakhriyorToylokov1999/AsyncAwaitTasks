using System;
using System.Threading;
using System.Threading.Tasks;
using AsyncAwait.Task2.CodeReviewChallenge.Headers;
using CloudServices.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AsyncAwait.Task2.CodeReviewChallenge.Middleware;

public class StatisticMiddleware
{
    private readonly RequestDelegate _next;

    private readonly IStatisticService _statisticService;

    public StatisticMiddleware(RequestDelegate next, IStatisticService statisticService)
    {
        _next = next;
        _statisticService = statisticService ?? throw new ArgumentNullException(nameof(statisticService));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string path = context.Request.Path;

        var staticRegTask = _statisticService.RegisterVisitAsync(path)
            .ContinueWith(async _ =>
            {
                await UpdateHeadersAsync();
            });

        Console.WriteLine(staticRegTask.Status); // just for debugging purposes

        
        async Task UpdateHeadersAsync()
        {
            var pageVisitsCount = await _statisticService.GetVisitsCountAsync(path);
            context.Response.Headers.Add(CustomHttpHeaders.TotalPageVisits, pageVisitsCount.ToString());
        }

        Thread.Sleep(3000); // without this the statistic counter does not work
        await _next(context);
    }
}
