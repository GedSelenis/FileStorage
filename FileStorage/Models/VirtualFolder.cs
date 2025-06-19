using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Models
{
    public class VirtualFolder
    {
        public Guid Id { get; set; }
        public Guid? ParentFolderId { get; set; }
        public string? FolderName { get; set; }

    }
}
