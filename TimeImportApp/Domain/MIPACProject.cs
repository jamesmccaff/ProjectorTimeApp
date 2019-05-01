using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeImportApp
{
    public class MIPACProject
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public int MIPACProjectID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
