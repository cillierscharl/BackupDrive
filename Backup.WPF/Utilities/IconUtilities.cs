using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;


namespace Backup.WPF.Utilities
{
    public static class IconUtilities
    {
        static Dictionary<string, BitmapSource> IconDictionary = new Dictionary<string, BitmapSource>();
        public static BitmapSource ResolveIcon(string path, string ext)
        {
            if (IconDictionary.ContainsKey(ext))
            {
                return IconDictionary[ext];
            }

            using (System.Drawing.Icon sysicon = System.Drawing.Icon.ExtractAssociatedIcon(path))
            {
                var icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                  sysicon.Handle,
                  System.Windows.Int32Rect.Empty,
                  System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                
                IconDictionary.Add(ext, icon);

                return icon;
            }
        }
    }
}
