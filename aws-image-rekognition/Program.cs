using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;

namespace aws_image_rekognition
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            //Set Logging level

            AWSConfigs.LoggingConfig.LogTo = LoggingOptions.Console;
            AWSConfigs.LoggingConfig.LogMetrics = true;
            AWSConfigs.LoggingConfig.LogResponses = ResponseLoggingOption.OnError;

            await Detect();
        }

        public static async Task Detect()
        {
            //Get picture from local storage
            var photo = "img/sheep.png";

            var image = new Image();
            try
            {
                await using var fs = new FileStream(photo, FileMode.Open, FileAccess.Read);
                var data = new byte[fs.Length];
                fs.Read(data, 0, (int)fs.Length);
                image.Bytes = new MemoryStream(data);
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load file " + photo);
                return;
            }

            var rekognitionClient = new AmazonRekognitionClient();

            //Set settings for label detection
            var detectlabelsRequest = new DetectLabelsRequest
            {
                Image = image,
                MaxLabels = 3,
                MinConfidence = 77F
            };


            var detectLabelsResponse = await rekognitionClient.DetectLabelsAsync(detectlabelsRequest);
            Console.WriteLine("Detected labels for " + photo);
            foreach (var label in detectLabelsResponse.Labels)
                Console.WriteLine("{0}: {1}", label.Name, label.Confidence);
        }
    }
}