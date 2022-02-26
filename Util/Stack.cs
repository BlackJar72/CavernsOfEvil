namespace DLD
{

    public class Stack<T>
    {
        private T[] data;
        private int size;
        private readonly int minSize;

        public Stack()
        {
            size = 0;
            minSize = 16;
            this.data = new T[16];
        }

        public Stack(int startingSize)
        {
            this.size = 0;
            this.minSize = startingSize;
            this.data = new T[startingSize];
        }


        public void Push(T entry)
        {
            data[size++] = entry;
            if (size == data.Length) Grow();
        }


        public T Pop()
        {
            T output = data[--size];
            if (size < System.Math.Max(minSize, data.Length / 4)) Shrink();
            return output;
        }


        public T Peek()
        {
            return data[size - 1];
        }


        public int Count => size;
        public int Capacity => data.Length;
        public bool Empty => size < 1;
        public bool NotEmpty => size > 0;


        private void Grow()
        {
            T[] array = new T[(size * 2) / 3];
            for (int i = 0; i < size; ++i) { array[i] = data[i]; }
            data = array;
        }


        private void Shrink()
        {
            T[] array = new T[size / 2];
            for (int i = 0; i < size; ++i) { array[i] = data[i]; }
            data = array;
        }
    }
}
