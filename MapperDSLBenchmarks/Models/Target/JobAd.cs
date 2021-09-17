namespace MapperDslUI.Models.Target
{
    public class JobAd
    {
        public string Reference { get; set; }
        public JobAdDetails JobAdDetails { get; set; } = new JobAdDetails();
        public Location Location { get; set; } = new Location();
        public Properties.Properties Properties { get; set; } = new Properties.Properties();
    }
}
