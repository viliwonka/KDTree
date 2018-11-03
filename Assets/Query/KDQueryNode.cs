using UnityEngine;
using UnityEditor;

namespace DataStructures.Query {

    public class KDQueryNode {

        public KDNode node;
        public Vector3 tempClosestPoint;

        public KDQueryNode() {

        }

        public KDQueryNode(KDNode node, Vector3 tempClosestPoint) {
            this.node = node;
            this.tempClosestPoint = tempClosestPoint;
        }
    }
}