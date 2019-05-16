using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models
{
    public class Arquivo : Node<Arquivo>
    {

        private string file;

        public Arquivo(string file)
        {
            this.File = file;
            Next = null;
            Previous = null;
        }

        public string File { get => file; set => file = value; }

        public static Boolean ListarFiles(FileInfo[] fileInfos, int cont, ref ListaEncadeada<Arquivo> arquivos)
        {
            if (cont < 0)
                return true;
            if (fileInfos[cont].Extension.Equals(".mp3"))
            {
                Arquivo arquivo = new Arquivo(fileInfos[cont].FullName);
                arquivos.AddFirst(arquivo);
            }
            ListarFiles(fileInfos, cont - 1, ref arquivos);
            return true;
        }

        public static Boolean Listarpastas(DirectoryInfo[] folders, int cont, ref ListaEncadeada<Arquivo> arquivos)
        {
            if (cont < 0) return true;

            ListarFiles(folders[cont].GetFiles(), folders[cont].GetFiles().Length - 1, ref arquivos);
            Listarpastas(folders[cont].GetDirectories(), folders[cont].GetDirectories().Length - 1, ref arquivos);
            Listarpastas(folders, cont - 1, ref arquivos);
            return true;
        }

    }
}
