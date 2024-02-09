from dotenv import load_dotenv
import os
from array import array
from PIL import Image, ImageDraw
import sys
import time
from matplotlib import pyplot as plt
import numpy as np

# Import namespaces
import azure.ai.vision as sdk


def main():
    global cv_client

    try:
        # Get Configuration Settings
        load_dotenv()
        ai_endpoint = os.getenv('AI_SERVICE_ENDPOINT')
        ai_key = os.getenv('AI_SERVICE_KEY')

        # Get image
        image_file = 'images/street.jpg'
        if len(sys.argv) > 1:
            image_file = sys.argv[1]

        # Authenticate Azure AI Vision client
        cv_client = sdk.VisionServiceOptions(ai_endpoint, ai_key)
        
        # Analyze image
        AnalyzeImage(image_file, cv_client)

        # Generate thumbnail
        BackgroundForeground(image_file, cv_client)

    except Exception as ex:
        print(ex)


def AnalyzeImage(image_file, cv_client):
    print('\nAnalyzing', image_file)

    # Specify features to be retrieved
    analysis_options = sdk.ImageAnalysisOptions()

    features = analysis_options.features = (
        sdk.ImageAnalysisFeature.CAPTION |
        sdk.ImageAnalysisFeature.DENSE_CAPTIONS |
        sdk.ImageAnalysisFeature.TAGS |
        sdk.ImageAnalysisFeature.OBJECTS |
        sdk.ImageAnalysisFeature.PEOPLE
    )



    # Get image analysis
    image = sdk.VisionSource(image_file)

    image_analyzer = sdk.ImageAnalyzer(cv_client, image, analysis_options)

    result = image_analyzer.analyze()

    if result.reason == sdk.ImageAnalysisResultReason.ANALYZED:
        # Get image captions
        if result.caption is not None:
            print("\nCaption:")
            print(" Caption: '{}' (confidence: {:.2f}%)".format(result.caption.content, result.caption.confidence * 100))

        # Get image dense captions
        if result.dense_captions is not None:
            print("\nDense Captions:")
            for caption in result.dense_captions:
                print(" Caption: '{}' (confidence: {:.2f}%)".format(caption.content, caption.confidence * 100))

        # Get image tags
        if result.tags is not None:
                print("\nTags:")
                for tag in result.tags:
                        print(" Tag: '{}' (confidence: {:.2f}%)".format(tag.name, tag.confidence * 100))


        # Get objects in the image


        # Get people in the image

    else:
        error_details = sdk.ImageAnalysisErrorDetails.from_result(result)
        print(" Analysis failed.")
        print(" Error reason: {}".format(error_details.reason))         
        print(" Error code: {}".format(error_details.error_code)) 
        print(" Error message: {}".format(error_details.message))


def BackgroundForeground(image_file, cv_client):
    print('\n')
    
    # Remove the background from the image or generate a foreground matte



if __name__ == "__main__":
    main()
