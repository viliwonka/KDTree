// The object used for querying. This object should be persistent - re-used for querying.
// Contains internal stack / pool so that it doesn't generate (too much) garbage.
// The stack never down-sizes, only up-sizes, so the more you use this object, less garbage will it make over time.

// Should be used only by 1 thread,
// which means each thread should have it's own KDQuery object in order for querying to be thread safe.

// KDQuery can switch target KDTree, on which querying is done.

using System.Collections.Generic;
using UnityEngine;
using System;

//TODO: finish
namespace DataStructures.Query {

    public class KDQueryRadius : KDQueryBase {

        public KDQueryRadius(int initialStackSize = 2048) : base(initialStackSize) {

        }

        /// <summary>
        /// Search by radius method.
        /// </summary>
        /// <param name="tree">Tree to do search on</param>
        /// <param name="queryPosition">Position</param>
        /// <param name="queryRadius">Radius</param>
        /// <param name="resultIndices">Initialized list, cleared.</param>
        public void SearchByRadius(KDTree tree, Vector3 queryPosition, float queryRadius, List<int> resultIndices) {

            ResetStack();

            float squaredRadius = queryRadius * queryRadius;

            var rootNode = tree.rootNode;

            var rootQueryNode = PushGet(

                rootNode,
                rootNode.bounds.ClosestPoint(queryPosition)
            );

            KDQueryNode queryNode = null;
            KDNode node = null;

            KDQueryNode negativeQueryNode = null;
            KDQueryNode positiveQueryNode = null;

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
                        negativeQueryNode = PushGet(node.negativeChild, tempClosestPoint);

                        tempClosestPoint[partitionAxis] = partitionCoord;

                        // testing other side
                        if(node.positiveChild.Count != 0 &&
                        Vector3.SqrMagnitude(tempClosestPoint - queryPosition) <= squaredRadius) {

                            positiveQueryNode = PushGet(node.positiveChild, tempClosestPoint);
                        }
                    }
                    else {

                        // we already know we are inside positive bound/node,
                        // so we don't need to test for distance
                        // push to stack for later querying

                        // tempClosestPoint is inside positive side
                        // assign it to positiveChild
                        positiveQueryNode = PushGet(node.positiveChild, tempClosestPoint);

                        // project the tempClosestPoint to other bound
                        tempClosestPoint[partitionAxis] = partitionCoord;

                        // testing other side
                        if(node.negativeChild.Count != 0 &&
                        Vector3.SqrMagnitude(tempClosestPoint - queryPosition) <= squaredRadius) {

                            negativeQueryNode = PushGet(node.negativeChild, tempClosestPoint);
                        }
                    }
                }
                else {
                    // LEAF
                    for(int i = node.start; i < node.end; i++) {

                        if(Vector3.SqrMagnitude(tree.points[tree.permutation[i]]) <= squaredRadius) {

                            resultIndices.Add(i);
                        }
                    }

                }
            }
        }


    }

}