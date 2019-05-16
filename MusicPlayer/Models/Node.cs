using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models
{
    public class Node<T> where T : Node<T>
    {
        private T next;
        private T previous;
        public T Next { get => next; set => next = value; }
        public T Previous { get => previous; set => previous = value; }
    }
}
