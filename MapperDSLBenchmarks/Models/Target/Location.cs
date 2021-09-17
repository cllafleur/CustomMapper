namespace MapperDslUI.Models.Target
{
    public class Location
    {
        public string Address { get; set; }
        public Coordinates Coordinates { get; set; } = new Coordinates();
    }
}