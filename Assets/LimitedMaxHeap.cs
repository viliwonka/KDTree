using System.Collections.Generic;
// structure of LimitedMaxHeap is same as MaxHeap, but it is Limited & throws biggest items out (so only minimums are left!!)
namespace DataStructures {

    // array start at index 1
    public class LimitedMaxHeap {

        protected int nodesCount;
        protected int maxSize;

        protected float[] heap;

        public LimitedMaxHeap(int maxNodes) {

            maxSize = maxNodes;
            heap = new float[maxNodes + 1];
        }

        public int Count { get { return nodesCount; } }

        public bool Full { get { return nodesCount == maxSize; } }

        public float HeadHeapValue { get { return heap[1]; } }

        public void Clear() {
            nodesCount = 0;
        }

        // in lots of cases, max head gets removed
        public virtual void Push(float h) {

            // if heap full
            if(nodesCount == maxSize) {

                // if Heads priority is smaller than input priority, then ignore that item
                if(HeadHeapValue < h) {

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

        protected int Parent(int index) { return (index >> 1);     }
        protected int Left  (int index) { return (index << 1);     }
        protected int Right (int index) { return (index << 1) + 1; }

        // if heap is full, we need to remove head and bubble down new item
        protected void BubbleDown(int index) {

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
        protected void BubbleUp(int index) {

            int P = Parent(index);

            //swap, until Heap property isn't violated anymore
            while (P > 0 && heap[P] < heap[index]) {

                Swap(P, index);

                index = P;
                P = Parent(index);
            }
        }


        protected float tempHeap;
        protected virtual void Swap(int A, int B) {

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

        public void FlushHeapResult(List<float> heapList) {

            for(int i = 1; i < Count; i++) {
                heapList.Add(heap[i]);
            }
        }
    }


    // array start at index 1
    // generic version
    public class LimitedMaxHeap<T> : LimitedMaxHeap {

        T[] objs; //objects

        public LimitedMaxHeap(int maxNodes) : base(maxNodes) {
            objs = new T[maxNodes + 1];
        }

        public T     HeadHeapObject { get { return objs[1]; } }

        T tempObjs;
        protected override void Swap(int A, int B) {

            tempHeap = heap[A];
            tempObjs = objs[A];

            heap[A] = heap[B];
            objs[A] = objs[B];

            heap[B] = tempHeap;
            objs[B] = tempObjs;
        }

        public override void Push(float h) {
            throw new System.ArgumentException("Use Push(T, float)!");
        }

        public void Push(T obj, float h) {

            // if heap full
            if(nodesCount == maxSize) {

                // if Heads priority is smaller than input priority, then ignore that item
                if(HeadHeapValue < h) {

                    return;
                }
                else {

                    heap[1] = h;   // remove top element
                    objs[1] = obj;
                    BubbleDown(1); // bubble it down
                }
            }
            else {

                nodesCount++;
                heap[nodesCount] = h;
                objs[nodesCount] = obj;
                BubbleUp(nodesCount);
            }
        }

        public void FlushResult(List<T> resultList, List<float> heapList = null) {

            int count = nodesCount + 1;

            for(int i = 1; i < count; i++) {
                resultList.Add(objs[i]);
            }

            if(heapList != null) {
                for(int i = 1; i < count; i++) {
                    heapList.Add(heap[i]);
                }
            }
        }
    }
}