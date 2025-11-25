public static class GameData
    {
        public static int currentHour = 0;
        public static bool isAnomalyPresent = false;
        public static bool spawnPlayerFromLeft = true; 
        public static bool isSprinting = false; 

        public static bool tutorialWalkShown = false;
        public static bool tutorialRunShown = false;
        public static bool tutorialLetterShown = false;

        public static void ResetAllData()
        {
            currentHour = 0;
            isAnomalyPresent = false;
            spawnPlayerFromLeft = true;
            isSprinting = false;
            
            tutorialWalkShown = false;
            tutorialRunShown = false;
            tutorialLetterShown = false;
        }
    }