using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Services.UIToolkitLayeredService.Implementation
{
    public class UIToolKitLayeredService : IUIToolkitLayeredService
    {
        private List<LayeredUIToolkit> _layeredDocuments = new List<LayeredUIToolkit>();
        private PanelSettings _panelSettings;
        
        public string Name => "UIToolKitLayeredService";

        public string ResourcesPath { get; private set; }
        
        private LayeredUIToolkit PoolLayer()
        {
            LayeredUIToolkit layer = _layeredDocuments.FirstOrDefault(layer => !layer.IsActive);
            if (layer == default)
            {
                layer = LayeredUIToolkit.CreateLayer(_panelSettings);
                _layeredDocuments.Add(layer);
            }
            
            layer.gameObject.SetActive(true);
            layer.transform.SetAsLastSibling();
            layer.SetSortingOrder(layer.transform.GetSiblingIndex());
            
            return layer;
        }

        private LayeredUIToolkit GetBelongingLayer(int panelId)
        {
            return _layeredDocuments.FirstOrDefault(layer => layer.GetPanel(panelId, out BaseLayeredPanelController panel));
        }

        private LayeredUIToolkit GetHigherActiveLayer()
        {
            var activeLayers = _layeredDocuments.Where(layer => layer.gameObject.activeSelf).ToList();
            if (activeLayers.Count == 0)
                return PoolLayer();
            
            activeLayers.Sort((layer1, layer2) => layer2.ChildIndex.CompareTo(layer1.ChildIndex));
            return activeLayers[0];
        }

        public Task AsyncSetup()
        {
            _layeredDocuments = new List<LayeredUIToolkit>();
            
            _panelSettings = Resources.Load<PanelSettings>(ResourcesPath);
            
            return Task.CompletedTask;
        }

        public int UseLayer(BaseLayeredPanelController visualTreeAsset)
        {
            LayeredUIToolkit layer = GetHigherActiveLayer();
            return layer.AddPanel(visualTreeAsset);
        }

        public int UseHigherLayer(BaseLayeredPanelController visualTreeAsset)
        {
            LayeredUIToolkit layer = PoolLayer();
            
#if UNITY_EDITOR
            layer.name = $"[Layer] {visualTreeAsset.GetType().Name}"; 
#endif
            return layer.AddPanel(visualTreeAsset);
        }

        public BaseLayeredPanelController GetPanel(int panelId)
        {
            var layer = _layeredDocuments.FirstOrDefault(layer => layer.GetPanel(panelId, out BaseLayeredPanelController panel));
            if (default == layer)
                return null;

            bool hasPanel = layer.GetPanel(panelId, out BaseLayeredPanelController panel);
            return hasPanel ? panel : null;
        }
        
        public void ClosePanel(int panelId)
        {
            LayeredUIToolkit layer = GetBelongingLayer(panelId);
            layer.RemovePanel(panelId);
        }
        
#if UNITY_EDITOR
        public void DebugService()
        {
        }
#endif
    }
}