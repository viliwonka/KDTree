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

        /// <summary>
        /// Returns an index to closest point
        /// </summary>
        /// <param name="tree">Tree to do search on</param>
        /// <param name="queryPosition">Position</param>
        /// <returns></returns>
        public int ClosestPoint(KDTree tree, Vector3 queryPosition) {

            ResetStack();

            var rootNode = tree.rootNode;

            var rootQueryNode = PushGet(rootNode, rootNode.bounds.ClosestPoint(queryPosition));

            KDQueryNode queryNode = null;
            KDNode node = null;

            KDQueryNode negativeQueryNode = null;
            KDQueryNode positiveQueryNode = null;

            Vector3[] points = tree.points;
            int[] permutation = tree.permutation;

            // searching for index that points to closest point
            float minSqrDist = Single.MaxValue;
            int minIndex = 0;

            // KD search with pruning (don't visit areas which distance is more away than range)
            // Recursion done on Stack
            while(LeftToProcess > 0) {

                queryNode = Pop();
                node = queryNode.node;

                // pruning!
                if(!node.Leaf) {

                    int partitionAxis = node.partitionAxis;
                    float partitionCoord = node.partitionCoordinate;

                    Vector3 tempClosestPoint = queryNode.tempClosestPoint;

                    if((tempClosestPoint[partitionAxis] - partitionCoord) < 0) {

                        negativeQueryNode = PushGet(node.negativeChild, tempClosestPoint);

                        tempClosestPoint[partitionAxis] = partitionCoord;

                        float dist = Vector3.SqrMagnitude(tempClosestPoint - queryPosition);

                        if(node.positiveChild.Count != 0 && dist <= minSqrDist) {

                            positiveQueryNode = PushGet(node.positiveChild, tempClosestPoint);
                        }
                    }
                    else {

                        positiveQueryNode = PushGet(node.positiveChild, tempClosestPoint);

                        tempClosestPoint[partitionAxis] = partitionCoord;

                        float dist = Vector3.SqrMagnitude(tempClosestPoint - queryPosition);

                        if(node.negativeChild.Count != 0 && dist <= minSqrDist) {

                            negativeQueryNode = PushGet(node.negativeChild, tempClosestPoint);
                        }
                    }
                }
                else {

                    for(int i = node.start; i < node.end; i++) {

                        int index = permutation[i];

                        float sqrDist = Vector3.SqrMagnitude(queryPosition - points[index]);

                        if(sqrDist < minSqrDist) {
                            minSqrDist = sqrDist;
                            minIndex = index;
                        }
                    }

                    // leaf node
                }
            }

            return minIndex;
        }

    }
}