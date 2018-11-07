using UnityEngine;
using UnityEditor;

namespace DataStructures.ViliWonka.KDTree {

    public struct KDBounds {

        public Vector3 min;
        public Vector3 max;

        public Vector3 size {

            get {
                return max - min;
            }
        }

        // returns unity bounds
        public Bounds Bounds {

            get {
                return new Bounds(
                    (min + max) / 2,
                    (max - min)
                );
            }
        }

        public Vector3 ClosestPoint(Vector3 point) {
            return Bounds.ClosestPoint(point);
        }
    }
}