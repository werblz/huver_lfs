// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:True,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:6071,x:32951,y:32669,varname:node_6071,prsc:2|emission-2304-OUT;n:type:ShaderForge.SFN_Tex2d,id:7559,x:32054,y:32707,ptovrint:False,ptlb:Additive,ptin:_Additive,varname:node_7559,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-9806-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:9850,x:32130,y:32945,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_9850,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ee7cd554c5024d940a3ebfea0f1953fc,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1503,x:32361,y:32776,varname:node_1503,prsc:2|A-7559-RGB,B-9850-RGB;n:type:ShaderForge.SFN_TexCoord,id:4005,x:31277,y:32632,varname:node_4005,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:4882,x:31739,y:32666,varname:node_4882,prsc:2,spu:1,spv:0|UVIN-4005-UVOUT,DIST-2497-OUT;n:type:ShaderForge.SFN_Time,id:1195,x:31263,y:32872,varname:node_1195,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2497,x:31489,y:32932,varname:node_2497,prsc:2|A-1195-T,B-3502-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3502,x:31263,y:33088,ptovrint:False,ptlb:U Speed,ptin:_USpeed,varname:node_3502,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:2436,x:31263,y:33216,ptovrint:False,ptlb:V Speed,ptin:_VSpeed,varname:_ScrollSpeed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:3430,x:31507,y:33089,varname:node_3430,prsc:2|A-1195-T,B-2436-OUT;n:type:ShaderForge.SFN_Panner,id:9806,x:31756,y:32904,varname:node_9806,prsc:2,spu:0,spv:1|UVIN-4882-UVOUT,DIST-3430-OUT;n:type:ShaderForge.SFN_Color,id:7133,x:32043,y:32509,ptovrint:False,ptlb:Diffuse_Color,ptin:_Diffuse_Color,varname:node_7133,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:6122,x:32631,y:32553,varname:node_6122,prsc:2|A-7133-RGB,B-1503-OUT;n:type:ShaderForge.SFN_Fresnel,id:2034,x:32159,y:32290,varname:node_2034,prsc:2|EXP-4753-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4753,x:31949,y:32290,ptovrint:False,ptlb:Fresnel Amount,ptin:_FresnelAmount,varname:node_4753,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:2304,x:32849,y:32371,varname:node_2304,prsc:2|A-6726-OUT,B-6122-OUT,C-1098-OUT;n:type:ShaderForge.SFN_OneMinus,id:6726,x:32640,y:32177,varname:node_6726,prsc:2|IN-514-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1098,x:32411,y:32463,ptovrint:False,ptlb:Fresnel Intensity,ptin:_FresnelIntensity,varname:node_1098,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:7242,x:32177,y:32170,ptovrint:False,ptlb:Fresnel Center,ptin:_FresnelCenter,varname:node_7242,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:514,x:32386,y:32136,varname:node_514,prsc:2|A-7242-OUT,B-2034-OUT;proporder:7559-9850-3502-2436-7133-4753-1098-7242;pass:END;sub:END;*/

Shader "Custom/Additive_Diffuse_Scroll_Overall_Alpha_Fresnel" {
    Properties {
        _Additive ("Additive", 2D) = "white" {}
        _Alpha ("Alpha", 2D) = "white" {}
        _USpeed ("U Speed", Float ) = 0
        _VSpeed ("V Speed", Float ) = 0
        _Diffuse_Color ("Diffuse_Color", Color) = (0.5,0.5,0.5,1)
        _FresnelAmount ("Fresnel Amount", Float ) = 1
        _FresnelIntensity ("Fresnel Intensity", Float ) = 1
        _FresnelCenter ("Fresnel Center", Float ) = 1
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 
            #pragma target 3.0
            uniform sampler2D _Additive; uniform float4 _Additive_ST;
            uniform sampler2D _Alpha; uniform float4 _Alpha_ST;
            uniform float _USpeed;
            uniform float _VSpeed;
            uniform float4 _Diffuse_Color;
            uniform float _FresnelAmount;
            uniform float _FresnelIntensity;
            uniform float _FresnelCenter;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 node_1195 = _Time;
                float2 node_9806 = ((i.uv0+(node_1195.g*_USpeed)*float2(1,0))+(node_1195.g*_VSpeed)*float2(0,1));
                float4 _Additive_var = tex2D(_Additive,TRANSFORM_TEX(node_9806, _Additive));
                float4 _Alpha_var = tex2D(_Alpha,TRANSFORM_TEX(i.uv0, _Alpha));
                float3 emissive = ((1.0 - (_FresnelCenter*pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelAmount)))*(_Diffuse_Color.rgb*(_Additive_var.rgb*_Alpha_var.rgb))*_FresnelIntensity);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
