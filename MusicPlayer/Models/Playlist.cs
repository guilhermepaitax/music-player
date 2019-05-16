using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models
{
    public class Playlist : Node<Playlist>
    {
        private string name;
        private string duration;
        private int bitRate;
        private string file;
        private string artist;

        public Playlist(string name, string duration, int bitRate, string file, string artist)
        {
            this.name = name;
            this.duration = duration;
            this.bitRate = bitRate;
            this.file = file;
            this.Artist = artist;
            Next = null;
            Previous = null;
        }

        public string Name { get => name; set => name = value; }
        public string Duration { get => duration; set => duration = value; }
        public int BitRate { get => bitRate; set => bitRate = value; }
        public string File { get => file; set => file = value; }
        public string Artist { get => artist; set => artist = value; }

        public static void CriarPlaylist(Arquivo music, Arquivo head, ref ListaEncadeada<Playlist> playlist)
        {
            TagLib.File tagFile = TagLib.File.Create(music.File);
            Playlist pl = new Playlist(tagFile.Tag.Title, tagFile.Properties.Duration.ToString(@"mm\:ss"), tagFile.Properties.AudioBitrate, music.File, tagFile.Tag.FirstPerformer);
            playlist.AddLast(pl);

            if (music.Next != head)
            {
                CriarPlaylist(music.Next, head, ref playlist);
            }

        }

    }
}
