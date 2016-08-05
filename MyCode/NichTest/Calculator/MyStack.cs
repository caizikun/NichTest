using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace NichTest
{
    class MyStack<T> : IEnumerable<T>
    {
        private Node first;
        private int N = 0;

        private class Node
        {
            public T item;
            public Node next;
        }

        public int Size()
        {
            return N;
        }

        public bool IsEmpty()
        {
            return N == 0 ? true : false;
        }

        public void Push(T item)
        {
            if (N == 0)
            {
                first = new Node();
                first.item = item;
                first.next = null;
            }
            else
            {
                Node oldFirst = new Node();
                oldFirst.item = first.item;
                oldFirst.next = first.next;
                first.item = item;
                first.next = oldFirst;
            }
            N++;
        }

        public void Clear()
        {
            while (N != 0)
            { 
                Pop();
                N--;
            }
        }

        public T Pop()
        {
            T item = first.item;
            first = first.next;
            N--;
            return item;
        }

        // Must implement GetEnumerator, which returns a new StreamReaderEnumerator.
        public IEnumerator<T> GetEnumerator()
        {
            while (N > 0)
            {
                yield return Pop();
            }
        }
        // Must also implement IEnumerable.GetEnumerator, but implement as a private method.        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

