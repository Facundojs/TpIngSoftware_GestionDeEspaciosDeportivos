using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class BackupFile
    {
        public string Nombre { get; set; }
        public string Fecha { get; set; }
        public long FileSize { get; set; }
    }
}
