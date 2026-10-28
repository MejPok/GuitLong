namespace GuitLong.Code.Models.Strumming
{
    public class StrummingPattern
    {
        public double BPM { get; set; } = 120;
        public List<Strum> Strums { get; set; } = new();

        public string GetStrumString()
        {
            string strumString = "";
            foreach (Strum strum in Strums)
            {
                if(strum.Direction == StrumDirection.Down)
                {
                    strumString += "D";
                }
                else if (strum.Direction == StrumDirection.Up)
                {
                    strumString += "U";
                }
                else
                {
                    strumString += "-";
                }
            }
            return strumString;
        }
    }


}
