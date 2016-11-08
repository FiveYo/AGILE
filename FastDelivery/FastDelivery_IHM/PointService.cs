using FastDelivery_Library.Modele;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace FastDelivery_IHM
{
    public static class PointService
    {
        public static DependencyProperty infoPoint = DependencyProperty.Register("infoPoint", typeof(Point), typeof(FrameworkElement), new PropertyMetadata(null));
    }
}
