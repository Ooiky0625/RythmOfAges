using System.Collections.Generic;

namespace VRSDK.Collections
{
    public class Buffer<T>
    {
        private List<T> elements;
        private int size;

        public T this[int index]
        {
            get
            {
                return elements[index];
            }

            set
            {
                elements[index] = value;
            }
        }

        public int Count { get { return elements.Count; } }

        public Buffer(int size)
        {
            this.size = size;
            elements = new List<T>( size );
        }

        public void Add(T item)
        {
            elements.Add( item );

            if (elements.Count > size)
            {
                elements.RemoveAt( 0 );
            }
        }

        public void Remove(T item)
        {
            elements.Remove( item );
        }

        public List<T> Sample(int size)
        {
            if (size > elements.Count)
                return elements;

            return elements.GetRange(elements.Count - size , size);
        }
    }

}

