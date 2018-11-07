/*MIT License

Copyright(c) 2018 Vili Volčini / viliwonka

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

#define KDTREE_VISUAL_DEBUG

using System.Collections.Generic;
using UnityEngine;
using System;

namespace DataStructures.ViliWonka.KDTree {

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