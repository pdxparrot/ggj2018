// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_ErosionShader"
{
	Properties
	{
		_ErossionTexture("Erossion Texture", 2D) = "white" {}
		_Color("Color", Color) = (0,0,0,0)
		_Opacity("Opacity", Range( 0 , 1)) = 1
		_DepthFadeDistance("Depth Fade Distance", Float) = 0
		_NoiseSpeed("Noise Speed", Range( 0 , 2)) = 1
		_NoiseTiling("Noise Tiling", Range( 0 , 5)) = 1
		_NoiseDirection("Noise Direction", Vector) = (0.5,0.5,0,0)
		_ShadowBrightness("Shadow Brightness", Range( 0 , 1)) = 1
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
		#pragma surface surf Unlit alpha:fade keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float4 screenPos;
		};

		uniform float4 _Color;
		uniform float _ShadowBrightness;
		uniform float _NoiseTiling;
		uniform float _NoiseSpeed;
		uniform float2 _NoiseDirection;
		uniform sampler2D _ErossionTexture;
		uniform float4 _ErossionTexture_ST;
		uniform float _Opacity;
		uniform sampler2D _CameraDepthTexture;
		uniform float _DepthFadeDistance;


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
			o.Emission = saturate( lerpResult38 ).rgb;
			float2 uv_ErossionTexture = i.uv_texcoord * _ErossionTexture_ST.xy + _ErossionTexture_ST.zw;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth15 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth15 = abs( ( screenDepth15 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthFadeDistance ) );
			o.Alpha = ( ( saturate( ( tex2D( _ErossionTexture, uv_ErossionTexture ).r - i.vertexColor.a ) ) * _Opacity ) * saturate( distanceDepth15 ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14301
1111;91;809;651;2237.728;637.9598;2.246096;True;False
Node;AmplifyShaderEditor.RangedFloatNode;28;-1752.606,155.0101;Float;False;Property;_NoiseSpeed;Noise Speed;4;0;Create;True;1;0.6;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-1534.774,-108.8726;Float;False;Property;_NoiseTiling;Noise Tiling;5;0;Create;True;1;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;2;-1660.909,299.779;Float;True;Property;_ErossionTexture;Erossion Texture;0;0;Create;True;None;None;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.Vector2Node;31;-1446.509,10.09175;Float;False;Property;_NoiseDirection;Noise Direction;6;0;Create;True;0.5,0.5;0.3,0.8;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;29;-1446.509,161.2714;Float;False;1;0;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1421.969,299.5184;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;33;-1239.631,12.74349;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;36;-1208.719,-106.4941;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.VertexColorNode;10;-1297.827,521.1852;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;34;-1011.789,-30.8624;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;4;-871.0154,325.9397;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-991.5167,606.7766;Float;False;Property;_DepthFadeDistance;Depth Fade Distance;3;0;Create;True;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-1065.266,-139.3264;Float;False;Property;_ShadowBrightness;Shadow Brightness;7;0;Create;True;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;3;-1006.177,-344.9214;Float;False;Property;_Color;Color;1;0;Create;True;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;6;-705.0598,326.9658;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-906.6823,493.9302;Float;False;Property;_Opacity;Opacity;2;0;Create;True;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-759.7966,-155.0491;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DepthFade;15;-733.0322,609.7321;Float;False;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;35;-750.978,-36.46287;Float;False;Simplex2D;1;0;FLOAT2;1,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-545.6064,326.9659;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;38;-504.6245,-196.4807;Float;False;3;0;COLOR;0.0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;19;-522.134,606.7764;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-368.2701,338.0011;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;14;-250.7758,-22.62308;Float;False;1;0;COLOR;0.0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;49;-47.53783,20.31911;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;SH_ErosionShader;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;True;False;Back;0;0;False;0;0;False;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;False;2;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;29;0;28;0
WireConnection;1;0;2;0
WireConnection;33;2;31;0
WireConnection;33;1;29;0
WireConnection;36;0;37;0
WireConnection;36;1;37;0
WireConnection;34;0;36;0
WireConnection;34;1;33;0
WireConnection;4;0;1;1
WireConnection;4;1;10;4
WireConnection;6;0;4;0
WireConnection;50;0;3;0
WireConnection;50;1;51;0
WireConnection;15;0;24;0
WireConnection;35;0;34;0
WireConnection;7;0;6;0
WireConnection;7;1;8;0
WireConnection;38;0;3;0
WireConnection;38;1;50;0
WireConnection;38;2;35;0
WireConnection;19;0;15;0
WireConnection;23;0;7;0
WireConnection;23;1;19;0
WireConnection;14;0;38;0
WireConnection;49;2;14;0
WireConnection;49;9;23;0
ASEEND*/
//CHKSM=F1A34CF1388E6CF8887EA3B9650F1D2E77AD78F7