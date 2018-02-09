// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_VFX_FogErosion"
{
	Properties
	{
		_ErossionTexture("Erossion Texture", 2D) = "white" {}
		_CameraFadeStart("Camera Fade Start", Float) = 600
		_CameraFadeEnd("Camera Fade End", Float) = 500
		_Color("Color", Color) = (0,0,0,0)
		_Opacity("Opacity", Range( 0 , 1)) = 1
		_GeometryFadeDistance("Geometry Fade Distance", Float) = 0
		_NoiseSpeed("Noise Speed", Range( 0 , 2)) = 1
		_NoiseTiling("Noise Tiling", Range( 0 , 5)) = 1
		_NoiseDirection("Noise Direction", Vector) = (0.5,0.5,0,0)
		_ShadowBrightness("Shadow Brightness", Range( 0 , 1)) = 1
		[Toggle]_NoNoiseShadows("No Noise Shadows", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float4 screenPos;
			float eyeDepth;
		};

		uniform float _NoNoiseShadows;
		uniform float4 _Color;
		uniform float _ShadowBrightness;
		uniform float _NoiseTiling;
		uniform float _NoiseSpeed;
		uniform float2 _NoiseDirection;
		uniform sampler2D _ErossionTexture;
		uniform float4 _ErossionTexture_ST;
		uniform sampler2D _CameraDepthTexture;
		uniform float _GeometryFadeDistance;
		uniform float _CameraFadeStart;
		uniform float _CameraFadeEnd;
		uniform float _Opacity;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
		}

		inline fixed4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return fixed4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 appendResult36 = (float2(_NoiseTiling , _NoiseTiling));
			float mulTime29 = _Time.y * _NoiseSpeed;
			float2 panner33 = ( float2( 0,0 ) + mulTime29 * _NoiseDirection);
			float2 uv_TexCoord34 = i.uv_texcoord * appendResult36 + panner33;
			float simplePerlin2D35 = snoise( uv_TexCoord34 );
			float4 lerpResult38 = lerp( _Color , ( _Color * _ShadowBrightness ) , simplePerlin2D35);
			o.Emission = saturate( lerp(lerpResult38,_Color,_NoNoiseShadows) ).rgb;
			float2 uv_ErossionTexture = i.uv_texcoord * _ErossionTexture_ST.xy + _ErossionTexture_ST.zw;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth15 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth15 = abs( ( screenDepth15 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _GeometryFadeDistance ) );
			float temp_output_66_0 = ( _CameraFadeStart + _ProjectionParams.y );
			o.Alpha = ( ( ( saturate( ( tex2D( _ErossionTexture, uv_ErossionTexture ).r - i.vertexColor.a ) ) * saturate( distanceDepth15 ) ) * saturate( ( ( i.eyeDepth + -temp_output_66_0 ) / ( _CameraFadeEnd - temp_output_66_0 ) ) ) ) * _Opacity );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14301
586;91;849;656;1633.351;594.1141;2.318964;True;False
Node;AmplifyShaderEditor.CommentaryNode;63;-1862.137,746.0908;Float;False;1047.541;403.52;Scale depth from start to end;8;73;71;70;69;68;67;66;65;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;62;-1848.946,1201.204;Float;False;297.1897;243;Correction for near plane clipping;1;64;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-1823.161,1028.785;Float;False;Property;_CameraFadeStart;Camera Fade Start;1;0;Create;True;600;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ProjectionParams;64;-1776.246,1250.504;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;28;-1752.606,155.0101;Float;False;Property;_NoiseSpeed;Noise Speed;6;0;Create;True;1;0.6;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;29;-1446.509,161.2714;Float;False;1;0;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-1534.774,-108.8726;Float;False;Property;_NoiseTiling;Noise Tiling;7;0;Create;True;1;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;31;-1446.509,10.09175;Float;False;Property;_NoiseDirection;Noise Direction;8;0;Create;True;0.5,0.5;0.3,0.8;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexturePropertyNode;2;-2070.473,324.2307;Float;True;Property;_ErossionTexture;Erossion Texture;0;0;Create;True;None;None;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleAddOpNode;66;-1537.67,1032.317;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;10;-1707.391,545.6368;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-1831.533,323.9701;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;24;-1420.822,631.2283;Float;False;Property;_GeometryFadeDistance;Geometry Fade Distance;5;0;Create;True;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SurfaceDepthNode;69;-1796.578,844.2036;Float;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-1356.698,989.7903;Float;False;Property;_CameraFadeEnd;Camera Fade End;2;0;Create;True;500;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;33;-1239.631,12.74349;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;36;-1208.719,-106.4941;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NegateNode;68;-1346.283,908.4373;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;70;-1139.898,1003.69;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;4;-1280.58,350.3914;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;15;-1142.597,634.1838;Float;False;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;71;-1137.198,848.8914;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-1065.266,-139.3264;Float;False;Property;_ShadowBrightness;Shadow Brightness;9;0;Create;True;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;34;-1011.789,-30.8624;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;3;-1006.177,-344.9214;Float;False;Property;_Color;Color;3;0;Create;True;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;35;-757.9349,-38.78183;Float;False;Simplex2D;1;0;FLOAT2;1,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;19;-931.6993,631.2281;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;6;-1114.624,351.4174;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;73;-951.3833,906.6873;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-759.7966,-155.0491;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;80;-770.4656,73.98006;Float;False;1;0;COLOR;0.0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;38;-541.728,-177.9289;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;76;-778.4331,905.2418;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-777.8353,362.4528;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;79;-443.724,73.74748;Float;False;Property;_NoNoiseShadows;No Noise Shadows;10;0;Create;True;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-579.0027,486.6025;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-561.4067,810.672;Float;False;Property;_Opacity;Opacity;4;0;Create;True;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-351.5078,488.0741;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;14;-185.8447,79.41131;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;49;-8.115452,34.23289;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;SH_VFX_FogErosion;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;True;False;Back;0;0;False;0;0;False;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;False;2;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;29;0;28;0
WireConnection;66;0;65;0
WireConnection;66;1;64;2
WireConnection;1;0;2;0
WireConnection;33;2;31;0
WireConnection;33;1;29;0
WireConnection;36;0;37;0
WireConnection;36;1;37;0
WireConnection;68;0;66;0
WireConnection;70;0;67;0
WireConnection;70;1;66;0
WireConnection;4;0;1;1
WireConnection;4;1;10;4
WireConnection;15;0;24;0
WireConnection;71;0;69;0
WireConnection;71;1;68;0
WireConnection;34;0;36;0
WireConnection;34;1;33;0
WireConnection;35;0;34;0
WireConnection;19;0;15;0
WireConnection;6;0;4;0
WireConnection;73;0;71;0
WireConnection;73;1;70;0
WireConnection;50;0;3;0
WireConnection;50;1;51;0
WireConnection;80;0;3;0
WireConnection;38;0;3;0
WireConnection;38;1;50;0
WireConnection;38;2;35;0
WireConnection;76;0;73;0
WireConnection;23;0;6;0
WireConnection;23;1;19;0
WireConnection;79;0;38;0
WireConnection;79;1;80;0
WireConnection;61;0;23;0
WireConnection;61;1;76;0
WireConnection;77;0;61;0
WireConnection;77;1;8;0
WireConnection;14;0;79;0
WireConnection;49;2;14;0
WireConnection;49;9;77;0
ASEEND*/
//CHKSM=BFB7708956F1CA65CC5BFF659CBCCF0C45143FE5