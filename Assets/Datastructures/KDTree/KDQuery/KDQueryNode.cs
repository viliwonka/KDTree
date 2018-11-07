using UnityEngine;
using UnityEditor;

namespace DataStructures.ViliWonka.KDTree {

    public class KDQueryNode {

        public KDNode node;
        public Vector3 tempClosestPoint;
        public float distance;

        public KDQueryNode() {

        }

        public KDQueryNode(KDNode node, Vector3 tempClosestPoint) {
            this.node = node;
            this.tempClosestPoint = tempClosestPoint;
        }

    }
}