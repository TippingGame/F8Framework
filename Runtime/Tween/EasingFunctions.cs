using UnityEngine;

public enum Ease
{
    EaseInQuad = 0,
    EaseOutQuad,
    EaseInOutQuad,
    EaseInCubic,
    EaseOutCubic,
    EaseInOutCubic,
    EaseInQuart,
    EaseOutQuart,
    EaseInOutQuart,
    EaseInQuint,
    EaseOutQuint,
    EaseInOutQuint,
    EaseInSine,
    EaseOutSine,
    EaseInOutSine,
    EaseInExpo,
    EaseOutExpo,
    EaseInOutExpo,
    EaseInCirc,
    EaseOutCirc,
    EaseInOutCirc,
    Linear,
    Spring,
    EaseInBounce,
    EaseOutBounce,
    EaseInOutBounce,
    EaseInBack,
    EaseOutBack,
    EaseInOutBack,
    EaseInElastic,
    EaseOutElastic,
    EaseInOutElastic,
}

public static class EasingFunctions
{
    

    private const float NATURAL_LOG_OF_2 = 0.693147181f;

    //
    // Easing functions
    //

    public static float Linear(float start, float end, float value)
    {
        return Mathf.Lerp( start, end, value );
    }

    public static float Spring(float start, float end, float value)
    {
        value = Mathf.Clamp01( value );
        value = ( Mathf.Sin( value * Mathf.PI * ( 0.2f + 2.5f * value * value * value ) ) * Mathf.Pow( 1f - value, 2.2f ) + value ) * ( 1f + ( 1.2f * ( 1f - value ) ) );
        return start + ( end - start ) * value;
    }

    public static float EaseInQuad(float start, float end, float value)
    {
        end -= start;
        return end * value * value + start;
    }

    public static float EaseOutQuad(float start, float end, float value)
    {
        end -= start;
        return -end * value * ( value - 2 ) + start;
    }

    public static float EaseInOutQuad(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * value * value + start;
        value--;
        return -end * 0.5f * ( value * ( value - 2 ) - 1 ) + start;
    }

    public static float EaseInCubic(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value + start;
    }

    public static float EaseOutCubic(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * ( value * value * value + 1 ) + start;
    }

    public static float EaseInOutCubic(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * value * value * value + start;
        value -= 2;
        return end * 0.5f * ( value * value * value + 2 ) + start;
    }

    public static float EaseInQuart(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value + start;
    }

    public static float EaseOutQuart(float start, float end, float value)
    {
        value--;
        end -= start;
        return -end * ( value * value * value * value - 1 ) + start;
    }

    public static float EaseInOutQuart(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * value * value * value * value + start;
        value -= 2;
        return -end * 0.5f * ( value * value * value * value - 2 ) + start;
    }

    public static float EaseInQuint(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value * value + start;
    }

    public static float EaseOutQuint(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * ( value * value * value * value * value + 1 ) + start;
    }

    public static float EaseInOutQuint(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * value * value * value * value * value + start;
        value -= 2;
        return end * 0.5f * ( value * value * value * value * value + 2 ) + start;
    }

    public static float EaseInSine(float start, float end, float value)
    {
        end -= start;
        return -end * Mathf.Cos( value * ( Mathf.PI * 0.5f ) ) + end + start;
    }

    public static float EaseOutSine(float start, float end, float value)
    {
        end -= start;
        return end * Mathf.Sin( value * ( Mathf.PI * 0.5f ) ) + start;
    }

    public static float EaseInOutSine(float start, float end, float value)
    {
        end -= start;
        return -end * 0.5f * ( Mathf.Cos( Mathf.PI * value ) - 1 ) + start;
    }

    public static float EaseInExpo(float start, float end, float value)
    {
        end -= start;
        return end * Mathf.Pow( 2, 10 * ( value - 1 ) ) + start;
    }

    public static float EaseOutExpo(float start, float end, float value)
    {
        end -= start;
        return end * ( -Mathf.Pow( 2, -10 * value ) + 1 ) + start;
    }

    public static float EaseInOutExpo(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * Mathf.Pow( 2, 10 * ( value - 1 ) ) + start;
        value--;
        return end * 0.5f * ( -Mathf.Pow( 2, -10 * value ) + 2 ) + start;
    }

    public static float EaseInCirc(float start, float end, float value)
    {
        end -= start;
        return -end * ( Mathf.Sqrt( 1 - value * value ) - 1 ) + start;
    }

    public static float EaseOutCirc(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * Mathf.Sqrt( 1 - value * value ) + start;
    }

    public static float EaseInOutCirc(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return -end * 0.5f * ( Mathf.Sqrt( 1 - value * value ) - 1 ) + start;
        value -= 2;
        return end * 0.5f * ( Mathf.Sqrt( 1 - value * value ) + 1 ) + start;
    }

    public static float EaseInBounce(float start, float end, float value)
    {
        end -= start;
        float d = 1f;
        return end - EaseOutBounce( 0, end, d - value ) + start;
    }

    public static float EaseOutBounce(float start, float end, float value)
    {
        value /= 1f;
        end -= start;
        if (value < ( 1 / 2.75f ))
        {
            return end * ( 7.5625f * value * value ) + start;
        }
        else if (value < ( 2 / 2.75f ))
        {
            value -= ( 1.5f / 2.75f );
            return end * ( 7.5625f * ( value ) * value + .75f ) + start;
        }
        else if (value < ( 2.5 / 2.75 ))
        {
            value -= ( 2.25f / 2.75f );
            return end * ( 7.5625f * ( value ) * value + .9375f ) + start;
        }
        else
        {
            value -= ( 2.625f / 2.75f );
            return end * ( 7.5625f * ( value ) * value + .984375f ) + start;
        }
    }

    public static float EaseInOutBounce(float start, float end, float value)
    {
        end -= start;
        float d = 1f;
        if (value < d * 0.5f) return EaseInBounce( 0, end, value * 2 ) * 0.5f + start;
        else return EaseOutBounce( 0, end, value * 2 - d ) * 0.5f + end * 0.5f + start;
    }

    public static float EaseInBack(float start, float end, float value)
    {
        end -= start;
        value /= 1;
        float s = 1.70158f;
        return end * ( value ) * value * ( ( s + 1 ) * value - s ) + start;
    }

    public static float EaseOutBack(float start, float end, float value)
    {
        float s = 1.70158f;
        end -= start;
        value = ( value ) - 1;
        return end * ( ( value ) * value * ( ( s + 1 ) * value + s ) + 1 ) + start;
    }

    public static float EaseInOutBack(float start, float end, float value)
    {
        float s = 1.70158f;
        end -= start;
        value /= .5f;
        if (( value ) < 1)
        {
            s *= ( 1.525f );
            return end * 0.5f * ( value * value * ( ( ( s ) + 1 ) * value - s ) ) + start;
        }
        value -= 2;
        s *= ( 1.525f );
        return end * 0.5f * ( ( value ) * value * ( ( ( s ) + 1 ) * value + s ) + 2 ) + start;
    }

    public static float EaseInElastic(float start, float end, float value)
    {
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s;
        float a = 0;

        if (value == 0) return start;

        if (( value /= d ) == 1) return start + end;

        if (a == 0f || a < Mathf.Abs( end ))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / ( 2 * Mathf.PI ) * Mathf.Asin( end / a );
        }

        return -( a * Mathf.Pow( 2, 10 * ( value -= 1 ) ) * Mathf.Sin( ( value * d - s ) * ( 2 * Mathf.PI ) / p ) ) + start;
    }

    public static float EaseOutElastic(float start, float end, float value)
    {
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s;
        float a = 0;

        if (value == 0) return start;

        if (( value /= d ) == 1) return start + end;

        if (a == 0f || a < Mathf.Abs( end ))
        {
            a = end;
            s = p * 0.25f;
        }
        else
        {
            s = p / ( 2 * Mathf.PI ) * Mathf.Asin( end / a );
        }

        return ( a * Mathf.Pow( 2, -10 * value ) * Mathf.Sin( ( value * d - s ) * ( 2 * Mathf.PI ) / p ) + end + start );
    }

    public static float EaseInOutElastic(float start, float end, float value)
    {
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s;
        float a = 0;

        if (value == 0) return start;

        if (( value /= d * 0.5f ) == 2) return start + end;

        if (a == 0f || a < Mathf.Abs( end ))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / ( 2 * Mathf.PI ) * Mathf.Asin( end / a );
        }

        if (value < 1) return -0.5f * ( a * Mathf.Pow( 2, 10 * ( value -= 1 ) ) * Mathf.Sin( ( value * d - s ) * ( 2 * Mathf.PI ) / p ) ) + start;
        return a * Mathf.Pow( 2, -10 * ( value -= 1 ) ) * Mathf.Sin( ( value * d - s ) * ( 2 * Mathf.PI ) / p ) * 0.5f + end + start;
    }

    //
    // These are derived functions that the motor can use to get the speed at a specific time.
    //
    // The easing functions all work with a normalized time (0 to 1) and the returned value here
    // reflects that. Values returned here should be divided by the actual time.
    //
    // TODO: These functions have not had the testing they deserve. If there is odd behavior around
    //       dash speeds then this would be the first place I'd look.

    public static float LinearD(float start, float end, float value)
    {
        return end - start;
    }

    public static float EaseInQuadD(float start, float end, float value)
    {
        return 2f * ( end - start ) * value;
    }

    public static float EaseOutQuadD(float start, float end, float value)
    {
        end -= start;
        return -end * value - end * ( value - 2 );
    }

    public static float EaseInOutQuadD(float start, float end, float value)
    {
        value /= .5f;
        end -= start;

        if (value < 1)
        {
            return end * value;
        }

        value--;

        return end * ( 1 - value );
    }

    public static float EaseInCubicD(float start, float end, float value)
    {
        return 3f * ( end - start ) * value * value;
    }

    public static float EaseOutCubicD(float start, float end, float value)
    {
        value--;
        end -= start;
        return 3f * end * value * value;
    }

    public static float EaseInOutCubicD(float start, float end, float value)
    {
        value /= .5f;
        end -= start;

        if (value < 1)
        {
            return ( 3f / 2f ) * end * value * value;
        }

        value -= 2;

        return ( 3f / 2f ) * end * value * value;
    }

    public static float EaseInQuartD(float start, float end, float value)
    {
        return 4f * ( end - start ) * value * value * value;
    }

    public static float EaseOutQuartD(float start, float end, float value)
    {
        value--;
        end -= start;
        return -4f * end * value * value * value;
    }

    public static float EaseInOutQuartD(float start, float end, float value)
    {
        value /= .5f;
        end -= start;

        if (value < 1)
        {
            return 2f * end * value * value * value;
        }

        value -= 2;

        return -2f * end * value * value * value;
    }

    public static float EaseInQuintD(float start, float end, float value)
    {
        return 5f * ( end - start ) * value * value * value * value;
    }

    public static float EaseOutQuintD(float start, float end, float value)
    {
        value--;
        end -= start;
        return 5f * end * value * value * value * value;
    }

    public static float EaseInOutQuintD(float start, float end, float value)
    {
        value /= .5f;
        end -= start;

        if (value < 1)
        {
            return ( 5f / 2f ) * end * value * value * value * value;
        }

        value -= 2;

        return ( 5f / 2f ) * end * value * value * value * value;
    }

    public static float EaseInSineD(float start, float end, float value)
    {
        return ( end - start ) * 0.5f * Mathf.PI * Mathf.Sin( 0.5f * Mathf.PI * value );
    }

    public static float EaseOutSineD(float start, float end, float value)
    {
        end -= start;
        return ( Mathf.PI * 0.5f ) * end * Mathf.Cos( value * ( Mathf.PI * 0.5f ) );
    }

    public static float EaseInOutSineD(float start, float end, float value)
    {
        end -= start;
        return end * 0.5f * Mathf.PI * Mathf.Sin( Mathf.PI * value );
    }
    public static float EaseInExpoD(float start, float end, float value)
    {
        return ( 10f * NATURAL_LOG_OF_2 * ( end - start ) * Mathf.Pow( 2f, 10f * ( value - 1 ) ) );
    }

    public static float EaseOutExpoD(float start, float end, float value)
    {
        end -= start;
        return 5f * NATURAL_LOG_OF_2 * end * Mathf.Pow( 2f, 1f - 10f * value );
    }

    public static float EaseInOutExpoD(float start, float end, float value)
    {
        value /= .5f;
        end -= start;

        if (value < 1)
        {
            return 5f * NATURAL_LOG_OF_2 * end * Mathf.Pow( 2f, 10f * ( value - 1 ) );
        }

        value--;

        return ( 5f * NATURAL_LOG_OF_2 * end ) / ( Mathf.Pow( 2f, 10f * value ) );
    }

    public static float EaseInCircD(float start, float end, float value)
    {
        return ( ( end - start ) * value ) / Mathf.Sqrt( 1f - value * value );
    }

    public static float EaseOutCircD(float start, float end, float value)
    {
        value--;
        end -= start;
        return ( -end * value ) / Mathf.Sqrt( 1f - value * value );
    }

    public static float EaseInOutCircD(float start, float end, float value)
    {
        value /= .5f;
        end -= start;

        if (value < 1)
        {
            return ( end * value ) / ( 2f * Mathf.Sqrt( 1f - value * value ) );
        }

        value -= 2;

        return ( -end * value ) / ( 2f * Mathf.Sqrt( 1f - value * value ) );
    }

    public static float EaseInBounceD(float start, float end, float value)
    {
        end -= start;
        float d = 1f;

        return EaseOutBounceD( 0, end, d - value );
    }

    public static float EaseOutBounceD(float start, float end, float value)
    {
        value /= 1f;
        end -= start;

        if (value < ( 1 / 2.75f ))
        {
            return 2f * end * 7.5625f * value;
        }
        else if (value < ( 2 / 2.75f ))
        {
            value -= ( 1.5f / 2.75f );
            return 2f * end * 7.5625f * value;
        }
        else if (value < ( 2.5 / 2.75 ))
        {
            value -= ( 2.25f / 2.75f );
            return 2f * end * 7.5625f * value;
        }
        else
        {
            value -= ( 2.625f / 2.75f );
            return 2f * end * 7.5625f * value;
        }
    }

    public static float EaseInOutBounceD(float start, float end, float value)
    {
        end -= start;
        float d = 1f;

        if (value < d * 0.5f)
        {
            return EaseInBounceD( 0, end, value * 2 ) * 0.5f;
        }
        else
        {
            return EaseOutBounceD( 0, end, value * 2 - d ) * 0.5f;
        }
    }

    public static float EaseInBackD(float start, float end, float value)
    {
        float s = 1.70158f;

        return 3f * ( s + 1f ) * ( end - start ) * value * value - 2f * s * ( end - start ) * value;
    }

    public static float EaseOutBackD(float start, float end, float value)
    {
        float s = 1.70158f;
        end -= start;
        value = ( value ) - 1;

        return end * ( ( s + 1f ) * value * value + 2f * value * ( ( s + 1f ) * value + s ) );
    }

    public static float EaseInOutBackD(float start, float end, float value)
    {
        float s = 1.70158f;
        end -= start;
        value /= .5f;

        if (( value ) < 1)
        {
            s *= ( 1.525f );
            return 0.5f * end * ( s + 1 ) * value * value + end * value * ( ( s + 1f ) * value - s );
        }

        value -= 2;
        s *= ( 1.525f );
        return 0.5f * end * ( ( s + 1 ) * value * value + 2f * value * ( ( s + 1f ) * value + s ) );
    }

    public static float EaseInElasticD(float start, float end, float value)
    {
        return EaseOutElasticD( start, end, 1f - value );
    }

    public static float EaseOutElasticD(float start, float end, float value)
    {
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s;
        float a = 0;

        if (a == 0f || a < Mathf.Abs( end ))
        {
            a = end;
            s = p * 0.25f;
        }
        else
        {
            s = p / ( 2 * Mathf.PI ) * Mathf.Asin( end / a );
        }

        return ( a * Mathf.PI * d * Mathf.Pow( 2f, 1f - 10f * value ) *
            Mathf.Cos( ( 2f * Mathf.PI * ( d * value - s ) ) / p ) ) / p - 5f * NATURAL_LOG_OF_2 * a *
            Mathf.Pow( 2f, 1f - 10f * value ) * Mathf.Sin( ( 2f * Mathf.PI * ( d * value - s ) ) / p );
    }

    public static float EaseInOutElasticD(float start, float end, float value)
    {
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s;
        float a = 0;

        if (a == 0f || a < Mathf.Abs( end ))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / ( 2 * Mathf.PI ) * Mathf.Asin( end / a );
        }

        if (value < 1)
        {
            value -= 1;

            return -5f * NATURAL_LOG_OF_2 * a * Mathf.Pow( 2f, 10f * value ) * Mathf.Sin( 2 * Mathf.PI * ( d * value - 2f ) / p ) -
                a * Mathf.PI * d * Mathf.Pow( 2f, 10f * value ) * Mathf.Cos( 2 * Mathf.PI * ( d * value - s ) / p ) / p;
        }

        value -= 1;

        return a * Mathf.PI * d * Mathf.Cos( 2f * Mathf.PI * ( d * value - s ) / p ) / ( p * Mathf.Pow( 2f, 10f * value ) ) -
            5f * NATURAL_LOG_OF_2 * a * Mathf.Sin( 2f * Mathf.PI * ( d * value - s ) / p ) / ( Mathf.Pow( 2f, 10f * value ) );
    }

    public static float SpringD(float start, float end, float value)
    {
        value = Mathf.Clamp01(value);
        float delta = end - start;

        // 提取公用部分
        float oneMinusValue = 1f - value;
        float power1 = Mathf.Pow(oneMinusValue, 1.2f);
        float power2 = Mathf.Pow(oneMinusValue, 2.2f);
        float sinPart = Mathf.Sin(Mathf.PI * value * (2.5f * value * value * value + 0.2f));
        float cosPart = Mathf.Cos(Mathf.PI * value * (2.5f * value * value * value + 0.2f));
        float factor = 2.5f * value * value * value + 0.2f;

        // 改进后的公式
        return delta * ((6f * oneMinusValue / 5f + 1f) * 
            (-2.2f * power1 * sinPart + power2 * (Mathf.PI * factor + 7.5f * Mathf.PI * value * value * value) * cosPart + 1f) -
            6f * power2 * sinPart + value / 5f);
    }

    public delegate float Function(float s, float e, float v);

    /// <summary>
    /// Returns the function associated to the easingFunction enum. This value returned should be cached as it allocates memory
    /// to return.
    /// </summary>
    /// <param name="easingFunction">The enum associated with the easing function.</param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="value"></param>
    /// <returns>The easing function</returns>
    public static float GetEasingFunction(Ease easingFunction, float start, float end, float value)
    {
        if (easingFunction == Ease.EaseInQuad)
        {
            return EaseInQuad(start, end, value);
        }

        if (easingFunction == Ease.EaseOutQuad)
        {
            return EaseOutQuad(start, end, value);
        }

        if (easingFunction == Ease.EaseInOutQuad)
        {
            return EaseInOutQuad(start, end, value);
        }

        if (easingFunction == Ease.EaseInCubic)
        {
            return EaseInCubic(start, end, value);
        }

        if (easingFunction == Ease.EaseOutCubic)
        {
            return EaseOutCubic(start, end, value);
        }

        if (easingFunction == Ease.EaseInOutCubic)
        {
            return EaseInOutCubic(start, end, value);
        }

        if (easingFunction == Ease.EaseInQuart)
        {
            return EaseInQuart(start, end, value);
        }

        if (easingFunction == Ease.EaseOutQuart)
        {
            return EaseOutQuart(start, end, value);
        }

        if (easingFunction == Ease.EaseInOutQuart)
        {
            return EaseInOutQuart(start, end, value);
        }

        if (easingFunction == Ease.EaseInQuint)
        {
            return EaseInQuint(start, end, value);
        }

        if (easingFunction == Ease.EaseOutQuint)
        {
            return EaseOutQuint(start, end, value);
        }

        if (easingFunction == Ease.EaseInOutQuint)
        {
            return EaseInOutQuint(start, end, value);
        }

        if (easingFunction == Ease.EaseInSine)
        {
            return EaseInSine(start, end, value);
        }

        if (easingFunction == Ease.EaseOutSine)
        {
            return EaseOutSine(start, end, value);
        }

        if (easingFunction == Ease.EaseInOutSine)
        {
            return EaseInOutSine(start, end, value);
        }

        if (easingFunction == Ease.EaseInExpo)
        {
            return EaseInExpo(start, end, value);
        }

        if (easingFunction == Ease.EaseOutExpo)
        {
            return EaseOutExpo(start, end, value);
        }

        if (easingFunction == Ease.EaseInOutExpo)
        {
            return EaseInOutExpo(start, end, value);
        }

        if (easingFunction == Ease.EaseInCirc)
        {
            return EaseInCirc(start, end, value);
        }

        if (easingFunction == Ease.EaseOutCirc)
        {
            return EaseOutCirc(start, end, value);
        }

        if (easingFunction == Ease.EaseInOutCirc)
        {
            return EaseInOutCirc(start, end, value);
        }

        if (easingFunction == Ease.Linear)
        {
            return Linear(start, end, value);
        }

        if (easingFunction == Ease.Spring)
        {
            return Spring(start, end, value);
        }

        if (easingFunction == Ease.EaseInBounce)
        {
            return EaseInBounce(start, end, value);
        }

        if (easingFunction == Ease.EaseOutBounce)
        {
            return EaseOutBounce(start, end, value);
        }

        if (easingFunction == Ease.EaseInOutBounce)
        {
            return EaseInOutBounce(start, end, value);
        }

        if (easingFunction == Ease.EaseInBack)
        {
            return EaseInBack(start, end, value);
        }

        if (easingFunction == Ease.EaseOutBack)
        {
            return EaseOutBack(start, end, value);
        }

        if (easingFunction == Ease.EaseInOutBack)
        {
            return EaseInOutBack(start, end, value);
        }

        if (easingFunction == Ease.EaseInElastic)
        {
            return EaseInElastic(start, end, value);
        }

        if (easingFunction == Ease.EaseOutElastic)
        {
            return EaseOutElastic(start, end, value);
        }

        if (easingFunction == Ease.EaseInOutElastic)
        {
            return EaseInOutElastic(start, end, value);
        }

        return 0.0f;
    }

    /// <summary>
    /// Gets the derivative function of the appropriate easing function. If you use an easing function for position then this
    /// function can get you the speed at a given time (normalized).
    /// </summary>
    /// <param name="easingFunction"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="value"></param>
    /// <returns>The derivative function</returns>
    public static float GetEasingFunctionDerivative(Ease easingFunction, float start, float end, float value)
    {
        if (easingFunction == Ease.EaseInQuad)
        {
            return EaseInQuadD(start, end, value);
        }

        if (easingFunction == Ease.EaseOutQuad)
        {
            return EaseOutQuadD(start, end, value);
        }

        if (easingFunction == Ease.EaseInOutQuad)
        {
            return EaseInOutQuadD(start, end, value);
        }

        if (easingFunction == Ease.EaseInCubic)
        {
            return EaseInCubicD(start, end, value);
        }

        if (easingFunction == Ease.EaseOutCubic)
        {
            return EaseOutCubicD(start, end, value);
        }

        if (easingFunction == Ease.EaseInOutCubic)
        {
            return EaseInOutCubicD(start, end, value);
        }

        if (easingFunction == Ease.EaseInQuart)
        {
            return EaseInQuartD(start, end, value);
        }

        if (easingFunction == Ease.EaseOutQuart)
        {
            return EaseOutQuartD(start, end, value);
        }

        if (easingFunction == Ease.EaseInOutQuart)
        {
            return EaseInOutQuartD(start, end, value);
        }

        if (easingFunction == Ease.EaseInQuint)
        {
            return EaseInQuintD(start, end, value);
        }

        if (easingFunction == Ease.EaseOutQuint)
        {
            return EaseOutQuintD(start, end, value);
        }

        if (easingFunction == Ease.EaseInOutQuint)
        {
            return EaseInOutQuintD(start, end, value);
        }

        if (easingFunction == Ease.EaseInSine)
        {
            return EaseInSineD(start, end, value);
        }

        if (easingFunction == Ease.EaseOutSine)
        {
            return EaseOutSineD(start, end, value);
        }

        if (easingFunction == Ease.EaseInOutSine)
        {
            return EaseInOutSineD(start, end, value);
        }

        if (easingFunction == Ease.EaseInExpo)
        {
            return EaseInExpoD(start, end, value);
        }

        if (easingFunction == Ease.EaseOutExpo)
        {
            return EaseOutExpoD(start, end, value);
        }

        if (easingFunction == Ease.EaseInOutExpo)
        {
            return EaseInOutExpoD(start, end, value);
        }

        if (easingFunction == Ease.EaseInCirc)
        {
            return EaseInCircD(start, end, value);
        }

        if (easingFunction == Ease.EaseOutCirc)
        {
            return EaseOutCircD(start, end, value);
        }

        if (easingFunction == Ease.EaseInOutCirc)
        {
            return EaseInOutCircD(start, end, value);
        }

        if (easingFunction == Ease.Linear)
        {
            return LinearD(start, end, value);
        }

        if (easingFunction == Ease.Spring)
        {
            return SpringD(start, end, value);
        }

        if (easingFunction == Ease.EaseInBounce)
        {
            return EaseInBounceD(start, end, value);
        }

        if (easingFunction == Ease.EaseOutBounce)
        {
            return EaseOutBounceD(start, end, value);
        }

        if (easingFunction == Ease.EaseInOutBounce)
        {
            return EaseInOutBounceD(start, end, value);
        }

        if (easingFunction == Ease.EaseInBack)
        {
            return EaseInBackD(start, end, value);
        }

        if (easingFunction == Ease.EaseOutBack)
        {
            return EaseOutBackD(start, end, value);
        }

        if (easingFunction == Ease.EaseInOutBack)
        {
            return EaseInOutBackD(start, end, value);
        }

        if (easingFunction == Ease.EaseInElastic)
        {
            return EaseInElasticD(start, end, value);
        }

        if (easingFunction == Ease.EaseOutElastic)
        {
            return EaseOutElasticD(start, end, value);
        }

        if (easingFunction == Ease.EaseInOutElastic)
        {
            return EaseInOutElasticD(start, end, value);
        }

        return 0.0f;
    }

    public static float ChangeFloat(float start, float end, float value, Ease ease)
    {
        return GetEasingFunction(ease, start, end, value);
    }

    public static void ChangeVector(Vector3 start , Vector3 end , float value, Ease ease, ref Vector3 outValue)
    {
        float x = GetEasingFunction(ease, start.x, end.x, value);
        float y = GetEasingFunction(ease, start.y, end.y, value);
        float z = GetEasingFunction(ease, start.z, end.z, value);
        outValue.x = x;
        outValue.y = y;
        outValue.z = z;
    }

    public static void ChangeVector(Vector2 start, Vector2 end, float value, Ease ease, ref Vector2 outValue)
    {
        float x = GetEasingFunction(ease, start.x, end.x, value);
        float y = GetEasingFunction(ease, start.y, end.y, value);
        outValue.x = x;
        outValue.y = y;
    }
}