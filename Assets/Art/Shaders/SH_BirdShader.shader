// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_BirdShader"
{
	Properties
	{
		_ASEOutlineWidth( "Outline Width", Float ) = 0
		_ASEOutlineColor( "Outline Color", Color ) = (0,0,0,0)
		_PlayerColor("PlayerColor", Color) = (0.2867647,0.2518643,0.1602509,0)
		_ColorBrightness("Color Brightness", Float) = 1
		_DiffuseTexture("Diffuse Texture", 2D) = "white" {}
		_ColorAlpha("Color Alpha", 2D) = "white" {}
		_Smoothness("Smoothness", Range( 0 , 1)) = 1
		_BodyColorHue("Body Color Hue", Range( 0 , 1)) = 0
		_MasterBrightness("Master Brightness", Float) = 1
		_BodyColorSaturation("Body Color Saturation", Range( 0 , 1)) = 0
		_TextureSaturation("Texture Saturation", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
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
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _MasterBrightness;
		uniform float _ColorBrightness;
		uniform float4 _PlayerColor;
		uniform float _BodyColorHue;
		uniform float _BodyColorSaturation;
		uniform sampler2D _ColorAlpha;
		uniform float4 _ColorAlpha_ST;
		uniform sampler2D _DiffuseTexture;
		uniform float4 _DiffuseTexture_ST;
		uniform float _TextureSaturation;
		uniform float _Smoothness;


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, clamp( p - K.xxx, 0.0, 1.0 ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 temp_output_59_0 = ( _ColorBrightness * _PlayerColor );
			float3 hsvTorgb65 = RGBToHSV( temp_output_59_0.rgb );
			float3 hsvTorgb64 = HSVToRGB( float3(( _BodyColorHue + hsvTorgb65.x ),( (-1.0 + (_BodyColorSaturation - 0.0) * (0.0 - -1.0) / (1.0 - 0.0)) + hsvTorgb65.y ),hsvTorgb65.z) );
			float2 uv_ColorAlpha = i.uv_texcoord * _ColorAlpha_ST.xy + _ColorAlpha_ST.zw;
			float4 lerpResult60 = lerp( temp_output_59_0 , float4( hsvTorgb64 , 0.0 ) , tex2D( _ColorAlpha, uv_ColorAlpha ).r);
			float2 uv_DiffuseTexture = i.uv_texcoord * _DiffuseTexture_ST.xy + _DiffuseTexture_ST.zw;
			float3 desaturateVar77 = lerp( tex2D( _DiffuseTexture, uv_DiffuseTexture ).rgb,dot(tex2D( _DiffuseTexture, uv_DiffuseTexture ).rgb,float3(0.299,0.587,0.114)).xxx,(1.0 + (_TextureSaturation - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)));
			o.Albedo = ( _MasterBrightness * ( lerpResult60 * float4( desaturateVar77 , 0.0 ) ) ).rgb;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14301
1164;91;756;651;1515.267;1275.246;3.087096;True;False
Node;AmplifyShaderEditor.RangedFloatNode;57;-2118.689,-889.897;Float;False;Property;_ColorBrightness;Color Brightness;1;0;Create;True;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;58;-2125.935,-790.608;Float;False;Property;_PlayerColor;PlayerColor;0;0;Create;True;0.2867647,0.2518643,0.1602509,0;0.3525109,0.3122297,0.5661765,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;-1833.432,-808.3035;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;71;-1381.157,-621.3367;Float;False;Property;_BodyColorSaturation;Body Color Saturation;7;0;Create;True;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-1671.633,-621.3367;Float;False;Property;_BodyColorHue;Body Color Hue;5;0;Create;True;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;83;-1118.482,-616.9357;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;-1.0;False;4;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;65;-1673.711,-491.5126;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TexturePropertyNode;19;-1244.768,-27.59873;Float;True;Property;_DiffuseTexture;Diffuse Texture;2;0;Create;True;None;None;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;55;-1044.661,-294.9338;Float;True;Property;_ColorAlpha;Color Alpha;3;0;Create;True;None;None;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleAddOpNode;74;-895.5504,-473.9535;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-1104.175,215.5135;Float;False;Property;_TextureSaturation;Texture Saturation;8;0;Create;True;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;72;-1318.861,-490.7239;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;64;-692.3653,-479.7582;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;56;-775.0977,-294.3631;Float;True;Property;_TextureSample1;Texture Sample 1;6;0;Create;True;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;20;-975.205,-27.028;Float;True;Property;_TextureSample0;Texture Sample 0;6;0;Create;True;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;66;-510.9133,-718.1362;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;84;-827.989,219.6432;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;1.0;False;4;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;60;-415.2634,-472.6772;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DesaturateOpNode;77;-648.4292,-20.34367;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-218.1851,-117.5194;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;63;-162.9147,-278.7113;Float;False;Property;_MasterBrightness;Master Brightness;6;0;Create;True;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;26.7709,-138.1764;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;COLOR;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-80.16706,-15.41293;Float;False;Property;_Smoothness;Smoothness;4;0;Create;True;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;358.3733,-140.3082;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SH_BirdShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;False;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;0;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;0;True;0;0,0,0,0;VertexScale;False;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;59;0;57;0
WireConnection;59;1;58;0
WireConnection;83;0;71;0
WireConnection;65;0;59;0
WireConnection;74;0;83;0
WireConnection;74;1;65;2
WireConnection;72;0;70;0
WireConnection;72;1;65;1
WireConnection;64;0;72;0
WireConnection;64;1;74;0
WireConnection;64;2;65;3
WireConnection;56;0;55;0
WireConnection;20;0;19;0
WireConnection;66;0;59;0
WireConnection;84;0;78;0
WireConnection;60;0;66;0
WireConnection;60;1;64;0
WireConnection;60;2;56;0
WireConnection;77;0;20;0
WireConnection;77;1;84;0
WireConnection;32;0;60;0
WireConnection;32;1;77;0
WireConnection;62;0;63;0
WireConnection;62;1;32;0
WireConnection;0;0;62;0
WireConnection;0;4;61;0
ASEEND*/
//CHKSM=EA79762FFA5D73E466F5D214D3732A761CF683F5