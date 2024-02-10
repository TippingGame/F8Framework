using UnityEngine;
namespace F8Framework.Core
{
    public static class AudioSourceExts
    {
        public static void Reset(this AudioSource @this)
        {
            @this.clip = null;
            @this.mute = false;
            @this.playOnAwake = true;
            @this.loop = false;
            @this.priority = 128;
            @this.volume = 1;
            @this.pitch = 1;
            @this.panStereo = 0;
            @this.spatialBlend = 0;
            @this.reverbZoneMix = 1;
            @this.dopplerLevel = 1;
            @this.spread = 0;
            @this.maxDistance = 500;
        }
    }
}
