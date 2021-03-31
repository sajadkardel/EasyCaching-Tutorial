#🚀 EasyCachin

<a href="https://github.com/dotnetcore/EasyCaching">EasyCaching</a> library is a Easy Implementation of Caching System.

In this solution, there are three projects for easycaching providers, including in_memory, redis and hybrid providers, in which the local and distributed cache system is implemented.

By referring to this article,
( <a href="https://vrgl.ir/SJlkR">First Part</a> 
and 
<a href="https://vrgl.ir/9mIBO">Second Part</a> )
you can learn the explanations related to the types of caches and the implementation and type of operation of each of the easycaching package providers.

 Another point in this solution is to implement the cache system of each project at a different level of the project, which is to implement it in the controller or service or as an annotation attribute or a dedicated middleware.

For example, in the in-memory project, the cache is implemented in service, in redis project, the cache is implemented as an attribute, and in the hybrid project, it is implemented as a dedicated middleware.

 Finally, we tried to implement caching for real and professional projects.
