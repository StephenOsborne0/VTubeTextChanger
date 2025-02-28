# VTubeTextChanger

To setup:

1) Download the .zip file from the releases section (https://github.com/StephenOsborne0/VTubeTextChanger/releases) and 
    extract the contents to a folder somewhere on your PC.

2) Register your twitch bot on your account:

    - Go to the twitch developer website and sign in (https://dev.twitch.tv/console)
    - Click on "Applications" then "+ Register Your Application"
    - Set the Name to whatever you want (I called it VTubeStudioTextSwapper)
    - Set the OAuth Redirect URL to "http://localhost"
    - Set the Category to "Chat Bot"
    - Keep the Client Type on "Confidential"
    - Click "Create"

3) Get credentials for bot:

    - Find your application in the list and click "Manage"
    - Scroll down to the bottom, copy the "Client ID" field and paste it into the appSettings.json file "BotClientId" field.
    - Click "New Secret" and copy the secret key and paste it into the appSettings.json file "BotClientSecret" field.

4) Change appSettings.json to your liking

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

5) Run VTubeStudioTextSwapper.exe

    - It will give you a link to log in as the bot via OAuth2
    - Once authenticated:
      Confirm the scopes in the URL are chat:read and chat:edit (&scope=chat%3Aread+chat%3Aedit)
      Copy the "code" from the URL and paste it into the program (i.e. xxxxxxxxxxxxxxxxxxxxxxx)
    
        The URL should look something like this:
        http://localhost/?code=xxxxxxxxxxxxxxxxxxxxxxx&scope=chat%3Aread+chat%3Aedit

      You should only have to do this once as it will save it in a "tokens.json" file for future reference.

    - You should see the following (but for your channel):

        ?? Enter the authorization code from Twitch: xxxxxxxxxxxxxxxxxxxxxxx
        Token saved successfully.
        Initialized Twitch client for ChaosHillZone
        Connected to Twitch as vtubestudiotextswapperbot
        Attempting to join ChaosHillZone's channel...
        Bot has joined the channel: chaoshillzone
        Bot has joined the channel: chaoshillzone

6) Setup complete. Leave it running and make sure it picks up chat. Will add command customisation later.

