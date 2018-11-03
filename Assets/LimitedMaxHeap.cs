// structure of LimitedMaxHeap is same as MaxHeap, but it is Limited & throws biggest items out (so only minimums are left!!)
namespace DataStructures {

    // array start at index 1
    public class LimitedMaxHeap {

        int nodesCount;
        int maxSize;

        float[] heap;

        public LimitedMaxHeap(int maxNodes) {

            maxSize = maxNodes;
            heap = new float[maxNodes + 1];
        }

        public int Count { get { return nodesCount; } }

        // public int MaxSize { get { return maxSize; } }

        public float HeadHeap { get { return heap[1]; } }

        public void Clear() {
            nodesCount = 0;
        }

        // in lots of cases, max head gets removed
        public void Push(float h) {

            // if heap full
            if(nodesCount == maxSize) {

                // if Heads priority is smaller than input priority, then ignore that item
                if(HeadHeap < h) {

                    return;
                }
                else {

                    heap[1] = h;   // remove top element
                    BubbleDown(1); // bubble it down
                }
            }
            else {

                nodesCount++;
                heap[nodesCount] = h;
                BubbleUp(nodesCount);
            }
        }

        int Parent(int index) { return (index >> 1);     }
        int Left  (int index) { return (index << 1);     }
        int Right (int index) { return (index << 1) + 1; }

        // if heap is full, we need to remove head and bubble down new item
        private void BubbleDown(int index) {

            int L = Left(index);
            int R = Right(index);

            // bubbling down, 2 kids
            while (R <= nodesCount) {

                // if heap property is violated between index and Left child
                if(heap[index] < heap[L]) {

                    if (heap[L] < heap[R]) {

                        Swap(index, R); // left has bigger priority
                        index = R;
                    }
                    else {

                        Swap(index, L); // right has bigger priority
                        index = L;
                    }
                }
                else {
                    // if heap property is violated between index and R
                    if (heap[index] < heap[R]) {

                        Swap(index, R);
                        index = R;
                    }
                    else {

                        index = L;
                        L = Left(index);
                        break;
                    }

                }

                L = Left(index);
                R = Right(index);
            }

            // only left & last children available to test and swap
            if (L <= nodesCount && heap[index] < heap[L]) {
                Swap(index, L);
            }
        }

        // if heap is not full
        private void BubbleUp(int index) {

            int P = Parent(index);

            //swap, until Heap property isn't violated anymore
            while (P > 0 && heap[P] < heap[index]) {

                Swap(P, index);

                index = P;
                P = Parent(index);
            }
        }


        float tempHeap;
        void Swap(int A, int B) {

           tempHeap = heap[A];
            heap[A] = heap[B];
            heap[B] = tempHeap;
        }

        public void Print() {

            UnityEngine.Debug.Log("HeapPropertyHolds? " + HeapPropertyHolds(1));
        }

        public bool HeapPropertyHolds(int index, int depth = 0) {

            if (index > nodesCount)
                return true;


            UnityEngine.Debug.Log(heap[index]);

            int L = Left(index);
            int R = Right(index);

            bool bothHold = true;

            if(L <= nodesCount) {

                UnityEngine.Debug.Log(heap[index] + " => " + heap[L]);

                if (heap[index] < heap[L])
                    bothHold = false;
            }

            // if L <= nodesCount, then R <= nodesCount can also happen
            if (R <= nodesCount) {

                UnityEngine.Debug.Log(heap[index] + " => " + heap[R]);

                if (bothHold && heap[index] < heap[R])
                    bothHold = false;

            }

            return bothHold & HeapPropertyHolds(L, depth + 1) & HeapPropertyHolds(R, depth + 1);
        }

    }


    // array start at index 1
    // generic version
    public class LimitedMaxHeap<T> {

        int nodesCount;
        int maxSize;

        float[] heap;
        T[] objs; //objects

        public LimitedMaxHeap(int maxNodes) {

            maxSize = maxNodes;
            heap = new float[maxNodes + 1];
            objs = new T[maxNodes + 1];
        }

        public int Count { get { return nodesCount; } }

        // public int MaxSize { get { return maxSize; } }

        public float HeadHeapValue  { get { return heap[1]; } }
        public T     HeadHeapObject { get { return objs[1]; } }

        public void Clear() {
            nodesCount = 0;
        }

        // in lots of cases, max head gets removed
        public void Push(float value, T obj) {

            // if heap full
            if(nodesCount == maxSize) {

                // if Heads priority is smaller than input priority, then ignore that item
                if(HeadHeapValue < value) {

                    return;
                }
                else {

                    heap[1] = value;   // remove top element
                    BubbleDown(1); // bubble it down
                }
            }
            else {

                nodesCount++;

                heap[nodesCount] = value;
                objs[nodesCount] = obj;

                BubbleUp(nodesCount);
            }
        }

        int Parent(int index) { return (index >> 1); }
        int Left(int index) { return (index << 1); }
        int Right(int index) { return (index << 1) + 1; }

        // if heap is full, we need to remove head and bubble down new item
        private void BubbleDown(int index) {

            int L = Left(index);
            int R = Right(index);

            // bubbling down, 2 kids
            while(R <= nodesCount) {

                // if heap property is violated between index and Left child
                if(heap[index] < heap[L]) {

                    if(heap[L] < heap[R]) {

                        Swap(index, R); // left has bigger priority
                        index = R;
                    }
                    else {

                        Swap(index, L); // right has bigger priority
                        index = L;
                    }
                }
                else {
                    // if heap property is violated between index and R
                    if(heap[index] < heap[R]) {

                        Swap(index, R);
                        index = R;
                    }
                    else {

                        index = L;
                        L = Left(index);
                        break;
                    }

                }

                L = Left(index);
                R = Right(index);
            }

            // only left & last children available to test and swap
            if(L <= nodesCount && heap[index] < heap[L]) {
                Swap(index, L);
            }
        }

        // if heap is not full
        private void BubbleUp(int index) {

            int P = Parent(index);

            //swap, until Heap property isn't violated anymore
            while(P > 0 && heap[P] < heap[index]) {

                Swap(P, index);

                index = P;
                P = Parent(index);
            }
        }

        float tempHeap;
        T tempObjs;
        void Swap(int A, int B) {

            tempHeap = heap[A];
            tempObjs = objs[A];

            heap[A] = heap[B];
            objs[A] = objs[B];

            heap[B] = tempHeap;
            objs[B] = tempObjs;
        }

        public void Print() {

            UnityEngine.Debug.Log("HeapPropertyHolds? " + HeapPropertyHolds(1));
        }

        public bool HeapPropertyHolds(int index, int depth = 0) {

            if(index > nodesCount)
                return true;

            UnityEngine.Debug.Log(heap[index]);

            int L = Left(index);
            int R = Right(index);

            bool bothHold = true;

            if(L <= nodesCount) {

                UnityEngine.Debug.Log(heap[index] + " => " + heap[L]);

                if(heap[index] < heap[L])
                    bothHold = false;
            }

            // if L <= nodesCount, then R <= nodesCount can also happen
            if(R <= nodesCount) {

                UnityEngine.Debug.Log(heap[index] + " => " + heap[R]);

                if(bothHold && heap[index] < heap[R])
                    bothHold = false;

            }

            return bothHold && HeapPropertyHolds(L, depth + 1) && HeapPropertyHolds(R, depth + 1);
        }

    }
}