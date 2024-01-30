using System.Collections.Generic;

namespace TweenTimeline
{
    public readonly struct ClipInputs
    {
        public readonly List<(float start, float end)> ClipIntervals;

        public ClipInputs(List<(float start, float end)> clipIntervals)
        {
            ClipIntervals = clipIntervals;
        }

        public bool IsAnyPlaying(float trackTime)
        {
            bool active = false;
            for (int i = 0; i < ClipIntervals.Count; i++)
            {
                if (ClipIntervals[i].start <= trackTime && trackTime <= ClipIntervals[i].end)
                {
                    active = true;
                    break;
                }
            }

            return active;
        }

        public bool HasAnyStarted(float trackTime)
        {
            bool active = false;
            for (int i = 0; i < ClipIntervals.Count; i++)
            {
                if (ClipIntervals[i].start <= trackTime)
                {
                    active = true;
                    break;
                }
            }

            return active;
        }
    }
}