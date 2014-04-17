using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerrariaApi.Server;
using TShockAPI;

namespace MusicPlayer.Music
{
    class Song
    {
        public string Name { get; set; }

        private List<List<Note>> notesToPlay = new List<List<Note>>();

        private SongPlayer player;
        private int currentPlayTime = 0;
        private bool isPlaying = false;
        private TerrariaPlugin plugin;
        private int tempo = NoteFileParser.Tempo;

        public Song(TerrariaPlugin plugin, String name, String path, SongPlayer ply)
        {
            Name = name;
            notesToPlay = NoteFileParser.Read(path, out tempo);
            player = ply;
            this.plugin = plugin;
        }

        private int delta = 0;
        private DateTime lastUpdate = DateTime.Now;
        public void Update(EventArgs args)
        {
            if (!isPlaying)
                return;

            if (notesToPlay.Count == 0)
            {
                EndSong();
                return;
            }

            delta += (int)((DateTime.Now - lastUpdate).TotalMilliseconds);
            if (delta > tempo)
            {
                List<Note> notes = notesToPlay[0];
                notesToPlay.RemoveAt(0);
                foreach (var note in notes)
                {
                    if (note.value >= -1 && note.value <= 1)
                        PlayNote(note.Value);
                }
                delta -= tempo;
            }

            currentPlayTime += (int)delta;

            lastUpdate = DateTime.Now;
        }

        public void StartSong()
        {
            isPlaying = true;
            player.Listening = true;
            currentPlayTime = 0;
            lastUpdate = DateTime.Now;
            ServerApi.Hooks.GamePostUpdate.Register(plugin, Update);
            player.Player.SendData(PacketTypes.Status, "Currently Playing: " + Name, 0);
        }

        public void EndSong()
        {
            isPlaying = false;
            ServerApi.Hooks.GamePostUpdate.Deregister(plugin, Update);
            player.Player.SendData(PacketTypes.Status, "");
            player.Listening = false;
        }

        private void PlayNote(float note)
        {
            player.Player.SendData((PacketTypes)58, "", player.Player.Index, note);
        }
    }
}
