// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_UI_AdditiveGlow"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		_Mask("Mask", 2D) = "white" {}
		_GlowColor("Glow Color", Color) = (1,1,1,0)
		_Maximum("Maximum", Float) = 1
		_Minimum("Minimum", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
			
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		
		ZTest [unity_GUIZTestMode]
		Blend One One
		ColorMask [_ColorMask]


		Pass
		{
			Name "Default"
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			#include "UnityShaderVariables.cginc"

			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			uniform fixed4 _Color;
			uniform fixed4 _TextureSampleAdd;
			uniform float4 _ClipRect;
			uniform sampler2D _MainTex;
			uniform float4 _GlowColor;
			uniform sampler2D _Mask;
			uniform float4 _Mask_ST;
			uniform float _Minimum;
			uniform float _Maximum;
			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.worldPosition = IN.vertex;
				
				OUT.worldPosition.xyz +=  float3( 0, 0, 0 ) ;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN  ) : SV_Target
			{
				float2 uv_Mask = IN.texcoord.xy * _Mask_ST.xy + _Mask_ST.zw;
				float4 appendResult4 = (float4(_GlowColor.r , _GlowColor.g , _GlowColor.b , tex2D( _Mask, uv_Mask ).r));
				float mulTime12 = _Time.y * 2.0;
				
				half4 color = ( appendResult4 * (_Minimum + (sin( mulTime12 ) - -1.0) * (_Maximum - _Minimum) / (1.0 - -1.0)) );
				
				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				
				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14301
477;91;634;656;1076.024;317.7887;1.545008;True;False
Node;AmplifyShaderEditor.RangedFloatNode;14;-878.4926,303.027;Float;False;Constant;_Float0;Float 0;1;0;Create;True;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;2;-1234.713,73.99255;Float;True;Property;_Mask;Mask;0;0;Create;True;7dd8b43def561b9409780041acb54f1b;7dd8b43def561b9409780041acb54f1b;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleTimeNode;12;-731.3521,306.7998;Float;False;1;0;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;5;-848.657,-150.4255;Float;False;Property;_GlowColor;Glow Color;1;0;Create;True;1,1,1,0;1,1,1,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;20;-593.071,467.0756;Float;False;Property;_Maximum;Maximum;2;0;Create;True;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-593.7719,382.1;Float;False;Property;_Minimum;Minimum;3;0;Create;True;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-925.4053,75.92932;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;10;-548.5941,306.4414;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;4;-501.6005,-0.4624577;Float;True;COLOR;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;15;-403.1158,306.7997;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;-1.0;False;2;FLOAT;1.0;False;3;FLOAT;0.0;False;4;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-175.7411,-3.277341;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMasterNode;0;1.105642,-3.664273;Float;False;True;2;Float;ASEMaterialInspector;0;3;SH_UI_AdditiveGlow;5056123faa0c79b47ab6ad7e8bf059a4;ASETemplateShaders/UIDefault;4;One;One;0;One;One;Off;0;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;12;0;14;0
WireConnection;1;0;2;0
WireConnection;10;0;12;0
WireConnection;4;0;5;1
WireConnection;4;1;5;2
WireConnection;4;2;5;3
WireConnection;4;3;1;0
WireConnection;15;0;10;0
WireConnection;15;3;19;0
WireConnection;15;4;20;0
WireConnection;7;0;4;0
WireConnection;7;1;15;0
WireConnection;0;0;7;0
ASEEND*/
//CHKSM=908DF6F21091B2FC68CDC10FB33FA8BB22D41A18