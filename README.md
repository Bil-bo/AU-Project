# AU-Project

The game name is Card Wars. The main controls we will make use of here are the W,A,S,D keys to move around and you can also utilise the mouse as a camera controller, to make it easier to direct which way you'll go.

The key aim of the game is to defeat the orange boss enemy, this cannot happen unless you have defeated the yellow boss enemy who has a ladder serving as a pathway to get to the orange boss. In order to make your battle with the orange boss easier, you must collect pickups to weaken him eventually, as well as unlocking new cards to help you in your battles. New cards can be unlocked via treasure chests, or via battles with smaller enemies (red ones). 

Firstly, you'll spawn at the beginning of a room, the content here may take a while to generate on the WebGL build, but once it does, you can walk around multiple rooms. Some rooms will have pickups, some may have enemies and some will have treasure chests. Upon entering a room with enemies, the doors will automatically lock, until you defeat every enemy in the room, there will also be some rooms that have ally characters you must free to join your quest. 

Once in battle, you will have multiple options displayed to you in the form of cards. The cards are used by clicking then essentially dragging and dropping them onto your desired enemy to attack.
However, be careful, as some cards have less range than others, so if you try to drag and drop a sword on the most far-back enemy, it will not work as they are out-of-range.
The cards you want to make use of here are bow and poison in this case, you can use all 3 for the closest enemy however though. A useful pointer implemented and marker above enemies will show you where your card is going and who it is applicable to, respectively. The marker will be red if the enemy is out of range, yellow if they are in range, the marker glows green when you are on an enemy while trying to drag a card onto them.

There is also a very useful feature you can utilise in battles. This is the feature of combining cards, there are certain cards you can drag and drop onto each other which will create a new combined card.
For example, if you clicked, held, then dragged and dropped the poison card onto the sword card, you can create a new card called 'Poison Knife' then use this card as you see fit.

If you are granted an undesired set of cards and are unable to take your turn as desired, there is an end turn button at the bottom left of the screen which can make you skip a turn, make sure to use this accordingly to your strategy as you don't want your characters to get killed.

Once you are done defeating an enemy, you will be shown a reward menu, where you can select the card you want and click on the character to apply this new reward to, then it will exit the battle and go back to the overworld. 

There are also some cards you can apply to yourself and allies, which are strengthen and bless, making it so that an individual or your whole party do 50% more damage, respectively.

Lastly, you can also select the difficulty you want to play, where easy difficulty has HP and ATK for enemies reduced by 20%, Medium is standard, Hard has them increased by 30%. 

There is also a continue feature implemented, where you will essentially respawn at a previous point you were in, in the game, usually before a room with enemies in it, serving as a checkpoint.


LINKS TO EXTERNAL CODE (also found in comments in code):
https://gist.github.com/jasonmarziani/7b4769673d0b593457609b392536e9f9 - Fisher-Yates shuffle
https://forum.unity.com/threads/third-person-camera-rotate.197592/ - Camera Controller
https://discussions.unity.com/t/what-is-the-most-effective-way-to-structure-card-effects-in-a-single-player-game/216011/2 - Inspiration for the Card Creation
https://learn.unity.com/project/fps-template
https://www.boristhebrave.com/2020/09/12/dungeon-generation-in-binding-of-isaac/
