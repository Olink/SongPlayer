using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TShockAPI;

namespace MusicPlayer.Music
{
    class NoteFileParser
    {
        public static int Tempo = 250;

        public static List<List<Note>> Read(string path)
        {
            List<List<Note>> Notes = new List<List<Note>>();
            using (var reader = new StreamReader(path))
            {
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    List<Note> ret = new List<Note>();
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
                    Notes.Add(ret);
                }
            }
            return Notes;
        }
    }
}
