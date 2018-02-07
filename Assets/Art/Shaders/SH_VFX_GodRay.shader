// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_VFX_GodRay"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_BaseBrightness("Base Brightness", Float) = 0
		_Color("Color", Color) = (0,0,0,0)
		_DitherEndDistance("Dither End Distance", Float) = 600
		_DitherStartDistance("Dither Start Distance", Float) = 500
		[Toggle]_Usealphatexture("Use alpha texture", Float) = 0
		_AlphaTexture("Alpha Texture", 2D) = "white" {}
		_PulseSpeed("Pulse Speed", Float) = 0
		_PulseBrightnessScale("Pulse Brightness Scale", Float) = 2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd vertex:vertexDataFunc 
		struct Input
		{
			float eyeDepth;
			float2 uv_texcoord;
		};

		uniform float4 _Color;
		uniform float _BaseBrightness;
		uniform float _PulseSpeed;
		uniform float _PulseBrightnessScale;
		uniform float _Usealphatexture;
		uniform float _DitherEndDistance;
		uniform float _DitherStartDistance;
		uniform sampler2D _AlphaTexture;
		uniform float4 _AlphaTexture_ST;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float mulTime32 = _Time.y * _PulseSpeed;
			o.Emission = ( _Color * ( _BaseBrightness * (1.0 + (sin( mulTime32 ) - -1.0) * (_PulseBrightnessScale - 1.0) / (1.0 - -1.0)) ) ).rgb;
			o.Alpha = 1;
			float temp_output_8_0 = ( _DitherEndDistance + _ProjectionParams.y );
			float2 uv_AlphaTexture = i.uv_texcoord * _AlphaTexture_ST.xy + _AlphaTexture_ST.zw;
			clip( lerp(1.0,( ( ( i.eyeDepth + -temp_output_8_0 ) / ( _DitherStartDistance - temp_output_8_0 ) ) - tex2D( _AlphaTexture, uv_AlphaTexture ).r ),_Usealphatexture) - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14301
913;91;1007;651;1951.237;568.2209;3.099059;True;False
Node;AmplifyShaderEditor.CommentaryNode;5;-1922.868,258.8643;Float;False;1047.541;403.52;Scale depth from start to end;8;15;13;12;11;10;9;8;7;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;4;-1974.547,703.5155;Float;False;297.1897;243;Correction for near plane clipping;1;6;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-1883.892,541.5589;Float;False;Property;_DitherEndDistance;Dither End Distance;3;0;Create;True;600;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ProjectionParams;6;-1901.847,752.8157;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;29;-1298.76,5.378798;Float;False;Property;_PulseSpeed;Pulse Speed;7;0;Create;True;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;8;-1598.401,545.0917;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;32;-1132.737,4.582179;Float;False;1;0;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SurfaceDepthNode;9;-1857.309,356.9772;Float;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-1436.025,508.143;Float;False;Property;_DitherStartDistance;Dither Start Distance;4;0;Create;True;500;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;11;-1407.014,421.2112;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;20;-1474.605,691.4474;Float;True;Property;_AlphaTexture;Alpha Texture;6;0;Create;True;None;None;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;35;-1242.24,888.3064;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;10,10;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;24;-948.2841,6.481894;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;13;-1197.929,361.6652;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-1108.657,91.39435;Float;False;Property;_PulseBrightnessScale;Pulse Brightness Scale;8;0;Create;True;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;12;-1200.629,516.4641;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;34;-994.4379,691.3933;Float;True;Property;_TextureSample0;Texture Sample 0;10;0;Create;True;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;3;-861.3848,-131.4572;Float;False;Property;_BaseBrightness;Base Brightness;1;0;Create;True;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;15;-1012.114,419.4613;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;27;-815.1502,5.511751;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;-1.0;False;2;FLOAT;1.0;False;3;FLOAT;1.0;False;4;FLOAT;2.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;17;-697.3018,420.4518;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-569.7886,-43.43039;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-602.9474,303.8684;Float;False;Constant;_Float2;Float 2;5;0;Create;True;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-616.356,-259.6853;Float;False;Property;_Color;Color;2;0;Create;True;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-347.9004,-133.5844;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;18;-374.8578,297.4695;Float;False;Property;_Usealphatexture;Use alpha texture;5;0;Create;True;0;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SH_VFX_GodRay;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;True;False;Back;0;0;False;0;0;False;0;Masked;0.5;True;False;0;False;TransparentCutout;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;False;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;8;0;7;0
WireConnection;8;1;6;2
WireConnection;32;0;29;0
WireConnection;11;0;8;0
WireConnection;35;2;20;0
WireConnection;24;0;32;0
WireConnection;13;0;9;0
WireConnection;13;1;11;0
WireConnection;12;0;10;0
WireConnection;12;1;8;0
WireConnection;34;0;20;0
WireConnection;34;1;35;0
WireConnection;15;0;13;0
WireConnection;15;1;12;0
WireConnection;27;0;24;0
WireConnection;27;4;33;0
WireConnection;17;0;15;0
WireConnection;17;1;34;1
WireConnection;28;0;3;0
WireConnection;28;1;27;0
WireConnection;2;0;1;0
WireConnection;2;1;28;0
WireConnection;18;0;16;0
WireConnection;18;1;17;0
WireConnection;0;2;2;0
WireConnection;0;10;18;0
ASEEND*/
//CHKSM=86C9A554BB0D3429A3FF2C059FFD70273BDADE37