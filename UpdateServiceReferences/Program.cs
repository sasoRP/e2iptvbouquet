using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateServiceReferences
{
    class Program
    {
        static void Main(string[] args)
        {
            E2Bouquet originalBouquet = new E2Bouquet(@"C:\Users\prosa\Desktop\test-iptv-references\original-bouquet.tv");
            E2Bouquet uniqueReferencesBouquet = originalBouquet.GetUniqueReferencesBouquet();

            Program.createBouquetFile(uniqueReferencesBouquet, @"C:\Users\prosa\Desktop\test-iptv-references\modified-bouquet.tv");
            Program.createCustomChannelsFile(uniqueReferencesBouquet, @"C:\Users\prosa\Desktop\test-iptv-references\custom.channels.xml");
        }

        private static void createCustomChannelsFile(E2Bouquet bouquet, string filename)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<channels>");

            foreach (TVService tvService in bouquet.Services.Values)
            {
                sb.AppendLine(String.Format("<channel id=\"REPLACEME\">{0}</channel><!-- {1} -->", tvService.ServiceReference + ":http%3a//example.com", tvService.Description));
            }

            sb.AppendLine("</channels>");

            using (System.IO.StreamWriter fileWriter = new System.IO.StreamWriter(filename, true))
            {
                fileWriter.Write(sb.ToString());
            }
        }

        private static void createBouquetFile(E2Bouquet bouquet, string filename)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("NAME {0}", bouquet.Name));

            foreach (TVService tvService in bouquet.Services.Values)
            {
                sb.AppendLine(String.Format("#SERVICE {0}:{1}", tvService.ServiceReference, tvService.Url));
                sb.AppendLine(String.Format("#DESCRIPTION {0}", tvService.Description));
            }

            using (System.IO.StreamWriter fileWriter = new System.IO.StreamWriter(filename, true))
            {
                fileWriter.Write(sb.ToString());
            }
        }
    }
}
