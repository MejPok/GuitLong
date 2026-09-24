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
            
            songBase = await findSections(_json, songBase);



            return songBase;

        }

        public async Task<Song> findRows(string _json, Song songBase)
        {
            foreach(var section in songBase.Sections)
            {
                int startIndex = _json.IndexOf(section.Name);
                if (startIndex != -1)
                {
                    int endIndex = _json.IndexOf(']', startIndex);
                    if (endIndex != -1)
                    {
                        string sectionContent = _json.Substring(startIndex + section.Name.Length, endIndex - startIndex - section.Name.Length);
                        string[] rows = sectionContent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var row in rows)
                        {
                            Row newRow = new Row
                            {
                                Lyrics = row.Trim()
                            };
                            section.Rows.Add(newRow);
                        }
                    }
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
            for (int i = 0; i < jsonProvided.Length - remove.Length; i++)
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
