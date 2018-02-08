// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_VFX_Fog"
{
	Properties
	{
		_FogColor("Fog Color", Color) = (0,0.6691177,0.08767758,0)
		_FogFadeDistance("Fog Fade Distance", Range( 0 , 1)) = 0
		_FogOpacity("Fog Opacity", Range( 0 , 1)) = 0
		_NoiseTiling2("Noise Tiling 2", Vector) = (1,1,0,0)
		_NoiseTiling1(" Noise Tiling 1", Vector) = (1,1,0,0)
		_NoiseMin("Noise Min", Range( 0 , 1)) = 0
		_FogSpeed2("Fog Speed 2", Range( 0 , 2)) = 1
		_FogSpeed("Fog Speed", Range( 0 , 2)) = 1
		_NoiseMax("Noise Max", Range( 0 , 1)) = 1
		_FogDirection("Fog Direction", Vector) = (0.5,0.5,0,0)
		_FogDirection2("Fog Direction 2", Vector) = (0.5,0.5,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		Cull Back
		Stencil
		{
			Ref 1
			Comp NotEqual
			Pass Zero
			Fail Keep
		}
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 5.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform float2 _NoiseTiling2;
		uniform float _FogSpeed2;
		uniform float2 _FogDirection2;
		uniform float2 _NoiseTiling1;
		uniform float _FogSpeed;
		uniform float2 _FogDirection;
		uniform float _NoiseMin;
		uniform float _NoiseMax;
		uniform float4 _FogColor;
		uniform sampler2D _CameraDepthTexture;
		uniform float _FogFadeDistance;
		uniform float _FogOpacity;


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


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float mulTime33 = _Time.y * _FogSpeed2;
			float2 panner35 = ( float2( 0,0 ) + mulTime33 * _FogDirection2);
			float2 uv_TexCoord36 = i.uv_texcoord * _NoiseTiling2 + panner35;
			float simplePerlin2D37 = snoise( uv_TexCoord36 );
			float mulTime25 = _Time.y * _FogSpeed;
			float2 panner22 = ( float2( 0,0 ) + mulTime25 * _FogDirection);
			float2 uv_TexCoord16 = i.uv_texcoord * _NoiseTiling1 + panner22;
			float simplePerlin2D14 = snoise( uv_TexCoord16 );
			float lerpResult38 = lerp( simplePerlin2D37 , simplePerlin2D14 , 0.5);
			o.Emission = saturate( ( (_NoiseMin + (lerpResult38 - -1.0) * (_NoiseMax - _NoiseMin) / (1.0 - -1.0)) * _FogColor ) ).rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float eyeDepth4 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float clampResult12 = clamp( ( abs( ( eyeDepth4 - ase_screenPos.w ) ) * (0.01 + (_FogFadeDistance - 0.0) * (0.4 - 0.01) / (1.0 - 0.0)) ) , 0.0 , _FogOpacity );
			o.Alpha = clampResult12;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14301
1168;91;752;656;1349.223;619.7467;2.122129;True;False
Node;AmplifyShaderEditor.RangedFloatNode;26;-1956.045,-42.77776;Float;False;Property;_FogSpeed;Fog Speed;7;0;Create;True;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-1983.631,-375.2155;Float;False;Property;_FogSpeed2;Fog Speed 2;6;0;Create;True;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;25;-1672.974,-36.51638;Float;False;1;0;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;23;-1672.974,-187.6963;Float;False;Property;_FogDirection;Fog Direction;9;0;Create;True;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;33;-1677.533,-368.9542;Float;False;1;0;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;32;-1677.533,-520.1339;Float;False;Property;_FogDirection2;Fog Direction 2;10;0;Create;True;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;35;-1470.655,-517.4822;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;22;-1466.096,-185.0447;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;34;-1468.431,-662.2452;Float;False;Property;_NoiseTiling2;Noise Tiling 2;3;0;Create;True;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;17;-1463.872,-329.8077;Float;False;Property;_NoiseTiling1; Noise Tiling 1;4;0;Create;True;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ScreenPosInputsNode;3;-1465.974,118.0486;Float;False;1;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;16;-1237.598,-289.5888;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;36;-1242.157,-622.0262;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenDepthNode;4;-1125.037,118.0495;Float;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;14;-981.3391,-226.0523;Float;False;Simplex2D;1;0;FLOAT2;1,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;37;-985.8976,-558.4898;Float;False;Simplex2D;1;0;FLOAT2;1,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-978.8294,-461.7045;Float;False;Constant;_Float1;Float 1;11;0;Create;True;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-1096.15,296.6253;Float;False;Property;_FogFadeDistance;Fog Fade Distance;1;0;Create;True;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;5;-885.7778,192.9471;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-943.3966,-96.97545;Float;False;Property;_NoiseMin;Noise Min;5;0;Create;True;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-937.1221,-9.143665;Float;False;Property;_NoiseMax;Noise Max;8;0;Create;True;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;38;-670.1821,-515.3287;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;19;-493.282,-396.3051;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;-1.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;10;-792.2691,302.1099;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.01;False;4;FLOAT;0.4;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;8;-733.8098,192.6375;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;-603.4571,18.24008;Float;False;Property;_FogColor;Fog Color;0;0;Create;True;0,0.6691177,0.08767758,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-606.3421,195.2427;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-859.2833,495.8392;Float;False;Property;_FogOpacity;Fog Opacity;2;0;Create;True;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-328.7028,-35.83202;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;41;-173.562,46.60169;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;12;-457.0417,194.0842;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;29;-973.8809,-997.9921;Float;True;Property;_TextureSample0;Texture Sample 0;8;0;Create;True;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;30;-1244.814,-1104.075;Float;True;Property;_NoiseTexture;Noise Texture;12;0;Create;True;None;None;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.ToggleSwitchNode;28;-566.7836,-917.2627;Float;False;Property;_ProceduralFog;Procedural Fog;11;0;Create;True;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;7;Float;ASEMaterialInspector;0;0;Standard;SH_VFX_Fog;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;True;False;Back;0;0;False;0;0;False;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;1;255;255;6;2;1;0;0;0;0;0;False;2;15;10;25;False;0.5;False;2;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;25;0;26;0
WireConnection;33;0;31;0
WireConnection;35;2;32;0
WireConnection;35;1;33;0
WireConnection;22;2;23;0
WireConnection;22;1;25;0
WireConnection;16;0;17;0
WireConnection;16;1;22;0
WireConnection;36;0;34;0
WireConnection;36;1;35;0
WireConnection;4;0;3;0
WireConnection;14;0;16;0
WireConnection;37;0;36;0
WireConnection;5;0;4;0
WireConnection;5;1;3;4
WireConnection;38;0;37;0
WireConnection;38;1;14;0
WireConnection;38;2;39;0
WireConnection;19;0;38;0
WireConnection;19;3;21;0
WireConnection;19;4;20;0
WireConnection;10;0;9;0
WireConnection;8;0;5;0
WireConnection;7;0;8;0
WireConnection;7;1;10;0
WireConnection;15;0;19;0
WireConnection;15;1;2;0
WireConnection;41;0;15;0
WireConnection;12;0;7;0
WireConnection;12;2;11;0
WireConnection;29;0;30;0
WireConnection;28;0;29;0
WireConnection;0;2;41;0
WireConnection;0;9;12;0
ASEEND*/
//CHKSM=D120609A2D51E05301E6F3FB210217279BFEB2D1