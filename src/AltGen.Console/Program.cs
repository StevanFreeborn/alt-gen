// TODO: We want to be able
// to provide a path to an image
// read the image
// and then post the image to our API
// get the response and display the alt text

using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
  .ConfigureServices(static (_, services) => { })
  .Build() // TODO: Need special extension for integrating host with Spectre.Console
  .RunAsync();

Console.WriteLine("Hello, World!");