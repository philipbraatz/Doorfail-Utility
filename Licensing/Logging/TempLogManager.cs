using System.Text;
using System.Text.Json;
using Fileio;

namespace Doorfail.Core.Logging;

public class TempLogManager<T>
{
    private readonly FileClient fileClient;
    protected Dictionary<string, FileDetails> AllFiles;
    protected TempLogManager(FileClient fileClient)
    {
        this.fileClient = fileClient;
    }

    public static async Task<TempLogManager<T>> Initialize(FileClient fileClient)
    {
        return new TempLogManager<T>(fileClient)
        {
            AllFiles = (await fileClient.GetFiles()).ToDictionary(f => f.Name, f => f)
        };
    }

    public async Task<List<T>> GetAllLogs()
    {
        var logs = new List<T>();

        foreach(var file in AllFiles)
        {
            var fileContent = await fileClient.DownloadFile(file.Key);
            logs.AddRange(ReadLog(fileContent));
        }

        return logs;
    }

    public async Task<List<T>> GetLogFile(string fileName)
    {
        var fileContent = await fileClient.DownloadFile(fileName);
        return ReadLog(fileContent);
    }

    public async Task AddLog(string fileName, T log)
    {
        var fileContent = JsonSerializer.Serialize(log);
        var fileKey = AllFiles.ContainsKey(fileName) ? AllFiles[fileName]?.Key : null;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        if(fileKey == null)
        {
            // File doesn't exist, create it
            await fileClient.UploadFile(stream, fileName);
            return;
        }

        await fileClient.UpdateFile(fileKey, stream);
    }


    public async Task<T> ReadLastLog(string fileName)
    {
        if(!AllFiles.TryGetValue(fileName, out var fileDetails))
            return default;

        var fileContent = await fileClient.DownloadFile(fileDetails.Key);
        var lastLine = GetLastLine(fileContent);

        if(ByteSizeLib.ByteSize.FromBits(fileDetails.Size) > ByteSizeLib.ByteSize.FromGigaBytes(.25)
            || fileDetails.Expires < DateTime.UtcNow.AddDays(2))
        {
            var lastLineStream = new MemoryStream(Encoding.UTF8.GetBytes(lastLine));
            await fileClient.UpdateFile(fileDetails.Key, lastLineStream, expiresIn: 14, unit: ExpirationUnit.days);
        }

        return JsonSerializer.Deserialize<T>(lastLine);
    }

    public List<T> ReadLog(Stream content)
    {
        var logs = new List<T>();

        using var reader = new StreamReader(content);
        while(!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            logs.Add(JsonSerializer.Deserialize<T>(line));
        }

        return logs;
    }

    public async Task DeleteLog(string fileName)
    {
        var fileDetails = AllFiles[fileName];
        await fileClient.DeleteFile(fileDetails.Key);
    }

    private string GetLastLine(Stream stream)
    {
        using var reader = new StreamReader(stream);
        string lastLine = null;
        while(!reader.EndOfStream)
        {
            lastLine = reader.ReadLine();
        }
        return lastLine;
    }
}