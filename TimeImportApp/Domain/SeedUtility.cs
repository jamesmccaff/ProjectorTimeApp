using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeImportApp.Interfaces;

namespace TimeImportApp.Domain
{
    public class SeedUtility : ISeedUtility
    {
        public SeedUtility()
        {
        }

        public void SeedDatabase(ProjectorIntegrationContext projectorIntegrationContext)
        {
            SeedErrors(projectorIntegrationContext);
            SeedProjects(projectorIntegrationContext);
        }

        private void SeedProjects(ProjectorIntegrationContext context)
        {
            var projector1=new ProjectorProject { Code = "P002078-001", Name = "BRC Service Level Agreement 18/19" };
            var mipac1=new MIPACProject { Code = "BRCR0001S", Name = "BRC Service Level Agreement 18/19" };
            context.SharedProjects.Add(new SharedProject { MIPACProject = mipac1, ProjectorProject = projector1 });
            var projector2=new ProjectorProject { Code = "P002178-001", Name = "BRC Retainer 2019" };
            var mipac2= new MIPACProject { Code = "BRCR0002S", Name = "BRC Retainer 2019" };
            context.SharedProjects.Add(new SharedProject { MIPACProject = mipac2, ProjectorProject = projector2 });
            var projector3=new ProjectorProject { Code = "P002135-001", Name = "Hiscox" };
            var mipac3 = new MIPACProject { Code = "HCOX0004S", Name = "Hiscox" };
            context.SharedProjects.Add(new SharedProject { MIPACProject = mipac3, ProjectorProject = projector3 });
            var projector4=new ProjectorProject { Code = "P002143-001", Name = "CMB UK Tools Migration" };
            var mipac4 = new MIPACProject { Code = "HSBC0126S", Name = "CMB UK Tools Migration" };
            context.SharedProjects.Add(new SharedProject { MIPACProject = mipac4, ProjectorProject = projector4 });
            var projector5=new ProjectorProject { Code = "P002188-001", Name = "HSBC eCO - TCE 1.1" };
            var mipac5 = new MIPACProject { Code = "HSBC0129S", Name = "HSBC eCO - TCE 1.1" };
            context.SharedProjects.Add(new SharedProject { MIPACProject = mipac5, ProjectorProject = projector5 });
            var projector6=new ProjectorProject { Code = "P002189-001", Name = "HSBC eCO - CP 1.2" };
            context.SharedProjects.Add(new SharedProject { MIPACProject = mipac5, ProjectorProject = projector6 });
            var projector7=new ProjectorProject { Code = "P002190-001", Name = "HSBC eCO- Build Support" };
            context.SharedProjects.Add(new SharedProject { MIPACProject = mipac5, ProjectorProject = projector7 });
            var projector8=new ProjectorProject { Code = "P002191-001", Name = "HSBC eCO - CLM 1.3" };
            context.SharedProjects.Add(new SharedProject { MIPACProject = mipac5, ProjectorProject = projector8 });
            var projector9 = new ProjectorProject { Code = "P002205-001", Name = "HSBC DTC – New UI project" };
            var mipac9 = new MIPACProject { Code = "HSBC0124S", Name = "HSBC DTC – New UI project" };
            context.SharedProjects.Add(new SharedProject { MIPACProject = mipac9, ProjectorProject = projector9 });
            var projector10 = new ProjectorProject { Code = "P002030-001", Name = "HSBC DTC - Connect H2 2018" };
            var mipac10 = new MIPACProject { Code = "HSBC0125S", Name = "HSBC DTC - Connect H2 2018" };
            context.SharedProjects.Add(new SharedProject { MIPACProject = mipac10, ProjectorProject = projector10 });
            var projector11 = new ProjectorProject { Code = "P002192-001", Name = "JH Continued Support Q1/Q2 2019" };
            var mipac11 = new MIPACProject { Code = "JKHW0001S", Name = "JH Continued Support Q1/Q2 2019" };
            context.SharedProjects.Add(new SharedProject { MIPACProject = mipac11, ProjectorProject = projector11 });
            var projector12 = new ProjectorProject { Code = "P002148-001", Name = "Nuffield OLJ Enhancements Q2 2019" };
            var mipac12 = new MIPACProject { Code = "NUFH0002S", Name = "Nuffield OLJ Enhancements Q2 2019" };
            context.SharedProjects.Add(new SharedProject { MIPACProject = mipac12, ProjectorProject = projector12 });
            var projector13 = new ProjectorProject { Code = "P002032-001", Name = "Nuffield SLA 18/19" };
            var mipac13 = new MIPACProject { Code = "NUFH0001S", Name = "Nuffield SLA 18/19" };
            context.SharedProjects.Add(new SharedProject { MIPACProject = mipac13, ProjectorProject = projector13 });
            var projector14 = new ProjectorProject { Code = "P002220-001", Name = "Nuffield SLA - Out of Hours Q2 19" };
            var mipac14 = new MIPACProject { Code = "NUFH0003S", Name = "Nuffield SLA - Out of Hours Q2 19" };
            context.SharedProjects.Add(new SharedProject { MIPACProject = mipac14, ProjectorProject = projector14 });
            var projector15 = new ProjectorProject { Code = "P002155-001", Name = "WWF Membership TCE" };
            var mipac15 = new MIPACProject { Code = "WWFL0001S", Name = "WWF Membership TCE" };
            context.SharedProjects.Add(new SharedProject { MIPACProject = mipac15, ProjectorProject = projector15 });
            context.SaveChanges();
        }

        private void SeedErrors(ProjectorIntegrationContext context)
        {
            context.Errors.Add(new Error { Description = "", Type=ErrorType.IncorrectFormatFromComment});
            context.Errors.Add(new Error { Description = "", Type = ErrorType.JobInMappingTableButNotInProjector });
            context.Errors.Add(new Error { Description = "", Type = ErrorType.JobNotInMappingTable });
            context.Errors.Add(new Error { Description = "", Type = ErrorType.JobNotOnProjectorFromComment });
            context.SaveChanges();
        }
    }
}
