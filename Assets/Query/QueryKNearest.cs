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

namespace DataStructures.Query {

    using Heap;

    public partial class KDQuery {

        SortedList<int, KSmallestHeap<int>> _heaps = new SortedList<int, KSmallestHeap<int>>();
        /// <summary>
        /// Returns indices to k closest points, and optionaly can return distances
        /// </summary>
        /// <param name="tree">Tree to do search on</param>
        /// <param name="queryPosition">Position</param>
        /// <param name="k">Max number of points</param>
        /// <param name="resultIndices">List where resulting indices will be stored</param>
        /// <param name="resultDistances">Optional list where resulting distances will be stored</param>
        public void KNearest(KDTree tree, Vector3 queryPosition, int k, List<int> resultIndices, List<float> resultDistances = null) {

            // pooled heap arrays
            KSmallestHeap<int> kHeap;

            _heaps.TryGetValue(k, out kHeap);

            if(kHeap == null) {

                kHeap = new KSmallestHeap<int>(k);
                _heaps.Add(k, kHeap);
            }

            kHeap.Clear();
            Reset();

            Vector3[] points = tree.Points;
            int[] permutation = tree.Permutation;

            ///Biggest Smallest Squared Radius
            float BSSR = Single.PositiveInfinity;

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

                if(queryNode.distance > BSSR)
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

                        if(sqrDist <= BSSR) {

                            kHeap.PushObj(index, sqrDist);

                            if(kHeap.Full) {
                                BSSR = kHeap.HeadValue;
                            }
                        }
                    }

                }
            }

            kHeap.FlushResult(resultIndices, resultDistances);

        }

    }

}