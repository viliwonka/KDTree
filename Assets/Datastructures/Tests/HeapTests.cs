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