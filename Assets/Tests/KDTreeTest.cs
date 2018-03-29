using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Floatlands.DataStructures.Tests {

    public class KDTreeTest : MonoBehaviour {
    
        Vector3[] points10k;
        Vector3[] points100k;
        Vector3[] points1m;

        Vector3[] testingArray;
        Stopwatch stopwatch;

        void Awake() {

            points10k = new Vector3[10000];
            points100k = new Vector3[100000];
            points1m = new Vector3[1000000];
            
            stopwatch = new Stopwatch();
        }

        // TODO dej v KDTree MAX DEPTH (da se ve koliko je max globina pri konstrukciji)
        // TODO dej v KDTree NODE COUNT (da se ve koliko nodeov)
        // TODO dej v KDTree še DEPTH COUNT (da se potem average DEPTH izračuna)
        // TODO simuliri enak scenarij kot v Unity projektu kjer zašteka
        private void Start() {

            LimitedMaxHeap h = new LimitedMaxHeap(50);

            System.Random r = new System.Random();

            h.Push(1);
            h.Push(5);
            h.Push(10);
            h.Push(3);
            h.Push(30);
            h.Push(2);
            h.Push(8);

            h.Print();
        /*
            testingArray = points10k;
            Debug.Log(" -- 10K THOUSAND POINTS --");
            TestSet();

            testingArray = points100k;
            Debug.Log(" -- 100K THOUSAND POINTS --");
            TestSet();
            
            testingArray = points1m;
            Debug.Log(" -- 1 MILLION POINTS --");
            TestSet();            
            */
        }

        void TestSet() {
            //Debug.Log(testingArray.Length + " random points for each test:");
            Test(5, "Uniform", RandomizeUniform);
            Test(5, "Triangular", RandomizeUniform);
            Test(5, "2D planar", Randomize2DPlane);
            Test(5, "2D planar, sorted", Sorted2DPlane);

        }

        void Test(int tests, string distributionName, System.Action randomize) {
        
            long sum = 0;
            for (int i = 0; i < tests; i++) {

                randomize();
                long time = ConstructionTest();
 

                sum += time;
            }

            UnityEngine.Debug.Log("Average " + distributionName + " distribution construction time: " + (long) (sum / (float) tests) + " ms");
        }

        // uniform distribution
        void RandomizeUniform() {

            for (int i = 0; i < testingArray.Length; i++)
                testingArray[i] = new Vector3(
                    Random.value, 
                    Random.value, 
                    Random.value);
        }

        // triangle distribution
        void RandomizeTriangle() {

            for (int i = 0; i < testingArray.Length; i++)
                testingArray[i] = new Vector3(
                    Random.value + Random.value, 
                    Random.value + Random.value, 
                    Random.value + Random.value
                );
        }
        
        // 2D plane, with 10% of noise
        void Randomize2DPlane() {
            //if U and V are very similar => degenerate plane aka line
            Vector3 U = Random.onUnitSphere;
            Vector3 V = Random.onUnitSphere;
            
            for (int i = 0; i < testingArray.Length; i++)
                testingArray[i] = Random.value * U + Random.value * V + Random.insideUnitSphere * 0.1f;

        }

        void Sorted2DPlane() {

            Randomize2DPlane();

            //Sort by all coordinates
            System.Array.Sort<Vector3>(testingArray, (v1, v2) => v1.x.CompareTo(v2.x));
            System.Array.Sort<Vector3>(testingArray, (v1, v2) => v1.y.CompareTo(v2.y));
            System.Array.Sort<Vector3>(testingArray, (v1, v2) => v1.z.CompareTo(v2.z));

        }


        KDTree tree;
        
        long ConstructionTest() {

            stopwatch.Reset();
            stopwatch.Start();

            tree = KDTreeBuilder.Instance.Build(testingArray);

            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }

        //to finish
        long RadiusQuery() {
        
            stopwatch.Reset();
            stopwatch.Start();

            tree = KDTreeBuilder.Instance.Build(testingArray);

            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }


    }
}