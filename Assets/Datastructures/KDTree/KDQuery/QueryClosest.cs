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