Shader "Unlit/Tessellation"
{
    Properties
    {
        [MainTexture] _MainTex ("MainTex", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        
        [MainTexture]_DispTex ("Disp Texture", 2D) = "gray" { }
        [MainTexture]_NormalMap ("Normalmap", 2D) = "bump" { }
        _SpecColor ("Spec color", color) = (0.5, 0.5, 0.5, 0.5)
        _MinDist ("Min Distance", Range(0.1, 50)) = 10
        _MaxDist ("Max Distance", Range(0.1, 50)) = 25
        _TessFactor ("Tessellation", Range(1, 50)) = 10
        _Displacement ("Displacement", Range(0, 1.0)) = 0.3
        
        [HideInInspector] _Cull("__cull", Float) = 2.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 200

        Pass
        {
            Name "ForwardVTX_Test"
            Tags
            {  "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma require tessellation
            #pragma fragment frag
            #pragma vertex TessellationVertexProgram
            #pragma hull hullExec
            #pragma domain domainExec
            #pragma target 5.0

            CBUFFER_START(UnityPerMaterial)
            sampler2D _MainTex;
            sampler2D _DispTex;
            sampler2D _NormalMap;
            float _Displacement;
            float _TessFactor;
            float _MinDist;
            float _MaxDist;
            half _Glossiness;
            half _Metallic;
            half4 _Color;
            CBUFFER_END
            
            
            struct Input
            {
                float2 uv_MainTex;
            };
            
            struct Attributes
            {
                float4 vertex   : POSITION;
                half3 normal      : NORMAL;
                float2 uv           : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct Varyings
            {
                float4 color : COLOR;
                float3 normal : NORMAL;
                float2 uv                       : TEXCOORD0;
                float4 positionCS               : POSITION;
                float3 lightTS                  : TEXCOORD3; // light Direction in tangent space
            };
            struct TessellationFactors
            {
                float edge[3] : SV_TessFactor;
                float inside : SV_InsideTessFactor;
            };

            struct ControlPoint
            {
                float4 vertex : INTERNALTESSPOS;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float3 normal : NORMAL;
            };

            float CalcDistanceTessFactor(float4 vertex, float minDist, float maxDist, float tess)
            {
                float dist = 0.;
                float f = clamp(1.0 - ( dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
                return (f);
            }
            
            [domain("tri")]
            [partitioning("integer")]
            [outputcontrolpoints(3)]
            [outputtopology("triangle_cw")]
            [patchconstantfunc("patchConstantFunction")]
            ControlPoint hullExec(InputPatch<ControlPoint, 3> patch, uint id : SV_OutputControlPointID)
            {
                return patch[id];
            }

            //これがパッチの設定
            TessellationFactors patchConstantFunction(const InputPatch<ControlPoint, 3> patch)
            {
                
                TessellationFactors f;
                
                float edge0 = CalcDistanceTessFactor(patch[0].vertex, _MinDist, _MaxDist, _TessFactor);
                float edge1 = CalcDistanceTessFactor(patch[1].vertex, _MinDist, _MaxDist, _TessFactor);
                float edge2 = CalcDistanceTessFactor(patch[2].vertex, _MinDist, _MaxDist, _TessFactor);

                
                f.edge[0] = (edge1 + edge2) / 2;
                f.edge[1] = (edge2 + edge0) / 2;
                f.edge[2] = (edge0 + edge1) / 2;
                f.inside = (edge0 + edge1 + edge2) / 3;
                
                return f;
            }


            //フラグメントシェーダーにわたす前の最後の頂点処理
            Varyings vert(Attributes input)
            {

                Varyings output;
                //凹みを適用させる処理-----------------------------------------------------------------------------
                const float d = tex2Dlod(_DispTex, float4(1 - input.uv.x, input.uv.y, 0, 0)).r * _Displacement; 
                input.vertex.xyz += input.normal * (1 - d);
                //----------------------------------------------------------------------------------------------
                
                output.positionCS = TransformObjectToHClip(input.vertex.xyz);   //Vertの反映
                output.normal = input.normal;
                
                output.uv = input.uv;   //UVテクスチャ反映
                output.color = input.color; //色反映
                
                VertexNormalInputs vertex_normal_input = GetVertexNormalInputs(input.normal, input.color);
                const Light main_light = GetMainLight();
                const float3x3 tangent_mat = float3x3(vertex_normal_input.tangentWS, vertex_normal_input.bitangentWS, vertex_normal_input.normalWS);
                output.lightTS = mul(tangent_mat, main_light.direction);
                return output;

            }

            [domain("tri")]
            Varyings domainExec(TessellationFactors factors, OutputPatch<ControlPoint, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
            {
                Attributes v;
         
                #define DomainPos(fieldName) v.fieldName = \
                        patch[0].fieldName * barycentricCoordinates.x + \
                        patch[1].fieldName * barycentricCoordinates.y + \
                        patch[2].fieldName * barycentricCoordinates.z;
         
                    DomainPos(vertex)
                    DomainPos(uv)
                    DomainPos(color)
                    DomainPos(normal)
         
                    return vert(v);
            }
            
            ControlPoint TessellationVertexProgram(Attributes v)
            {   
                ControlPoint p;
         
                p.vertex = v.vertex;
                p.uv = v.uv;
                p.normal = v.normal;
                p.color = v.color;
         
                return p;
            }

            
            half4 frag (const Varyings input) : SV_Target
            {
                half4 c = tex2D (_MainTex, input.uv);
                
                
                const float3 normal = UnpackNormal(tex2D(_NormalMap, input.uv));
                const float diff = saturate(dot(input.lightTS, normal));
                
                c *= diff;
                return c;
            }
            
            ENDHLSL
        }

    }
    FallBack "Diffuse"
}

