// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_ToonShader"
{
	Properties
	{
		_ASEOutlineWidth( "Outline Width", Float ) = 0
		_ASEOutlineColor( "Outline Color", Color ) = (0,0,0,0)
		_SecondaryColor("Secondary Color", Color) = (0.2867647,0.2518643,0.1602509,0)
		_PrimaryColor("Primary Color", Color) = (0.2867647,0.2518643,0.1602509,0)
		[Toggle]_Texture("Texture", Float) = 0
		_PrimaryBrightness("Primary Brightness", Range( 0 , 2)) = 1
		_SecondaryBrightness("Secondary Brightness", Range( 0 , 2)) = 1
		_DiffuseTexture("Diffuse Texture", 2D) = "white" {}
		_ColorAlpha("Color Alpha", 2D) = "white" {}
		_RampTexture("Ramp Texture", 2D) = "white" {}
		_ShadowBrightness("Shadow Brightness", Range( 0 , 1)) = 0
		_ShadowDistance("Shadow Distance", Range( -1 , 1)) = 0.6709523
		_TextureBlackpoint("Texture Blackpoint", Range( 0 , 1)) = 0
		_TextureWhitepoint("Texture Whitepoint", Range( 0 , 1)) = 1
		_TextureContrast("Texture Contrast", Range( 0 , 10)) = 0
		[Toggle]_ToonRamp("Toon Ramp", Float) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		struct Input {
			fixed filler;
		};
		uniform fixed4 _ASEOutlineColor;
		uniform fixed _ASEOutlineWidth;
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz *= ( 1 + _ASEOutlineWidth);
		}
		inline fixed4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return fixed4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _ASEOutlineColor.rgb;
			o.Alpha = 1;
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float _ToonRamp;
		uniform float _ShadowBrightness;
		uniform fixed _Texture;
		uniform float _SecondaryBrightness;
		uniform float4 _SecondaryColor;
		uniform float _PrimaryBrightness;
		uniform float4 _PrimaryColor;
		uniform sampler2D _ColorAlpha;
		uniform float4 _ColorAlpha_ST;
		uniform float _TextureContrast;
		uniform sampler2D _DiffuseTexture;
		uniform float4 _DiffuseTexture_ST;
		uniform float _TextureBlackpoint;
		uniform float _TextureWhitepoint;
		uniform float _ShadowDistance;
		uniform sampler2D _RampTexture;
		uniform float _Smoothness;


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_ColorAlpha = i.uv_texcoord * _ColorAlpha_ST.xy + _ColorAlpha_ST.zw;
			float4 lerpResult60 = lerp( ( _SecondaryBrightness * _SecondaryColor ) , ( _PrimaryBrightness * _PrimaryColor ) , tex2D( _ColorAlpha, uv_ColorAlpha ).r);
			float2 uv_DiffuseTexture = i.uv_texcoord * _DiffuseTexture_ST.xy + _DiffuseTexture_ST.zw;
			float4 temp_cast_1 = (_TextureBlackpoint).xxxx;
			float4 temp_cast_2 = (_TextureWhitepoint).xxxx;
			float4 clampResult37 = clamp( tex2D( _DiffuseTexture, uv_DiffuseTexture ) , temp_cast_1 , temp_cast_2 );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float dotResult5 = dot( ase_worldlightDir , ase_worldNormal );
			float4 lerpResult9 = lerp( ( _ShadowBrightness * lerp(lerpResult60,( lerpResult60 * saturate( CalculateContrast(_TextureContrast,clampResult37) ) ),_Texture) ) , lerp(lerpResult60,( lerpResult60 * saturate( CalculateContrast(_TextureContrast,clampResult37) ) ),_Texture) , step( _ShadowDistance , dotResult5 ));
			float2 appendResult42 = (float2(( dotResult5 - _ShadowDistance ) , 0.0));
			o.Emission = saturate( lerp(lerpResult9,( lerp(lerpResult60,( lerpResult60 * saturate( CalculateContrast(_TextureContrast,clampResult37) ) ),_Texture) * tex2D( _RampTexture, appendResult42 ) ),_ToonRamp) ).rgb;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14301
1093;91;827;927;276.9365;403.0912;1;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;19;-2755.586,58.41811;Float;True;Property;_DiffuseTexture;Diffuse Texture;5;0;Create;True;None;None;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-2481.911,273.9033;Float;False;Property;_TextureBlackpoint;Texture Blackpoint;10;0;Create;True;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;20;-2486.021,58.98884;Float;True;Property;_TextureSample0;Texture Sample 0;6;0;Create;True;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;38;-2479.674,366.3891;Float;False;Property;_TextureWhitepoint;Texture Whitepoint;11;0;Create;True;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;37;-2127.098,65.53709;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-2135.082,206.5806;Float;False;Property;_TextureContrast;Texture Contrast;12;0;Create;True;0;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;51;-1771.779,353.9199;Float;False;1299.706;704.6351;LightingStyle;10;3;4;31;5;44;42;29;28;30;53;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;55;-2346.27,-178.9023;Float;True;Property;_ColorAlpha;Color Alpha;6;0;Create;True;None;None;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.ColorNode;58;-2180.382,-697.5601;Float;False;Property;_SecondaryColor;Secondary Color;0;0;Create;True;0.2867647,0.2518643,0.1602509,0;0.3525109,0.3122297,0.5661765,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;11;-2232.098,-478.7182;Float;False;Property;_PrimaryBrightness;Primary Brightness;3;0;Create;True;1;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;-2200.308,-369.6702;Float;False;Property;_PrimaryColor;Primary Color;1;0;Create;True;0.2867647,0.2518643,0.1602509,0;0.3525109,0.3122297,0.5661765,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;57;-2173.136,-796.8491;Float;False;Property;_SecondaryBrightness;Secondary Brightness;4;0;Create;True;1;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;4;-1721.779,516.8288;Float;False;1;0;FLOAT;0.0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleContrastOpNode;54;-1824.244,65.48643;Float;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;0.0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldNormalVector;3;-1705.288,680.5178;Float;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;-1887.875,-715.2556;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-1907.804,-387.3656;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;56;-2076.705,-178.3316;Float;True;Property;_TextureSample1;Texture Sample 1;6;0;Create;True;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;31;-1564.521,403.9199;Float;False;Property;_ShadowDistance;Shadow Distance;9;0;Create;True;0.6709523;0.6709523;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;5;-1448.068,516.9752;Float;False;2;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;60;-1712.058,-407.1855;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;40;-1669.371,65.99231;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;53;-1288.575,591.5353;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-1514.979,-52.0278;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-1295.719,736.7173;Float;False;Constant;_Float0;Float 0;10;0;Create;True;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;18;-1058.782,-79.19268;Fixed;False;Property;_Texture;Texture;2;0;Create;True;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;29;-1238.568,828.5543;Float;True;Property;_RampTexture;Ramp Texture;7;0;Create;True;None;None;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-1104.259,-281.7786;Float;False;Property;_ShadowBrightness;Shadow Brightness;8;0;Create;True;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;42;-1135.986,679.191;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StepOpNode;30;-1198.187,409.5002;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-807.5316,-183.8215;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;28;-963.915,647.7704;Float;True;Global;rampTex;rampTex;5;0;Create;True;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-648.9271,28.28817;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;9;-660.3505,-98.3128;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;47;-469.6958,-103.6442;Float;False;Property;_ToonRamp;Toon Ramp;13;0;Create;True;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;24;-216.2821,-97.25121;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-3.936493,67.90884;Float;False;Property;_Smoothness;Smoothness;14;0;Create;True;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;338.3733,-141.9115;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SH_ToonShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;Back;0;0;False;0;0;False;0;Opaque;0.5;True;False;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;False;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;True;0;0,0,0,0;VertexScale;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;20;0;19;0
WireConnection;37;0;20;0
WireConnection;37;1;34;0
WireConnection;37;2;38;0
WireConnection;54;1;37;0
WireConnection;54;0;41;0
WireConnection;59;0;57;0
WireConnection;59;1;58;0
WireConnection;10;0;11;0
WireConnection;10;1;2;0
WireConnection;56;0;55;0
WireConnection;5;0;4;0
WireConnection;5;1;3;0
WireConnection;60;0;59;0
WireConnection;60;1;10;0
WireConnection;60;2;56;0
WireConnection;40;0;54;0
WireConnection;53;0;5;0
WireConnection;53;1;31;0
WireConnection;32;0;60;0
WireConnection;32;1;40;0
WireConnection;18;0;60;0
WireConnection;18;1;32;0
WireConnection;42;0;53;0
WireConnection;42;1;44;0
WireConnection;30;0;31;0
WireConnection;30;1;5;0
WireConnection;23;0;22;0
WireConnection;23;1;18;0
WireConnection;28;0;29;0
WireConnection;28;1;42;0
WireConnection;49;0;18;0
WireConnection;49;1;28;0
WireConnection;9;0;23;0
WireConnection;9;1;18;0
WireConnection;9;2;30;0
WireConnection;47;0;9;0
WireConnection;47;1;49;0
WireConnection;24;0;47;0
WireConnection;0;2;24;0
WireConnection;0;4;61;0
ASEEND*/
//CHKSM=42FAC114DE5E91B773B1E26F6050960666145427