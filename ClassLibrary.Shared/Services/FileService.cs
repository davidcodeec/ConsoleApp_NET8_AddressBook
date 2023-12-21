using ClassLibrary.Shared.Interfaces;
using System.Diagnostics;

namespace ClassLibrary.Shared.Services;

public class FileService : IFileService
{
    private readonly string _filePath;

    public FileService(string filePath)
    {
        _filePath = filePath;
    }

    public string GetContentFromFile(string filePath)
    {
        try
        {
           if (File.Exists(_filePath))
            {
                using var sr = new StreamReader(_filePath);
                return sr.ReadToEnd();
            }
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return null!;
    }

    public bool SaveToFile(string filePath, string content)
    {
        try
        {
            using (var sw = new StreamWriter(_filePath))
            {
                sw.WriteLine(content);
            } 
            return true;
        }
        catch(Exception ex){ Debug.WriteLine(ex.Message); }
        return false;
    }
}
