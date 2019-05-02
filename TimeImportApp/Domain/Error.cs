using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeImportApp.Domain
{
    public class Error
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public int ErrorID { get; set; }
        public string Description { get; set; } 
        public ErrorType Type { get; set; }
    }
}
