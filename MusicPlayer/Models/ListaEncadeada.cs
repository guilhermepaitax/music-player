using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models
{
    public class ListaEncadeada<T> where T : Node<T>
    {
        private T head;
        private int count;

        public ListaEncadeada()
        {
            this.head = null;
            this.count = 0;
        }

        public int Count { get => count; }

        public T First()
        {
            return head;
        }

        public T Last()
        {
            return head.Previous;
        }

        public void AddFirst(T node)
        {
            if (head == null)
            {
                node.Next = node;
                node.Previous = node;
                head = node;
            }
            else
            {
                node.Next = head;
                node.Previous = head.Previous;
                head.Previous.Next = node;
                head.Previous = node;
                head = node;
            }
            count++;
        }

        public void AddLast(T node)
        {
            if (head == null)
            {
                node.Next = node;
                node.Previous = node;
                head = node;
            }
            else
            {
                node.Next = head;
                node.Previous = head.Previous;
                head.Previous.Next = node;
                head.Previous = node;
            }
            count++;
        }

        public void RemoveFirst()
        {
            if (head == null) return;
            if (head.Next == head) head = null;
            else
            {
                head.Next.Previous = head.Previous;
                head.Previous.Next = head.Next;
                head = head.Next;
            }
            count--;
        }

        public void RemoveLast()
        {
            if (head == null) return;
            if (head.Next == head) head = null;
            else
            {
                head.Previous = head.Previous.Previous;
                head.Previous.Previous.Next = head;
            }
            count--;
        }

        public void Remove(T node)
        {
            if (head == null) return;
            if (node.Next == node) head = null;
            else
            {
                node.Next.Previous = node.Previous;
                node.Previous.Next = node.Next;
            }
            count--;
        }


    }
}
