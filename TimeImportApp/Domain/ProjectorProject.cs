using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeImportApp
{
    public class ProjectorProject
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public int ProjectorProjectID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
