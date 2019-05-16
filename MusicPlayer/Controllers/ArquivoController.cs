using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MusicPlayer.Models;

namespace MusicPlayer.Controllers
{
    public class ArquivoController
    {

        public static void BuscarArquivos(ref ListaEncadeada<Arquivo> arquivos, string caminho, string extensao)
        {
            DirectoryInfo folder = new DirectoryInfo(caminho);
            Arquivo.ListarFiles(folder.GetFiles(), folder.GetFiles().Length - 1, ref arquivos);
            Arquivo.Listarpastas(folder.GetDirectories(), folder.GetDirectories().Length - 1, ref arquivos);
        }

    }
}
