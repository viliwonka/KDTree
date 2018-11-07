// The object used for querying. This object should be persistent - re-used for querying.
// Contains internal stack / pool so that it doesn't generate (too much) garbage.
// The stack never down-sizes, only up-sizes, so the more you use this object, less garbage will it make over time.

// Should be used only by 1 thread,
// which means each thread should have it's own KDQuery object in order for querying to be thread safe.

// KDQuery can switch target KDTree, on which querying is done.

#define KDTREE_VISUAL_DEBUG

using System.Collections.Generic;
using UnityEngine;
using System;

namespace DataStructures.ViliWonka.KDTree {

    using Heap;

    public partial class KDQuery {

        public void ClosestPoint(KDTree tree, Vector3 queryPosition, List<int> resultIndices, List<float> resultDistances = null) {

            Reset();

            Vector3[] points = tree.Points;
            int[] permutation = tree.Permutation;

            int smallestIndex = 0;
            /// Smallest Squared Radius
            float SSR = Single.PositiveInfinity;


            var rootNode = tree.RootNode;

            Vector3 rootClosestPoint = rootNode.bounds.ClosestPoint(queryPosition);

            PushToHeap(rootNode, rootClosestPoint, queryPosition);

            KDQueryNode queryNode = null;
            KDNode node = null;

            int partitionAxis;
            float partitionCoord;

            Vector3 tempClosestPoint;

            // searching
            while(minHeap.Count > 0) {

                queryNode = PopFromHeap();

                if(queryNode.distance > SSR)
                    continue;

                node = queryNode.node;

                if(!node.Leaf) {

                    partitionAxis = node.partitionAxis;
                    partitionCoord = node.partitionCoordinate;

                    tempClosestPoint = queryNode.tempClosestPoint;

                    if((tempClosestPoint[partitionAxis] - partitionCoord) < 0) {

                        // we already know we are on the side of negative bound/node,
                        // so we don't need to test for distance
                        // push to stack for later querying

                        PushToHeap(node.negativeChild, tempClosestPoint, queryPosition);
                        // project the tempClosestPoint to other bound
                        tempClosestPoint[partitionAxis] = partitionCoord;

                        if(node.positiveChild.Count != 0) {

                            PushToHeap(node.positiveChild, tempClosestPoint, queryPosition);
                        }

                    }
                    else {

                        // we already know we are on the side of positive bound/node,
                        // so we don't need to test for distance
                        // push to stack for later querying

                        PushToHeap(node.positiveChild, tempClosestPoint, queryPosition);
                        // project the tempClosestPoint to other bound
                        tempClosestPoint[partitionAxis] = partitionCoord;

                        if(node.positiveChild.Count != 0) {

                            PushToHeap(node.negativeChild, tempClosestPoint, queryPosition);
                        }

                    }
                }
                else {

                    float sqrDist;
                    // LEAF
                    for(int i = node.start; i < node.end; i++) {

                        int index = permutation[i];

                        sqrDist = Vector3.SqrMagnitude(points[index] - queryPosition);

                        if(sqrDist <= SSR) {

                            SSR = sqrDist;
                            smallestIndex = index;
                        }
                    }

                }
            }

            resultIndices.Add(smallestIndex);

            if(resultDistances != null) {
                resultDistances.Add(SSR);
            }

        }

    }

}