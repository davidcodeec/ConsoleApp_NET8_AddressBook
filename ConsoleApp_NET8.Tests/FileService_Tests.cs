using ClassLibrary.Shared.Interfaces;
using ClassLibrary.Shared.Services;

namespace ConsoleApp_NET8.Tests;

public class FileService_Tests
{
    [Fact]
    public void SaveToFileShould_ReturnTrue_IfFilePathExists()
    {
        // Arrange
        string filePath = @"C:\Exercises\CSharp-Exercise\test.txt";
        IFileService fileService = new FileService(filePath);
        string content = "Test content";

        // Act
        bool result = fileService.SaveToFile(filePath,content);

        // Assert
        Assert.True(result, "SaveToFile returned false");
    }

    [Fact]

    public void SaveToFileShould_ReturnFalse_IfFilePathDoNotExists()
    {
        // Arrange
        string filePath = @$"C:\Exercises\CSharp-Exercise\{Guid.NewGuid()}\test.txt";
        IFileService fileService = new FileService(filePath);
        string content = "Test content";

        // Act
        bool result = fileService.SaveToFile(filePath, content);

        // Assert
        Assert.False(result, "SaveToFile returned false");
    }
}
