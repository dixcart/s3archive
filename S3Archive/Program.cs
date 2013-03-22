using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;
using System.Xml.Linq;
using System.Security.Cryptography;

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
                Console.WriteLine("Reading path: " + folder.Element("path").Value);
                string[] files = Directory.GetFiles(folder.Element("path").Value, folder.Element("pattern").Value, (Convert.ToBoolean(folder.Attribute("recursive").Value)) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                Console.WriteLine("Found " + files.Length + " files to upload");

                // Loop through selected files
                foreach (string file in files)
                {
                    //Work out Amazon path
                    string S3Path = (folder.Element("basePath").Value + file.Replace(folder.Element("path").Value, "").Replace("\\", "/")).Replace("//", "/");

                    try
                    {
                        bool uploaded = false;                        
                        // If we want to ignore open files, we'll ask for read/write permissions.  Open files will cause an exception.
                        using (Stream stream = new FileStream(file, FileMode.Open, (Convert.ToBoolean(folder.Attribute("includeOpen").Value)) ? FileAccess.Read : FileAccess.ReadWrite))
                        {
                            //Get MD5 hash for verification later.
                            var md5 = MD5.Create();
                            string md5Hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                            md5.Dispose();

                            PutObjectRequest request = new PutObjectRequest();
                            request.InputStream = stream;
                            request.Key = S3Path;
                            request.BucketName = folder.Element("bucket").Value;
                            request.Timeout = -1;
                            S3Response response = S3Client.PutObject(request);
                            string eTag = ((PutObjectResponse)response).ETag.Replace("\"", "");
                            if (md5Hash == eTag) uploaded = true;
                        }
                        if (uploaded)
                        {
                            if ((Convert.ToBoolean(folder.Attribute("deleteOnUpload").Value))) File.Delete(file);
                            Console.WriteLine("Uploaded: " + file);
                        }
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
