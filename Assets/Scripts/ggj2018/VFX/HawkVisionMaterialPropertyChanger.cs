using ggj2018.Core.Util;

using UnityEngine;

[ExecuteInEditMode]
public class HawkVisionMaterialPropertyChanger : MonoBehaviour
{
    [SerializeField]
    private bool _enabled;

    public bool Enabled { get { return _enabled; } set { _enabled = value; } }

	public Color color1; // color you want the camera to render it as
	public Color color2;
	public float brightness1;
	public float brightness2;
	public Material material; // material you want the camera to change
	public string colorProperty1; // name of the color property in the material's shader
	public string colorProperty2;
	public string brightnessProperty1;
	public string brightnessProperty2;

    void Awake() {
        _defaultColor1 = material.GetColor(colorProperty1);
        _defaultColor2 = material.GetColor(colorProperty2);
        _defaultFloat1 = material.GetFloat(brightnessProperty1);
        _defaultFloat2 = material.GetFloat(brightnessProperty2);
    }

	void OnPreRender() {
        if(!Enabled) {
            return;
        }

		material.SetColor(colorProperty1, color1);
		material.SetColor(colorProperty2, color2);
		material.SetFloat(brightnessProperty1, brightness1);
		material.SetFloat(brightnessProperty2, brightness2);
	}

	void OnPostRender() {
	    if(!Enabled) {
	        return;
	    }

		material.SetColor(colorProperty1, _defaultColor1);
		material.SetColor(colorProperty2, _defaultColor2);
		material.SetFloat(brightnessProperty1, _defaultFloat1);
		material.SetFloat(brightnessProperty2, _defaultFloat2);
	}

    [SerializeField]
    [ReadOnly]
	private Color _defaultColor1;

    [SerializeField]
    [ReadOnly]
	private Color _defaultColor2;

    [SerializeField]
    [ReadOnly]
	private float _defaultFloat1;

    [SerializeField]
    [ReadOnly]
	private float _defaultFloat2;
}