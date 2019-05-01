namespace TimeImportApp
{
    public class Job
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ClientName { get; set; }
        public string Comment { get; set; }
        public string Type { get; set; }
        public double SoldHours { get; set; }
        public double NonSoldHours { get; set; }
        public double TrainingJobHours { get; set; }
        public double UnresolvedHours { get; set; }
        public double IllnessHours { get; set; }
        public double StatutoryHolidayHours { get; set; }
        public double AnnualHolidayHours { get; set; }
        public double TrainingHours { get; set; }
        public double RecruitmentHours { get; set; }
        public double OtherHours { get; set; }
        public double UnutilisedHours { get; set; }
        public double ExternalBillingRate { get; set; }
    }
}