namespace GuitLong.Code.Models
{
    public class Row 
    { 
        public List<Chord> Chords { get; set; } = new List<Chord>();
        public string Lyrics { get; set; } = "";

        public string GetChordText()
        {
            int length = Chords[Chords.Count - 1].CharacterPosition + Chords[Chords.Count - 1].Name.Length;
            string chordText = String.Concat(Enumerable.Repeat(" ", length));


            foreach (Chord chord in Chords)
            {
                string newText = chordText.Remove(chord.CharacterPosition, chord.Name.Length);
                newText = newText.Insert(chord.CharacterPosition, chord.Name);
                chordText = newText;
            }

            return chordText;
        }

        public string GetLyricsText()
        {
            return Lyrics;
        }
    }


}
