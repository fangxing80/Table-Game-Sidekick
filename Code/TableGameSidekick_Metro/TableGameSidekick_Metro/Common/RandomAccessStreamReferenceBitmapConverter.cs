

using Windows.UI.Xaml.Data;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
public class RandomAccessStreamReferenceBitmapConverter : IValueConverter
{
    public object Convert(object value, System.Type targetType, object parameter, string language)
    {
        if (value is IRandomAccessStreamReference)
        {
            var s = value as IRandomAccessStreamReference;
            BitmapImage bi = new BitmapImage();
            bi.SetSource(s.OpenReadAsync().GetResults());
            return bi;
        }

        return null;
    }



    public object ConvertBack(object value, System.Type targetType, object parameter, string language)
    {
        return null;
    }
}
