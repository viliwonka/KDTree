// The object used for querying. This object should be persistent - re-used for querying.
// Contains internal stack / pool so that it doesn't generate (too much) garbage.
// The stack never down-sizes, only up-sizes, so the more you use this object, less garbage will it make over time.

// Should be used only by 1 thread,
// which means each thread should have it's own KDQuery object in order for querying to be thread safe.

// KDQuery can switch target KDTree, on which querying is done.

using System.Collections.Generic;
using UnityEngine;
using System;

namespace DataStructures.Query {

    public class KDQueryKNearest : KDQueryBase {

        private LimitedMaxHeap<int> maxHeap;

        public int KNearestCount { get; private set; }

        public KDQueryKNearest(int kNearestCount, int initialStackSize = 2048) : base(initialStackSize) {

            KNearestCount = kNearestCount;
            maxHeap = new LimitedMaxHeap<int>(kNearestCount);
        }


        public void KNearestQuery(KDTree tree, Vector3 queryPosition, int count, List<int> indices) {

            maxHeap.Clear();
            ResetStack();

            KDNode node = tree.rootNode;

            PushGet(tree.rootNode, );

        }
    }

}