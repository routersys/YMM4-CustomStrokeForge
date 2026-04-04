Texture2D InputTexture : register(t0);
SamplerState InputSampler : register(s0);

cbuffer Constants : register(b0)
{
    float2 textureSize;
    float stepWidth;
    int mode;
    float4 outlineColor;
    float borderWidth;
    float softness;
    float threshold;
    float styleMode;
    float dashLen;
    float gapLen;
    float shortDashLen;
    float waveAmp;
    float waveFreq;
    float phaseOffset;
    float doubleGap;
    float positionMode;
};

float4 main(float4 pos : SV_POSITION, float4 posScene : SCENE_POSITION, float4 uv : TEXCOORD0) : SV_TARGET
{
    float2 px = uv.xy * textureSize;

    [branch]
    if (mode == 0)
    {
        float4 src = InputTexture.SampleLevel(InputSampler, uv.xy, 0);
        if (src.a >= threshold)
            return float4(px.x, px.y, 0.0, src.a);
        return float4(-1.0, -1.0, 1e10, 0.0);
    }

    [branch]
    if (mode == 1)
    {
        float bestDist = 1e10;
        float4 best = float4(-1.0, -1.0, 1e10, 0.0);

        float4 center = InputTexture.SampleLevel(InputSampler, uv.xy, 0);
        if (center.x >= 0.0)
        {
            float2 d = px - center.xy;
            bestDist = dot(d, d);
            best = center;
        }

        [unroll]
        for (int dy = -1; dy <= 1; dy++)
        {
            [unroll]
            for (int dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0)
                    continue;

                float2 nuv = uv.xy + float2((float) dx, (float) dy) * stepWidth / textureSize;
                if (nuv.x < 0.0 || nuv.x > 1.0 || nuv.y < 0.0 || nuv.y > 1.0)
                    continue;

                float4 seed = InputTexture.SampleLevel(InputSampler, nuv, 0);
                if (seed.x < 0.0)
                    continue;

                float2 d = px - seed.xy;
                float dd = dot(d, d);
                if (dd < bestDist)
                {
                    bestDist = dd;
                    best = seed;
                }
            }
        }

        best.z = bestDist;
        return best;
    }

    float4 seed = InputTexture.SampleLevel(InputSampler, uv.xy, 0);
    if (seed.x < 0.0)
        return float4(0.0, 0.0, 0.0, 0.0);

    float dist = sqrt(max(seed.z, 0.0));

    float aa = max(softness, 0.5);

    float effBorder = borderWidth;
    [branch]
    if (positionMode > 0.5 && positionMode < 1.5)
        effBorder = borderWidth * 0.5;

    float2 shapeCenter = textureSize * 0.5;
    float2 toSeed = seed.xy - shapeCenter;
    float seedAngle = atan2(toSeed.y, toSeed.x);
    float seedRadius = max(length(toSeed), 1.0);
    float pCoord = seedAngle * seedRadius + phaseOffset;

    float effDist = dist;

    [branch]
    if (styleMode > 4.5)
    {
        float wave = sin(seedAngle * waveFreq) * waveAmp;
        effDist = dist + wave;
    }

    float strokeMask = smoothstep(effBorder + aa, max(effBorder - aa, 0.0), effDist);

    [branch]
    if (positionMode < 0.5)
    {
        strokeMask *= smoothstep(0.0, 1.0, dist);
    }
    else if (positionMode > 1.5)
    {
        strokeMask *= 1.0 - smoothstep(0.0, 1.0, dist);
    }

    float pMask = 1.0;

    [branch]
    if (styleMode > 0.5 && styleMode < 1.5)
    {
        float period = max(dashLen + gapLen, 0.001);
        float t = fmod(abs(pCoord), period);
        pMask = 1.0 - smoothstep(dashLen - aa, dashLen + aa, t);
    }
    else if (styleMode > 1.5 && styleMode < 2.5)
    {
        float period = max(dashLen + gapLen, 0.001);
        float t = fmod(abs(pCoord), period);
        pMask = 1.0 - smoothstep(dashLen - aa, dashLen + aa, t);
    }
    else if (styleMode > 2.5 && styleMode < 3.5)
    {
        float period = max(dashLen + gapLen + shortDashLen + gapLen, 0.001);
        float t = fmod(abs(pCoord), period);
        float m1 = 1.0 - smoothstep(dashLen - aa, dashLen + aa, t);
        float off2 = dashLen + gapLen;
        float m2 = smoothstep(off2 - aa, off2 + aa, t) *
                    (1.0 - smoothstep(off2 + shortDashLen - aa, off2 + shortDashLen + aa, t));
        pMask = max(m1, m2);
    }
    else if (styleMode > 3.5 && styleMode < 4.5)
    {
        float lw = max((effBorder - doubleGap) * 0.5, 0.1);
        float gapEnd = lw + doubleGap;
        float gapMask = smoothstep(lw - aa, lw + aa, effDist) *
                        (1.0 - smoothstep(gapEnd - aa, gapEnd + aa, effDist));
        pMask = 1.0 - gapMask;
    }

    float finalMask = strokeMask * pMask;
    return float4(outlineColor.rgb * outlineColor.a * finalMask, outlineColor.a * finalMask);
}
