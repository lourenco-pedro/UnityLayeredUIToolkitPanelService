namespace Services.UIToolkitLayeredService
{
    public interface IUIToolkitLayeredService : IService
    {
        int UseLayer(BaseLayeredPanelController visualTreeAsset);
        int UseHigherLayer(BaseLayeredPanelController visualTreeAsset);
        BaseLayeredPanelController GetPanel(int panelId);
        void ClosePanel(int panelId);
    }
}
