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

        public void Interval(KDTree tree, Vector3 min, Vector3 max, List<int> resultIndices) {

            ResetStack();

            var rootNode = tree.rootNode;

            PushGet(
                rootNode,
                rootNode.bounds.ClosestPoint((min + max) / 2)
            );

            KDQueryNode queryNode = null;
            KDNode node = null;


            // KD search with pruning (don't visit areas which distance is more away than range)
            // Recursion done on Stack
            while(LeftToProcess > 0) {

                queryNode = Pop();
                node = queryNode.node;

                if(!node.Leaf) {

                    int partitionAxis = node.partitionAxis;
                    float partitionCoord = node.partitionCoordinate;

                    Vector3 tempClosestPoint = queryNode.tempClosestPoint;

                    if((tempClosestPoint[partitionAxis] - partitionCoord) < 0) {

                        // we already know we are inside negative bound/node,
                        // so we don't need to test for distance
                        // push to stack for later querying

                        // tempClosestPoint is inside negative side
                        // assign it to negativeChild
                        PushGet(node.negativeChild, tempClosestPoint);

                        tempClosestPoint[partitionAxis] = partitionCoord;

                        // testing other side
                        if(node.positiveChild.Count != 0
                        && tempClosestPoint[partitionAxis] <= max[partitionAxis]) {

                            PushGet(node.positiveChild, tempClosestPoint);
                        }
                    }
                    else {

                        // we already know we are inside positive bound/node,
                        // so we don't need to test for distance
                        // push to stack for later querying

                        // tempClosestPoint is inside positive side
                        // assign it to positiveChild
                        PushGet(node.positiveChild, tempClosestPoint);

                        // project the tempClosestPoint to other bound
                        tempClosestPoint[partitionAxis] = partitionCoord;

                        // testing other side
                        if(node.negativeChild.Count != 0
                        && tempClosestPoint[partitionAxis] >= min[partitionAxis]) {

                            PushGet(node.negativeChild, tempClosestPoint);
                        }
                    }
                }
                else {

                    // LEAF

                    // testing if node bounds are inside the query interval
                    if(node.bounds.min[0] >= min[0]
                    && node.bounds.min[1] >= min[1]
                    && node.bounds.min[2] >= min[2]

                    && node.bounds.max[0] <= max[0]
                    && node.bounds.max[1] <= max[1]
                    && node.bounds.max[2] <= max[2]) {

                        for(int i = node.start; i < node.end; i++) {

                            resultIndices.Add(i);
                        }

                    }
                    // node is not inside query interval, need to do test on each point separately
                    else {

                        for(int i = node.start; i < node.end; i++) {

                            int index = tree.permutation[i];

                            Vector3 v = tree.points[index];

                            if(v[0] >= min[0]
                            && v[1] >= min[1]
                            && v[2] >= min[2]

                            && v[0] <= max[0]
                            && v[1] <= max[1]
                            && v[2] <= max[2]) {

                                resultIndices.Add(index);
                            }
                        }
                    }

                }
            }
        }
    }

}