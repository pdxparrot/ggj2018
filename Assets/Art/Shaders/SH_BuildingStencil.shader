// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_BuildingStencil"
{
	Properties
	{
		_DitherEndDistance("Dither End Distance", Float) = 600
		_DitherStartDistance("Dither Start Distance", Float) = 500
		_Color("Color", Color) = (0,0,0,0)
		[Toggle]_AlphaDither("Alpha Dither", Float) = 0
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+1" }
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
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float eyeDepth;
			float4 screenPosition;
		};

		uniform float4 _Color;
		uniform float _Smoothness;
		uniform float _AlphaDither;
		uniform float _DitherEndDistance;
		uniform float _DitherStartDistance;
		uniform float _Cutoff = 0.5;


		inline float Dither4x4Bayer( int x, int y )
		{
			const float dither[ 16 ] = {
				 1,  9,  3, 11,
				13,  5, 15,  7,
				 4, 12,  2, 10,
				16,  8, 14,  6 };
			int r = y * 4 + x;
			return dither[r] / 16;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			o.screenPosition = ase_screenPos;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Albedo = _Color.rgb;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
			float temp_output_68_0 = ( _DitherEndDistance + _ProjectionParams.y );
			float4 ase_screenPos = i.screenPosition;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 clipScreen75 = ase_screenPosNorm.xy * _ScreenParams.xy;
			float dither75 = Dither4x4Bayer( fmod(clipScreen75.x, 4), fmod(clipScreen75.y, 4) );
			clip( lerp(1.0,( ( ( i.eyeDepth + -temp_output_68_0 ) / ( _DitherStartDistance - temp_output_68_0 ) ) - dither75 ),_AlphaDither) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14301
483;91;480;656;940.5069;911.8198;2.979879;True;False
Node;AmplifyShaderEditor.CommentaryNode;65;-1445.594,506.3217;Float;False;297.1897;243;Correction for near plane clipping;1;66;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;64;-1458.785,51.20762;Float;False;1047.541;403.52;Scale depth from start to end;8;73;72;71;70;69;68;67;74;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ProjectionParams;66;-1372.894,555.6218;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;67;-1419.809,333.9024;Float;False;Property;_DitherEndDistance;Dither End Distance;0;0;Create;True;600;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;68;-1134.318,337.4352;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;69;-953.3459,294.9074;Float;False;Property;_DitherStartDistance;Dither Start Distance;1;0;Create;True;500;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;71;-942.9308,213.5544;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SurfaceDepthNode;70;-1393.226,149.3204;Float;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;73;-736.5458,308.8073;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;72;-733.8461,154.0084;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DitheringNode;75;-393.5178,290.5694;Float;False;0;2;0;FLOAT;0.0;False;1;SAMPLER2D;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;74;-548.0303,211.8045;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-113.6645,64.7026;Float;False;Constant;_Float0;Float 0;5;0;Create;True;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;76;-137.5178,210.5694;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;85;-25.4089,-360.4232;Float;False;Property;_Color;Color;2;0;Create;True;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;88;-93.73583,-52.32181;Float;False;Property;_Smoothness;Smoothness;5;0;Create;True;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;86;87.23779,65.96389;Float;False;Property;_AlphaDither;Alpha Dither;3;0;Create;True;0;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;358.3733,-140.3082;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SH_BuildingStencil;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;False;0;Custom;0.5;True;True;1;True;Opaque;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;1;255;255;6;2;1;0;7;3;0;0;False;2;15;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexScale;False;False;Cylindrical;False;Relative;0;;4;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;68;0;67;0
WireConnection;68;1;66;2
WireConnection;71;0;68;0
WireConnection;73;0;69;0
WireConnection;73;1;68;0
WireConnection;72;0;70;0
WireConnection;72;1;71;0
WireConnection;74;0;72;0
WireConnection;74;1;73;0
WireConnection;76;0;74;0
WireConnection;76;1;75;0
WireConnection;86;0;87;0
WireConnection;86;1;76;0
WireConnection;0;0;85;0
WireConnection;0;4;88;0
WireConnection;0;10;86;0
ASEEND*/
//CHKSM=33E56903179542881B8745865B3A842B848C111E