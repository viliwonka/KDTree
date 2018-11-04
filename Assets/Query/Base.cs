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

        protected KDQueryNode[] queryNodes;  // StackPool
        protected int count = 0;             // size of stack
        protected int queryIndex = 0;        // current index at stack

        // gives you initialized node from stack that acts also as a pool
        // automatically pushed onto stack
        private KDQueryNode PushGet() {

            KDQueryNode node = null;

            if (count < queryNodes.Length) {

                if (queryNodes[count] == null)
                    queryNodes[count] = node = new KDQueryNode();
                else
                    node = queryNodes[count];
            }
            else {

                // automatic resize of pool
                Array.Resize(ref queryNodes, queryNodes.Length * 2);
                queryNodes[count] = new KDQueryNode();

            }

            count++;

            return node;
        }

        protected KDQueryNode PushGet(KDNode node, Vector3 tempClosestPoint) {

            var queryNode = PushGet();
            queryNode.node = node;
            queryNode.tempClosestPoint = tempClosestPoint;
            return queryNode;
        }

        protected int LeftToProcess {

            get {
                return count - queryIndex;
            }
        }

        // just gets unprocessed node from stack
        // increases queryIndex
        protected KDQueryNode Pop() {

            var node = queryNodes[queryIndex];
            queryIndex++;
            return node;
        }

        protected void ResetStack() {

            count = 0;
            queryIndex = 0;
        }

        protected KDQuery(int initialStackSize = 2048) {
            queryNodes = new KDQueryNode[initialStackSize];
        }

        // Finds closest node (which doesn't necesarily contain closest point!!)
        //! TO FINISH, TRICKY MATH
        public KDNode NearestNode(KDTree tree, Vector3 qPosition) {

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
            KDNode minNode = null;

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

                    // leaf node
                    float sqrDist = Vector3.SqrMagnitude(queryNode.tempClosestPoint - qPosition);

                    if(sqrDist < minSqrDist) {
                        minSqrDist = sqrDist;
                        minNode = node;
                    }

                }
            }

            return minNode;
        }
    }

}