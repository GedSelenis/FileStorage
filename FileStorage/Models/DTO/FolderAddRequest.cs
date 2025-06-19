using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Models.DTO
{
    public class FolderAddRequest
    {
        public string? FolderName { get; set; }

        public VirtualFolder ToVirtualFolder()
        {
            return new VirtualFolder { FolderName = FolderName };
        }
    }
}
