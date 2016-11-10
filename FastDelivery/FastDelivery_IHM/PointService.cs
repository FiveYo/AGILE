using FastDelivery_Library.Modele;
using Windows.UI.Xaml;

namespace FastDelivery_IHM
{
    /// <summary>
    /// Permet d'associer aux éléments graphiques des infos de type Point
    /// </summary>
    public static class PointService
    {
        public static DependencyProperty infoPoint = DependencyProperty.Register("infoPoint", typeof(Point), typeof(FrameworkElement), new PropertyMetadata(null));
    }
}
