# KDTree
## Still WIP, do not use until I remove this notice

### Description

3D KDTree for Unity, with fast construction and fast & thread-safe querying, with zero memory garbage.

### It was designed:

* to be working with Unity Vector3 structs, but can be modified to work with any other 3D (or 2D & 4D or higher) struct/arrays
* for speedy & light Construction & Reconstruction,
* to be light on memory, everything is pooled,
* for fast querying, 
* queryable from multiple threads (thread-safe),

### Query modes:
* K-Nearest
* Closest point query
* Radius query
* Interval query

#### How to use


##### Construction

First you need some array of points.

Example:

```cs
    Vector3[] pointCloud = new Vector3[10000];

    for(int i = 0; i < pointCloud.Length; i++)
		pointCloud[i] = Random.insideUnitSphere
```

Then build the tree out of it. Note that original pointCloud shouldn't change, since tree is referencing it!

```cs
	KDTree tree = KDTreeBuilder.Instance.Build(pointCloud);
```

Now that tree has been constructed, make a KDQuery object. 

Note: if you wish to do querying from multiple threads, then each own thread should have it's own KDQuery object.

```cs
    Query.KDQuery query = new Query.KDQuery();
```

##### Querying

For most query methods you need pre-initialized results list. 
Results list will contain indexes for pointCloud array.

List should be cleared; but it's not necesary to clear it (if you wish to do multiple queries), but this way you will have duplicate indexes.
```cs
    List<int> results = new List<int>();

    query.Radius(tree, position, radius, results);

    query.KNearest(tree, position, k, results);

    query.Interval(tree, min, max, results);

	int index = query.ClosestPoint(tree, position);
```

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
