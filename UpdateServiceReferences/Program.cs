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
            string orginalBouquetFile = @"C:\Users\prosa\Desktop\test-iptv-references\original-bouquet.tv";
            string originalCustomChannelsFile = @"C:\Users\prosa\Desktop\test-iptv-references\old-custom.channels.xml";

            E2Bouquet e2Bouquet = new E2Bouquet(orginalBouquetFile, originalCustomChannelsFile);

            e2Bouquet.CreateUniqueReferences();
            e2Bouquet.CreateBouquetFile(@"C:\Users\prosa\Desktop\test-iptv-references\modified-bouquet.tv");
            e2Bouquet.CreateEpgXmlFile(@"C:\Users\prosa\Desktop\test-iptv-references\custom.channels.xml");
        }
    }
}
