using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InternetKillSwitchApplication.Extensions;

namespace InternetKillSwitchApplication
{
    /// <summary>
    /// Interaction logic for NetworkDragDropAdornerBase.xaml
    /// </summary>
    public partial class NetworkDragDropAdornerBase : DragDropAdornerBase
    {
        public NetworkDragDropAdornerBase()
        {
            InitializeComponent();
        }

        public override void StateChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var myclass = (NetworkDragDropAdornerBase)d;

            //switch ((DropState)e.NewValue)
            //{
            //    case DropState.CanDrop:
            //        myclass.back.Stroke = Application.Current.Resources["canDropBrush"] as SolidColorBrush;
            //        myclass.indicator.Source = Application.Current.Resources["dropIcon"] as DrawingImage;
            //        break;
            //    case DropState.CannotDrop:
            //        myclass.back.Stroke = Application.Current.Resources["solidRed"] as SolidColorBrush;
            //        myclass.indicator.Source = Application.Current.Resources["noDropIcon"] as DrawingImage;
            //        break;
            //}
        }
    }
}
