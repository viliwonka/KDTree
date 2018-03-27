# KDTree
## Still WIP, do not use until I remove this notice

### Description

Array based KDTree. Uses Hoare partitioning to sort indices inplace. Uses slidding mid-point for node splitting.

### It was designed to be:
* working with Unity Vector3D, can easily be modified to work with any other 3D (or 2D) struct
* fast for construction, since everything it allocates is permament permutation array and KDNodes
* light for memory, leaves no garbage in construction phase - everything is permuted in one permutation array
* because it works with permutation array, original input array of points is not modified
* multithreaded construction (WIP)
* fast-querying
* thread-safe querying (WIP)

#### Use cases

#### How to use

##### Construction

##### Querying

#### How it works?

##### Construction

##### Querying

#### Sources

https://www.cs.umd.edu/~mount/Papers/cgc99-smpack.pdf - Paper about slidding mid-point rule for node splitting.
