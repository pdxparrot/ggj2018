// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_VFX_ImmunityBall"
{
	Properties
	{
		[Header(Refraction)]
		_NoiseTexture1("Noise Texture 1", 2D) = "white" {}
		_NoiseTexture2("Noise Texture 2", 2D) = "white" {}
		_ChromaticAberration("Chromatic Aberration", Range( 0 , 0.3)) = 0.1
		_Noise2HorzontalSpeed("Noise2 Horzontal Speed", Float) = 0
		_NoiseStrength("Noise Strength", Float) = 0
		_Noise1HorzontalSpeed("Noise1 Horzontal Speed", Float) = 0
		_Noise2VerticalSpeed("Noise2 Vertical Speed", Float) = 0
		_NoiseSize("Noise Size", Float) = 1
		_Noise1VerticalSpeed("Noise1 Vertical Speed", Float) = 0
		_MainColor("Main Color", Color) = (0.5294118,0.5294118,0.5294118,0)
		_SecondaryColor("Secondary Color", Color) = (0.5294118,0.5294118,0.5294118,0)
		_Refraction("Refraction", Float) = 1.1
		_Emissive("Emissive", Float) = 1
		_Opacity("Opacity", Range( 0 , 1)) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile _ALPHAPREMULTIPLY_ON
		#pragma surface surf Standard alpha:fade keepalpha finalcolor:RefractionF noshadow exclude_path:deferred noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
			float3 worldPos;
		};

		uniform float4 _MainColor;
		uniform float _Emissive;
		uniform float4 _SecondaryColor;
		uniform sampler2D _NoiseTexture1;
		uniform float _Noise1HorzontalSpeed;
		uniform float _NoiseSize;
		uniform float _Noise1VerticalSpeed;
		uniform sampler2D _NoiseTexture2;
		uniform float _Noise2HorzontalSpeed;
		uniform float _Noise2VerticalSpeed;
		uniform float _NoiseStrength;
		uniform float _Opacity;
		uniform sampler2D _GrabTexture;
		uniform float _ChromaticAberration;
		uniform float _Refraction;

		inline float4 Refraction( Input i, SurfaceOutputStandard o, float indexOfRefraction, float chomaticAberration ) {
			float3 worldNormal = o.Normal;
			float4 screenPos = i.screenPos;
			#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
			#else
				float scale = 1.0;
			#endif
			float halfPosW = screenPos.w * 0.5;
			screenPos.y = ( screenPos.y - halfPosW ) * _ProjectionParams.x * scale + halfPosW;
			#if SHADER_API_D3D9 || SHADER_API_D3D11
				screenPos.w += 0.00000000001;
			#endif
			float2 projScreenPos = ( screenPos / screenPos.w ).xy;
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float3 refractionOffset = ( ( ( ( indexOfRefraction - 1.0 ) * mul( UNITY_MATRIX_V, float4( worldNormal, 0.0 ) ) ) * ( 1.0 / ( screenPos.z + 1.0 ) ) ) * ( 1.0 - dot( worldNormal, worldViewDir ) ) );
			float2 cameraRefraction = float2( refractionOffset.x, -( refractionOffset.y * _ProjectionParams.x ) );
			float4 redAlpha = tex2D( _GrabTexture, ( projScreenPos + cameraRefraction ) );
			float green = tex2D( _GrabTexture, ( projScreenPos + ( cameraRefraction * ( 1.0 - chomaticAberration ) ) ) ).g;
			float blue = tex2D( _GrabTexture, ( projScreenPos + ( cameraRefraction * ( 1.0 + chomaticAberration ) ) ) ).b;
			return float4( redAlpha.r, green, blue, redAlpha.a );
		}

		void RefractionF( Input i, SurfaceOutputStandard o, inout fixed4 color )
		{
			#ifdef UNITY_PASS_FORWARDBASE
			color.rgb = color.rgb + Refraction( i, o, _Refraction, _ChromaticAberration ) * ( 1 - color.a );
			color.a = 1;
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float4 appendResult44 = (float4(_Emissive , _Emissive , _Emissive , 0.0));
			float2 temp_cast_0 = (_Noise1HorzontalSpeed).xx;
			float2 uv_TexCoord18 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float2 temp_output_19_0 = ( uv_TexCoord18 * _NoiseSize );
			float2 panner21 = ( temp_output_19_0 + 1.0 * _Time.y * temp_cast_0);
			float2 temp_cast_1 = (_Noise1VerticalSpeed).xx;
			float2 panner22 = ( temp_output_19_0 + 1.0 * _Time.y * temp_cast_1);
			float2 appendResult7 = (float2((panner21).x , (panner22).y));
			float2 temp_cast_2 = (_Noise2HorzontalSpeed).xx;
			float2 panner23 = ( temp_output_19_0 + 1.0 * _Time.y * temp_cast_2);
			float2 temp_cast_3 = (_Noise2VerticalSpeed).xx;
			float2 panner24 = ( temp_output_19_0 + 1.0 * _Time.y * temp_cast_3);
			float2 appendResult8 = (float2((panner23).x , (panner24).y));
			float4 lerpResult48 = lerp( ( _MainColor * appendResult44 ) , ( appendResult44 * _SecondaryColor ) , saturate( ( ( (tex2D( _NoiseTexture1, appendResult7 )).r + (tex2D( _NoiseTexture2, appendResult8 )).g ) * _NoiseStrength ) ));
			o.Emission = lerpResult48.rgb;
			o.Alpha = _Opacity;
			o.Normal = o.Normal + 0.00001 * i.screenPos * i.worldPos;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14301
586;91;578;656;790.6652;334.9977;2.040517;True;False
Node;AmplifyShaderEditor.RangedFloatNode;20;-2620.981,68.6971;Float;False;Property;_NoiseSize;Noise Size;6;0;Create;True;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;18;-2691.196,-86.44567;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-2421.035,-8.874304;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-2483.98,575.0955;Float;False;Property;_Noise2HorzontalSpeed;Noise2 Horzontal Speed;2;0;Create;True;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-2495.654,398.0395;Float;False;Property;_Noise1VerticalSpeed;Noise1 Vertical Speed;7;0;Create;True;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-2509.597,301.3394;Float;False;Property;_Noise1HorzontalSpeed;Noise1 Horzontal Speed;4;0;Create;True;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-2479.231,710.4635;Float;False;Property;_Noise2VerticalSpeed;Noise2 Vertical Speed;5;0;Create;True;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;24;-2174.945,691.9435;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;22;-2172.271,427.1308;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;23;-2172.27,563.5498;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;21;-2169.595,296.062;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;28;-1987.704,424.456;Float;False;False;True;False;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;27;-1990.379,683.9191;Float;False;False;True;False;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;25;-1987.704,296.062;Float;False;True;False;False;False;1;0;FLOAT2;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;26;-1990.379,558.1998;Float;False;True;False;False;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;2;-1907.951,69.13921;Float;True;Property;_NoiseTexture1;Noise Texture 1;0;0;Create;True;None;None;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.DynamicAppendNode;8;-1746.451,586.1852;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;7;-1751.201,343.9478;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;42;-1818.999,813.785;Float;True;Property;_NoiseTexture2;Noise Texture 2;1;0;Create;True;None;None;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;3;-1528.204,317.7079;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;-1517.396,560.0033;Float;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;4;-1159.859,322.5739;Float;False;True;False;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;6;-1165.674,562.4946;Float;False;False;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-870.9413,568.1627;Float;False;Property;_NoiseStrength;Noise Strength;3;0;Create;True;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;29;-857.3871,326.9643;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-866.7544,-44.97068;Float;False;Property;_Emissive;Emissive;11;0;Create;True;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;44;-688.6175,-65.17175;Float;False;COLOR;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;1;-779.154,-263.3412;Float;False;Property;_MainColor;Main Color;8;0;Create;True;0.5294118,0.5294118,0.5294118,0;0.5294118,0.5294118,0.5294118,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;46;-781.0912,99.98512;Float;False;Property;_SecondaryColor;Secondary Color;9;0;Create;True;0.5294118,0.5294118,0.5294118,0;0.5294118,0.5294118,0.5294118,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-697.1022,329.3217;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-476.1096,14.30438;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-481.103,-163.2501;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;41;-534.6823,330.6568;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;48;-276.598,49.92986;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-182.2975,391.3128;Float;False;Property;_Refraction;Refraction;10;0;Create;True;1.1;1.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-170.9025,486.904;Float;False;Property;_Opacity;Opacity;12;0;Create;True;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;142.6359,218.4657;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SH_VFX_ImmunityBall;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;True;False;Back;0;0;False;0;0;False;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;1;255;255;6;2;1;0;0;0;0;0;False;2;15;10;25;False;0.5;False;2;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;False;False;Cylindrical;False;Relative;0;;-1;-1;0;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;19;0;18;0
WireConnection;19;1;20;0
WireConnection;24;0;19;0
WireConnection;24;2;12;0
WireConnection;22;0;19;0
WireConnection;22;2;10;0
WireConnection;23;0;19;0
WireConnection;23;2;11;0
WireConnection;21;0;19;0
WireConnection;21;2;9;0
WireConnection;28;0;22;0
WireConnection;27;0;24;0
WireConnection;25;0;21;0
WireConnection;26;0;23;0
WireConnection;8;0;26;0
WireConnection;8;1;27;0
WireConnection;7;0;25;0
WireConnection;7;1;28;0
WireConnection;3;0;2;0
WireConnection;3;1;7;0
WireConnection;5;0;42;0
WireConnection;5;1;8;0
WireConnection;4;0;3;0
WireConnection;6;0;5;0
WireConnection;29;0;4;0
WireConnection;29;1;6;0
WireConnection;44;0;45;0
WireConnection;44;1;45;0
WireConnection;44;2;45;0
WireConnection;30;0;29;0
WireConnection;30;1;31;0
WireConnection;47;0;44;0
WireConnection;47;1;46;0
WireConnection;34;0;1;0
WireConnection;34;1;44;0
WireConnection;41;0;30;0
WireConnection;48;0;34;0
WireConnection;48;1;47;0
WireConnection;48;2;41;0
WireConnection;0;2;48;0
WireConnection;0;8;32;0
WireConnection;0;9;49;0
ASEEND*/
//CHKSM=DA99C12618D6BCBCB1B6D6858B657CC973072C86