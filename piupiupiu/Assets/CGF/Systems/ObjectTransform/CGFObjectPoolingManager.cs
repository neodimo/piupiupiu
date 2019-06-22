/// INFORMATION
/// 
/// Project: Chloroplast Games Framework
/// Game: Chloroplast Games Framework
/// Date: 07/02/2017
/// Author: Chloroplast Games
/// Website: http://www.chloroplastgames.com
/// Programmers: David Cuenca, Adan Baró 
/// Description: Manager that allows to the associated gameobject manages the object pooling.
///


using System.Collections.Generic;
using UnityEngine;

namespace Assets.CGF.Systems.ObjectTransform
{

	
	/// \english
	/// <summary>
    /// Manager that allows to the associated gameobject manages the object pooling.
	/// </summary>
	/// \endenglish
	/// \spanish
    /// <summary>
    /// Gestor que permite al gameobject asociado gestionar el object pooling.
    /// </summary>
	/// \endspanish
    public class CGFObjectPoolingManager : CGFSingletonClass<CGFObjectPoolingManager>
    {

        #region Public Variables

			/// \english
			/// <summary>
			/// Event delegate of ObjectPoolEventHandler.
			/// </summary>
			/// \endenglish
			/// \spanish
			/// <summary>
			/// Delegado del evento ObjectPoolEventHandler.
			/// </summary>
			/// \endspanish
			public delegate void ObjectPoolEventHandler();

			/// \english
			/// <summary>
			/// Event ObjectPoolCreated.
			/// </summary>
			/// \endenglish
			/// \spanish
			/// <summary>
			/// Evento ObjectPoolCreated.
			/// </summary>
			/// \endspanish
			public event ObjectPoolEventHandler ObjectPoolCreated;

			/// \english
			/// <summary>
			/// Event ObjectPoolCleared.
			/// </summary>
			/// \endenglish
			/// \spanish
			/// <summary>
			/// Evento ObjectPoolCleared.
			/// </summary>
			/// \endspanish
			public event ObjectPoolEventHandler ObjectPoolCleared;

			/// \english
			/// <summary>
			/// Event delegate of ObjectSpawnedEventHandler.
			/// </summary>
			/// \endenglish
			/// \spanish
			/// <summary>
			/// Delegado del evento ObjectSpawnedEventHandler.
			/// </summary>
			/// \endspanish
			public delegate void ObjectSpawnedEventHandler(GameObject obj);

			/// \english
			/// <summary>
			/// Event ObjectSpawned.
			/// </summary>
			/// \endenglish
			/// \spanish
			/// <summary>
			/// Evento ObjectSpawned.
			/// </summary>
			/// \endspanish
			public event ObjectSpawnedEventHandler ObjectSpawned;

        #endregion


        #region Private Variables

			/// \english
			/// <summary>
			/// Instance all objects on Awake.
			/// </summary>
			/// \endenglish
			/// \spanish
			/// <summary>
			/// Instancia todos los objetos pooleables en el Awake.
			/// </summary>
			/// \endspanish
			[SerializeField]
			protected bool _instantiateOnAwake = true;

			/// \english
			/// <summary>
			/// Pooleable object.
			/// </summary>
			/// \endenglish
			/// \spanish
			/// <summary>
			/// Objeto pooleable.
			/// </summary>
			/// \endspanish
			[SerializeField]
            protected List<CGFObjectPoolingItemClass> _objectsToPool = new List<CGFObjectPoolingItemClass>();


            /// \english
            /// <summary>
            /// Dictionary to be accessed through an index to the pool.
            /// </summary>
            /// \endenglish
            /// \spanish
            /// <summary>
            /// Diccionario para poder acceder a través de un índice a la pila.
            /// </summary>
            /// \endspanish
            protected Dictionary<GameObject, CGFObjectPoolingItemClass> _objectPoolingItem = new Dictionary<GameObject, CGFObjectPoolingItemClass>();

            /// \english
            /// <summary>
            /// Dictionary to be accessed through an index to the pool.
            /// </summary>
            /// \endenglish
            /// \spanish
            /// <summary>
            /// Diccionario para poder acceder a través de un índice a la pila.
            /// </summary>
            /// \endspanish
            protected Dictionary<string, GameObject> _poolNameToGameObject = new Dictionary<string, GameObject>();
        
        #endregion


        #region Main Methods

			void Awake()
			{
			
				if (_instantiateOnAwake)
				{
				
					CreatePool();
					
				}
				
			}

        #endregion


        #region Utility Methods


            /// \english
            /// <summary>
            /// Destroy all objects of all pools.
            /// </summary>
            /// \endenglish
            /// \spanish
            /// <summary>
            /// Destuye todos los objetos de todas las pilas.
            /// </summary>
            /// \endspanish
            public virtual void DestroyAll()
	        {
                
                for (int i = 0; i < _objectsToPool.Count; i++)
	            {

	                _objectsToPool[i].DestroyAllObjects();

	            }

	        }

            /// \english
            /// <summary>
            /// Destroy all objects of a type.
            /// </summary>
            /// \endenglish
            /// \spanish
            /// <summary>
            /// Destuye todos los objetos de un tipo.
            /// </summary>
            /// \endspanish
            public virtual void DestroyAllObjectOfType(GameObject go)
	        {

                if (_poolNameToGameObject.ContainsKey(go.name))
                {

                    _objectPoolingItem[go].DestroyAllObjects();

                }

	        }

	        /// \english
	        /// <summary>
	        /// Add gameobjects to the pool.
	        /// </summary>
            /// <param name="item">Object pooling Class.</param>
	        /// \endenglish
	        /// \spanish
	        /// <summary>
	        /// Añade gameobjects a la pila.
	        /// </summary>
	        /// <param name="item">Clase del object pooling.</param>
	        /// \endspanish
	        public virtual void AddObject(CGFObjectPoolingItemClass item)
			{

                if (_objectPoolingItem.ContainsKey(item.ObjectToPool))
                {

                    Debug.LogError(item.ObjectToPool.name + " ya tiene pool");

                    return;

                }

                _objectsToPool.Add(item);

                item.Initialize();

                _objectPoolingItem.Add(item.ObjectToPool, item);

                _poolNameToGameObject.Add(item.ObjectToPool.name, item.ObjectToPool);
				
			}

			/// \english
			/// <summary>
			/// Create a object pool.
			/// </summary>
			/// \endenglish
			/// \spanish
			/// <summary>
			/// Crea una pila de objetos.
			/// </summary>
			/// \endspanish
			public virtual void CreatePool()
			{

			    for (int i = 0; i < _objectsToPool.Count; i++)
			    {

                    if (_objectsToPool[i].ObjectToPool != null)
			        {

                        _objectsToPool[i].Initialize();

                        _objectPoolingItem.Add(_objectsToPool[i].ObjectToPool, _objectsToPool[i]);

                        _poolNameToGameObject.Add(_objectsToPool[i].ObjectToPool.name, _objectsToPool[i].ObjectToPool);

			        }

			    }

			}

	        /// \english
			/// <summary>
			/// Instance an pool object.
			/// </summary>
			/// <param name="prefab">Object to instance.</param>
			/// <param name="spawnPosition">Instantiation position.</param>
			/// <param name="spawnRotation">Instantiation rotation.</param>
			/// <returns>Instantiated gameobject.</returns>
			/// \endenglish
			/// \spanish
			/// <summary>
			/// Instancia un objeto de la pila.
			/// </summary>
			/// <param name="prefab">Objeto a instanciar.</param>
			/// <param name="spawnPosition">Posición de instanciación.</param>
			/// <param name="spawnRotation">Rotación de instanciación.</param>
			/// <returns>gameobject instanciado.</returns>
			/// \endspanish
			public virtual GameObject InstantiatePoolObject(GameObject prefab, Vector3 spawnPosition, Quaternion spawnRotation)
			{
			
				GameObject gameObjectPool = GetPooledObject(prefab);

                if (gameObjectPool == null)
                {

                    gameObjectPool = Instantiate(prefab, spawnPosition, spawnRotation) as GameObject;

                    gameObjectPool.name = prefab.name;

                }

				if (gameObjectPool != null)
				{
				
					gameObjectPool.transform.position = spawnPosition;

					gameObjectPool.transform.rotation = spawnRotation;

					gameObjectPool.SetActive(true);
					
				}

				return gameObjectPool;
				
			}

			/// \english
			/// <summary>
			/// Instance an pool object inside a parent object.
			/// </summary>
			/// <param name="prefab">Object to instance.</param>
			/// <param name="spawnPosition">Instantiation position.</param>
			/// <param name="spawnRotation">Instantiation rotation.</param>
			/// <param name="parentTransform">Parent object.</param>
			/// <returns>Instantiated gameobject.</returns>
			/// \endenglish
			/// \spanish
			/// <summary>
			/// Instancia un objeto de la pila en un objeto padre.
			/// </summary>
			/// <param name="prefab">Objeto a instanciar.</param>
			/// <param name="spawnPosition">Posición de instanciación.</param>
			/// <param name="spawnRotation">Rotación de instanciación.</param>
			/// <param name="parentTransform">Objeto padre.</param>
			/// <returns>gameobject instanciado.</returns>
			/// \endspanish
			public virtual GameObject InstantiatePoolObject(GameObject prefab, Vector3 spawnPosition, Quaternion spawnRotation, Transform parentTransform)
			{
			
				GameObject gameObjectPool = GetPooledObject(prefab);

                if (gameObjectPool == null)
                {

                    gameObjectPool = Instantiate(prefab, spawnPosition, spawnRotation, parentTransform) as GameObject;

                    gameObjectPool.name = prefab.name;

                }

                if (gameObjectPool != null)
				{

					gameObjectPool.transform.SetParent(parentTransform);

                    gameObjectPool.transform.position = spawnPosition;

                    gameObjectPool.transform.rotation = spawnRotation;

					gameObjectPool.SetActive(true);
					
				}

				OnObjectSpawned(gameObjectPool);

				return gameObjectPool;
				
			}

            /// \english
            /// <summary>
            /// Disable object.
            /// </summary>
            /// \endenglish
            /// \spanish
            /// <summary>
            /// Desactiva el objeto.
            /// </summary>
            /// \endspanish
            /// <summary>
            /// Desactiva el objeto.
            /// </summary>
            /// <param name="go">Objeto.</param>
            public virtual void DeSpawn(GameObject go)
	        {

                if (go == null)
                    return;

                if (!_poolNameToGameObject.ContainsKey(go.name))
                {

                    go.SetActive(false);

                }
                else
                {

                    _objectPoolingItem[_poolNameToGameObject[go.name]].DeSpawn(go);


                }

	        }

            /// \english
            /// <summary>
            /// Return the next available object to enable, if doesn't exist, return null.
            /// </summary>
            /// <returns>Object to enable.</returns>
            /// \endenglish
            /// \spanish
            /// <summary>
            /// Devuelve el siguiente objeto disponible para activar, si no existe, devuelve null.
            /// </summary>
            /// <returns>Objeto a activar.</returns>
            /// \endspanish
            protected virtual GameObject GetPooledObject(GameObject prefab)
            {

                if (_objectPoolingItem.ContainsKey(prefab))
                {
                    
                    GameObject go = _objectPoolingItem[prefab].GetPooledObject();

                    if (go != null)
                    {
                        return go;

                    }

                }

                return null;

            }

        #endregion


        #region Utility Events

			/// \english
			/// <summary>
			/// Created pool event.
			/// </summary>
			/// \endenglish
			/// \spanish
			/// <summary>
			/// Evento de pila creada.
			/// </summary>
			/// \endspanish
			protected virtual void OnObjectPoolCreated()
			{
			
				if (ObjectPoolCreated != null)
				{
				
					ObjectPoolCreated();
					
				}

			}

			/// \english
			/// <summary>
			/// Cleared pool event.
			/// </summary>
			/// \endenglish
			/// \spanish
			/// \spanish
			/// <summary>
			/// Evento de objeto de pila vaciada.
			/// </summary>
			/// \endspanish
			protected virtual void OnObjectPoolCleared()
			{
			
				if (ObjectPoolCleared != null)
				{
				
					ObjectPoolCleared();
					
				}
				
			}

			/// \english
			/// <summary>
			/// Instantiated object event.
			/// </summary>
			/// \endenglish
			/// \spanish
			/// \spanish
			/// <summary>
			/// Evento de objeto instanciado.
			/// </summary>
			/// \endspanish
			protected virtual void OnObjectSpawned(GameObject obj)
			{
			
				if (ObjectSpawned != null)
				{
				
					ObjectSpawned(obj);
					
				}
				
			}

        #endregion
		
    }

}