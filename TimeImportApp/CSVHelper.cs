using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TimeImportApp
{
    public class CSVHelper : ICSVHelper
    {
        public CSVHelper()
        {

        }

        public List<Person> ParseReportToObjects(string reportFilePath)
        {
            string line;
            var people = new List<Person>();
            FileStream aFile = new FileStream(reportFilePath, FileMode.Open);
            StreamReader streamReader = new StreamReader(aFile);
            // read data in line by line
            while ((line = streamReader.ReadLine()) != null)
            {
                //If Person parse person line
                if (IsPerson(line))
                {
                    var currentPerson = ExtractPersonFromCSVLine(line, streamReader);
                }
            }
            streamReader.Close();
            return people;
        }

        private Person ExtractPersonFromCSVLine(string line, StreamReader streamReader)
        {
            string personLine = line.Split(',').FirstOrDefault(a => !String.IsNullOrWhiteSpace(a));
            string[] personDetails = personLine.Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
            string fullName = personDetails[0];
            string personCode = personDetails[1].Replace("(", string.Empty).Replace(")", string.Empty);

            Person person = new Person
            {
                Name = fullName,
                Code = personCode,
                Jobs = new List<Job>()
            };

            var innerLinesForPerson = ReadInnerLines(streamReader);
            person.Jobs.AddRange(GetJobs(innerLinesForPerson));
            return person;
        }

        private List<Job> GetJobs(List<string> innerLinesForPerson)
        {
            Job currentJob = null;
            var jobsList = new List<Job>();
            foreach (var innerLine in innerLinesForPerson)
            {
                //Extract Job
                if (IsJob(innerLine))
                {
                    currentJob = GetJob(innerLine);
                    jobsList.Add(currentJob);
                }
                else if (IsJobComment(innerLine))
                {
                    if (currentJob != null)
                    {
                        currentJob.Comment = GetJobComment(innerLine);
                    }
                }
            }
            return jobsList;
        }

        private string GetJobComment(string innerLine)
        {
            IEnumerable<string> stringArray = innerLine.Split(',').Where(a => !String.IsNullOrWhiteSpace(a));
            return stringArray.FirstOrDefault();
        }

        private Job GetJob(string innerLine)
        {
            Job job = new Job();
            IEnumerable<string> stringArray = innerLine.Split(',');
            job.Code = stringArray.ElementAtOrDefault(0);
            job.Name = stringArray.ElementAtOrDefault(1);
            job.ClientName = stringArray.ElementAtOrDefault(2);
            job.Type = stringArray.ElementAtOrDefault(6);
            job.SoldHours = Double.Parse(stringArray.ElementAtOrDefault(7));
            job.NonSoldHours = Double.Parse(stringArray.ElementAtOrDefault(8));
            job.TrainingJobHours = Double.Parse(stringArray.ElementAtOrDefault(9));
            job.UnresolvedHours = Double.Parse(stringArray.ElementAtOrDefault(10));
            job.IllnessHours = Double.Parse(stringArray.ElementAtOrDefault(11));
            job.StatutoryHolidayHours = Double.Parse(stringArray.ElementAtOrDefault(12));
            job.AnnualHolidayHours = Double.Parse(stringArray.ElementAtOrDefault(13));
            job.TrainingHours = Double.Parse(stringArray.ElementAtOrDefault(14));
            job.RecruitmentHours = Double.Parse(stringArray.ElementAtOrDefault(15));
            job.OtherHours = Double.Parse(stringArray.ElementAtOrDefault(16));
            job.UnutilisedHours = Double.Parse(stringArray.ElementAtOrDefault(17));
            job.ExternalBillingRate = Double.Parse(stringArray.ElementAtOrDefault(19));
            return job;
        }

        private bool IsJobComment(string jobCommentLine)
        {
            IEnumerable<string> stringArray = jobCommentLine.Split(',').Where(a => !String.IsNullOrWhiteSpace(a));
            return (stringArray.Count() == 1 && !String.IsNullOrEmpty(stringArray.FirstOrDefault()));
        }

        private List<String> ReadInnerLines(StreamReader sr)
        {
            string line;
            List<String> innerLineList = new List<String>();
            while ((line = sr.ReadLine()) != null && !IsEmptyLine(line))
            {
                innerLineList.Add(line);
            }
            return innerLineList;
        }

        private bool IsEmptyLine(string line)
        {
            int count = line.Split(new[] { ',', ',' }, StringSplitOptions.RemoveEmptyEntries).Count();
            return count == 0;
        }

        private bool IsPerson(string personLine)
        {
            if (personLine.Contains('(') && personLine.Contains(')'))
            {
                IEnumerable<string> stringArray = personLine.Split(',').Where(a => !String.IsNullOrWhiteSpace(a));
                return (personLine[0] != ',' && stringArray.Count() == 1);
            }
            return false;
        }

        private static bool IsJob(string jobLine)
        {
            IEnumerable<string> stringArray = jobLine.Split(',');
            string jobType = stringArray.ElementAtOrDefault(6);

            if (jobType != null)
            {
                return (String.Equals(jobType, "r", StringComparison.OrdinalIgnoreCase)
                    || String.Equals(jobType, "t", StringComparison.OrdinalIgnoreCase)
                    || String.Equals(jobType, "u", StringComparison.OrdinalIgnoreCase)
                    || String.Equals(jobType, "a", StringComparison.OrdinalIgnoreCase));
            }

            return false;
        }

    }
}
