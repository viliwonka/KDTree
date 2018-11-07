/*MIT License

Copyright(c) 2018 Vili Volčini / viliwonka

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections.Generic;

namespace DataStructures.ViliWonka.Heap {

    public class KSmallestHeap : BaseHeap {

        public KSmallestHeap(int maxEntries) : base(maxEntries) {

        }

        public bool Full {
            get {
                return maxSize == nodesCount;
            }
        }

        // in lots of cases, max head gets removed
        public override void PushValue(float h) {

            // if heap full
            if(nodesCount == maxSize) {

                // if Heads priority is smaller than input priority, then ignore that item
                if(HeadValue < h) {

                    return;
                }
                else {

                    heap[1] = h;   // remove top element
                    BubbleDownMax(1); // bubble it down
                }
            }
            else {

                nodesCount++;
                heap[nodesCount] = h;
                BubbleUpMax(nodesCount);
            }
        }

        public override float PopValue() {

            if(nodesCount == 0)
                throw new System.ArgumentException("Heap is empty!");

            float result = heap[1];

            heap[1] = heap[nodesCount];
            nodesCount--;
            BubbleDownMax(1);

            return result;
        }

        public void Print() {

            UnityEngine.Debug.Log("HeapPropertyHolds? " + HeapPropertyHolds(1));
        }

        //should remove
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
    public class KSmallestHeap<T> : KSmallestHeap {

        T[] objs; //objects

        public KSmallestHeap(int maxEntries) : base(maxEntries) {
            objs = new T[maxEntries + 1];
        }

        public T HeadHeapObject { get { return objs[1]; } }

        T tempObjs;
        protected override void Swap(int A, int B) {

            tempHeap = heap[A];
            tempObjs = objs[A];

            heap[A] = heap[B];
            objs[A] = objs[B];

            heap[B] = tempHeap;
            objs[B] = tempObjs;
        }

        public override void PushValue(float h) {
            throw new System.ArgumentException("Use Push(T, float)!");
        }

        public void PushObj(T obj, float h) {

            // if heap full
            if(nodesCount == maxSize) {

                // if Heads priority is smaller than input priority, then ignore that item
                if(HeadValue < h) {

                    return;
                }
                else {

                    heap[1] = h;   // remove top element
                    objs[1] = obj;
                    BubbleDownMax(1); // bubble it down
                }
            }
            else {

                nodesCount++;
                heap[nodesCount] = h;
                objs[nodesCount] = obj;
                BubbleUpMax(nodesCount);
            }
        }

        public override float PopValue() {
            throw new System.ArgumentException("Use PopObj()!");
        }

        public T PopObj() {

            if(nodesCount == 0)
                throw new System.ArgumentException("Heap is empty!");

            T result = objs[1];

            heap[1] = heap[nodesCount];
            objs[1] = objs[nodesCount];

            nodesCount--;
            BubbleDownMax(1);

            return result;
        }

        public T PopObj(ref float heapValue) {

            if(nodesCount == 0)
                throw new System.ArgumentException("Heap is empty!");

            heapValue = heap[1];
            T result = PopObj();

            return result;
        }

        //flush internal results, returns ordered data
        public void FlushResult(List<T> resultList, List<float> heapList = null) {

            int count = nodesCount + 1;


            if(heapList == null) {

                for(int i = 1; i < count; i++) {
                    resultList.Add(PopObj());
                }
            }
            else {

                float h = 0f;

                for(int i = 1; i < count; i++) {
                    resultList.Add(PopObj(ref h));
                    heapList.Add(h);
                }
            }
        }
    }
}