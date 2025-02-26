using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Services.UIToolkitLayeredService
{
    public class LayeredUIToolkit : MonoBehaviour
    {
        public int ChildIndex => transform.GetSiblingIndex();
        public bool IsActive => _stackedPanels != null && _stackedPanels.Count > 0;
        
        [SerializeField] private UIDocument _attatchedUIDocument;
        [SerializeField] private PanelSettings _settings;
        
        VisualElement _root;
        private Dictionary<int, BaseLayeredPanelController> _stackedPanels = new Dictionary<int, BaseLayeredPanelController>();
        
        private void OnValidate()
        {
            if(null == _attatchedUIDocument)
                _attatchedUIDocument = GetComponent<UIDocument>();
        }

        private VisualElement CreateRoot(string name)
        {
            VisualElement root = new VisualElement();
            root.name = name;
            
            _attatchedUIDocument.rootVisualElement.Add(root);
            root.StretchToParentSize();
            
            return root;
        }

        public int AddPanel(BaseLayeredPanelController panelController)
        {
            int id = panelController.GetHashCode();
            _root = _attatchedUIDocument.rootVisualElement.Q<VisualElement>("root");
            _root.Add(panelController.Open());
            _stackedPanels.Add(id, panelController);
            
            return id;
        }

        public bool GetPanel(int panelId, out BaseLayeredPanelController panelController)
        {
            return _stackedPanels.TryGetValue(panelId, out panelController);
        }

        public void RemovePanel(int panelId)
        {
            if (GetPanel(panelId, out BaseLayeredPanelController panelControllerToClose))
            {
                panelControllerToClose.FadeOut(() =>
                {
                    _root.Remove(panelControllerToClose.Root);
                    _stackedPanels.Remove(panelId);
                    
                    if(!IsActive)
                        gameObject.SetActive(false);
                });
            }
        }

        public void SetSortingOrder(int sortingOrder)
        {
            _attatchedUIDocument.sortingOrder = sortingOrder;
        }

        public static LayeredUIToolkit CreateLayer(PanelSettings panelSettings)
        {
            GameObject layeredUIToolkitGO = new GameObject("[Layer]");
            LayeredUIToolkit layer = layeredUIToolkitGO.AddComponent<LayeredUIToolkit>();
            layer._attatchedUIDocument = layeredUIToolkitGO.AddComponent<UIDocument>();
            layer._attatchedUIDocument.panelSettings = panelSettings;
            layer._settings = panelSettings;
            layer._settings.sortingOrder = layer.transform.GetSiblingIndex();
            layer._attatchedUIDocument.visualTreeAsset = Resources.Load<VisualTreeAsset>("Panels/Layer");
            
            return layer;
        }
    }
}