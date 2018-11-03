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

    public abstract class KDQueryBase {

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

        protected KDQueryBase(int initialStackSize = 2048) {
            queryNodes = new KDQueryNode[initialStackSize];
        }

    }

}