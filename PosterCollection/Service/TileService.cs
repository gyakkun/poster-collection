using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using System.Xml.Linq;
using PosterCollection.Models;
using PosterCollection.ViewModels;

namespace PosterCollection.Service
{
    class TileService
    {

        public static void SendTileNotification(Star item)
         {
                XDocument xdoc = XDocument.Load("tiles.xml");
                // Create the tile notification
                string temp = xdoc.ToString();
                string destXml = temp.Replace("title", item.title);
                destXml = destXml.Replace("comment", item.comment);
                destXml = destXml.Replace("postersrc", item.posterpath);
                destXml = destXml.Replace("backgroundsrc", item.imagepath);
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(destXml);

                //3.然后用Update方法来更新这个磁贴
                TileNotification notification = new TileNotification(xml);
            
                // Send the notification
                App.GetTileUpdater().Update(notification);

         }
        public static void GenerateTiles()
        {
            foreach (var item in ViewModel.Instance.Starlist)
            {
                TileService.SendTileNotification(item);
            }

        }
    }

}
