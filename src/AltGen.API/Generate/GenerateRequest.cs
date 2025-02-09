namespace AltGen.API.Generate;

record GenerateRequest(
  [FromForm]
  string Provider,
  [FromForm]
  string ProviderKey,
  IFormFile File
) : IValidatableObject
{
  public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
  {
    if (string.IsNullOrWhiteSpace(Provider))
    {
      yield return new ValidationResult($"The {nameof(Provider)} field is required.", [nameof(Provider)]);
    }

    if (string.IsNullOrWhiteSpace(Provider) is false && LLMProvider.IsValid(Provider) is false)
    {
      yield return new ValidationResult($"The {nameof(Provider)} field is invalid.", [nameof(Provider)]);
    }

    if (string.IsNullOrWhiteSpace(ProviderKey))
    {
      yield return new ValidationResult($"The {nameof(ProviderKey)} field is required.", [nameof(ProviderKey)]);
    }

    // TODO: Probably need to consider upper bound for file size
    // this also needs to take into account provider-specific limits
    if (File.Length is 0)
    {
      yield return new ValidationResult($"The {nameof(File)} has no content.", [nameof(File)]);
    }

    if (File.Length > 0)
    {
      var extension = Path.GetExtension(File.FileName);

      // TODO: prob needs to be specific to the provider
      // different LLMs support different image formats
      if (extension is not ".jpeg" and not ".jpg" and not ".png")
      {
        yield return new ValidationResult($"The {nameof(File)} must be a valid image file.", [nameof(File)]);
      }
    }
  }
}