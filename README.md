# SpotSync
An application which lets you synchronize your spotify with others.

## CI Builds
Master [![Build Status](https://dev.azure.com/Kipley/Spotibro/_apis/build/status/Spotibro?branchName=master)](https://dev.azure.com/Kipley/Spotibro/_build/latest?definitionId=5&branchName=master)

## Getting Setup
1. Clone repository ```git clone https://github.com/hibroseph/SpotSync.git```
1. Obtain Spotify Application Credientals
    1. If you have confidence with project owner, ask for Client Secret and Client Id of project owners Spotify application
    1. Create a Spotify application
        1. Go to [Spotify Developer Dashboard](https://developer.spotify.com/dashboard/login) and Login and Create Application
        1. Go to application dashboard and obtain Client ID and Client Secret 
        **DO NOT SHARE THIS CLIENT SECRET WITH ANYONE**
1. Add Spotify application credientals in ```appsettings.json```
1. Determine what port your application will use when debugging (This can be determined by running the application and seeing what the URL is)
1. Make sure the Redirect URL in the ```appsettings.json``` is using the port that your app uses by default
1. Copy the Redirect URL and add it as a Redirect URL in your Spotify Application in the [Spotify Developer Dashboard](https://developer.spotify.com/dashboard/applications)
    1. Click Edit Settings and scroll down to Redirect URIs
    1. Enter in the Redirect URL as shown in the ```appsettings.json``` (this includes the /account/authorized)
1. Running SpotSync you should see the main app page and be able to login.

## Helpful Development/Debugging Techniques
### Debugging/Debugging Party Commands
If you would like to debug how other users will be affected by party commands (such as Sync Current Song), you can create a party and add yourself to that party with the following code added into the PartyController in the Index action after the User is found
```csharp
/******************* DEBUGGING CODE TO TEST PARTIES (do not send this to production) ******************************/
    var partyCode = _partyService.StartNewParty(user);

    await _partyService.JoinPartyAsync(new PartyCodeDTO { PartyCode = partyCode }, user);
/******************************************************************************************************************/
```
Any action you do that affects the party will affect your own Spotify so you can see what it does to your playback (and queue etc)
