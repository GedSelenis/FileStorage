using FileStorage.Models;
using FileStorage.Models.DTO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using System.Xml.Serialization;


HttpClient client = new HttpClient();
client.BaseAddress = new Uri("https://localhost:7261");

await StartApplication();
async Task StartApplication()
{
    Console.WriteLine("Welcome to the File Storage Application!");
    await ChoseOption();
}

async Task PrintCurrentFiles()
{
    using StringContent jsonContent = new(
        "",
        Encoding.UTF8,
        "application/json");
    using HttpResponseMessage response = await client.PostAsync(
        "API/Index",
        jsonContent);
    List<FileResponse> files = JsonSerializer.Deserialize<List<FileResponse>>(
        await response.Content.ReadAsStringAsync(),
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<FileResponse>();

    Console.WriteLine("Current Files:\n");
    foreach (var file in files)
    {
        Console.WriteLine($"ID: {file.Id}, Name: {file.FileName}, Path: {file.FilePath}, VirtualFolder: {file.VirualFolderName}");
    }
}

async Task PrintCurrentFolders()
{
    using StringContent jsonContent = new(
        "",
        Encoding.UTF8,
        "application/json");
    using HttpResponseMessage response = await client.PostAsync(
        "API/FolderIndex",
        jsonContent);
    List<FolderResponse> folders = JsonSerializer.Deserialize<List<FolderResponse>>(
        await response.Content.ReadAsStringAsync(),
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<FolderResponse>();

    Console.WriteLine("Current Folders:\n");
    foreach (var folder in folders)
    {
        Console.WriteLine($"ID: {folder.Id}, Name: {folder.FolderName}, Folder path: {folder.VirtualPath}");
    }
}

async Task ChoseOption()
{
    await PrintCurrentFiles();
    await PrintCurrentFolders();
    Console.WriteLine("Please choose an option:");
    Console.WriteLine("1. Add File");
    Console.WriteLine("2. Rename File");
    Console.WriteLine("3. Add Text to File");
    Console.WriteLine("4. Delete file");
    Console.WriteLine("5. Move file to folder");
    Console.WriteLine("6. Add folder");
    Console.WriteLine("10. Exit");
    String? input = Console.ReadLine();
    if (int.TryParse(input, out int option))
    {
        switch (option)
        {
            case 1:
                FileAddRequest fileAddRequest = InputAddRequest();
                await SendAddFile(fileAddRequest);
                //fileService.AddFile(fileAddRequest);
                break;
            case 2:
                FileRenameRequest fileRenameRequest = InputRenameRequest();
                await SendRenameFile(fileRenameRequest);
                //fileService.RenameFileAsync(fileRenameRequest);
                break;
            case 3:
                FileAddTextToFileRequest fileAddTextToFileRequest = InputAddTextToFile();
                await SendAddTextToFile(fileAddTextToFileRequest);
                //fileService.AddTextToFileAsync(fileAddTextToFileRequest);
                break;
            case 4:
                Guid id = GetFileId();
                await SendDeleteFile(id);
                //fileService.DeleteFile(id);
                break;
            case 10:
                Console.WriteLine("Exiting the application. Goodbye!");
                return;
            default:
                Console.WriteLine("Invalid option. Please try again.");
                break;
        }
        await ChoseOption();
    }
    else
    {
        Console.WriteLine("Invalid input. Please enter a number between 1 and 4.");
        await ChoseOption();
    }
}

// Inpusts

#region Inputs for file operations
FileAddRequest InputAddRequest()
{
    Console.WriteLine("Enter file name:");
    string? fileName = Console.ReadLine();
    Console.WriteLine("Enter file path:");
    string? filePath = Console.ReadLine();
    Console.WriteLine("Enter file virtual folder:");
    string? virtualFolder = Console.ReadLine();
    if (Guid.TryParse(virtualFolder, out Guid FolderId))
    {
        Console.WriteLine("Enter new file name:");
        string? newFileName = Console.ReadLine();
        return new FileAddRequest
        {
            FileName = fileName,
            FilePath = filePath,
            VirualFolderId = FolderId
        };
    }
    else
    {
        throw new ArgumentException("Invalid file ID format.");
    }
}

FileRenameRequest InputRenameRequest()
{
    Console.WriteLine("Enter file ID to rename:");
    string? fileIdInput = Console.ReadLine();
    if (Guid.TryParse(fileIdInput, out Guid fileId))
    {
        Console.WriteLine("Enter new file name:");
        string? newFileName = Console.ReadLine();
        return new FileRenameRequest
        {
            Id = fileId,
            NewFileName = newFileName
        };
    }
    else
    {
        throw new ArgumentException("Invalid file ID format.");
    }
}

FileAddTextToFileRequest InputAddTextToFile()
{
    Console.WriteLine("Enter file ID to add text to:");
    string? fileIdInput = Console.ReadLine();
    if (Guid.TryParse(fileIdInput, out Guid fileId))
    {
        Console.WriteLine("Enter text to add to the file:");
        string? textToAdd = Console.ReadLine();
        return new FileAddTextToFileRequest
        {
            Id = fileId,
            TextToAdd = textToAdd
        };
    }
    else
    {
        throw new ArgumentException("Invalid file ID format.");
    }
}

Guid GetFileId()
{
    Console.WriteLine("Enter file ID to get details:");
    string? fileIdInput = Console.ReadLine();
    if (Guid.TryParse(fileIdInput, out Guid fileId))
    {
        return fileId;
    }
    else
    {
        throw new ArgumentException("Invalid file ID format.");
    }

}
#endregion

#region Inputs for folder operations

#endregion

// API calls

#region Calls for file operations

async Task SendAddFile(FileAddRequest fileAddRequest)
{
    var formContent = new FormUrlEncodedContent(new[]
        {
        new KeyValuePair<string, string>("FileName", fileAddRequest.FileName),
        new KeyValuePair<string, string>("FilePath", fileAddRequest.FilePath),
        new KeyValuePair<string, string>("VirualFolderId", fileAddRequest.VirualFolderId.ToString())
        });
    using HttpResponseMessage response = await client.PostAsync(
        "API/AddFile",
        formContent);

    if (response.IsSuccessStatusCode)
    {
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"File added successfully: {responseBody}");
    }
    else
    {
        Console.WriteLine($"Error adding file: {response.ReasonPhrase}");
    }
}

async Task SendRenameFile(FileRenameRequest fileRenameRequest)
{
    var formContent = new FormUrlEncodedContent(new[]
        {
        new KeyValuePair<string, string>("NewFileName", fileRenameRequest.NewFileName)
        });
    using HttpResponseMessage response = await client.PostAsync(
        $"API/Rename/{fileRenameRequest.Id}",
        formContent);

    if (response.IsSuccessStatusCode)
    {
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"File added successfully: {responseBody}");
    }
    else
    {
        Console.WriteLine($"Error adding file: {response.ReasonPhrase}");
    }
}

async Task SendAddTextToFile(FileAddTextToFileRequest fileTextToAddRequest)
{
    var formContent = new FormUrlEncodedContent(new[]
        {
        new KeyValuePair<string, string>("TextToAdd", fileTextToAddRequest.TextToAdd)
        });
    using HttpResponseMessage response = await client.PostAsync(
        $"API/AddText/{fileTextToAddRequest.Id}",
        formContent);

    if (response.IsSuccessStatusCode)
    {
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"File added successfully: {responseBody}");
    }
    else
    {
        Console.WriteLine($"Error adding file: {response.ReasonPhrase}");
    }
}

async Task SendDeleteFile(Guid id)
{
    var formContent = new FormUrlEncodedContent(new[]
        {
        new KeyValuePair<string, string>("Id", id.ToString())
        });
    using HttpResponseMessage response = await client.PostAsync(
        $"API/delete/{id}",
        formContent);

    if (response.IsSuccessStatusCode)
    {
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"File added successfully: {responseBody}");
    }
    else
    {
        Console.WriteLine($"Error adding file: {response.ReasonPhrase}");
    }
}
#endregion