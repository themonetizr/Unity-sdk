using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;

namespace MagneticScrollView
{
    /// <summary>
    /// Manages how the Index Table should behave.
    /// </summary>
    [ExecuteInEditMode]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class IndexTableManager : MonoBehaviour
    {
        #region FIELDS
        [Tooltip ("Indexes alignment ordering (Horizontal or Vertical).")]
        [SerializeField] private Alignment m_alignment = Alignment.Horizontal;
        [SerializeField] private MagneticScrollRect scrollRect;
        [SerializeField] private GameObject indicator;
        [SerializeField] private List<GameObject> indexes;
        [SerializeField] private Vector2 indicatorSize = new Vector2 (0.5f, 0.5f);
        [SerializeField] private Vector2 indexSize;
        [Tooltip ("Reference to Index Prefab, the indexes will be automatically modified")]
        [SerializeField] private GameObject indexPrefab;
        [Tooltip ("Reference to Indicator Prefab, the indicator will be automatically modified")]
        [SerializeField] private GameObject indicatorPrefab;
        [SerializeField] private Sprite knobSprite;
        [SerializeField] private Sprite bgSprite;

        private int elementCount = 0;
        #endregion

        #region Constants
#if UNITY_EDITOR

        private const string bgSpritePath = "UI/Skin/Background.psd";
        private const string knobPath = "UI/Skin/Knob.psd";
#endif
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Indexes alignment ordering (Horizontal or Vertical).
        /// </summary>
        public Alignment AlignmentEnum
        {
            get { return m_alignment; }
            set
            {
                m_alignment = value;
                ChangeLayoutGroup ();
            }
        }
        /// <summary>
        /// Indexes alignment ordering (Horizontal or Vertical).
        /// </summary>
        public int AlignmentInt
        {
            get { return (int)m_alignment; }
            set { AlignmentEnum = (Alignment)value; }
        }
        #endregion

        #region Methods

        private void LateUpdate ()
        {
            elementCount = scrollRect.Elements.Length;
            if (transform.childCount != elementCount || transform.childCount != indexes.Count || indexes.Count != elementCount)
            {
                SetupIndexes ();
            }

            CheckIndicator ();
        }

        private void OnEnable ()
        {
            //Debug.Log ("Enabled");
            scrollRect = GetComponentInParent<MagneticScrollRect> ();
#if UNITY_EDITOR

            EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCallBack;

            knobSprite = AssetDatabase.GetBuiltinExtraResource<Sprite> (knobPath);
            bgSprite = AssetDatabase.GetBuiltinExtraResource<Sprite> (bgSpritePath);

            //if (!EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying)
            //{ 
            //    if (indexPrefab == null)
            //        indexPrefab = CreateNewPrefab ("IndexPrefab", bgSpritePath, Color.white);
            //    if (indicatorPrefab == null)
            //        indicatorPrefab = CreateNewPrefab ("IndicatorPrefab", knobPath, Color.black);
            //}
#endif

            indexes = new List<GameObject> ();
            foreach (Transform index in transform)
                indexes.Add (index.gameObject);

            elementCount = scrollRect.Elements.Length;

            SetupIndexes ();
            ResizeIndicator ();
            ChangeLayoutGroup ();
        }

#if UNITY_EDITOR
        private void OnDisable ()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyItemCallBack;
        }

        private void HierarchyItemCallBack (int instance, Rect rect)
        {
            Event evt = Event.current;
            if (evt.type == EventType.ValidateCommand && evt.commandName == "UndoRedoPerformed")
            {
                AssignIndexes ();
                SetupIndexes ();
            }
        }

        //private GameObject CreateNewPrefab (string name, string spritePath, Color color)
        //{
        //    string oldPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour (this));            
        //    string [] strings = oldPath.Split ('/');
        //    string newPath = "";
        //    for (int i = 0; i < strings.Length - 2; i++)
        //        newPath += strings [i] + "/";

        //    if (!AssetDatabase.IsValidFolder (newPath + "Prefabs/"))
        //        AssetDatabase.CreateFolder (newPath, "Prefabs/");
        //    newPath += "Prefabs/Default_" + name + ".prefab";

        //    //Debug.Log (newPath);
        //    Object newPrefab = PrefabUtility.CreateEmptyPrefab (newPath);
        //    GameObject go = new GameObject (name, typeof (RectTransform), typeof (CanvasRenderer));            
        //    Image img = go.AddComponent<Image> ();
        //    img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite> (spritePath);
        //    img.color = color;
        //    go.layer = 5;
        //    GameObject prefabGO = PrefabUtility.ReplacePrefab (go, newPrefab, ReplacePrefabOptions.ConnectToPrefab);

        //    //Debug.Log (go, go);
        //    if (Application.isPlaying)
        //        Destroy (go);
        //    else
        //        DestroyImmediate (go);

        //    return prefabGO;
        //}

        private void ReplaceIndexes ()
        {
            Undo.RecordObject (this, "Removing objects from list");

            var list = new List<GameObject> ();
            list.AddRange (indexes);
            indexes.Clear ();
            for (int i = 0; i < list.Count; i++)
            {
                Undo.DestroyObjectImmediate (list [i]);
            }

            for (int i = 0; i < indexes.Count; i++)
            {
                indexes [i] = NewUIObject ("Index", indexPrefab, transform, bgSprite, Color.white);
            }

            SetupIndexes ();
            SetupIndicator ();
        }

        private void ReplaceIndicator ()
        {
            Undo.RecordObject (this, "Removing objects from list");

            if (indicator != null)
                Undo.DestroyObjectImmediate (indicator);
            if (indexes.Count > 0 && indexes [0] != null)
            {
                indicator = NewUIObject ("Indicator", indicatorPrefab, indexes [0].transform, knobSprite, Color.black);
                ResizeIndicator ();
            }
        }
#endif

        /// <summary>
        /// Move Indicator to the target index position.
        /// </summary>
        /// <param name="index">The target index</param>
        public void MoveIndicator (int index, bool invertOrder)
        {
            if (indicator != null && indexes.Count > index && indexes [index] != null)
            {
                if (invertOrder)
                    index = (indexes.Count - 1) - index;
                //Debug.Log ("Moved");

#if UNITY_EDITOR
                Undo.SetTransformParent (indicator.transform, indexes [index].transform, "Setting indicator parent");
#else                
                indicator.transform.SetParent (indexes [index].transform, false);
#endif
                indicator.transform.localPosition = Vector2.zero;
            }
        }

        private void SetupIndicator ()
        {
            //Debug.Log (indicator);
            if (indicator == null && indexes.Count > 0)
            {
                if (indicatorPrefab != null)
                    indicator = SafeOperations.Instantiate (indicatorPrefab, indexes [0].transform);
                else
                {
                    indicator = SafeOperations.NewGameObject ("Indicator", indexes [0].transform, typeof (RectTransform), typeof (CanvasRenderer));
                    Image img = indicator.SafeAddComponent<Image> ();
                    img.sprite = knobSprite;
                    img.color = Color.black;
                }
            }

            if (indicator != null)
            {
                indicator.name = "Indicator";
                indicator.layer = 5;
                indicator.transform.localPosition = Vector3.zero;
            }

            ResizeIndicator ();

        }

        private void ResizeIndicator ()
        {
            if (indicator != null)
            {
                RectTransform indicatorRT = indicator.GetComponent<RectTransform> ();

                if (indicatorRT != null)
                {
                    indicatorRT.anchorMin = Vector2.zero;
                    indicatorRT.anchorMax = Vector2.one;
                    indicatorRT.sizeDelta = Vector2.zero;

                    //indicatorRT.sizeDelta = new Vector2 (indexSize.x * Mathf.Lerp (-1f, 1f, indicatorSize.x / 2f), indexSize.y * Mathf.Lerp (-1f, 1f, indicatorSize.y / 2f));
                    //Debug.Log (indicatorRT.sizeDelta);
                }
            }
        }

        private void CheckIndicator ()
        {
            if (indicator == null)
                SetupIndicator ();

            //Removes non Indicator objects.
            for (int i = 0; i < indexes.Count; i++)
            {
                //Debug.Log (indexes [i]);
                foreach (Transform child in indexes [i].transform)
                {
                    if (child.gameObject != indicator)
                        if (Application.isPlaying)
                            Destroy (child.gameObject);
                        else
                            DestroyImmediate (child.gameObject);
                }
            }
        }

        private void AssignIndexes ()
        {
            if (indexes.Count == transform.childCount)
                for (int i = 0; i < indexes.Count; i++)
                    indexes [i] = transform.GetChild (i).gameObject;
        }

        private void SetupIndexes ()
        {

            if (indexes.Count > transform.childCount)
                indexes = indexes.Where (item => item != null).ToList ();
            else if (indexes.Count < transform.childCount)
            {
                indexes = new List<GameObject> ();
                foreach (Transform child in transform)
                {
                    //if (PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject) == null)                        
                    indexes.Add (child.gameObject);
                }
            }

            //Debug.Log(transform.childCount);
            //Debug.Log(elementCount);

            if (transform.childCount < elementCount)
            {
                if (indexes.Count < elementCount)
                {
                    int startIndexCount = indexes.Count;
                    for (int i = startIndexCount; i < elementCount; i++)
                    {
                        GameObject newObject = NewUIObject ("Index", indexPrefab, transform, bgSprite, Color.white);
                        indexes.Add (newObject);
                    }
                }
                else
                {
                    for (int i = 0; i < indexes.Count; i++)
                        if (indexes [i] == null)
                            indexes [i] = NewUIObject ("Index", indexPrefab, transform, bgSprite, Color.white);
                }
            }
            else if (transform.childCount > elementCount)
            {
                for (int i = transform.childCount - 1; i >= elementCount; i--)
                {
                    GameObject go = transform.GetChild (i).gameObject;
                    SafeOperations.Destroy (go);
                    if (i < indexes.Count)
                        indexes.RemoveAt (i);
                }
            }

            for (int i = 0; i < indexes.Count; i++)
                indexes [i].name = "Index " + (i + 1).ToString ();


            //// -- Cleaning up the list --

            //if (indexes.Count > transform.childCount)
            //    indexes = indexes.Where (item => item != null).ToList ();
            //else
            //{
            //    for (int i = transform.childCount - 1; i >= 0; --i)
            //    {
            //        var go = transform.GetChild (i).gameObject;
            //        if (!indexes.Contains (go))
            //            SafeOperations.Destroy (go);
            //    }
            //}

            //if (indexes.Count > elementCount)
            //{
            //    for (int i = transform.childCount - 1; i >= elementCount; i--)
            //    {
            //        GameObject go = transform.GetChild (i).gameObject;
            //        SafeOperations.Destroy (go);
            //        if (i < indexes.Count)
            //            indexes.RemoveAt (i);
            //    }
            //}
            //else
            //{
            //    for (int i = indexes.Count; i < elementCount; i++)
            //    {
            //        GameObject newObject = NewUIObject ("Index", indexPrefab, transform, bgSprite, Color.white);
            //        indexes.Add (newObject);
            //    }
            //}

            //for (int i = 0; i < indexes.Count; i++)
            //    if(indexes[i] != null) indexes [i].name = "Index " + (i + 1).ToString ();

            ResizeIndexes ();
            SetupIndicator ();
        }

        private GameObject NewUIObject (string name, GameObject prefab, Transform parent, Sprite sprite, Color color)
        {
            GameObject GO;
            if (prefab != null)
            {
                GO = SafeOperations.Instantiate (prefab, parent);
            }
            else
            {
                GO = SafeOperations.NewGameObject (name, parent, typeof (RectTransform), typeof (CanvasRenderer));
                Image img = GO.SafeAddComponent<Image> ();
                img.sprite = sprite;
                img.color = color;
                GO.layer = 5;
            }

            return GO;
        }

        private void ResizeIndexes ()
        {
            for (int i = 0; i < indexes.Count; i++)
            {
                if (indexes [i] == null)
                    return;

                RectTransform indexRT = indexes [i].GetComponent<RectTransform> ();
                RectTransform rt_Table = GetComponent<RectTransform> ();
                if (indexRT != null && rt_Table != null)
                {
                    float indexSizeFactor = 0.95f;
                    Vector2 tableSize = new Vector2 (rt_Table.rect.width, rt_Table.rect.height);
                    float sizeChanged =
                        Mathf.Clamp (Mathf.Max (tableSize.x, tableSize.y) / elementCount, 0f, Mathf.Min (tableSize.x, tableSize.y)) * indexSizeFactor;
                    indexSize = new Vector2 (sizeChanged, sizeChanged);

                    indexRT.sizeDelta = indexSize;
                }
            }
        }

        private void ChangeLayoutGroup ()
        {
            RectTransform tableRT = GetComponent<RectTransform> ();
            Vector2 oldRectSize = new Vector2 (tableRT.rect.width, tableRT.rect.height);

            float minSize = Mathf.Min (oldRectSize.x, oldRectSize.y);
            float maxSize = Mathf.Max (oldRectSize.x, oldRectSize.y);

            if (m_alignment == Alignment.Vertical)
            {
                if (GetComponent<VerticalLayoutGroup> () == null)
                {
                    HorizontalLayoutGroup hGroup = GetComponent<HorizontalLayoutGroup> ();
                    if (hGroup)
                        SafeOperations.Destroy (hGroup);

                    VerticalLayoutGroup layoutGroup = gameObject.SafeAddComponent<VerticalLayoutGroup> ();
                    tableRT.sizeDelta = new Vector2 (minSize, maxSize);
                    layoutGroup.childAlignment = TextAnchor.MiddleCenter;
                    layoutGroup.childControlWidth = false;
                    layoutGroup.childControlHeight = false;
                }
            }
            else
            {
                if (GetComponent<HorizontalLayoutGroup> () == null)
                {
                    VerticalLayoutGroup vGroup = GetComponent<VerticalLayoutGroup> ();
                    if (vGroup)
                        SafeOperations.Destroy (vGroup);

                    HorizontalLayoutGroup layoutGroup = gameObject.SafeAddComponent<HorizontalLayoutGroup> ();
                    tableRT.sizeDelta = new Vector2 (maxSize, minSize);
                    layoutGroup.childAlignment = TextAnchor.MiddleCenter;
                    layoutGroup.childControlWidth = false;
                    layoutGroup.childControlHeight = false;
                }
            }
        }

        private void OnRectTransformDimensionsChange ()
        {
            ResizeIndexes ();
            ResizeIndicator ();
        }


        #endregion
    }
}