using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using TShockAPI;

namespace MusicPlayer.Music
{
    class NoteFileParser
    {
        public static int Tempo = 250;

        public static List<List<Note>> Read(string path, out int tempo)
        {
            List<List<Note>> Notes = new List<List<Note>>();
            int t = Tempo;
            using (var reader = new StreamReader(path))
            {
                string line = "";
                bool readTempo = false;
                while ((line = reader.ReadLine()) != null)
                {
	                if ((line.Trim())[0] == '#')
	                {
						//this line is a comment, skip
		                continue;
	                }

                    if (!readTempo)
                    {
                        int tryOut = 0;
                        if (int.TryParse(line, out tryOut))
                        {
                            t = tryOut;
                        }

                        readTempo = true;
                        continue;
                    }

                    List<Note> ret = new List<Note>();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        foreach (var note in line.Split(',').ToList())
                        {
                            try
                            {
                                ret.Add(new Note()
                                            {
                                                Value = NoteName.GetNoteByName(note)
                                            }
                                        );
                            }
                            catch (ArgumentException e)
                            {
                                Log.ConsoleError("Failed to read note: {0}", note );
                            }
                        }
                    }
                    {
                        ret.Add(new Note()
                                    {
                                        Value = -2   
                                    }
                                );
                    }
                    Notes.Add(ret);
                }
            }
            tempo = t;
            return Notes;
        }
    }
}
