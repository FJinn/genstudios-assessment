# Hello from Gen Studios
Hi, we are excited to bring in passionate game and software developer into our team! By completing assessment project below (a simple Unity project), we can learn more about who you are! But you can be creative to show off your capability!

Have fun and good luck!

You can see our expected result in [Docs Directory](Docs)

# Firstly
1. Install Unity editor version `2022.3.62f`.
1. Clone this project to your local machine.
1. The Unity project that you should use and work on is the `GenStudiosAssessmentUnityProject` directory.
1. All assets required can be found in `GenStudiosAssessmentUnityProject\Assets\Assessment\Assets` directory.

# Instruction
1. Use Unity Editor `2022.3.62f`
1. All new assets must be added in `GenStudiosAssessmentUnityProject\Assets\Assessment\Assets` directory.
1. All scripts must be located in `GenStudiosAssessmentUnityProject\Assets\Assessment\Scripts` directory.
1. All prefabs must be located in `GenStudiosAssessmentUnityProject\Assets\Assessment\Prefabs` directory.
1. Any new assets must be located in `GenStudiosAssessmentUnityProject\Assets\Assessment\`
1. Assessment scene: Create a new scene in Unity named `AssessmentScene` by cloning `SampleScene`

## File submission
1. Zip your unity project as `UnityAssessment_{yourname}.zip`
1. Compile to an APK as `UnityAssessment_{yourname}.apk`
1. Send us the download link to the files above.
1. Send us the Github link to your project.

## Assessment criteria
For this assessment, you can see a list of [specification](#game-specification) that is expected of a working simple game.
It gets overwhelming very quickly, but rest assured, you don't have to do all of it.
At minimum, your game should have the functions/features below:
1. A joystick that can control player movement.
1. Player is animated according to its action, e.g. idle and walking.
1. A camera that follows player as center target.
1. An area that spawns food when Player enters it.

But try not to do the minimum, we want to see what you can do!

*During the interview, we might ask you to change or add some feature.

# Game settings
Player plays as a fast food restaurant staff that only serves burger and soft drink.
Customers will appear and form a queue at the counter. When it's a customer's turn, they will place an order for either a burger or a soft drink. The player must then head to the kitchen, prepare the requested item, and deliver it to the counter. After receiving their order, the customer pays, takes their food, and exits the restaurant

# Game specification

## Camera view
1. Always follows player movement as the center target.
1. The game view perspective must be similar to what it looks like when `SampleScene/Main Camera` setting is used.

## Player
1. Player can carry max 3 items. Add option to allow whether type of items can be mixed. E.g. burger only or softdrink only or burger&softdrink
1. Player can place object to counter.
1. Only player can collect money.
1. Only player can trigger action timer.

## Player controller
1. Player movement must be controlled using a joystick that will appears by touching anywhere on the screen.
1. Use image assets in 
    - Outline: `C:\Users\austh\Documents\Unity\GenStudiosAssessment\Assets\Assessment\Assets\2D\Joystick_Handle_Outline.png`
    - Handle: `C:\Users\austh\Documents\Unity\GenStudiosAssessment\Assets\Assessment\Assets\2D\Joystick_Handle_Plain.png`
1. Expectation [demo](Docs/PlayerController_demo.mp4)

## Customer
### Spawning
1. Customer spawns from `SampleScene/Customer Spawn Point` and walks to counter to queue
1. Spawning frequency is 1 customer every 2 seconds.
1. Only 5 customers may exists at a time, stop spawning when maximum number has been reached and resume when possible.
1. Cycle the outfit available in the assets for customer look variation. Look in the prefab `Foodcrt_Rig_Customer` outfit node.

### Ordering flow
1. When it is the customer's turn, show a speech bubble with words either "Burger" or "Cola" as its order. Use 2D asset `SpeechBubble_base.png` as the bubble base and add "Burger" or "Cola" text to it.
1. If the customer have waited for more than 3 seconds, display an angry speech bubble. Use the 2D asset `UI_Img_Emotion_Angry.png` as the bubble.
1. When the customer have taken the food and paid for it, it will then leaves the counter and display a happy speech bubble. Use the 2D asset `UI_Img_Emotion_Happy.png` as the bubble.
1. Payment amount is based on the food ordered price.
1. When leaving, customer walks to `SampleScene/Customer Exit Point`

### Other
1. Customer can only carry maximum 1 item.
1. Use carry animation when customer is carrying food. 

## Kitchen
1. Food can only be spawned at specific area after action timer is completed.
1. Use following node as the food spawn area in the scene
    - Burger: `SampleScene/Kitchen/Burger`
    - Softdrink: `SampleScene/Kitchen/Softdrink`
1. When food object is spawned, let it jump from initial position to final position(player hand).

## Action timer
1. Only player can start action timer.
1. Action timer can only be started when the player is in its interactive area.
    - Burger: `SampleScene/Kitchen/Burger/Interactive Area`, duration 2 seconds to complete.
    - Softdrink: `SampleScene/Kitchen/Softdrink/Interactive Area`, duration 1 seconds to complete.
1. When player leaves action timer interactive area, apply these behavior to each type of food:
    - Burger: Resets to zero
    - Softdrink: Stops at where it is, and continue again when player re-enter the interactive area 
1. When action timer started, player must be in Cooking animation state and will only stopped whenever the action timer is stopped(by exiting or completing the action timer)
    - Cooking animation: `Assets\Assessment\Assets\3D\Animation\Person\Cooking`
1. Expectation [demo](Docs/FoodCollect_demo.mp4)

## Food
1. Pricing:
    - Burger: $3,
    - Softdrink: $1,
1. Be creative with the animation when food object is moving!

## Counter
1. Only 4 customers can queue at a time
1. Let the food to stay on the table for 0.5s before the customer can take it
1. Money received from customer should be placed at the `SampleScene/Counter/Money` position. 2x3 grid arrangement and stacked upward.
1. Expectation [demo](Docs/Counter_demo.mp4)

## Money
1. Use prefab `Money` as money object.
1. Each money object is equivalent to $1.
1. Only player can collect money.
1. Player can only collect money when it is close to the money.
1. Whenever money is collected, update the money text at `SampleScene/UI/Canvas/Player_Money/Text`
1. The money amount must be saved and available in next session(game is closed and restarted).
1. Expectation [demo](Docs/MoneyCollect_demo.mp4)

## Player and Customer animation
Animation assets can be found in: `Assets\Assessment\Assets\3D\Animation\Person`.
Please use appropriate animation for each action!