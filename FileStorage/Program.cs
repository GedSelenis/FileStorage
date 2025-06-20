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
    Console.WriteLine("5. Move file to folder"); // Form here
    Console.WriteLine("6. Add folder");
    Console.WriteLine("7. Delete folder");
    Console.WriteLine("8. Update folder");
    Console.WriteLine("9. Move folder to another folder");
    Console.WriteLine("10. Exit");
    String? input = Console.ReadLine();
    if (int.TryParse(input, out int option))
    {
        switch (option)
        {
            case 1:
                FileAddRequest fileAddRequest = InputAddRequest();
                await SendAddFile(fileAddRequest);
                break;
            case 2:
                FileRenameRequest fileRenameRequest = InputRenameRequest();
                await SendRenameFile(fileRenameRequest);
                break;
            case 3:
                FileAddTextToFileRequest fileAddTextToFileRequest = InputAddTextToFile();
                await SendAddTextToFile(fileAddTextToFileRequest);
                break;
            case 4:
                Guid id = GetFileId();
                await SendDeleteFile(id);
                break;
            case 5:
                FileToFolderRequest fileToFolderRequest = InputFileToFolderRequest();
                await SendMoveFileToFolder(fileToFolderRequest);
                break;
            case 6:
                FolderAddRequest folderAddRequest = InputFolderAddRequest();
                await SendAddFolder(folderAddRequest);
                break;
            case 7:
                Guid folderID = GetFolderId();
                await SendDeleteFolder(folderID);
                break;
            case 8:
                FolderUpdateRequest folderUpdateRequest = InputFolderUpdateRequest();
                await SendUpdateFolder(folderUpdateRequest);
                break;
            case 9:
                FolderToFolderRequest folderToFolderRequest = InputFolderToFolderRequest();
                await SendMoveFolderToAnotherFolder(folderToFolderRequest);
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

FileToFolderRequest InputFileToFolderRequest()
{
    Console.WriteLine("Enter file id:");
    string? fileIdInput = Console.ReadLine();
    Console.WriteLine("Enter folder id:");
    string? folderIdInput = Console.ReadLine();
    if (Guid.TryParse(folderIdInput, out Guid FolderId) && Guid.TryParse(fileIdInput, out Guid fileID))
    {
        return new FileToFolderRequest
        {
            Id = fileID,
            VirualFolderId = FolderId
        };
    }
    else
    {
        throw new ArgumentException("Invalid file ID format.");
    }
}

#endregion

#region Inputs for folder operations

FolderAddRequest InputFolderAddRequest()
{
    Console.WriteLine("Enter folder name:");
    string? FolderName = Console.ReadLine();
    return new FolderAddRequest
    {
        FolderName = FolderName
    };
}

FolderUpdateRequest InputFolderUpdateRequest()
{
    Console.WriteLine("Enter folder ID to update:");
    string? folderIdInput = Console.ReadLine();
    if (Guid.TryParse(folderIdInput, out Guid folderId))
    {
        Console.WriteLine("Enter new folder name:");
        string? newFolderName = Console.ReadLine();
        return new FolderUpdateRequest
        {
            Id = folderId,
            FolderName = newFolderName
        };
    }
    else
    {
        throw new ArgumentException("Invalid folder ID format.");
    }
}

FolderToFolderRequest InputFolderToFolderRequest()
{
    Console.WriteLine("Enter folder ID to move:");
    string? folderIdInput = Console.ReadLine();
    Console.WriteLine("Enter new parent folder ID:");
    string? parentFolderIdInput = Console.ReadLine();
    if (Guid.TryParse(folderIdInput, out Guid folderId) && Guid.TryParse(parentFolderIdInput, out Guid parentFolderId))
    {
        return new FolderToFolderRequest
        {
            Id = folderId,
            ParentFolderId = parentFolderId
        };
    }
    else
    {
        throw new ArgumentException("Invalid folder ID format.");
    }
}

Guid GetFolderId()
{
    Console.WriteLine("Enter file ID to get details:");
    string? folderIdInput = Console.ReadLine();
    if (Guid.TryParse(folderIdInput, out Guid fileId))
    {
        return fileId;
    }
    else
    {
        throw new ArgumentException("Invalid file ID format.");
    }

}

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

async Task SendMoveFileToFolder(FileToFolderRequest fileToFolderRequest)
{
    var formContent = new FormUrlEncodedContent(new[]
        {
        new KeyValuePair<string, string>("VirualFolderId", fileToFolderRequest.VirualFolderId.ToString()),
        new KeyValuePair<string, string>("Id", fileToFolderRequest.Id.ToString())
        });
    using HttpResponseMessage response = await client.PostAsync(
        $"API/MoveToFolder",
        formContent);
    if (response.IsSuccessStatusCode)
    {
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"File moved successfully: {responseBody}");
    }
    else
    {
        Console.WriteLine($"Error moving file: {response.ReasonPhrase}");
    }
}

#endregion

#region Calls for folder operations

async Task SendAddFolder(FolderAddRequest folderAddRequest)
{
    var formContent = new FormUrlEncodedContent(new[]
        {
        new KeyValuePair<string, string>("FolderName", folderAddRequest.FolderName)
        });
    using HttpResponseMessage response = await client.PostAsync(
        "API/AddFolder",
        formContent);
    if (response.IsSuccessStatusCode)
    {
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Folder added successfully: {responseBody}");
    }
    else
    {
        Console.WriteLine($"Error adding folder: {response.ReasonPhrase}");
    }
}

async Task SendDeleteFolder(Guid id)
{
    var formContent = new FormUrlEncodedContent(new[]
        {
        new KeyValuePair<string, string>("Id", id.ToString())
        });
    using HttpResponseMessage response = await client.PostAsync(
        $"API/DeleteFolder/{id}",
        formContent);
    if (response.IsSuccessStatusCode)
    {
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Folder deleted successfully: {responseBody}");
    }
    else
    {
        Console.WriteLine($"Error deleting folder: {response.ReasonPhrase}");
    }
}

async Task SendUpdateFolder(FolderUpdateRequest folderUpdateRequest)
{
    var formContent = new FormUrlEncodedContent(new[]
        {
        new KeyValuePair<string, string>("FolderName", folderUpdateRequest.FolderName),
        new KeyValuePair<string, string>("Id", folderUpdateRequest.Id.ToString())
        });
    using HttpResponseMessage response = await client.PostAsync(
        $"API/UpdateFolder",
        formContent);
    if (response.IsSuccessStatusCode)
    {
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Folder updated successfully: {responseBody}");
    }
    else
    {
        Console.WriteLine($"Error updating folder: {response.ReasonPhrase}");
    }
}

async Task SendMoveFolderToAnotherFolder(FolderToFolderRequest folderToFolderRequest)
{
    var formContent = new FormUrlEncodedContent(new[]
        {
        new KeyValuePair<string, string>("ParentFolderId", folderToFolderRequest.ParentFolderId.ToString()),
        new KeyValuePair<string, string>("Id", folderToFolderRequest.Id.ToString())
        });
    using HttpResponseMessage response = await client.PostAsync(
        $"API/MoveFolderToFolder",
        formContent);
    if (response.IsSuccessStatusCode)
    {
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Folder moved successfully: {responseBody}");
    }
    else
    {
        Console.WriteLine($"Error moving folder: {response.ReasonPhrase}");
    }
}

#endregion