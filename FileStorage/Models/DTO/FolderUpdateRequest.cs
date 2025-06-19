using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Models.DTO
{
    public class FolderUpdateRequest
    {
        public Guid Id { get; set; }
        public string? FolderName { get; set; }

    }
}
