// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_VFX_Blood"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_BloodColor("BloodColor", Color) = (1,0,0,0)
		_DiffuseTexture("Diffuse Texture", 2D) = "white" {}
		_Brightness("Brightness", Range( 0 , 1)) = 0
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
			float4 vertexColor : COLOR;
		};

		uniform float4 _BloodColor;
		uniform sampler2D _DiffuseTexture;
		uniform float4 _DiffuseTexture_ST;
		uniform float _Brightness;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_DiffuseTexture = i.uv_texcoord * _DiffuseTexture_ST.xy + _DiffuseTexture_ST.zw;
			float4 tex2DNode20 = tex2D( _DiffuseTexture, uv_DiffuseTexture );
			o.Emission = saturate( ( ( _BloodColor * tex2DNode20.r ) + ( tex2DNode20.g * _Brightness ) ) ).rgb;
			o.Alpha = 1;
			clip( ( tex2DNode20.r * i.vertexColor.a ) - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14301
1440;91;480;927;1395.456;622.4456;1.688959;False;False
Node;AmplifyShaderEditor.TexturePropertyNode;19;-1396.201,-138.7806;Float;True;Property;_DiffuseTexture;Diffuse Texture;2;0;Create;True;None;None;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-1031.004,166.9172;Float;False;Property;_Brightness;Brightness;3;0;Create;True;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;20;-1126.636,-138.2099;Float;True;Property;_TextureSample0;Texture Sample 0;6;0;Create;True;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;2;-977.8083,-370.049;Float;False;Property;_BloodColor;BloodColor;1;0;Create;True;1,0,0,0;0.3525109,0.3122297,0.5661765,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-712.1976,-133.3532;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-713.4795,-42.51369;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;68;-350.353,-103.3162;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;55;-677.3736,93.7483;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;71;-678.3611,196.9344;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;24;-216.2821,-97.25121;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-423.7743,103.4532;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-43.49139,-141.9115;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SH_VFX_Blood;False;False;False;False;True;True;True;True;True;False;False;False;False;False;False;True;False;Off;0;0;False;0;0;False;0;Custom;0.5;True;False;0;True;TransparentCutout;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;False;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexScale;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;20;0;19;0
WireConnection;32;0;2;0
WireConnection;32;1;20;1
WireConnection;69;0;20;2
WireConnection;69;1;70;0
WireConnection;68;0;32;0
WireConnection;68;1;69;0
WireConnection;55;0;20;1
WireConnection;24;0;68;0
WireConnection;72;0;55;0
WireConnection;72;1;71;4
WireConnection;0;2;24;0
WireConnection;0;10;72;0
ASEEND*/
//CHKSM=B836B25F850C21773DAE4D4F21C1FC6F192675D5