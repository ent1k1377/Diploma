using System;

namespace Resources.Scripts.Storage
{
    [Serializable]
    public class SettingsData
    {
        public int VolumeLevelMusic;
        public int VolumeLevelSound;

        public SettingsData()
        {
            VolumeLevelMusic = 20;
            VolumeLevelSound = 20;
        }
    }
}