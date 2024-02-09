using System;
using System.Drawing;
using Microsoft.Extensions.Configuration;
using Azure;
using System.IO;

// Import namespaces
using Azure.AI.Vision.Common;
using Azure.AI.Vision.ImageAnalysis;

namespace image_analysis
{
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string aiSvcEndpoint = configuration["AIServicesEndpoint"];
                string aiSvcKey = configuration["AIServicesKey"];

                // Get image
                string imageFile = "images/street.jpg";
                if (args.Length > 0)
                {
                    imageFile = args[0];
                }

                // Authenticate Azure AI Vision client
                var cvClient = new VisionServiceOptions(aiSvcEndpoint, new AzureKeyCredential(aiSvcKey));

                
                // Analyze image
                AnalyzeImage(imageFile, cvClient);

                // Remove the background or generate a foreground matte from the image
                BackgroundForeground(imageFile, cvClient);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void AnalyzeImage(string imageFile, VisionServiceOptions serviceOptions)
        {
            Console.WriteLine($"\nAnalyzing {imageFile} \n");

            var analysisOptions = new ImageAnalysisOptions()
            {
                // Specify features to be retrieved
                Features = ImageAnalysisFeature.Caption
                            | ImageAnalysisFeature.DenseCaptions
                            | ImageAnalysisFeature.Objects
                            | ImageAnalysisFeature.People
                            | ImageAnalysisFeature.Text
                            | ImageAnalysisFeature.Tags
            };

            // Get image analysis
            using var imageSource = VisionSource.FromFile(imageFile);

            using var analyzer = new ImageAnalyzer(serviceOptions, imageSource, analysisOptions);

            var result = analyzer.Analyze();

            if (result.Reason == ImageAnalysisResultReason.Analyzed) 
            {
                // get image coptions
                if (result.Caption != null)
                {
                    Console.WriteLine(" Caption:");
                    Console.WriteLine($"    \"{result.Caption.Content}\", Confidence {result.Caption.Confidence:0.0000}");                     
                }

                // get image dense captions
                if (result.DenseCaptions != null)
                {
                    Console.WriteLine(" Dense Captions:");
                    foreach (var caption in result.DenseCaptions)
                    {
                        Console.WriteLine($"    \"{caption.Content}\", Confidence: {caption.Confidence:0.0000}");
                    }
                    Console.WriteLine($"\n"); 
                }

                // Get image tags
                if (result.Tags != null)
                {
                    Console.WriteLine($"    Tags:");
                    foreach (var tag in result.Tags)
                    {
                        Console.WriteLine($"    \"{tag.Name}\", Confidence {tag.Confidence:0.0000}");
                    }
                    Console.WriteLine($"\n");
                }

            }
            else 
            {
                var errorDetails = ImageAnalysisErrorDetails.FromResult(result);
                Console.WriteLine(" Analysis Failed");
                Console.WriteLine($"    Error reason: {errorDetails.Reason}");
                Console.WriteLine($"    Error code: {errorDetails.ErrorCode}");
                Console.WriteLine($"    Error message: {errorDetails.Message}\n");
            }

        }
        static void BackgroundForeground(string imageFile, VisionServiceOptions serviceOptions)
        {
            // Remove the background from the image or generate a foreground matte

        }
    }
}
