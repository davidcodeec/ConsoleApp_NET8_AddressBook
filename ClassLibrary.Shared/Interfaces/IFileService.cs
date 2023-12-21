namespace ClassLibrary.Shared.Interfaces;

public interface IFileService
{
    string GetContentFromFile(string filePath);
    bool SaveToFile(string filePath, string content);
}
