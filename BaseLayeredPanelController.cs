using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

namespace Services.UIToolkitLayeredService
{
    public abstract class BaseLayeredPanelController
    {
        private VisualTreeAsset _asset;
        protected VisualElement _root;
        
        public VisualTreeAsset Asset => _asset;
        public VisualElement Root => _root;
        
        protected abstract string GetPath();
        protected abstract void Setup();
        
        public VisualElement Open()
        {
            _root = Resources.Load<VisualTreeAsset>(GetPath()).Instantiate();
            _root.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            _root.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            
            Vector2 fromScale = Vector2.one * 1.2f;

            LerpUtils.LerpVector2(fromScale, Vector2.one, .2f, scaleStep =>
            {
                _root.style.scale = new StyleScale(scaleStep);
                return scaleStep;
            }).Forget();

            LerpUtils.LerpFloat(0, 1, .2f, fadeStep =>
            {
                _root.style.opacity = fadeStep;
                return fadeStep;
            }).Forget();
            
            Setup();

            return _root;
        }

        public void FadeOut(Action onComplete)
        {
            Vector2 toScale = Vector2.one * 1.2f;
            
            LerpUtils.LerpVector2(Vector2.one, toScale, .2f, scaleStep =>
            {
                if (scaleStep.x >= toScale.x)
                {
                    onComplete?.Invoke();
                    return scaleStep;
                }
                
                _root.style.scale = new StyleScale(scaleStep);
                return scaleStep;
            }).Forget();
            
            LerpUtils.LerpFloat(1, 0, .2f, fadeStep =>
            {
                _root.style.opacity = fadeStep;
                return fadeStep;
            }).Forget();
        }
    }
}