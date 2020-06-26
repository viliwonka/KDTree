/*MIT License

Copyright(c) 2018 Vili Volčini / viliwonka

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataStructures.ViliWonka.Tests {

    using KDTree;

    public class KDTreeQueryResizeTests : MonoBehaviour {

        public QType QueryType;

        public int K = 13;

        [Range(0f, 100f)]
        public float Radius = 0.1f;

        public bool DrawQueryNodes = true;

        public Vector3 IntervalSize = new Vector3(0.2f, 0.2f, 0.2f);

        KDTree tree;

        KDQuery query;

        void Awake() {

            Vector3[] pointCloud = new Vector3[1];

            query = new KDQuery();

            for(int i = 0; i < pointCloud.Length; i++) {

                pointCloud[i] = new Vector3(

                    (1f + Random.value * 0.25f),
                    (1f + Random.value * 0.25f),
                    (1f + Random.value * 0.25f)
                );

            }

            for(int i = 0; i < pointCloud.Length; i++) {

                for(int j = 0; j < i; j++) {

                    pointCloud[i] += LorenzStep(pointCloud[i]) * 0.01f;
                }
            }

            tree = new KDTree(pointCloud, 32);
        }

        Vector3 LorenzStep(Vector3 p) {

            float ρ = 28f;
            float σ = 10f;
            float β = 8 / 3f;

            return new Vector3(

                σ * (p.y - p.x),
                p.x * (ρ - p.z) - p.y,
                p.x * p.y - β * p.z
            );
        }

        void Update() {

            int oldLen = tree.Count;
            
            tree.SetCount(oldLen + 3);

            for(int i = oldLen; i < tree.Points.Length; i++) {
                tree.Points[i] = new Vector3(
                    (1f + Random.value * 0.9f),
                    (1f + Random.value * 0.9f),
                    (1f + Random.value * 0.9f)
                );
            }

            // repeat 5 times
            for(int r = 0; r < 5; r++) {

                for(int i = 0; i < tree.Count; i++) {

                    tree.Points[i] += LorenzStep(tree.Points[i]) * Time.deltaTime * 0.1f;
                }
            }
            
            tree.Rebuild();
        }

        private void OnDrawGizmos() {

            if(query == null) {
                return;
            }

            Vector3 size = 0.2f * Vector3.one;

            for(int i = 0; i < tree.Points.Length; i++) {

                Gizmos.DrawCube(tree.Points[i], size);
            }

            var resultIndices = new List<int>();

            Color markColor = Color.red;
            markColor.a = 0.5f;
            Gizmos.color = markColor;

            switch(QueryType) {

                case QType.ClosestPoint: {

                    query.ClosestPoint(tree, transform.position, resultIndices);
                }
                break;

                case QType.KNearest: {

                    query.KNearest(tree, transform.position, K, resultIndices);
                }
                break;

                case QType.Radius: {

                    query.Radius(tree, transform.position, Radius, resultIndices);

                    Gizmos.DrawWireSphere(transform.position, Radius);
                }
                break;

                case QType.Interval: {

                    query.Interval(tree, transform.position - IntervalSize/2f, transform.position + IntervalSize/2f, resultIndices);

                    Gizmos.DrawWireCube(transform.position, IntervalSize);
                }
                break;

                default:
                break;
            }

            for(int i = 0; i < resultIndices.Count; i++) {

                Gizmos.DrawCube(tree.Points[resultIndices[i]], 2f * size);
            }

            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.position, 4f * size);

            if(DrawQueryNodes) {
                query.DrawLastQuery();
            }
        }
    }
}