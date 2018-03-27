# KDTree
## Still WIP, do not use until I remove this notice

### Description

Array based KDTree. Uses Hoare partitioning to sort indices inplace. Uses slidding mid-point for node splitting.

### It was designed to:
* be working with Unity Vector3D, can easily be modified to work with any other 3D (or 2D) struct
* be fast for construction, since everything it allocates is permament permutation array and KDNodes
* be light for memory, leaves no garbage in construction phase and leave no garbage when querying (WIP)
* work permutation array, original input array of points is not modified
* be multithreaded construction (WIP)
* be fast for querying, also 
* work from multiple threads, thread-safe (WIP)
* work only with Euclid metric for now

### Query modes:
* Closest point query
* K-Neighbors query
* Range query

#### How to use

##### Construction


##### Querying

#### How it works?

##### Construction
0. Permutation array is identity array, root node indices consists of whole permutation array, bounds are calculated
1. Selects longest axis to split (max axis)
2. Uses slidding-mid point rule to calculate splitting pivot (CalculatePivot function) 
3. Modified Hoare partitioning runs through to split array based on splitting pivot. (Partition function)
4. For each sub-node, Termination function is called to determine when-ever it needs to continue splitting (ContinueSplit function)

##### Querying


#### Sources

https://www.cs.umd.edu/~mount/Papers/cgc99-smpack.pdf - Paper about slidding mid-point rule for node splitting.
