AUDIO SYSTEM INSTRUCTIONS

1. MAIN MENU MUSIC
   To control audio in the Main Menu:
   a. Create an Empty GameObject in your Main Menu Scene hierarchy.
   b. Name it "AudioController" (or similar).
   c. Add Component -> Search for "MainMenuAudioManager".
   d. In the Inspector, you will see a 'Menu Music' field.
   e. Drag and Drop your audio file (mp3/wav) from the Project window into this field.
   f. Adjust volume is needed. The music will play automatically.

2. PAUSING AUDIO IN GAME (New & Improved)
   Use the 'PauseMenuController' script to handle UI and Audio correctly.

   SETUP:
   a. Create your Pause Menu UI (e.g. a Panel with a Resume Button).
   b. Create an empty GameObject (or use the Canvas) and attach `PauseMenuController.cs`.
   c. Drag your Pause Panel GameObject into the "Pause Menu UI" slot in the script.

   BUTTONS:
   a. **Pause Button** (on gameplay HUD):
      - OnClick() -> Drag the object with `PauseMenuController` -> Select `OnPauseButtonPressed()`.
      - This will SHOW the menu and PAUSE the game + audio.
   
   b. **Resume Button** (inside the Pause Menu):
      - OnClick() -> Drag the object with `PauseMenuController` -> Select `OnResumeButtonPressed()`.
      - This will HIDE the menu and RESUME the game + audio.

   WHY THIS WORKS:
   The script calls `GameManager.Instance.PauseGame()`, which sets `AudioListener.pause = true`, immediately finding and silencing ALL audio sources.
