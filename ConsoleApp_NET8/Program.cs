using ClassLibrary.Shared.Interfaces;
using ClassLibrary.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder().ConfigureServices(services =>
{

    // Register services
    services.AddScoped<IContactService>(provider =>
    {
        var fileService = provider.GetRequiredService<IFileService>();
        var filePath = @"C:\Exercises\CSharp-Exercise\Contact.json";
        return new ContactService(fileService, filePath);
    });


    services.AddScoped<IFileService>(provider =>
    {
        // File Path
        var filePath = @"C:\Exercises\CSharp-Exercise\Contact.json";
        return new FileService(filePath);
    });

    services.AddScoped<IMenuService,MenuService>();


}).Build();


// Retrieve an instance of IFileService
var fileService = builder.Services.GetRequiredService<IFileService>();

builder.Start();
Console.Clear();

var menuService = builder.Services.GetRequiredService<IMenuService>();

menuService.ShowMainMenu();