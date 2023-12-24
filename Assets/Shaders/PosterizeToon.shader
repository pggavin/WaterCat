Shader "Custom/ColoredToonShading" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _ColorRamp("Color Ramp", Range(2, 8)) = 4
        _Color1("Color 1", Color) = (1,0,0,1)
        _Color2("Color 2", Color) = (0,1,0,1)
        _Color3("Color 3", Color) = (0,0,1,1)
    }

        SubShader{
            Tags {"Queue" = "Transparent" "RenderType" = "Opaque"}
            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normal : NORMAL;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float3 worldNormal : TEXCOORD1;
                    float4 vertex : SV_POSITION;
                    float3 worldPos : TEXCOORD2;
                };

                sampler2D _MainTex;
                float _ColorRamp;
                float3 _Color1;
                float3 _Color2;
                float3 _Color3;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.worldNormal = UnityObjectToWorldNormal(v.normal);
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    // Step 1: Calculate the lighting
                    float3 worldNormal = normalize(i.worldNormal);
                    float3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                    float diffuse = dot(worldNormal, worldLightDir);

                    // Step 2: Posterize the lighting
                    float posterize = floor(diffuse * (_ColorRamp - 1.0)) / (_ColorRamp - 1.0);

                    // Step 3: Calculate the color
                    float3 color = _Color1;
                    if (posterize > 0.33) {
                        color = lerp(_Color1, _Color2, (posterize - 0.33) / 0.33);
                    }
                    if (posterize > 0.66) {
                        color = lerp(_Color2, _Color3, (posterize - 0.66) / 0.33);
                    }

                    // Step 4: Output final color
                    return tex2D(_MainTex, i.uv) * float4(color, 1.0);
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}