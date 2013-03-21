using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;
using System.Xml.Linq;

namespace S3Archive
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!File.Exists("s3archive.xml")) {

                new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XComment("S3Archive Local Config File"),
                    new XElement("root",
                        new XElement("AWSKey", "someValue"),
                        new XElement("AWSSecret", "someValue"),
                        new XElement("AWSRegion", "eu-west-1"),
                        new XElement("folder",
                            new XAttribute("includeOpen", false),
                            new XAttribute("deleteOnUpload", true),
                            new XAttribute("recursive", true),
                            new XElement("path", "c:\\data\\something"),
                            new XElement("bucket", "my-archive"),
                            new XElement("basePath", "thisfolder/"),
                            new XElement("pattern", "*.*")
                        )
                    )
                )
                .Save("s3archive.xml");
                Console.WriteLine("s3archive.xml not found, blank one created.");
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
                Environment.Exit(1);
            }

            //We know the config file exists, so open and read values
            XDocument config = XDocument.Load("s3archive.xml");
            
            //Get AWS config
            string AWSKey = config.Element("root").Element("AWSKey").Value;
            string AWSSecret = config.Element("root").Element("AWSSecret").Value;
            RegionEndpoint region = RegionEndpoint.EUWest1;
            switch (config.Element("root").Element("AWSRegion").Value)
            {
                case "eu-west-1":
                    region = RegionEndpoint.EUWest1;
                    break;
                case "sa-east-1":
                    region = RegionEndpoint.SAEast1;
                    break;
                case "us-east-1":
                    region = RegionEndpoint.USEast1;
                    break;
                case "ap-northeast-1":
                    region = RegionEndpoint.APNortheast1;
                    break;
                case "us-west-2":
                    region = RegionEndpoint.USWest2;
                    break;
                case "us-west-1":
                    region = RegionEndpoint.USWest1;
                    break;
                case "ap-southeast-1":
                    region = RegionEndpoint.APSoutheast1;
                    break;
                case "ap-southeast-2":
                    region = RegionEndpoint.APSoutheast2;
                    break;
                default:
                    region = RegionEndpoint.EUWest1;
                    break;
            }

            //Create a connection to S3
            AmazonS3 S3Client = AWSClientFactory.CreateAmazonS3Client(AWSKey, AWSSecret, region);

            //Loop folders
            foreach(var folder in config.Element("root").Descendants("folder")) {
                string[] files = Directory.GetFiles(folder.Element("path").Value, folder.Element("pattern").Value, (Convert.ToBoolean(folder.Attribute("recursive").Value)) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                // Loop through selected files
                foreach (string file in files)
                {
                    //Work out Amazon path
                    string S3Path = (folder.Element("basePath").Value + file.Replace(folder.Element("path").Value, "").Replace("\\", "/")).Replace("//", "/");

                    try
                    {
                        // If we want to ignore open files, we'll ask for read/write permissions.  Open files will cause an exception.
                        using (Stream stream = new FileStream(file, FileMode.Open, (Convert.ToBoolean(folder.Attribute("includeOpen").Value)) ? FileAccess.Read : FileAccess.ReadWrite))
                        {
                            PutObjectRequest request = new PutObjectRequest();
                            request.InputStream = stream;
                            request.Key = S3Path;
                            request.BucketName = folder.Element("bucket").Value;
                            S3Response response = S3Client.PutObject(request);
                            Console.WriteLine("Uploaded: " + file);
                        }
                        if ((Convert.ToBoolean(folder.Attribute("deleteOnUpload").Value))) File.Delete(file);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(file + " not uploaded: " + e.Message);
                    }
                }
            }

#if DEBUG
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
#endif
        }
    }
}
