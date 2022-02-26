namespace DLD
{

    public class Queue<T>
    {
        private T[] data;
        private int size;
        private readonly int minSize;

        public Queue()
        {
            size = 0;
            minSize = 16;
            this.data = new T[16];
        }

        public Queue(int startingSize)
        {
            this.size = 0;
            this.minSize = startingSize;
            this.data = new T[startingSize];
        }


        public void Push(T entry)
        {
            if (size >= data.Length) Grow();
            data[size] = entry;
            size++;
        }


        public T Pop()
        {
            T ouput = data[0];
            for (int i = 1; i < size; i++) data[i - 1] = data[i];
            size--;
            if (size < System.Math.Max(minSize, data.Length / 4)) Shrink();
            return ouput;
        }


        public void Clear()
        {
            // If size is 0 then existing data will simply be overritten by 
            // new data push into the queue.
            size = 0;
        }


        public T Peek => data[0];
        public int Count => size;
        public int Capacity => data.Length;
        public bool Empty => size < 1;
        public bool NotEmpty => size > 0;


        private void Grow()
        {
            T[] array = new T[(size * 3) / 2];
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
