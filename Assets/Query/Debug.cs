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

    public partial class KDQuery {

        // uses gizmos
        public void DrawLastQuery() {

            Color green = Color.green;
            Color red   = Color.red;

            for(int i = 0; i < queryIndex; i++) {

                float val = i / (float)queryIndex;

                Gizmos.color = Color.Lerp(red, green, val);

                Bounds b = queueArray[i].node.bounds.Bounds;

                Gizmos.DrawWireCube(b.center, b.size);
            }
        }
    }

}