// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_PigeonTracker"
{
	Properties
	{
		_Texture0("Texture 0", 2D) = "black" {}
		_Cutoff( "Mask Clip Value", Float ) = 0
		_Ring1("Ring1", Color) = (1,0,0,1)
		_Brightness("Brightness", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TreeTransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		Cull Off
		Stencil
		{
			Ref 1
			CompFront Always
			PassFront Replace
			CompBack Always
			PassBack Replace
		}
		CGPROGRAM
		#pragma target 3.5
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Texture0;
		uniform float4 _Texture0_ST;
		uniform float4 _Ring1;
		uniform float _Brightness;
		uniform float _Cutoff = 0;

		inline fixed4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return fixed4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_Texture0 = i.uv_texcoord * _Texture0_ST.xy + _Texture0_ST.zw;
			float4 tex2DNode118 = tex2D( _Texture0, uv_Texture0 );
			o.Emission = ( tex2DNode118 * _Ring1 * _Brightness ).rgb;
			o.Alpha = 1;
			clip( saturate( tex2DNode118.a ) - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14301
568;91;632;651;1826.135;1295.094;1.968903;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;117;-1539.698,-968.7891;Float;True;Property;_Texture0;Texture 0;0;0;Create;True;None;None;False;black;LockedToTexture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;118;-1249.447,-968.9281;Float;True;Property;_TextureSample1;Texture Sample 1;3;0;Create;True;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;119;-1154.357,-747.9512;Float;False;Property;_Ring1;Ring1;2;0;Create;True;1,0,0,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;149;-1132.697,-553.5921;Float;False;Property;_Brightness;Brightness;3;0;Create;True;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;120;-868.0447,-762.8265;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;151;-745.2072,-588.2575;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-579.2667,-787.7971;Float;False;True;3;Float;ASEMaterialInspector;0;0;Unlit;SH_PigeonTracker;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;True;False;Off;0;0;False;0;0;False;0;Custom;0;True;False;0;True;TreeTransparentCutout;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;1;255;255;7;3;0;0;7;3;0;0;False;2;15;10;25;False;0.5;False;0;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;0;False;0.03;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;1;-1;-1;-1;0;0;0;False;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;118;0;117;0
WireConnection;120;0;118;0
WireConnection;120;1;119;0
WireConnection;120;2;149;0
WireConnection;151;0;118;4
WireConnection;0;2;120;0
WireConnection;0;10;151;0
ASEEND*/
//CHKSM=FE851BEC8B80F6A13B8D6C97232A134A3441D361