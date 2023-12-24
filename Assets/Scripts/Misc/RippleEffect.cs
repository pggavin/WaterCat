using UnityEngine;
using SF = UnityEngine.SerializeField;

public class RippleEffect : MonoSingleton<RippleEffect>
{
    [SF] AnimationCurve _waveform = new(
        new Keyframe(0.00f, 0.50f, 0, 0),
        new Keyframe(0.05f, 1.00f, 0, 0),
        new Keyframe(0.15f, 0.10f, 0, 0),
        new Keyframe(0.25f, 0.80f, 0, 0),
        new Keyframe(0.35f, 0.30f, 0, 0),
        new Keyframe(0.45f, 0.60f, 0, 0),
        new Keyframe(0.55f, 0.40f, 0, 0),
        new Keyframe(0.65f, 0.55f, 0, 0),
        new Keyframe(0.75f, 0.46f, 0, 0),
        new Keyframe(0.85f, 0.52f, 0, 0),
        new Keyframe(0.99f, 0.50f, 0, 0)
    );

    [Range(0.01f, 1.0f)]
    [SF] float _refractionStrength = 0.5f;
    [SF] Color _reflectionColor = new(0.0f, 0.943f, 1.0f);

    [Range(0.01f, 1.0f)]
    [SF] float _reflectionStrength = 0.7f;

    [Range(1.0f, 5.0f)]
    [SF] float _waveSpeed = 1.25f;

    [Range(0.0f, 2.0f)]
    [SF] float _dropInterval = 0.5f;

    [SF] Shader _shader;

    Droplet[] _droplets;
    Texture2D _gradientTexture;
    Material _material;
    float _timer;
    int _dropCount;

    void UpdateShaderParameters()
    {
        var c = GetComponent<Camera>();

        _material.SetVector("_Drop1", _droplets[0].MakeShaderParameter(c.aspect));
        _material.SetVector("_Drop2", _droplets[1].MakeShaderParameter(c.aspect));
        _material.SetVector("_Drop3", _droplets[2].MakeShaderParameter(c.aspect));

        _material.SetColor("_Reflection", _reflectionColor);
        _material.SetVector("_Params1", new Vector4(c.aspect, 1, 1 / _waveSpeed, 0));
        _material.SetVector("_Params2", new Vector4(1, 1 / c.aspect, _refractionStrength, _reflectionStrength));
    }

    protected override void Initialize()
    {
        _droplets = new Droplet[3];
        _droplets[0] = new Droplet();
        _droplets[1] = new Droplet();
        _droplets[2] = new Droplet();

        _gradientTexture = new Texture2D(2048, 1, TextureFormat.Alpha8, false);
        _gradientTexture.wrapMode = TextureWrapMode.Clamp;
        _gradientTexture.filterMode = FilterMode.Bilinear;
        for (var i = 0; i < _gradientTexture.width; i++)
        {
            var x = 1.0f / _gradientTexture.width * i;
            var a = _waveform.Evaluate(x);
            _gradientTexture.SetPixel(i, 0, new Color(a, a, a, a));
        }
        _gradientTexture.Apply();

        _material = new Material(_shader);
        _material.hideFlags = HideFlags.DontSave;
        _material.SetTexture("_GradTex", _gradientTexture);

        UpdateShaderParameters();
    }

    void Update()
    {
        if (_dropInterval > 0)
        {
            _timer += Time.deltaTime;

            while (_timer > _dropInterval)
            {
                _timer -= _dropInterval;
            }
        }

        foreach (var d in _droplets)
        {
            d.Update();
        }

        UpdateShaderParameters();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, _material);
    }

    public void Emit(Vector2 pos)
    {
        _dropCount++;
        var dropletIndex = _dropCount % _droplets.Length;
        _droplets[dropletIndex].Reset(pos);
    }

    class Droplet
    {
        Vector2 position;
        float time;

        public Droplet()
        {
            time = 1000;
        }

        public void Reset(Vector2 pos)
        {
            position = pos;
            time = 0;
        }

        public void Update()
        {
            time += Time.deltaTime * 2;
        }

        public Vector4 MakeShaderParameter(float aspect)
        {
            return new Vector4(position.x * aspect, position.y, time, 0);
        }
    }
}