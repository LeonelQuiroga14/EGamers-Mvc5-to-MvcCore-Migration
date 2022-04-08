using System.Collections.Generic;
using System.Web;

namespace Gamers.Web.Models
{
    public class ImagenViewModel
    {

#if NETCOREAPP
        public ImagenViewModel()
        {
            Image = new List<IFormFile>();
        }

        public List<IFormFile> Image { get; set; }
#else
              public ImagenViewModel()
        {
            Image = new List<HttpPostedFileBase>();
        }
     
      public List<HttpPostedFileBase> Image { get; set; }

#endif

        public int JuegoId { get; set; }
    }
}