namespace ClassLibrary.Shared.Interfaces;

public interface IFileService
{
    /// <summary>
    /// Get content from a specified file filepath
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    string GetContentFromFile(string filePath);

    /// <summary>
    /// Save content to a specified filepath
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="content">enter you content as a string</param>
    /// <returns>returns true if saved, else false if failed</returns>
    bool SaveToFile(string filePath, string content);
}
