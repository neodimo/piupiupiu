/// INFORMATION
/// 
/// Project: Chloroplast Games Framework
/// Game: Chloroplast Games Framework
/// Date: 03/09/2016
/// Author: Chloroplast Games
/// Website: http://www.chloroplastgames.com
/// Programmers: David Cuenca
/// Description: Editor of CGFObjectPoolingManager.
///
 
using UnityEditor;
using UnityEditorInternal;
using Assets.CGF.Systems.ObjectTransform;

// Local Namespace
namespace Assets.CGF.Editor.ObjectTransform
{

    [CustomEditor(typeof(CGFObjectPoolingManager))]
    [CanEditMultipleObjects]

	/// \english
	/// <summary>
	/// Editor of CGFObjectPoolingManager.
	/// </summary>
	/// \endenglish
	/// \spanish
	/// <summary>
	/// Editor de CGFObjectPoolingManager.
	/// </summary>
	/// \endspanish
    public class CGFObjectPoolingManagerEditor : UnityEditor.Editor
    {
 
		#region Public Variables
	 
		#endregion
	 
	 
		#region Private Variables

            private SerializedProperty _instantiateOnAwake;

            private SerializedProperty _objectsToPool;

            private ReorderableList _objectsToPoolReorderableList;

            private int _objectsToPoolListSize;

		#endregion
	 
	 
		#region Main Methods

            private void OnEnable()
            {
			
                _instantiateOnAwake = serializedObject.FindProperty("_instantiateOnAwake");

                _objectsToPool = serializedObject.FindProperty("_objectsToPool");

                _objectsToPoolReorderableList = new ReorderableList(serializedObject, _objectsToPool, true, true, true, true);
				
            }

            public override void OnInspectorGUI()
            {

                serializedObject.Update();

                CGFEditorUtilitiesClass.BuildComponentTools("http://chloroplastgames.com/cg-framework-user-manual/#Object_Pooling_Manager", serializedObject);

                CGFEditorUtilitiesClass.ManageComponentValues<CGFObjectPoolingManager>();

                CGFEditorUtilitiesClass.BackUpComponentValues<CGFObjectPoolingManager>(serializedObject);

                CGFEditorUtilitiesClass.BuildBoolean("Instantiate On Awake", "Instance all objects on Awake.", _instantiateOnAwake);

                _objectsToPoolListSize = CGFEditorUtilitiesClass.BuildListButtons(_objectsToPool, _objectsToPoolListSize);

                _objectsToPoolReorderableList = CGFEditorUtilitiesClass.BuildListCustom(_objectsToPool, _objectsToPoolReorderableList, "Object To Pool",true,new int[]{4,4,4},"ObjectToPool","Amount","Dynamic");

                serializedObject.ApplyModifiedProperties();

            }

		#endregion
	 
	 
		#region Utility Methods
	 
		#endregion
		
		
		#region Utility Events
	 
		#endregion
 
    }
 
}