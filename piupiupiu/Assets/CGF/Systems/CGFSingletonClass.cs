///
/// INFORMATION
/// 
/// Project: Chloroplast Games Framework
/// Game: Chloroplast Games Framework
/// Date: 27/07/2016
/// Author: Chloroplast Games
/// Website: http://www.chloroplastgames.com
/// Programmers: David Cuenca
/// Description: Clase que permite hacer singletons.
///

using System;
using UnityEngine;

// Local Namespace
namespace Assets.CGF.Systems
{
    /// <summary>
    /// Comportamiento que permite al gameobject asociado al Sistema de Singleton.
    /// </summary>
    /// <typeparam name="T">Tipo de la clase.</typeparam>
    public abstract class CGFSingletonClass<T> : MonoBehaviour where T : MonoBehaviour
    {
 
        #region Public Variables

            static internal Type _myType = typeof(T);

            /// <summary>
            /// Singleton.
            /// </summary>
            public static T Instance
            {
                get
                {
                    if (!_instance)
                    {
                        _instance = FindObjectOfType<T>();

                        if (!_instance)
                        {

                            GameObject newSingleton = new GameObject(_myType.Name);

                            _instance = newSingleton.AddComponent<T>();

                        }

                    }

                    return _instance;  
    
                }

            }

        #endregion
 
 
        #region Private Variables

            /// <summary>
            /// Singleton privado.
            /// </summary>
            private static T _instance;  

        #endregion
 
 
        #region Main Methods


        #endregion
 
 
        #region Utility Methods
 
        #endregion
    
    
        #region Utility Events
 
        #endregion

    }

}