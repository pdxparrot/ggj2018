// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_VFX_Feather"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_PlayerColor("PlayerColor", Color) = (0.2867647,0.2518643,0.1602509,0)
		_Brightness("Brightness", Range( 0 , 2)) = 1
		[Toggle]_Texture("Texture", Float) = 0
		_DiffuseTexture("Diffuse Texture", 2D) = "white" {}
		_BlackPoint("BlackPoint", Range( 0 , 1)) = 0
		_WhitePoint("WhitePoint", Range( 0 , 1)) = 1
		_TextureContrast("Texture Contrast", Range( 0 , 10)) = 0
		[Toggle]_FeatherSwitch("Feather Switch", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform fixed _Texture;
		uniform float _Brightness;
		uniform float4 _PlayerColor;
		uniform float _TextureContrast;
		uniform sampler2D _DiffuseTexture;
		uniform float _FeatherSwitch;
		uniform float _BlackPoint;
		uniform float _WhitePoint;
		uniform float _Cutoff = 0.5;


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 temp_output_10_0 = ( _Brightness * _PlayerColor );
			float2 uv_TexCoord61 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float4 tex2DNode20 = tex2D( _DiffuseTexture, lerp(uv_TexCoord61,( uv_TexCoord61 + float2( 0.5,0 ) ),_FeatherSwitch) );
			float4 temp_cast_0 = (_BlackPoint).xxxx;
			float4 temp_cast_1 = (_WhitePoint).xxxx;
			float4 clampResult37 = clamp( tex2DNode20 , temp_cast_0 , temp_cast_1 );
			o.Emission = saturate( lerp(temp_output_10_0,( temp_output_10_0 * saturate( CalculateContrast(_TextureContrast,clampResult37) ) ),_Texture) ).rgb;
			o.Alpha = 1;
			clip( tex2DNode20.a - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14301
924;91;996;651;3014.088;1623.349;4.200231;True;False
Node;AmplifyShaderEditor.Vector2Node;59;-2239.747,292.7747;Float;False;Constant;_Vector1;Vector 1;13;0;Create;True;0.5,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;61;-2283.946,40.57465;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;62;-2044.746,222.5746;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;19;-1911.221,-54.65948;Float;True;Property;_DiffuseTexture;Diffuse Texture;4;0;Create;True;None;None;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.ToggleSwitchNode;56;-1906.517,148.2421;Float;False;Property;_FeatherSwitch;Feather Switch;8;0;Create;True;0;2;0;FLOAT2;0,0;False;1;FLOAT2;0.0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;20;-1641.655,-54.08875;Float;True;Property;_TextureSample0;Texture Sample 0;6;0;Create;True;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;38;-1635.308,253.3115;Float;False;Property;_WhitePoint;WhitePoint;6;0;Create;True;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-1637.545,160.8257;Float;False;Property;_BlackPoint;BlackPoint;5;0;Create;True;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-1287.878,181.3792;Float;False;Property;_TextureContrast;Texture Contrast;7;0;Create;True;0;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;37;-1279.894,40.3357;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;2;-1124.625,-278.7183;Float;False;Property;_PlayerColor;PlayerColor;1;0;Create;True;0.2867647,0.2518643,0.1602509,0;0.3525109,0.3122297,0.5661765,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;11;-1117.379,-378.0073;Float;False;Property;_Brightness;Brightness;2;0;Create;True;1;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;54;-977.0385,40.28504;Float;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;0.0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;40;-822.1655,40.79092;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-832.1185,-296.4138;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-667.7734,-77.22918;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;18;-504.3513,-104.3941;Fixed;False;Property;_Texture;Texture;3;0;Create;True;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;55;-1282.198,283.0896;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;24;-216.2821,-97.25121;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-43.49139,-141.9115;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SH_VFX_Feather;False;False;False;False;True;True;True;True;True;False;False;False;False;False;False;True;False;Off;0;0;False;0;0;False;0;Custom;0.5;True;False;0;True;TransparentCutout;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;False;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexScale;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;62;0;61;0
WireConnection;62;1;59;0
WireConnection;56;0;61;0
WireConnection;56;1;62;0
WireConnection;20;0;19;0
WireConnection;20;1;56;0
WireConnection;37;0;20;0
WireConnection;37;1;34;0
WireConnection;37;2;38;0
WireConnection;54;1;37;0
WireConnection;54;0;41;0
WireConnection;40;0;54;0
WireConnection;10;0;11;0
WireConnection;10;1;2;0
WireConnection;32;0;10;0
WireConnection;32;1;40;0
WireConnection;18;0;10;0
WireConnection;18;1;32;0
WireConnection;55;0;20;4
WireConnection;24;0;18;0
WireConnection;0;2;24;0
WireConnection;0;10;55;0
ASEEND*/
//CHKSM=6E827C112C54CCAECA641021BA9C118514BDB3CF