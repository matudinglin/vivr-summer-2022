<!--
 * @Author: Hsuan-Wei Fan
 * @Date: 2021-07-28 23:24:10
 * @LastEditors: Hsuan-Wei Fan
 * @LastEditTime: 2021-08-07 21:22:35
 * @Description: 
-->

# A VR exploration tool for visual impaired people

## Environment

Unity 2020.3.14f1

### Minimum requirements

Android 8 (API Level 26)

> Required by Lofelt Studio Plugin.

## Assets

Thanks to the ecosystem of Unity, these good assets and plugins from Unity Asset Store saved us a lot of time and greatly improve the experiense of our project.

1. [Fingers Lite - Free Finger Touch Gestures for Unity](https://assetstore.unity.com/packages/tools/input-management/fingers-lite-free-finger-touch-gestures-for-unity-64276)
2. [Office set - Environment](https://assetstore.unity.com/packages/3d/environments/office-set-environment-90938)
3. [Absolutely Free Music](https://assetstore.unity.com/packages/audio/music/absolutely-free-music-4883)
4. Lofelt: predefined haptic feedback
5. Google Resonance Audio: support spatialize audio

## Features

This section introduces the features we have so far.

### Overview Mode

1. Move single finger: explore the scene, haptic feedback will be triggered when the finger encounters a border. When a finger is exploring a room, the background audio will be played.

2. Double tap the screen when you're exploring a room: read the description of the room.

3. Pinch fingers apart: switch to the area of interest mode based the most recent room you're exploring.

> Background audio means a music clip binded with the room. It is used for hinting the users the current room they're exploring. The pitch of the audio might be changed based on the hue and lightness.

### Area of Interest Mode

This mode is used for exploring the area of interest. In the demo, it means room.

1. Move single finger: explore the room, haptic feedback will be triggerd when the finger encounters a border or point of interest (POI).

2. Pinch finger apart: switch to the immersion mode

3. Pinch finger together: switch to the overview mode.

### Immersion Mode

1. Hold the device and scan the surrounding: rotate the avatar to look around. You will hear the audio feedback (TTS phrase) of the object in front of you.

2. **Walk gesture**: use two fingers to swipe the screen alternatively.

3. Tap the screen: the spatialized audio hint of POIs nearby will be played sequentially.

4. Pinch finger together: switch to the area of interest mode.

## Structure

The main control logic is defined in the Sripts folder. 

### CameraController

Since the overview/area of interest mode and immersive mode are rendered by different camera. This script is used for controlling the switch between the cameras as well as provide the information of a camera.

### GestureController

This script is based on Finger Lite, and it is used for supporting the gesture detection. Other controllers will be attached to this script, and so that the controller and trigger specific action within a gesture is recognized. If you would like to modify the behavior of a certain gesture, you are likely looking for function `{gesture}_callback` in this script.

### AudioController

This script is used for controlling the background audio as well as hint audio feedback.

### AvatarController

 It controls the avatar behavior as well as make avatar and (immersive mode) camera's position and rotation consistent. Camera's position comes first, and then the avatar position. Though it might be different from common game implementation, it is more convenitent to controll camera relevant behavior within a same script.

### TTSController

It controls the text-to-speech behavior. I use the Azure TTS API and so that this script relies on network. Replace the `azureSubscriptionKey` with the updated on when it is expired. If the TTS does not work, check the key configuration first. The controller also provide some parameters to control the TTS, you can also define your own parameters.

### IntersectionController

Since I use collision to detect which space user is exploring. In the other word, I will emit a raycast from the touch point world position along with the overview camera `forward` direction. This controller serve for handling the intersected room or POI.

## Sequntial Graph

There will be two sequentail graphs listed below to demonstrate the main calling chain controllers. There are some scripts not being included in the graphs because they are used for controller game object state only or simply a semantic enhancement.

1. `GyroCamera`: use the gyro information to update avatar camera's rotation
2. `POI`: some methods for POI, it serves like `poi.do_something()`
3. `Semantic`: additional information for a room

### Overview/Area of Interest Mode

![](./img/overview_mode_flow.png)

### Immersive Mode

![](./img/immersive_mode_flow.png)
