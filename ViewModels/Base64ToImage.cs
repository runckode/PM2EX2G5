﻿using System.Globalization;

namespace PM2EX2G5.ViewModels
{
    public class Base64ToImage : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            ImageSource? imageSource = null;
            if (value != null)
            {
                String Base64Image = (String)value;
                byte[] imageByte = System.Convert.FromBase64String(Base64Image);
                var stream = new MemoryStream(imageByte);
                imageSource = ImageSource.FromStream(() => stream);
            }

            return imageSource;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
