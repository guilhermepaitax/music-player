using CSCore;
using CSCore.Codecs;
using CSCore.DSP;
using CSCore.Streams;
using CSCore.Streams.Effects;
using MusicPlayer.Controllers;
using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicPlayer
{
    public partial class FormMain : Form
    {
        //private Arquivo head;
        private Playlist musicaAtiva = null;
        private bool tocando = false, aleatorio = false, circular = true;
        private ListaEncadeada<Arquivo> listaArquivos = new ListaEncadeada<Arquivo>();
        private ListaEncadeada<Playlist> listaPlaylist = new ListaEncadeada<Playlist>();

        public FormMain()
        {
            InitializeComponent();
        }

        //Buscar musicas
        private void btnFolders_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                ArquivoController.BuscarArquivos(ref listaArquivos, folderBrowserDialog.SelectedPath, ".mp3");
                if(listaArquivos.Count > 0)
                {
                    Playlist.CriarPlaylist(listaArquivos.First(), listaArquivos.First(), ref listaPlaylist);
                    treeView1.Nodes.Clear();
                    ListarMusicas(listaPlaylist.First());
                }
            }
        }
        //Listar musicas na TreeView
        private void ListarMusicas(Playlist music)
        {
            if (music == null) return;

            TreeNode node = new TreeNode(music.Name);
            node.ImageIndex = 0;
            node.SelectedImageIndex = 0;
            node.Nodes.Add("Duração: " + music.Duration).ImageIndex = 1;
            node.Nodes.Add("BitRate: " + music.BitRate).ImageIndex = 2;
            node.Nodes.Add("Arquivo: " + music.File).ImageIndex = 3;
            node.Nodes.Add("Artista: " + music.Artist).ImageIndex = 4;
            node.Nodes[0].SelectedImageIndex = 1;
            node.Nodes[1].SelectedImageIndex = 2;
            node.Nodes[2].SelectedImageIndex = 3;
            node.Nodes[3].SelectedImageIndex = 4;
            treeView1.Nodes.Add(node);
            if (music.Next != listaPlaylist.First())
            {
                ListarMusicas(music.Next);
            }
        }

        //Verifica se existe alguma musica
        private bool ValidaControle()
        {
            return (listaPlaylist.First() != null);
        }

        //Ações para tocar as musicas
        private void TocarMusica(string acao)
        {
            if (musicaAtiva == null) musicaAtiva = listaPlaylist.First();
            if (acao.Equals("inicio")) musicaAtiva = listaPlaylist.First();
            else if (acao.Equals("prox")) musicaAtiva = musicaAtiva.Next;
            else if (acao.Equals("anterior")) musicaAtiva = musicaAtiva.Previous;
            else if (acao.Equals("fim")) musicaAtiva = listaPlaylist.Last();
            tocando = true;
            timer.Start();
            btnPlay.BackgroundImage = Properties.Resources.icon_pause;
            MusicController.Play(musicaAtiva.File);
            lblMusicName.Text = musicaAtiva.Name;
            lblArtist.Text = musicaAtiva.Artist;
        }

        //Seleciona musica aleatoria
        private void MusicaAleatoria()
        {
            Playlist tempMusica = musicaAtiva;
            Random random = new Random();
            for(int i = 0; i < random.Next(listaPlaylist.Count); i++)
            {
                musicaAtiva = musicaAtiva.Next;
            }
            if (musicaAtiva == tempMusica) musicaAtiva = musicaAtiva.Next;
        }


        //Tempo da musica
        private void timer_Tick(object sender, EventArgs e)
        {
            if (musicaAtiva != null)
            {
                //pictureBoxLine.Image = MusicController.GenerateLineSpectrum(pictureBoxLine.Image, pictureBoxLine.Size);
                GenerateLineSpectrum();
                string total, atual;
                total = musicaAtiva.Duration;
                atual = MusicController.CurrentTime();
                if (total == atual && circular) TocarMusica("prox");
                else if (total == atual && !circular && !aleatorio)
                {
                    tocando = false;
                    atual = "00:00";
                    btnPlay.BackgroundImage = Properties.Resources.icon_play;
                }
                else if (total == atual && aleatorio)
                {

                    MusicaAleatoria();
                    TocarMusica("");
                }
                lblMusicTime.Text = atual + "/" + total;
            }
        }

        /////////////Controles de Midia/////////////

        //Play
        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (ValidaControle())
            {
                if (!tocando)
                {
                    if (musicaAtiva == null) TocarMusica("inicio");
                    tocando = true;
                    btnPlay.BackgroundImage = Properties.Resources.icon_pause;
                    MusicController.Play();
                    timer.Start();
                }
                else
                {
                    tocando = false;
                    btnPlay.BackgroundImage = Properties.Resources.icon_play;
                    MusicController.Pause();
                    timer.Stop();
                }
            }
        }

        //Proxima Musica
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (ValidaControle()) TocarMusica("prox");
        }

        //Primeira Musica
        private void btnFirst_Click(object sender, EventArgs e)
        {
            if (ValidaControle()) TocarMusica("inicio");
        }

        //Ultima musica
        private void btnLast_Click(object sender, EventArgs e)
        {
            if (ValidaControle()) TocarMusica("fim");
        }

        //Parar
        private void btnStop_Click(object sender, EventArgs e)
        {
            if (ValidaControle())
            {
                timer.Stop();
                MusicController.Stop();
                musicaAtiva = null;
                btnPlay.BackgroundImage = Properties.Resources.icon_play;
                tocando = false;
                lblArtist.Text = "";
                lblMusicName.Text = "";
                lblMusicTime.Text = "00:00/00:00";
                pictureBoxLine.Image = null;
            }
        }

        //Musica anterior
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (ValidaControle()) TocarMusica("anterior");
        }

        //Ativa ou desativa modo Aleatorio
        private void btnAleatorio_Click(object sender, EventArgs e)
        {
            if (!aleatorio)
            {
                aleatorio = true;
                circular = false;
                btnAleatorio.BackgroundImage = Properties.Resources.icon_random_true;
                btnCircular.BackgroundImage = Properties.Resources.icon_circ_false;
            }
            else
            {
                aleatorio = false;
                btnAleatorio.BackgroundImage = Properties.Resources.icon_random_false;
            }
        }

        //Ativa ou desativa modo Circular
        private void btnCircular_Click(object sender, EventArgs e)
        {
            if (!circular)
            {
                aleatorio = false;
                circular = true;
                btnAleatorio.BackgroundImage = Properties.Resources.icon_random_false;
                btnCircular.BackgroundImage = Properties.Resources.icon_circ_true;
            }
            else
            {
                circular = false;
                btnCircular.BackgroundImage = Properties.Resources.icon_circ_false;
            }
        }

        //Para de tocar quando fechar
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            MusicController.Dispose();
        }

        //Gerar Ondas no Form
        private void GenerateLineSpectrum()
        {
            Image image = pictureBoxLine.Image;
            var newImage = MusicController.LineSpectrum.CreateSpectrumLine(pictureBoxLine.Size, Color.GreenYellow, Color.HotPink, Color.Black, true);
            if (newImage != null)
            {
                pictureBoxLine.Image = newImage;
                if (image != null)
                    image.Dispose();
            }
        }

    }
}
