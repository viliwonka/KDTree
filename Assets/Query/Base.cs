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

        protected KDQueryNode[] queueArray;  // queue array
        protected Heap.MinHeap<KDQueryNode>  minHeap; //heap for k-nearest
        protected int count = 0;             // size of queue
        protected int queryIndex = 0;        // current index at stack

        /// <summary>
        /// Returns initialized node from stack that also acts as a pool
        /// The returned reference to node stays in stack
        /// </summary>
        /// <returns>Reference to pooled node</returns>
        private KDQueryNode PushGetQueue() {

            KDQueryNode node = null;

            if (count < queueArray.Length) {

                if (queueArray[count] == null)
                    queueArray[count] = node = new KDQueryNode();
                else
                    node = queueArray[count];
            }
            else {

                // automatic resize of pool
                Array.Resize(ref queueArray, queueArray.Length * 2);
                node = queueArray[count] = new KDQueryNode();
            }

            count++;

            return node;
        }

        protected void PushToQueue(KDNode node, Vector3 tempClosestPoint) {

            var queryNode = PushGetQueue();
            queryNode.node = node;
            queryNode.tempClosestPoint = tempClosestPoint;
        }

        protected void PushToHeap(KDNode node, Vector3 tempClosestPoint, Vector3 queryPosition) {

            var queryNode = PushGetQueue();
            queryNode.node = node;
            queryNode.tempClosestPoint = tempClosestPoint;

            float sqrDist = Vector3.SqrMagnitude(tempClosestPoint - queryPosition);
            queryNode.distance = sqrDist;
            minHeap.PushObj(queryNode, sqrDist);
        }

        protected int LeftToProcess {

            get {
                return count - queryIndex;
            }
        }

        // just gets unprocessed node from stack
        // increases queryIndex
        protected KDQueryNode PopFromQueue() {

            var node = queueArray[queryIndex];
            queryIndex++;

            return node;
        }

        protected KDQueryNode PopFromHeap() {

            KDQueryNode heapNode = minHeap.PopObj();

            queueArray[queryIndex].node = heapNode.node;
            queryIndex++;

            return heapNode;
        }

        protected void Reset() {

            count = 0;
            queryIndex = 0;
            minHeap.Clear();
        }

        public KDQuery(int queryNodesContainersInitialSize = 2048) {
            queueArray = new KDQueryNode[queryNodesContainersInitialSize];
            minHeap = new Heap.MinHeap<KDQueryNode>(queryNodesContainersInitialSize);
        }

        // Finds closest node (which doesn't necesarily contain closest point!!)
        //! TO FINISH, TRICKY MATH
        /*public KDNode NearestNode(KDTree tree, Vector3 qPosition) {

            ResetStack();

            var rootNode = tree.rootNode;

            PushGet(
                rootNode,
                rootNode.bounds.ClosestPoint(qPosition)
            );

            KDQueryNode queryNode = null;
            KDNode node = null;


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

                        PushGet(node.negativeChild, tempClosestPoint);

                        tempClosestPoint[partitionAxis] = partitionCoord;

                        float dist = Vector3.SqrMagnitude(tempClosestPoint - qPosition);

                        if(node.positiveChild.Count != 0 && dist <= minSqrDist) {

                            PushGet(node.positiveChild, tempClosestPoint);
                        }
                    }
                    else {

                        PushGet(node.positiveChild, tempClosestPoint);

                        tempClosestPoint[partitionAxis] = partitionCoord;

                        if(node.negativeChild.Count != 0 &&
                        Vector3.SqrMagnitude(tempClosestPoint - qPosition) <= minSqrDist) {

                            PushGet(node.negativeChild, tempClosestPoint);
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
        }*/


    }

}