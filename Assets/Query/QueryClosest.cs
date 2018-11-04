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

        // Finds closest node (which doesn't necesarily contain closest point!!)
        //! TO FINISH, TRICKY MATH
        //! DIFFERENT VERSION THAN IN KDQueryBase
        public KDNode ClosestNode(KDTree tree, Vector3 qPosition) {

            ResetStack();

            var rootNode = tree.rootNode;

            var rootQueryNode = PushGet(rootNode, rootNode.bounds.ClosestPoint(qPosition));

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

                        float dist = Vector3.SqrMagnitude(tempClosestPoint - qPosition);

                        if(node.positiveChild.Count != 0 && dist <= minSqrDist) {

                            positiveQueryNode = PushGet(node.positiveChild, tempClosestPoint);
                        }
                    }
                    else {

                        positiveQueryNode = PushGet(node.positiveChild, tempClosestPoint);

                        tempClosestPoint[partitionAxis] = partitionCoord;

                        if(node.negativeChild.Count != 0 &&
                        Vector3.SqrMagnitude(tempClosestPoint - qPosition) <= minSqrDist) {

                            negativeQueryNode = PushGet(node.negativeChild, tempClosestPoint);
                        }
                    }
                }
                else {

                    for(int i = node.start; i < node.end; i++) {

                        int index = permutation[i];

                        float sqrDist = Vector3.SqrMagnitude(qPosition - points[index]);

                        if(sqrDist < minSqrDist) {
                            minSqrDist = sqrDist;
                            minIndex = index;
                        }
                    }

                    // leaf node
                }
            }
            throw new NotImplementedException();
        }

        public int ClosestPoint(KDTree tree, Vector3 queryPosition) {

            var node = SearchNearestNode(tree, queryPosition);

            Vector3[] points = tree.points;
            int[] permutation = tree.permutation;

            float minSqrDist = Single.MaxValue;
            int minIndex = 0;

            for(int i = node.start; i < node.end; i++) {

                int index = permutation[i];

                float sqrDist = Vector3.SqrMagnitude(queryPosition - points[index]);

                if(sqrDist < minSqrDist) {
                    minSqrDist = sqrDist;
                    minIndex = index;
                }
            }

            return minIndex;
        }

    }

}