# KDTree
## Still WIP, do not use until I remove this notice

### Description

3D KDTree for Unity, with fast construction and fast & thread-safe querying, with zero memory garbage.

### It was designed:

* to be working with Unity Vector3 structs, but can be modified to work with any other 3D (or 2D & 4D or higher) struct/arrays
* to be fast & light for Construction & Reconstruction,
* to be light on memory, everything is pooled,
* to be fast for querying, 
* queryable from multiple threads,

### Query modes:
* K-Nearest
* Closest point query
* Radius query
* Interval query

#### How to use

##### Construction



##### Querying

#### How it works?

Uses internal permutation array, so it doesn't modify original data array. 
Hoare partitioning enables to sort permutation array inplace. (Quicksort uses hoare partitioning, too).
Mid-point rule is used for node splitting - not optimal split but makes construction very fast.


##### Construction
0. Permutation array is identity array at first, root node indices consists of whole permutation array, bounds are calculated
1. Selects longest axis to split (max axis)
2. Uses slidding-mid point rule to calculate splitting pivot (CalculatePivot function) 
3. Modified Hoare partitioning runs through to split array based on splitting pivot. (Partition function)
4. For each sub-node, Termination function is called to determine when-ever it needs to continue splitting (ContinueSplit function)

##### Querying



#### Sources

https://www.cs.umd.edu/~mount/Papers/cgc99-smpack.pdf - Paper about slidding mid-point rule for node splitting.
