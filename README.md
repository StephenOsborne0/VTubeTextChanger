# VTubeTextChanger

To setup:

1) Download the .zip file from the releases section (https://github.com/StephenOsborne0/VTubeTextChanger/releases) and 
    extract the contents to a folder somewhere on your PC.

2) Register your twitch bot on your account:

    ![image](https://github.com/user-attachments/assets/44f05c9f-1228-42c0-a59c-4aee3ba94b9d)

    - Go to the twitch developer website and sign in (https://dev.twitch.tv/console)
    - Click on "Applications" then "+ Register Your Application"
    - Set the Name to whatever you want (I called it VTubeStudioTextSwapper)
    - Set the OAuth Redirect URL to "http://localhost"
    - Set the Category to "Chat Bot"
    - Keep the Client Type on "Confidential"
    - Click "Create"
  
   ![image](https://github.com/user-attachments/assets/c7b18191-ba7f-4be1-9c9b-e0f857f1b2ea)


3) Get credentials for bot:

    - Find your application in the list and click "Manage"
    - Scroll down to the bottom, copy the "Client ID" field and paste it into the appSettings.json file "BotClientId" field.
    - Click "New Secret" and copy the secret key and paste it into the appSettings.json file "BotClientSecret" field.

    ![image](https://github.com/user-attachments/assets/9899fdd8-03a3-40b0-8d46-e692bbb6852c)


5) Change appSettings.json to your liking

    - Change the "ChannelName" field to your channel name.
    - Change the "OutputPath" field to your desired VTubeStudio file path.

    It should look something like this:

    ```

    {
        "TwitchSettings": {
            "ChannelName": "ChaosHillZone",
            "BotUsername": "VTubeStudioTextSwapperBot",
            "BotClientId": "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            "BotClientSecret": "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"
        },
        "TextSettings": {
            "FontName": "Arial",
            "FontSize": 48,
            "Padding": 20,
            "OutputPath": "C:\\VTubeStudio\\items\\wherever-your-image-is\\text.png"
        }
    }

    ```

6) Run VTubeStudioTextSwapper.exe

       ![image](https://github.com/user-attachments/assets/2e1d3e33-cc61-4885-b7b3-6e17d51be847)

    - It will give you a link to log in as the bot via OAuth2

      ![image](https://github.com/user-attachments/assets/ccb2394f-04a3-490b-99c5-9eb82f672f33)

    - Once authenticated:
      Confirm the scopes in the URL are chat:read and chat:edit (&scope=chat%3Aread+chat%3Aedit)
      Copy the "code" from the URL and paste it into the program (i.e. xxxxxxxxxxxxxxxxxxxxxxx)
    
        The URL should look something like this:
        http://localhost/?code=xxxxxxxxxxxxxxxxxxxxxxx&scope=chat%3Aread+chat%3Aedit

      You should only have to do this once as it will save it in a "tokens.json" file for future reference.
      
      The reason you have to do all this copy and paste step is because otherwise I'd have to host the bot on a
      server and set up a website to redirect to and I can't be bothered with that for a silly project like this.

    - You should see the following:

    ![image](https://github.com/user-attachments/assets/e85f97d1-1329-4ca8-8d55-88bdf5592157)


8) Setup complete. Leave it running and make sure it picks up chat. Will add command customisation later.

   
    ![image](https://github.com/user-attachments/assets/9fcf4728-9eef-4a59-bc09-51cf47ad26d6)

    ![image](https://github.com/user-attachments/assets/ce3df61a-6860-4531-b71e-8cf4ccf86f85)

9) When you're done streaming, close it. When you next stream, just run the VTubeStudioTextSwapper.exe again.
   You may occassionally need to do the last authentication step again if the token expires.


