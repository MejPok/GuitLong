namespace GuitLong.Code.Models.Strumming
{
    public class StrummingPattern
    {
        public double BPM { get; set; } = 120;
        public List<Strum> Strums { get; set; } = new();
    }


}
