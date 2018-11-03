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

    public class KDQueryInterval : KDQueryBase {

        public KDQueryInterval(int initialStackSize = 2048) : base(initialStackSize) {

        }

        public void KNearestSearch(KDTree tree, Vector3 queryPosition, int count, List<int> resultIndices) {

        }
    }

}