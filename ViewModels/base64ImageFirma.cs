using CommunityToolkit.Maui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM2EX2G5.ViewModels
{
    public static class base64ImageFirma
    {
        public static async Task<string> ConvertToBase64Async(DrawingView drawingView)
        {
            var imageStream = await drawingView.GetImageStream(200, 200);

            if (imageStream != null)
            {
                byte[] imageData;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await imageStream.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }

                return Convert.ToBase64String(imageData);
            }

            return null; // or throw an exception, depending on your requirements
        }
    }
}
