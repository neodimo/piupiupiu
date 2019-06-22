/// INFORMACION
/// 
/// Project: Chloroplast Games Framework
/// Game: Chloroplast Games Framework
/// Date: 03/09/2016
/// Author: Chloroplast Games
/// Website: http://www.chloroplastgames.com
/// Programmers: David Cuenca
/// Description: Class that allows the object to manage the gameobject of a pool type.
///

// Local Namespace
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CGF.Systems.ObjectTransform
{

    /// \english
    /// <summary>
    /// Class that allows the object to manage the gameobject of a pool type.
    /// </summary>
    /// \endenglish
    /// \spanish
    /// <summary>
    /// Clase que permite al objeto gestionar los gameobject de un tipo de la pila.
    /// </summary>
    /// \endspanish
    [Serializable]
    public class CGFObjectPoolingItemClass
    {
 
		#region Public Variables

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
            public GameObject ObjectToPool;

		    /// \english
		    /// <summary>
		    /// Total amount of instances.
		    /// </summary>
		    /// \endenglish
		    /// \spanish
            /// <summary>
            /// Cantidad total de instancias.
            /// </summary>
		    /// \endspanish
            public int Amount;

		    /// \english
		    /// <summary>
		    /// Allows to exceed the total amount of instances if needed.
		    /// </summary>
		    /// \endenglish
		    /// \spanish
            /// <summary>
            /// Permite superar la cantidad total de instancias si se necesita.
            /// </summary>
		    /// \endspanish
            public bool Dynamic;
		
		    /// \english
		    /// <summary>
		    /// Enable Object list.
		    /// </summary>
		    /// \endenglish
		    /// \spanish
		    /// <summary>
		    /// Lista de objetos activados.
		    /// </summary>
		    /// \endspanish
            protected List<GameObject> EnabledObjects;

        	/// \english
		    /// <summary>
		    /// Disabled Object list.
		    /// </summary>
		    /// \endenglish
		    /// \spanish
		    /// <summary>
		    /// Lista de objetos desactivados.
		    /// </summary>
		    /// \endspanish
            protected List<GameObject> DisabledObjects;

		#endregion
	 
	 
		#region Private Variables
	 
		#endregion
	 
	 
		#region Main Methods

            /// \english
            /// <summary>
            /// Class constructor.
            /// </summary>
            /// <param name="objectToAdd">Pooleable object.</param>
            /// <param name="amount">Total amount of instances.</param>
            /// <param name="dynamic">Allows to exceed the total amount of instances if needed.</param>
            /// \endenglish
            /// \spanish
            /// <summary>
            /// Contructor de la clase.
            /// </summary>
            /// <param name="objectToAdd">Objeto pooleable.</param>
            /// <param name="amount">Cantidad total de instancias.</param>
            /// <param name="dynamic">Permite superar la cantidad total de instancias si se necesita.</param>
            /// \endspanish
            public CGFObjectPoolingItemClass(GameObject objectToAdd, int amount, bool dynamic)
            {

                ObjectToPool = objectToAdd;

                Amount = amount;

                Dynamic = dynamic;

            }

        #endregion


        #region Utility Methods


            /// \english
            /// <summary>
            /// Initialize pool.
            /// </summary>
            /// \endenglish
            /// \spanish
            /// <summary>
            /// Inicializa la pila.
            /// </summary>
            /// \endspanish
            public void Initialize()
            {

                EnabledObjects = new List<GameObject>();

                DisabledObjects = new List<GameObject>();

                try
                {

                    for (int i = 0; i < Amount; i++)
                    {

                        GameObject obj = GameObject.Instantiate(ObjectToPool) as GameObject;

                        DisabledObjects.Add(obj);

                        obj.name = ObjectToPool.name;

                        obj.SetActive(false);

                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }

            }

            /// \english
            /// <summary>
            /// Destroy all pool objects.
            /// </summary>
            /// \endenglish
            /// \spanish
            /// <summary>
            /// Destruye todos los objetos de la pila.
            /// </summary>
            /// \endspanish
            public void DestroyAllObjects()
            {

                for (int i = 0; i < DisabledObjects.Count; i++)
                {

                    GameObject.Destroy(DisabledObjects[i]);
                
                }

                for (int i = 0; i < EnabledObjects.Count; i++)
                {

                    GameObject.Destroy(EnabledObjects[i]);

                }

            }

            /// \english
            /// <summary>
            /// Disable pool object.
            /// </summary>
            /// <param name="gameObject">Objet.</param>
            /// \endenglish
            /// \spanish
            /// <summary>
            /// Desactiva el objeto de la pila.
            /// </summary>
            /// <param name="gameObject">Objeto.</param>
            /// \endspanish
            public virtual void DeSpawn(GameObject gameObject)
            {

                gameObject.SetActive(false);

                EnabledObjects.Remove(gameObject);

                DisabledObjects.Add(gameObject);

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
            public virtual GameObject GetPooledObject()
            {

                if (DisabledObjects.Count > 0)
                {

                    GameObject obj = DisabledObjects[0];

                    EnabledObjects.Add(obj);

                    DisabledObjects.Remove(obj);

                    return obj;

                }

                if (Dynamic)
                {

                    GameObject obj = GameObject.Instantiate(ObjectToPool) as GameObject;

                    obj.name = ObjectToPool.name;

                    obj.SetActive(false);

                    EnabledObjects.Add(obj);

                    return obj;

                }

                return null;

            }

        #endregion
		
		
		#region Utility Events
	 
		#endregion
 
    }
 
}
