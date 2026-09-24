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
