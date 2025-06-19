using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Models.DTO
{
    public class FileToFolderRequest
    {
        public Guid Id { get; set; }
        public Guid? VirualFolderId { get; set; }
    }
}
