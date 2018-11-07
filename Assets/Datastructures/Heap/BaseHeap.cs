using System.Collections.Generic;
// structure of LimitedMaxHeap is same as MaxHeap, but it is Limited & throws biggest items out (so only minimums are left!!)
namespace DataStructures.ViliWonka.Heap {

    // array start at index 1
    public abstract class BaseHeap {

        protected int nodesCount;
        protected int maxSize;

        protected float[] heap;

        protected BaseHeap(int initialSize) {

            maxSize = initialSize;
            heap = new float[initialSize + 1];
        }

        public int Count { get { return nodesCount; } }

        public float HeadValue { get { return heap[1]; } }

        public void Clear() {
            nodesCount = 0;
        }

        protected int Parent(int index) { return (index >> 1);     }
        protected int Left  (int index) { return (index << 1);     }
        protected int Right (int index) { return (index << 1) | 1; }

        // bubble down, MaxHeap version
        protected void BubbleDownMax(int index) {

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

        // bubble up, MaxHeap version
        protected void BubbleUpMax(int index) {

            int P = Parent(index);

            //swap, until Heap property isn't violated anymore
            while (P > 0 && heap[P] < heap[index]) {

                Swap(P, index);

                index = P;
                P = Parent(index);
            }
        }

        // bubble down, MinHeap version
        protected void BubbleDownMin(int index) {

            int L = Left(index);
            int R = Right(index);

            // bubbling down, 2 kids
            while(R <= nodesCount) {

                // if heap property is violated between index and Left child
                if(heap[index] > heap[L]) {

                    if(heap[L] > heap[R]) {

                        Swap(index, R); // right has smaller priority
                        index = R;
                    }
                    else {

                        Swap(index, L); // left has smaller priority
                        index = L;
                    }
                }
                else {
                    // if heap property is violated between index and R
                    if(heap[index] > heap[R]) {

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
            if(L <= nodesCount && heap[index] > heap[L]) {
                Swap(index, L);
            }
        }

        // bubble up, MinHeap version
        protected void BubbleUpMin(int index) {

            int P = Parent(index);

            //swap, until Heap property isn't violated anymore
            while(P > 0 && heap[P] > heap[index]) {

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

        protected virtual void UpsizeHeap() {

            maxSize *= 2;
            System.Array.Resize(ref heap, maxSize + 1);
        }

        public virtual void PushValue(float h) {
            throw new System.NotImplementedException();
        }

        public virtual float PopValue() {
            throw new System.NotImplementedException();
        }

        public void FlushHeapResult(List<float> heapList) {

            for(int i = 1; i < Count; i++) {
                heapList.Add(heap[i]);
            }
        }
    }
}