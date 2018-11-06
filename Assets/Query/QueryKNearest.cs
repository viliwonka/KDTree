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

            ///Biggest Smallest Squared Radius
            float BSSR = Single.PositiveInfinity;

            var rootNode = tree.rootNode;

            Vector3 rootClosestPoint = rootNode.bounds.ClosestPoint(queryPosition);

            PushToHeap(rootNode, rootClosestPoint, queryPosition);

            KDQueryNode queryNode = null;
            KDNode node = null;

            int partitionAxis;
            float partitionCoord;

            Vector3 tempClosestPoint;
            Vector3 tempClosestPoint2;

            // KD search with pruning (don't visit areas which distance is more away than range)
            // Recursion done on Stack
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

                        // tempClosestPoint is inside negative side
                        // assign it to negativeChild

                        float sqrDist = partitionCoord - queryPosition[partitionAxis];
                        sqrDist = sqrDist * sqrDist;

                        // testing other side
                        if(node.positiveChild.Count != 0
                        && sqrDist <= BSSR) {

                            tempClosestPoint2 = tempClosestPoint;
                            tempClosestPoint2[partitionAxis] = partitionCoord;

                            PushToHeap(node.positiveChild, tempClosestPoint2, queryPosition);
                        }

                        PushToHeap(node.negativeChild, tempClosestPoint, queryPosition);
                    }
                    else {

                        // we already know we are on the side of positive bound/node,
                        // so we don't need to test for distance
                        // push to stack for later querying


                        // project the tempClosestPoint to other bound
                        //tempClosestPoint[partitionAxis] = partitionCoord;

                        float sqrDist = partitionCoord - queryPosition[partitionAxis];
                        sqrDist = sqrDist * sqrDist;

                        // testing other side
                        if(node.negativeChild.Count != 0
                        && sqrDist <= BSSR) {

                            tempClosestPoint2 = tempClosestPoint;
                            tempClosestPoint2[partitionAxis] = partitionCoord;

                            PushToHeap(node.negativeChild, tempClosestPoint, queryPosition);
                        }

                        // tempClosestPoint is inside positive side
                        // assign it to positiveChild
                        PushToHeap(node.positiveChild, tempClosestPoint, queryPosition);
                    }
                }
                else {

                    float dist;
                    // LEAF
                    for(int i = node.start; i < node.end; i++) {

                        int index = tree.permutation[i];

                        dist = Vector3.SqrMagnitude(tree.points[index] - queryPosition);

                        if(dist <= BSSR) {

                            kHeap.PushObj(index, dist);

                            if(kHeap.Full) {
                                BSSR = kHeap.HeadHeapValue;
                            }
                        }
                    }

                }
            }

            kHeap.FlushResult(resultIndices, resultDistances);

        }

    }

}