using UnityEngine;

namespace SmartifyOS.UI.TranslucentImage
{
[CreateAssetMenu(fileName = "New Scalable Blur Config",
                 menuName = "Translucent Image/ Scalable Blur Config",
                 order = 100)]
public class ScalableBlurConfig : BlurConfig
{
    [SerializeField]
    [Tooltip("Blurriness. Does NOT affect performance")]
    float radius = 4;
    [SerializeField]
    [Tooltip("The number of times to run the algorithm to increase the smoothness of the effect. Can affect performance when increase")]
    [Range(0, 8)]
    int iteration = 4;
    [SerializeField]
    [Tooltip("How strong the blur is")]
    float strength;

    /// <summary>
    /// Distance between the base texel and the texel to be sampled.
    /// </summary>
    public float Radius
    {
        get { return radius; }
        set { radius = Mathf.Max(0, value); }
    }

    /// <summary>
    /// Half the number of time to process the image. It is half because the real number of iteration must alway be even. Using half also make calculation simpler
    /// </summary>
    /// <value>
    /// Must be non-negative
    /// </value>
    public int Iteration
    {
        get { return iteration; }
        set { iteration = Mathf.Max(0, value); }
    }

    /// <summary>
    /// User friendly property to control the amount of blur
    /// </summary>
    ///<value>
    /// Must be non-negative
    /// </value>
    public float Strength
    {
        get { return strength = Radius * Mathf.Pow(2, Iteration); }
        set
        {
            strength = Mathf.Clamp(value, 0, (1 << 14) * (1 << 14));

            // Bit fiddling would be faster, but need unsafe or .NET Core 3.0+
            // for BitOperations, and BitConverter that doesn't creates garbages :(
            radius    = Mathf.Sqrt(strength);
            iteration = 0;
            while ((1 << iteration) < radius)
                iteration++;
            radius = strength / (1 << iteration);
        }
    }
}
}
