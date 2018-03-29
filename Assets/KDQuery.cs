// The object used for querying.
// Contains internal stack / pool so that it doesn't generate (too much) garbage
// The stack never down-sizes, only up-sizes, so more u use this object, less garbage will it make.

// Should be used only by 1 thread,
// so each thread should have it's own KDQuery object in order for querying to be thread safe

// can switch tree on which you query.

using System.Collections.Generic;
using UnityEngine;
using System;

namespace Floatlands.DataStructures {

    public class KDQuery {

        private KDQueryNode[] queryNodes;  // StackPool
        private int count = 0;             // size of stack
        private int queryIndex = 0;        // current index at stack
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
                Array.Resize(ref queryNodes, queryNodes.Length * 2);
                queryNodes[count] = new KDQueryNode();
                
            }
            count++;

            return node;
        }

        private KDQueryNode PushGet(KDNode node, Vector3 tempClosestPoint) {

            var queryNode = PushGet();
            queryNode.node = node;
            queryNode.tempClosestPoint = tempClosestPoint;
            return queryNode;
        }

        private int LeftToProcess { 
            get {
                return count - queryIndex;
            }
        }

        // just gets unprocessed node from stack
        // increases queryIndex
        private KDQueryNode Pop() {
        
            var node = queryNodes[queryIndex];
            queryIndex++;
            return node;
        }

        private void ResetStack() {
            
            count = 0;
            queryIndex = 0;
        }

        public KDQuery(int initialStackSize = 2048) {

            queryNodes = new KDQueryNode[initialStackSize];
        }

        // Finds closest node (which doesn't necesarily contain closest point!!)
        //! TO FINISH, TRICKY MATH
        public KDNode ClosestNodeQuery(KDTree tree, Vector3 qPosition) {

            ResetStack();
            
            var rootNode = tree.rootNode;

            var rootQueryNode = PushGet();

            rootQueryNode.node = rootNode;
            rootQueryNode.tempClosestPoint = rootNode.bounds.ClosestPoint(qPosition);

            KDQueryNode queryNode = null;
            KDNode node = null;

            KDQueryNode negativeQueryNode = null;
            KDQueryNode positiveQueryNode = null;

            Vector3[] points = tree.points;
            int[] permutation = tree.permutation;

            // searching for index that points to closest point
            float minSqrDist = Single.MaxValue;
            int minIndex = 0;

            // KD search with pruning (don't visit areas which distance is more away than range)
            // Recursion done on Stack
            while (LeftToProcess > 0) {

                queryNode = Pop();
                node = queryNode.node;
                
                // pruning!
                if (!node.Leaf) {

                    int partitionAxis = node.partitionAxis;
                    float partitionCoord = node.partitionCoordinate;

                    Vector3 tempClosestPoint = queryNode.tempClosestPoint;
                    

                    if ((tempClosestPoint[partitionAxis] - partitionCoord) < 0) {
                    
                        negativeQueryNode = PushGet(node.negativeChild, tempClosestPoint);

                        tempClosestPoint[partitionAxis] = partitionCoord;
                        
                        float dist = Vector3.SqrMagnitude(tempClosestPoint - qPosition);

                        if (node.positiveChild.Count != 0 && dist <= minSqrDist) {
                        
                            positiveQueryNode = PushGet(node.positiveChild, tempClosestPoint);
                        }
                    }
                    else {
                        
                        positiveQueryNode = PushGet(node.positiveChild, tempClosestPoint);

                        tempClosestPoint[partitionAxis] = partitionCoord;

                        if (node.negativeChild.Count != 0 &&
                        Vector3.SqrMagnitude(tempClosestPoint - qPosition) <= minSqrDist) {

                            negativeQueryNode = PushGet(node.negativeChild, tempClosestPoint);
                        }
                    }
                }
                else {

                    for (int i = node.start; i < node.end; i++) {
                    
                        int index = permutation[i];

                        float sqrDist = Vector3.SqrMagnitude(qPosition - points[index]);
                        
                        if (sqrDist < minSqrDist) {
                            minSqrDist = sqrDist;
                            minIndex = index;
                        }
                    }

                    // leaf node
                }
            }
            throw new NotImplementedException();
        }
        

        public int ClosestPointQuery(KDTree tree, Vector3 qPosition) {

            var node = ClosestNodeQuery(tree, qPosition);
            
            Vector3[] points = tree.points;
            int[] permutation = tree.permutation;

            float minSqrDist = Single.MaxValue;
            int minIndex = 0;
            
            for(int i = node.start;i < node.end; i++) {

                int index = permutation[i];

                float sqrDist = Vector3.SqrMagnitude(qPosition - points[index]);

                if(sqrDist < minSqrDist) {
                    minSqrDist = sqrDist;
                    minIndex = index;
                }
            }

            return minIndex;
        }

        
        public void RadiusQueryNodes(KDTree tree, Vector3 qPosition, float qRadius, List<KDNode> nodes) {

            ResetStack();
            
            float squaredRadius = qRadius * qRadius;

            var rootNode = tree.rootNode;

            var rootQueryNode = PushGet();

            rootQueryNode.node = rootNode;
            rootQueryNode.tempClosestPoint = rootNode.bounds.ClosestPoint(qPosition);

            KDQueryNode queryNode = null;
            KDNode node = null;

            KDQueryNode negativeQueryNode = null;
            KDQueryNode positiveQueryNode = null;

            // KD search with pruning (don't visit areas which distance is more away than range)
            // Recursion done on Stack
            while (LeftToProcess > 0) {
            
                queryNode = Pop();
                node = queryNode.node;
                // pruning!
                if (!node.Leaf) {

                    int partitionAxis = node.partitionAxis;
                    float partitionCoord = node.partitionCoordinate;

                    Vector3 tempClosestPoint = queryNode.tempClosestPoint;
                    
                    if ((tempClosestPoint[partitionAxis] - partitionCoord) < 0) {

                        // we already know we are inside negative bound/node, 
                        // so we don't need to test for distance
                        // push to stack for later querying
                        
                        // tempClosestPoint is inside negative side
                        // assign it to negativeChild
                        negativeQueryNode = PushGet(node.negativeChild, tempClosestPoint);
                        
                        tempClosestPoint[partitionAxis] = partitionCoord;

                        // testing other side
                        if (node.positiveChild.Count != 0 &&
                        Vector3.SqrMagnitude(tempClosestPoint - qPosition) <= squaredRadius) {

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
                        
                        //project the tempClosestPoint to other bound
                        tempClosestPoint[partitionAxis] = partitionCoord;

                        // testing other side
                        if (node.negativeChild.Count != 0 &&
                        Vector3.SqrMagnitude(tempClosestPoint - qPosition) <= squaredRadius) {

                            negativeQueryNode = PushGet(node.negativeChild, tempClosestPoint);
                        }
                    }
                }
                else {

                    //! LEAF NODE found
                    if (node.Count > 0) {
                        nodes.Add(node);
                    }
                }
            }
        }
        
        public void RangeQueryNodes(KDTree tree, Vector3 qPosition, float qRadius, List<KDNode> nodes) {

        }

        // need min heap
        public void KNearestQuery(KDTree tree, Vector3 qPosition, int count, List<int> indices) {

        }
    }
}