namespace AltGen.API.Generate;

static class GenerateEndpoint
{
  const string Route = "/generate";

  public static RouteHandlerBuilder MapGenerateEndpoint(this WebApplication app)
  {
    return app.MapPost(Route, HandleAsync).DisableAntiforgery();
  }

  static async Task<IResult> HandleAsync([AsParameters] GenerateRequest request, [FromServices] IAltTextProviderFactory factory)
  {
    var validationResults = request.Validate(new ValidationContext(request));

    if (validationResults.Any())
    {
      return Results.ValidationProblem(validationResults.ToErrors());
    }

    var provider = factory.Create(request.Provider);

    var imageStream = new MemoryStream();
    await request.File.CopyToAsync(imageStream);

    var altText = await provider.GenerateAltTextAsync(
      request.ProviderKey,
      request.File.ContentType,
      imageStream
    );

    return Results.Ok(new GenerateResponse(altText));
  }
}