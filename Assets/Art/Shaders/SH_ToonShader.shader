// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_ToonShader"
{
	Properties
	{
		_ASEOutlineWidth( "Outline Width", Float ) = 0
		_ASEOutlineColor( "Outline Color", Color ) = (0,0,0,0)
		_Color("Color", Color) = (0.2867647,0.2518643,0.1602509,0)
		_Brightness("Brightness", Range( 0 , 2)) = 1
		[Toggle]_Texture("Texture", Float) = 0
		_DiffuseTexture("Diffuse Texture", 2D) = "white" {}
		_RampTexture("Ramp Texture", 2D) = "white" {}
		_ShadowBrightness("Shadow Brightness", Range( 0 , 1)) = 0
		_ShadowDistance("Shadow Distance", Range( -1 , 1)) = 0.6709523
		_BlackPoint("BlackPoint", Range( 0 , 1)) = 0
		_WhitePoint("WhitePoint", Range( 0 , 1)) = 1
		_TextureContrast("Texture Contrast", Range( 0 , 10)) = 0
		[Toggle]_ToonRamp("Toon Ramp", Float) = 0
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
		#pragma surface surf Standard keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float _ToonRamp;
		uniform float _ShadowBrightness;
		uniform fixed _Texture;
		uniform float _Brightness;
		uniform float4 _Color;
		uniform float _TextureContrast;
		uniform sampler2D _DiffuseTexture;
		uniform float4 _DiffuseTexture_ST;
		uniform float _BlackPoint;
		uniform float _WhitePoint;
		uniform float _ShadowDistance;
		uniform sampler2D _RampTexture;


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 temp_output_10_0 = ( _Brightness * _Color );
			float2 uv_DiffuseTexture = i.uv_texcoord * _DiffuseTexture_ST.xy + _DiffuseTexture_ST.zw;
			float4 temp_cast_0 = (_BlackPoint).xxxx;
			float4 temp_cast_1 = (_WhitePoint).xxxx;
			float4 clampResult37 = clamp( tex2D( _DiffuseTexture, uv_DiffuseTexture ) , temp_cast_0 , temp_cast_1 );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float dotResult5 = dot( ase_worldlightDir , ase_worldNormal );
			float4 lerpResult9 = lerp( ( _ShadowBrightness * lerp(temp_output_10_0,( temp_output_10_0 * saturate( CalculateContrast(_TextureContrast,clampResult37) ) ),_Texture) ) , lerp(temp_output_10_0,( temp_output_10_0 * saturate( CalculateContrast(_TextureContrast,clampResult37) ) ),_Texture) , step( _ShadowDistance , dotResult5 ));
			float2 appendResult42 = (float2(( dotResult5 - _ShadowDistance ) , 0.0));
			o.Emission = saturate( lerp(lerpResult9,( lerp(temp_output_10_0,( temp_output_10_0 * saturate( CalculateContrast(_TextureContrast,clampResult37) ) ),_Texture) * tex2D( _RampTexture, appendResult42 ) ),_ToonRamp) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14301
1440;91;480;927;1048.993;909.2224;3.174141;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;19;-2465.65,-29.45809;Float;True;Property;_DiffuseTexture;Diffuse Texture;3;0;Create;True;None;None;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-2189.738,278.5129;Float;False;Property;_WhitePoint;WhitePoint;8;0;Create;True;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;20;-2196.085,-28.88736;Float;True;Property;_TextureSample0;Texture Sample 0;6;0;Create;True;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;34;-2191.975,186.0271;Float;False;Property;_BlackPoint;BlackPoint;7;0;Create;True;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;37;-1834.324,65.53709;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;51;-1771.779,353.9199;Float;False;1299.706;704.6351;LightingStyle;10;3;4;31;5;44;42;29;28;30;53;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-1842.308,206.5806;Float;False;Property;_TextureContrast;Texture Contrast;9;0;Create;True;0;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-1671.809,-352.8059;Float;False;Property;_Brightness;Brightness;1;0;Create;True;1;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;-1679.055,-253.5169;Float;False;Property;_Color;Color;0;0;Create;True;0.2867647,0.2518643,0.1602509,0;0.3525109,0.3122297,0.5661765,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;3;-1705.288,680.5178;Float;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;4;-1721.779,516.8288;Float;False;1;0;FLOAT;0.0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleContrastOpNode;54;-1531.469,65.48643;Float;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;0.0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-1386.549,-271.2124;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-1564.521,403.9199;Float;False;Property;_ShadowDistance;Shadow Distance;6;0;Create;True;0.6709523;0.6709523;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;5;-1448.068,516.9752;Float;False;2;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;40;-1376.596,65.99231;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-1295.719,736.7173;Float;False;Constant;_Float0;Float 0;10;0;Create;True;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-1222.204,-52.0278;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;53;-1288.575,591.5353;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;18;-1058.782,-79.19268;Fixed;False;Property;_Texture;Texture;2;0;Create;True;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;29;-1238.568,828.5543;Float;True;Property;_RampTexture;Ramp Texture;4;0;Create;True;4843888f9a32269418493b51aa44f700;4843888f9a32269418493b51aa44f700;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-1104.259,-281.7786;Float;False;Property;_ShadowBrightness;Shadow Brightness;5;0;Create;True;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;42;-1135.986,679.191;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;28;-963.915,647.7704;Float;True;Global;rampTex;rampTex;5;0;Create;True;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-807.5316,-183.8215;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;30;-1198.187,409.5002;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-648.9271,28.28817;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;9;-660.3505,-98.3128;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;47;-469.6958,-103.6442;Float;False;Property;_ToonRamp;Toon Ramp;10;0;Create;True;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;24;-216.2821,-97.25121;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-43.49139,-141.9115;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SH_ToonShader;False;False;False;False;True;True;True;True;True;False;False;False;False;False;False;True;False;Back;0;0;False;0;0;False;0;Opaque;0.5;True;False;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;False;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;True;0;0,0,0,0;VertexScale;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;20;0;19;0
WireConnection;37;0;20;0
WireConnection;37;1;34;0
WireConnection;37;2;38;0
WireConnection;54;1;37;0
WireConnection;54;0;41;0
WireConnection;10;0;11;0
WireConnection;10;1;2;0
WireConnection;5;0;4;0
WireConnection;5;1;3;0
WireConnection;40;0;54;0
WireConnection;32;0;10;0
WireConnection;32;1;40;0
WireConnection;53;0;5;0
WireConnection;53;1;31;0
WireConnection;18;0;10;0
WireConnection;18;1;32;0
WireConnection;42;0;53;0
WireConnection;42;1;44;0
WireConnection;28;0;29;0
WireConnection;28;1;42;0
WireConnection;23;0;22;0
WireConnection;23;1;18;0
WireConnection;30;0;31;0
WireConnection;30;1;5;0
WireConnection;49;0;18;0
WireConnection;49;1;28;0
WireConnection;9;0;23;0
WireConnection;9;1;18;0
WireConnection;9;2;30;0
WireConnection;47;0;9;0
WireConnection;47;1;49;0
WireConnection;24;0;47;0
WireConnection;0;2;24;0
ASEEND*/
//CHKSM=38743D692A6192E8DAD2BDEFA0FAE8AE42EE484A