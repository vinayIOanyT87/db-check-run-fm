using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMExportService.DTO
{
    public class FtpThreadLog
    {
        public DateTime Published { get; set; }
        public bool LastEventBeforeLoop { get; set; }
        public string Description { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
