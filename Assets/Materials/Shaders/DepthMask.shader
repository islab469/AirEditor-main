Shader "Custom/DepthMask"
{
    SubShader
    {
        Tags { "Queue" = "Geometry-1" }
        Pass
        {
            Cull Off       // 雙面都遮
            ColorMask 0
            ZWrite On
            ZTest LEqual
        }
    }
}
