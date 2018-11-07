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

    public partial class KDQuery {

        public void Interval(KDTree tree, Vector3 min, Vector3 max, List<int> resultIndices) {

            Reset();

            Vector3[] points = tree.Points;
            int[] permutation = tree.Permutation;

            var rootNode = tree.RootNode;

            PushToQueue(

                rootNode,
                rootNode.bounds.ClosestPoint((min + max) / 2)
            );

            KDQueryNode queryNode = null;
            KDNode node = null;


            // KD search with pruning (don't visit areas which distance is more away than range)
            // Recursion done on Stack
            while(LeftToProcess > 0) {

                queryNode = PopFromQueue();
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
                        PushToQueue(node.negativeChild, tempClosestPoint);

                        tempClosestPoint[partitionAxis] = partitionCoord;

                        // testing other side
                        if(node.positiveChild.Count != 0
                        && tempClosestPoint[partitionAxis] <= max[partitionAxis]) {

                            PushToQueue(node.positiveChild, tempClosestPoint);
                        }
                    }
                    else {

                        // we already know we are inside positive bound/node,
                        // so we don't need to test for distance
                        // push to stack for later querying

                        // tempClosestPoint is inside positive side
                        // assign it to positiveChild
                        PushToQueue(node.positiveChild, tempClosestPoint);

                        // project the tempClosestPoint to other bound
                        tempClosestPoint[partitionAxis] = partitionCoord;

                        // testing other side
                        if(node.negativeChild.Count != 0
                        && tempClosestPoint[partitionAxis] >= min[partitionAxis]) {

                            PushToQueue(node.negativeChild, tempClosestPoint);
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

                            resultIndices.Add(permutation[i]);
                        }

                    }
                    // node is not inside query interval, need to do test on each point separately
                    else {

                        for(int i = node.start; i < node.end; i++) {

                            int index = permutation[i];

                            Vector3 v = points[index];

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