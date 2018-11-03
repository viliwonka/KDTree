using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataStructures {

    public class KDNode {

        public float partitionCoordinate;
        public int partitionAxis = -1;

        public KDNode negativeChild;
        public KDNode positiveChild;

        public int start;
        public int end;

        public int Count { get { return end - start; } }

        public bool Leaf { get { return partitionAxis == -1; } }

        public KDBounds bounds;

        //public Vector3 tempClosestPoint; // temporary constant for faster querying
    };

}
