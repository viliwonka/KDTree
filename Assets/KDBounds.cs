using UnityEngine;
using UnityEditor;

namespace Floatlands.DataStructures {

    public struct KDBounds {

        public Vector3 min;
        public Vector3 max;

        public Vector3 size {

            get {
                return max - min;
            }
        }

        // returns real bounds
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