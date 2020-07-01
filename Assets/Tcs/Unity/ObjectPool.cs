using UnityEngine;
using System.Collections.Generic;

// From website: http://forum.unity3d.com/threads/76851-Simple-Reusable-Object-Pool-Help-limit-your-instantiations!
public class ObjectPool : MonoBehaviour
{
	public static ObjectPool Instance;

	/// <summary>
	/// The object prefabs which the pool can handle.
	/// </summary>	
	public GameObject[] ObjectPrefabs;

	/// <summary>
	/// The pooled objects currently available.
	/// </summary>
	public List<GameObject>[] PooledObjects;

	/// <summary>
	/// The amount of objects of each type to buffer.
	/// </summary>
	public int[] AmountToBuffer;
	public int DefaultBufferAmount = 3;

	/// <summary>
	/// The container object that we will keep unused pooled objects so we dont clog up the editor with objects.
	/// </summary>
	protected GameObject containerObject;

	private int _pooled;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(this);
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start()
	{
		containerObject = new GameObject("Container");
		containerObject.transform.parent = transform;

		//Loop through the object prefabs and make a new list for each one.		
		//We do this because the pool can only support prefabs set to it in the editor,
		//so we can assume the lists of pooled objects are in the same order as object prefabs in the array
		PooledObjects = new List<GameObject>[ObjectPrefabs.Length];

		int i = 0;
		foreach (GameObject objectPrefab in ObjectPrefabs)
		{
			PooledObjects[i] = new List<GameObject>();

			int bufferAmount;

			if (i < AmountToBuffer.Length)
				bufferAmount = AmountToBuffer[i];
			else
				bufferAmount = DefaultBufferAmount;

			for (int n = 0; n < bufferAmount; n++)
			{
				GameObject newObj = Instantiate(objectPrefab) as GameObject;
				newObj.name = objectPrefab.name;
				PoolObject(newObj);
			}

			i++;
		}
	}

	/// <summary>
	/// Gets a new object for the name type provided.  If no object type exists or if onlypooled is true and there is no objects of that type in the pool
	/// then null will be returned.
	/// </summary>
	/// <returns>
	/// The object for type.
	/// </returns>
	/// <param name='objectType'>
	/// Object type.
	/// </param>
	/// <param name='onlyPooled'>
	/// If true, it will only return an object if there is one currently pooled.
	/// </param>
	public GameObject GetObjectForType(string objectType, bool onlyPooled)
	{
		for (int i = 0; i < ObjectPrefabs.Length; i++)
		{
			GameObject prefab = ObjectPrefabs[i];
			if (prefab.name == objectType)
			{
				if (PooledObjects[i].Count > 0)
				{
					GameObject pooledObject = PooledObjects[i][0];
					PooledObjects[i].RemoveAt(0);
					pooledObject.transform.SetParent(null);
					pooledObject.SetActive(true);

					//#if UNITY_EDITOR_OSX
					//Debug.Log(pooledObjects[i].Count);					
					//#endif

					return pooledObject;
				}
				else if (!onlyPooled)
				{
					GameObject newInstance = Instantiate(ObjectPrefabs[i]) as GameObject;
					newInstance.name = objectType;
					return newInstance;
				}
				break;
			}
		}
		//If we have gotten here either there was no object of the specified type or non were left in the pool with onlyPooled set to true
		return null;
	}


	/// <summary>
	/// Pools the object specified.  Will not be pooled if there is no prefab of that type.
	/// </summary>
	/// <param name='obj'>
	/// Object to be pooled.
	/// </param>
	public void PoolObject(GameObject obj)
	{
		for (int i = 0; i < ObjectPrefabs.Length; i++)
		{
			if (ObjectPrefabs[i].name == obj.name)
			{
				obj.SetActive(false);
				obj.transform.SetParent(containerObject.transform);
				PooledObjects[i].Add(obj);

#if UNITY_EDITOR_OSX
				//Debug.Log(pooledObjects[i].Count);
#endif

				return;
			}
		}
	}
}