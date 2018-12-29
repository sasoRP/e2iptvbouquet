using System;
using System.Collections.Generic;
using System.IO;

namespace UpdateServiceReferences
{
    public class E2Bouquet
    {
        public IDictionary<string, TVService> Services { get; }
        public string Name { get; set; }

        public E2Bouquet()
        {
            this.Services = new Dictionary<string, TVService>();
        }

        public E2Bouquet(string filename) :this()
        {
            this.LoadBoquetFile(filename);
        }

        public void LoadBoquetFile(string filename)
        {
            using (StreamReader reader = new StreamReader(File.OpenRead(filename)))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    string directiveName = this.parseDirective(line);

                    switch (directiveName)
                    {
                        case "NAME":
                            this.Name = this.parseBouquetName(line);
                            break;
                        case "SERVICE":
                            string descriptionLine = reader.ReadLine();
                            TVService tvService = this.parseService(line, descriptionLine);

                            this.Services.Add(tvService.Url, tvService);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public E2Bouquet GetUniqueReferencesBouquet()
        {
            E2Bouquet uniqueReferencesBouquet = new E2Bouquet();
            uniqueReferencesBouquet.Name = this.Name;

            int counter = 1;

            foreach(TVService originalTvService in this.Services.Values)
            {
                TVService uniqueReferenceTvService = new TVService();
                uniqueReferenceTvService.Url = originalTvService.Url;
                uniqueReferenceTvService.Description = originalTvService.Description;
                uniqueReferenceTvService.ServiceReference = String.Format("1:0:1:{0}:0:0:0:0:0:0", counter.ToString("X"));

                uniqueReferencesBouquet.Services.Add(originalTvService.Url, uniqueReferenceTvService);
                counter++;
            }

            return uniqueReferencesBouquet;
        }

        private TVService parseService(string line, string descriptionLine)
        {
            TVService tvService = new TVService();

            tvService.ServiceReference = line.Substring(8, line.IndexOf(":http") - 8);
            tvService.Url = line.Substring(line.IndexOf("http"));
            tvService.Description = descriptionLine.Substring(13);

            return tvService;
        }

        private string parseBouquetName(string line)
        {
            return line.Substring(line.IndexOf("#NAME ") + 6);
        }

        private string parseDirective(string line)
        {
            string directiveName = String.Empty;

            if (line.StartsWith("#"))
            {
                directiveName = line.Substring(line.IndexOf('#') + 1, line.IndexOf(' ') - 1);
            }

            return directiveName;
        }
    }
}