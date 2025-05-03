using System.Threading.RateLimiting;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRateLimiter(options =>
{
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            await context.HttpContext.Response.WriteAsync(
                $"Too many requests. Please try again after {retryAfter.TotalMinutes} minute(s)." +
                $"Read more about our rate limits at.",cancellationToken: token);
            
        }
        else
        {
            await context.HttpContext.Response.WriteAsync(
                "Too many requests. Please try again later." +
                "Read more about our rate limits at.", cancellationToken: token);
            
        }
    };

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey : httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
});
builder.Services.AddResponseCaching();
builder.Services.AddScoped<IDateFetcherService, DateFetcherService>();
builder.Services.AddScoped<IFormatFetcherService, FormatDateFetcherService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<InputValidators>();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Add Swagger/OpenAPI
builder.Services.AddSwaggerGen(); // This is the correct method for adding Swagger

builder.Logging.ClearProviders(); // Clear existing loggers
builder.Logging.AddConsole(); // Add console logging
builder.Logging.SetMinimumLevel(LogLevel.Debug);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // This enables the Swagger middleware
    app.UseSwaggerUI(); // This enables the Swagger UI
    app.UseDeveloperExceptionPage();
}
else
{
   app.UseMiddleware<ErrorHandlingMiddleware>();
}

app.UseResponseCaching();
app.UseRateLimiter();
app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
