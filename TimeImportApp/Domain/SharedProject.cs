using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeImportApp
{
    public class SharedProject
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public int SharedProjectID { get; set; }
        public MIPACProject MIPACProject { get; set; }
        public ProjectorProject ProjectorProject { get; set; }
    }
}
