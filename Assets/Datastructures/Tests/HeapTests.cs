using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataStructures.ViliWonka.Tests {

    public class HeapTests : MonoBehaviour {

        void Start() {

            Heap.KSmallestHeap<int> minHeap = new Heap.KSmallestHeap<int>(13);

            for(int i = 0; i < 100; i++) {
                minHeap.PushObj(i, Random.value);
            }

            List<int> objList = new List<int>();
            List<float> valList = new List<float>();

            minHeap.FlushResult(objList, valList);

            for(int i = 0; i < objList.Count; i++) {
                Debug.Log(objList[i] + " " + valList[i]);
            }

            minHeap.HeapPropertyHolds(0);
        }

    }
}