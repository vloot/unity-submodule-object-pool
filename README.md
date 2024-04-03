# Object Pool for Unity

### Usage
1. Create a class that implements `IPoolObject<T>` interface:
```
public  class  ObjectToSpawn:  MonoBehaviour,  IPoolObject<ObjectToSpawn> {
	
	public  GameObject  GetGameObject() {
		return  gameObject;
	} 

	public  Transform  GetTransform() {
		return  transform;
	}

}
```
2.  Create an object pool:
```
objectPool  =  new  ObjectPool<ObjectToSpawn>(20,  prefab,  parentTransform,  20);
```
3.  Spawn/Despawn objects using `Spawn( )` and `Despawn( )` methods
