// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_BuildingStencil"
{
	Properties
	{
		_Color("Color", Color) = (0.7411765,0.8039216,0.9098039,1)
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_AO("AO", 2D) = "black" {}
		_AOMult("AO Mult", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+2" }
		Cull Back
		Stencil
		{
			Ref 1
			Comp NotEqual
			Pass Zero
			Fail Keep
		}
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Color;
		uniform float _AOMult;
		uniform sampler2D _AO;
		uniform float4 _AO_ST;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_AO = i.uv_texcoord * _AO_ST.xy + _AO_ST.zw;
			o.Albedo = ( _Color * ( _AOMult * tex2D( _AO, uv_AO ) ) ).rgb;
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
2357;107;1083;879;1357.503;957.6637;1.898408;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;90;-908.2507,-147.3096;Float;True;Property;_AO;AO;3;0;Create;True;None;None;False;black;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;93;-425.4391,-261.7488;Float;False;Property;_AOMult;AO Mult;4;0;Create;True;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;89;-645.8497,-148.8902;Float;True;Property;_TextureSample0;Texture Sample 0;6;0;Create;True;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;-241.1606,-170.0743;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;85;-227.8415,-350.8154;Float;False;Property;_Color;Color;0;0;Create;True;0.7411765,0.8039216,0.9098039,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;88;-93.73583,-52.32181;Float;False;Property;_Smoothness;Smoothness;2;0;Create;True;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;35.16333,-191.8872;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;358.3733,-140.3082;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SH_BuildingStencil;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;False;0;Custom;0.5;True;True;2;True;Opaque;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;1;255;255;6;2;1;0;7;3;0;0;False;2;15;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexScale;False;False;Cylindrical;False;Relative;0;;1;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;89;0;90;0
WireConnection;92;0;93;0
WireConnection;92;1;89;0
WireConnection;91;0;85;0
WireConnection;91;1;92;0
WireConnection;0;0;91;0
WireConnection;0;4;88;0
ASEEND*/
//CHKSM=44EC42EE36D9351D393B1F5A48D19029145A0AB9