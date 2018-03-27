// Not thread safe, because it doesn't have it's own iterator and also contains temporary variable tempClosestPoint in node
// read KDNode.cs for more info
// But is faster to query because it can pass all information down the tree

// #define DEBUG_KD
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Floatlands.DataStructures {
   
    public class KDTree {
    
        public KDNode rootNode;

        public Vector3[] points;
        public int[] permutation;
        
        public KDTree(KDNode rootNode, Vector3[] points, int[] permutation) {

            this.points = points;
            this.permutation = permutation;
            this.rootNode = rootNode;
        }
        /*
        // Radius Query
        public void RadiusQuery(Vector3 worldCenter, List<KDNode> nodes, float range = 100f, KDQuery query) {
        
            if(traversalNodes == null)
                traversalNodes = new Stack<KDQueryNode>(1024);
        
            // clear queue
            traversalNodes.Clear();

            float squaredRange = range * range;
            
            traversalNodes.Push(new KDQueryNode(rootNode, rootNode.bounds.ClosestPoint(worldCenter)));
            
            // push root tree
            nodesToProcess.Push(rootNode);
            
            // KD search with pruning (don't visit areas which distance is more away than range)
            // Recursion done on Stack
            while (nodesToProcess.Count > 0) {

                var node = nodesToProcess.Pop();

                // pruning!
                if (!node.Leaf) {
                
                    int partitionAxis = node.partitionAxis;
                    float partitionCoord = node.partitionCoordinate;

                    Vector3 tempClosestPoint = node.tempClosestPoint;

                    if ((tempClosestPoint[partitionAxis] - partitionCoord) < 0) {

                        // tempClosestPoint inside negative side
                        // assign it to negativeChild
                        node.negativeChild.tempClosestPoint = tempClosestPoint;
                        
                        // we already know we are inside negative bound/node, 
                        // so we don't need to test for distance
                        // push to stack for later querying
                        nodesToProcess.Push(node.negativeChild);


                        tempClosestPoint[partitionAxis] = partitionCoord;
                        node.positiveChild.tempClosestPoint = tempClosestPoint;
                        
                        // testing other side
                        if (Vector3.SqrMagnitude(node.positiveChild.tempClosestPoint - worldCenter) <= squaredRange
                        && node.positiveChild.Count != 0)
                            nodesToProcess.Push(node.positiveChild);
                    }
                    else {
                        // tempClosestPoint inside positive side
                        // assign it to positiveChild
                        node.positiveChild.tempClosestPoint = tempClosestPoint;

                        // we already know we are inside positive bound/node, 
                        // so we don't need to test for distance
                        // push to stack for later querying
                        nodesToProcess.Push(node.positiveChild);
                        
                        tempClosestPoint[partitionAxis] = partitionCoord;
                        node.negativeChild.tempClosestPoint = tempClosestPoint;
                        
                        // but we have to test other side
                        if (Vector3.SqrMagnitude(node.negativeChild.tempClosestPoint - worldCenter) <= squaredRange
                        && node.negativeChild.Count != 0)
                            nodesToProcess.Push(node.negativeChild);
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
        */
    }
    
}