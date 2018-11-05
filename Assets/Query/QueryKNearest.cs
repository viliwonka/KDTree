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

        SortedList<int, LimitedMaxHeap<int>> _heaps = new SortedList<int, LimitedMaxHeap<int>>();

        /// <summary>
        /// Returns indices to k closest points, and optionaly can return distances
        /// </summary>
        /// <param name="tree">Tree to do search on</param>
        /// <param name="queryPosition">Position</param>
        /// <param name="k">Max number of points</param>
        /// <param name="resultIndices">List where resulting indices will be stored</param>
        /// <param name="resultDistances">Optional list where resulting distances will be stored</param>
        public void KNearest(KDTree tree, Vector3 queryPosition, int k, List<int> resultIndices, List<float> resultDistances = null) {

            //pooled heap arrays
            LimitedMaxHeap<int> heap;

            _heaps.TryGetValue(k, out heap);

            if(heap == null) {
                heap = new LimitedMaxHeap<int>(k);
                _heaps.Add(k, heap);
            }

            heap.Clear();
            ResetStack();

            ///Biggest Smallest Squared Radius
            float BSSR = Single.PositiveInfinity;

            var rootNode = tree.rootNode;

            PushGet(rootNode, queryPosition);

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

                        float sqrDist = tempClosestPoint[partitionAxis] - queryPosition[partitionAxis];
                        sqrDist = sqrDist * sqrDist;

                        // testing other side
                        if(node.positiveChild.Count != 0
                        && sqrDist <= BSSR) {

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

                        float sqrDist = tempClosestPoint[partitionAxis] - queryPosition[partitionAxis];
                        sqrDist = sqrDist * sqrDist;

                        // testing other side
                        if(node.negativeChild.Count != 0
                        && sqrDist <= BSSR) {

                            PushGet(node.negativeChild, tempClosestPoint);
                        }
                    }
                }
                else {

                    float dist;
                    // LEAF
                    for(int i = node.start; i < node.end; i++) {

                        int index = tree.permutation[i];

                        dist = Vector3.SqrMagnitude(tree.points[index] - queryPosition);

                        if(dist <= BSSR) {

                            heap.Push(index, dist);

                            if(heap.Full) {
                                BSSR = heap.HeadHeapValue;
                            }
                        }
                    }

                }
            }

            heap.FlushResult(resultIndices, resultDistances);

        }

    }

}