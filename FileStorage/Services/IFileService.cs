﻿using FileStorage.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Services
{
    public interface IFileService
    {
        public FileResponse AddFile(FileAddRequest fileAddRequest);
        public FileResponse RenameFileAsync(FileRenameRequest fileRenameRequest);
        public Task<FileResponse> AddTextToFileAsync(FileAddTextToFileRequest fileAddTextToFileRequest);
        public bool DeleteFile(Guid? fileId);
        public List<FileResponse> ListFiles();
        public FileResponse GetFileDetails(Guid? fileId);
    }
}
