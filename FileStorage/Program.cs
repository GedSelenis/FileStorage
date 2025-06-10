using FileStorage.Models;
using System.Xml.Serialization;
using FileStorage.Services;
using FileStorage.Models.DTO;


IFileService fileService = new FileDetailsService();
ChoseOption();
void StartApplication()
{
    Console.WriteLine("Welcome to the File Storage Application!");
    Console.WriteLine("Please choose an option:");
    Console.WriteLine("1. Add File");
    Console.WriteLine("2. Rename File");
    Console.WriteLine("3. Add Text to File");
}

void PrintCurrentFiles()
{
    Console.WriteLine("Current Files:");
    List<FileResponse> files = fileService.ListFiles();
    if (files.Count == 0)
    {
        Console.WriteLine("No files available.");
    }
    else
    {
        foreach (var file in files)
        {
            Console.WriteLine($"ID: {file.Id}, Name: {file.FileName}, Path: {file.FilePath}");
        }
    }
}

void ChoseOption()
{
    PrintCurrentFiles();
    Console.WriteLine("Please choose an option:");
    Console.WriteLine("1. Add File");
    Console.WriteLine("2. Rename File");
    Console.WriteLine("3. Add Text to File");
    Console.WriteLine("4. Delete file");
    Console.WriteLine("5. Exit");
    String? input = Console.ReadLine();
    if (int.TryParse(input, out int option))
    {
        switch (option)
        {
            case 1:
                FileAddRequest fileAddRequest = InputAddRequest();
                fileService.AddFile(fileAddRequest);
                break;
            case 2:
                FileRenameRequest fileRenameRequest = InputRenameRequest();
                fileService.RenameFileAsync(fileRenameRequest);
                break;
            case 3:
                FileAddTextToFileRequest fileAddTextToFileRequest = InputAddTextToFile();
                fileService.AddTextToFileAsync(fileAddTextToFileRequest);
                break;
            case 4:
                FileResponse fileResponse = GetFileDetails();
                fileService.DeleteFile(fileResponse.Id);
                break;
            case 5:
                Console.WriteLine("Exiting the application. Goodbye!");
                return;
            default:
                Console.WriteLine("Invalid option. Please try again.");
                break;
        }
        ChoseOption();
    }
    else
    {
        Console.WriteLine("Invalid input. Please enter a number between 1 and 4.");
        ChoseOption();
    }
}

FileAddRequest InputAddRequest()
{
    Console.WriteLine("Enter file name:");
    string? fileName = Console.ReadLine();
    Console.WriteLine("Enter file path:");
    string? filePath = Console.ReadLine();
    return new FileAddRequest
    {
        FileName = fileName,
        FilePath = filePath
    };
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

FileResponse GetFileDetails()
{
    Console.WriteLine("Enter file ID to get details:");
    string? fileIdInput = Console.ReadLine();
    if (Guid.TryParse(fileIdInput, out Guid fileId))
    {
        return fileService.GetFileDetails(fileId);
    }
    else
    {
        throw new ArgumentException("Invalid file ID format.");
    }
}