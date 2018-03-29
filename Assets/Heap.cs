// The object used for querying.
// Contains internal stack / pool so that it doesn't generate (too much) garbage
// The stack never down-sizes, only up-sizes, so more u use this object, less garbage will it make.

// Should be used only by 1 thread,
// so each thread should have it's own KDQuery object in order for querying to be thread safe

// can switch tree on which you query.

using System.Collections.Generic;
using UnityEngine;
using System;

namespace Floatlands.DataStructures {

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
        
        public void ClearAndResize(int newSize = -1) {
        
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
        
        int Parent(int index) { return index >> 1;     }
        int Left  (int index) { return index >> 1;     }
        int Right (int index) { return index << 1 + 1; }
        
        // if heap is full, we need to remove head and bubble down new item
        private void BubbleDown(int index) {
        
            int L = Left(index);
            int R = Right(index);
        
            // bubbling down, 2 kids
            while (R <= heap.Length) {

                // if heap property violated between index and L
                if(heap[index] < heap[L]) {
                
                    if (heap[L] < heap[R]) {

                        Swap(index, L); // left has bigger priority
                        index = L;
                    } 
                    else {

                        Swap(index, R); // right has bigger priority            
                        index = R;
                    }

                }
                else {
                    // if heap property violated between index and R
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
            if (L < nodesCount && heap[index] < heap[L]) {
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

    }
}