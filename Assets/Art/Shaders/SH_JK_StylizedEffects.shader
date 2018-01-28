// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_JK_StylizedEffects"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0
		_Texture0("Texture 0", 2D) = "black" {}
		_Energy("Energy", Color) = (0,0,1,1)
		_Ring2("Ring2", Color) = (0,1,0,1)
		_Ring1("Ring1", Color) = (1,0,0,1)
		_EnergyEmissive("EnergyEmissive", Float) = 1
		_Ring1Emissive("Ring1Emissive", Float) = 1
		_Ring2Emissive("Ring2Emissive", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 3.5
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Ring1;
		uniform sampler2D _Texture0;
		uniform float4 _Texture0_ST;
		uniform float _Ring1Emissive;
		uniform float4 _Ring2;
		uniform float _Ring2Emissive;
		uniform float4 _Energy;
		uniform float _EnergyEmissive;
		uniform float _Cutoff = 0;

		inline fixed4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return fixed4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_Texture0 = i.uv_texcoord * _Texture0_ST.xy + _Texture0_ST.zw;
			float4 tex2DNode118 = tex2D( _Texture0, uv_Texture0 );
			o.Emission = ( ( _Ring1 * tex2DNode118.r * _Ring1Emissive ) + ( _Ring2 * tex2DNode118.g * _Ring2Emissive ) + ( _Energy * tex2DNode118.b * _EnergyEmissive ) ).rgb;
			o.Alpha = 1;
			float clampResult146 = clamp( ( tex2DNode118.r + tex2DNode118.g + tex2DNode118.b ) , 0.0 , 1.0 );
			clip( clampResult146 - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14301
1440;91;480;927;1857.289;1922.719;1.902061;False;False
Node;AmplifyShaderEditor.TexturePropertyNode;117;-2046.717,-788.9103;Float;True;Property;_Texture0;Texture 0;1;0;Create;True;None;None;False;black;LockedToTexture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.ColorNode;119;-1655.64,-1587.211;Float;False;Property;_Ring1;Ring1;4;0;Create;True;1,0,0,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;121;-1653.043,-1336.459;Float;False;Property;_Ring2;Ring2;3;0;Create;True;0,1,0,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;118;-1756.466,-789.0493;Float;True;Property;_TextureSample1;Texture Sample 1;3;0;Create;True;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;123;-1653.645,-1089.42;Float;False;Property;_Energy;Energy;2;0;Create;True;0,0,1,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;150;-1645.938,-1169.13;Float;False;Property;_Ring2Emissive;Ring2Emissive;7;0;Create;True;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;149;-1645.141,-1416.424;Float;False;Property;_Ring1Emissive;Ring1Emissive;6;0;Create;True;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;148;-1654.7,-920.6791;Float;False;Property;_EnergyEmissive;EnergyEmissive;5;0;Create;True;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;122;-1134.73,-1063.45;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;145;-1412.199,-585.3135;Float;False;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;120;-1137.271,-1165.968;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;124;-1133.713,-961.7164;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;146;-774.765,-575.7388;Float;False;3;0;FLOAT;0,0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;129;-968.2708,-1086.048;Float;False;3;3;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-579.2667,-787.7971;Float;False;True;3;Float;ASEMaterialInspector;0;0;Unlit;SH_JK_StylizedEffects;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;True;False;Off;0;0;False;0;0;False;0;Custom;0;True;False;0;True;TransparentCutout;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;False;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0.03;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;False;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;118;0;117;0
WireConnection;122;0;121;0
WireConnection;122;1;118;2
WireConnection;122;2;150;0
WireConnection;145;0;118;1
WireConnection;145;1;118;2
WireConnection;145;2;118;3
WireConnection;120;0;119;0
WireConnection;120;1;118;1
WireConnection;120;2;149;0
WireConnection;124;0;123;0
WireConnection;124;1;118;3
WireConnection;124;2;148;0
WireConnection;146;0;145;0
WireConnection;129;0;120;0
WireConnection;129;1;122;0
WireConnection;129;2;124;0
WireConnection;0;2;129;0
WireConnection;0;10;146;0
ASEEND*/
//CHKSM=FCC7BC7E08B201145FBA3450E709B1E26C71AAF2