using GuitLong.Code.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace GuitLong.Code.Models
{
    public class ConvertToSongData
    {
        public string json = "";
        public ConvertToSongData(string _json = "") {
            json = _json;
        }
        
        public async Task<Song> CreateSongData(string _json, Song songBase) 
        {
            await File.WriteAllTextAsync($"{songBase.Title}.json", _json);
            
            songBase = await findUsedChords(_json, songBase);
            songBase = await findSections(_json, songBase);
            songBase = await findRows(_json, songBase);

            return songBase;

        }
        public async Task<Song> findUsedChords(string _json, Song songBase)
        {
            string[] rows = _json.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int startIndex = 0;
            int endIndex = _json.IndexOf("[", startIndex) - 1;
            for(int i = 0; i < rows.Length; i++)
            {
                string row = rows[i].Trim();
                if (row.Length == 0)
                {
                    continue;
                }
                if (row.StartsWith("[") && row.EndsWith("]"))
                {
                    break;
                }

                string[] words = row.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (words[0].Length == 0)
                {
                    break;
                } else
                {
                    songBase.Chords.Add(words[0]);
                }

            }


            return songBase;
        }
        public async Task<Song> findRows(string _json, Song songBase)
        {
            foreach(var section in songBase.Sections)
            {
                int startIndex = _json.IndexOf(section.Name) + section.Name.Length;
                int endIndex = _json.IndexOf("[", startIndex) - 1;

                if(endIndex == -1 || endIndex == -1)
                {
                    throw new Exception("Error: Could not find the start or end of the section in the JSON string.");
                }

                string sectionContent = _json.Substring(startIndex, endIndex - startIndex);
                string[] rows = sectionContent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                List<Chord> lastRowChords = new List<Chord>();
                foreach (var row in rows)
                {
                    if(row.Trim().Length == 0)
                    {
                        continue;
                    }
                    bool isChordRow = false;

                    foreach (var chordsFound in songBase.Chords)
                    {
                        if ((" " + row + " ").Contains($" {chordsFound} "))
                        {
                            isChordRow = true;
                            
                        }
                    }

                    if (isChordRow)
                    {
                        
                        List<Chord> chordsInRow = new List<Chord>();

                        for (int i = 0; i < row.Length; i++)
                        {
                            foreach (var chord in songBase.Chords)
                            {
                                if (row.Substring(i).StartsWith(chord))
                                {
                                    Chord newChord = new Chord
                                    {
                                        Name = chord,
                                        CharacterPosition = i
                                    };
                                    chordsInRow.Add(newChord);
                                }
                            }
                        }
                        lastRowChords = chordsInRow;

                        continue;

                    }
                    Row newRow = new Row
                        {
                            Chords = (List<Chord>)lastRowChords,
                            Lyrics = row.Trim()
                        };
                    
                    
                    section.Rows.Add(newRow);
                }

            }
            return songBase;
        }
        public async Task<Song> findSections(string _json, Song songBase)
        {
            int lastIndex = 0;
            for (int i = 0; i < _json.Length; i++)
            {
                if (_json[i] == '[')
                {
                    int endIndex = _json.IndexOf(']', i);
                    if (endIndex != -1)
                    {
                        string sectionJson = _json.Substring(i, endIndex - i + 1);
                        Section section = new Section
                        {
                            Name = sectionJson
                        };

                        songBase.Sections.Add(section);

                        i = endIndex;
                    }
                }

            }
            return songBase;
        }
        public string TryConvert()
        {
            json = CleanString();

            return json;
        }
        public string CleanString()
        {
            string[] removeSubstrings = { "[tab]", "[/tab]", "[ch]", "[/ch]" };

            string currentJson = json;
            for (int i = 0; i < removeSubstrings.Length; i++)
            {
                currentJson = RemovedFromString(currentJson, removeSubstrings[i]);
            }

            return currentJson;
        }
        string RemovedFromString(string jsonProvided, string remove) 
        {
            for (int i = 0; i <= jsonProvided.Length - remove.Length; i++)
            {
                bool found = true;
                for(int j = 0; j < remove.Length; j++)
                {
                    if (jsonProvided[i + j] != remove[j])
                    {
                        found = false;
                        break;
                    }
                }

                if (found) 
                {
                    jsonProvided = jsonProvided.Remove(i, remove.Length);
                }
            }
            return jsonProvided;
        }
        
    }
}
