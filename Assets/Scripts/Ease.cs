using UnityEngine;

public static class Ease
{
    public static float EaseOutQuad(float t)
    {
        return 1f - (1f - t) * (1f - t);
    }
}

