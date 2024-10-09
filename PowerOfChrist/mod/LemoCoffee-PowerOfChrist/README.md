# Power of Christ

Howdy hi hello! This is a small joke mod made upon request from a friend.
Upon reaching low health, flashes of Jesus will fill the players screen as their time approaches, church bell ringing.

## Demos

Default Mod \
https://youtu.be/LpLVtxpxqXg \
Customization Demo \
https://youtu.be/ynli4SweoIs \

## Installation

Download with a mod manager, or throw the contents of the mod into your plugins folder

```
...\BepInEx\plugins\LemoCoffee-PowerOfChrist\LethalPowerOfChrist.dll
```
## Customization

Both the images and sound that the mod uses can be swapped out for each client. \ 
For replacing the sound, either replace the file titled `sound.wav` or add a new file in the mod's folder and change the name of the file name in the config. \ 
For the images, just throw any `.png` file into `\LemoCoffee-PowerOfChrist\images\` and next time you start up the game, those images should now be in the pool that the flashes can randomly draw from!

## Configuration
```
[Audio]
Volume - Changes the volume of the sound clip that plays accompanying each flash [0, 1].
File Name - Name of the audio clip file, including extension. Supports ogg, mp3, and wav.

[General]
Flash Once - Determines whether flashes will repeat or will only trigger once after taking damage.
Allow Overlap - If set to true, the flasher will be able to interrupt itself if the player triggers the flasher.
Trigger Point - The amount of health the player must reach before the flashes will trigger.
Flash Delay - How long in seconds between each flash.
```

## Credits

Thank you so so much to Sluka for their (involuntary) bug testing!

## Links
Github \
https://github.com/LemoCoffee/LethalMods