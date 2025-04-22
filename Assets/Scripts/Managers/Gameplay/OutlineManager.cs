using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;

using Custom.Settings;

namespace Custom.Manager
{
    public class OutlineManager : MonoBehaviour
    {
        private class OutlineEntry
        {
            public SpriteRenderer renderer;
            public float thickness;
            public Color color;
            public Sprite originalSprite;
        }



        private static OutlineManager instance;

        private readonly Dictionary<SpriteRenderer, OutlineEntry> outlineLookup = new();

        private readonly UnityEngine.Pool.ObjectPool<SpriteRenderer> outlinePool;



        public OutlineManager() : base()
        {
            outlinePool = new(
                createFunc: CreateFunc,
                actionOnGet: ActionOnGet,
                actionOnRelease: ActionOnRelease,
                collectionCheck: false,
                defaultCapacity: 50);
        }



        private void Awake()
        {
            #region Singleton
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
            #endregion
        }

        private void Start()
        {
            var tagged = GameObject.FindGameObjectsWithTag(VisualSettings.OutlineSettings.defaultTag);
            foreach (var go in tagged)
                Register(go, false);
        }

        private void Update()
        {
            SpriteRenderer renderer;

            foreach (var outlinePair in outlineLookup)
            {
                renderer = outlinePair.Value.renderer;

                renderer.flipX = outlinePair.Key.flipX;
                renderer.flipY = outlinePair.Key.flipY;

                if (outlinePair.Key.sprite != outlinePair.Value.originalSprite)
                {
                    renderer.sprite = CreatePaddedSpriteGPU(outlinePair.Key.sprite, outlinePair.Value.thickness);
                    outlinePair.Value.originalSprite = outlinePair.Key.sprite;
                }
            }
        }



        #region Static 
        /// <summary>
        /// Registers a <see cref="SpriteRenderer"/> to receive an outline effect with optional custom thickness and color.
        /// </summary>
        /// <param name="_target">    The <see cref="SpriteRenderer"/> to apply the outline effect to. </param>
        /// <param name="_thickness"> Optional outline thickness; <br/>
        ///                           uses the default from <see cref="VisualSettings.OutlineSettings"/> if not provided. </param>
        /// <param name="_color">     Optional outline color; <br/>
        ///                           uses the default from <see cref="VisualSettings.OutlineSettings"/> if not provided. </param>
        /// <returns>
        /// <see langword="true"/> if the sprite renderer was successfully registered for outlining.  
        /// </returns>
        public static bool Register(SpriteRenderer _target, float? _thickness = null, Color? _color = null)
        {
            if (instance == null) return false;

            return instance.Register_Core(_target, _thickness, _color);
        }

        /// <inheritdoc cref="Register(SpriteRenderer, float?, Color?)"/>
        /// <param name="_includeChild"> Search for renderer in child? </param>
        public static bool Register(GameObject _target, bool _includeChild = false, float? _thickness = null, Color? _color = null)
        {
            if (instance == null) return false;

            return instance.Register_Core(_target, _includeChild, _thickness, _color);
        }



        /// <summary>
        /// Unregisters a <see cref="SpriteRenderer"/> from receiving the outline effect and releases its associated resources.
        /// </summary>
        /// <param name="_target"> The <see cref="SpriteRenderer"/> to unregister. </param>
        /// <returns>
        /// <see langword="true"/> if the sprite renderer was successfully unregistered and its renderer was released.  
        /// </returns>
        public static bool Unregister(SpriteRenderer _target)
        {
            if (instance == null) return false;

            return instance.Unregister_Core(_target);
        }

        /// <inheritdoc cref="Unregister(SpriteRenderer)"/>
        /// <param name="_includeChild"> Search for renderer in child? </param>
        public static bool Unregister(GameObject _target, bool _includeChild = false)
        {
            if (instance == null) return false;

            return instance.Unregister_Core(_target, _includeChild);
        }
        #endregion

        #region Core
        public bool Register_Core(SpriteRenderer _target, float? _thickness = null, Color? _color = null)
        {
            if (outlineLookup.ContainsKey(_target)) return false;

            var renderer = outlinePool.Get();
            renderer.transform.parent = _target.transform;
            renderer.transform.localScale = Vector3.one;
            renderer.transform.localPosition = Vector3.zero;

            renderer.material.SetFloat("_Thickness", _thickness ?? VisualSettings.OutlineSettings.thickness);
            renderer.material.SetColor("_Color", _color ?? VisualSettings.OutlineSettings.color);

            outlineLookup.Add(_target, new OutlineEntry
            {
                renderer = renderer,
                thickness = _thickness ?? VisualSettings.OutlineSettings.thickness,
                color = _color ?? VisualSettings.OutlineSettings.color,
                originalSprite = null,
            });

            return true;
        }

        public bool Register_Core(GameObject _target, bool _includeChild = false, float? _thickness = null, Color? _color = null)
        {
            SpriteRenderer asRenderer;
            if (_includeChild)
            {
                asRenderer = _target.GetComponentInChildren<SpriteRenderer>();
                if (!asRenderer) return false;
            }
            else
            {
                if (!_target.TryGetComponent(out asRenderer)) return false;
            }

            return Register(asRenderer, _thickness, _color);
        }



        private bool Unregister_Core(SpriteRenderer _target)
        {
            if (!outlineLookup.ContainsKey(_target)) return false;

            outlinePool.Release(outlineLookup[_target].renderer);
            outlineLookup.Remove(_target);

            return true;
        }

        private bool Unregister_Core(GameObject _target, bool _includeChild = false)
        {
            SpriteRenderer asRenderer;
            if (_includeChild)
            {
                asRenderer = _target.GetComponentInChildren<SpriteRenderer>();
                if (!asRenderer) return false;
            }
            else
            {
                if (!_target.TryGetComponent(out asRenderer)) return false;
            }

            return Unregister(asRenderer);
        }
        #endregion



        #region Pool Callbacks
        private static SpriteRenderer CreateFunc()
        {
            GameObject go = new($"Outline Renderer ({instance.outlinePool.CountAll})");
            go.transform.parent = instance.transform;

            SortingGroup sortGroup = go.AddComponent<SortingGroup>();
            sortGroup.sortAtRoot = true;
            sortGroup.sortingLayerID = VisualSettings.OutlineSettings.defaultSortingLayer;

            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            renderer.material = new Material(VisualSettings.OutlineSettings.material);

            return renderer;
        }

        private static void ActionOnGet(SpriteRenderer _renderer)
        {
            _renderer.gameObject.SetActive(true);
        }

        private static void ActionOnRelease(SpriteRenderer _renderer)
        {
            _renderer.gameObject.SetActive(false);
            _renderer.transform.parent = instance.transform;
            _renderer.sprite = null;
        }
        #endregion

        #region Utility
        /// <summary>
        /// Creates a new Sprite with transparent padding around the original Sprite.
        /// </summary>
        /// <param name="_originalSprite">  The source <see cref="Sprite"/>. </param>
        /// <param name="_padding">         The amount of transparent padding (in pixels). </param>
        /// <returns>
        /// A new Sprite with padding.
        /// </returns>
        public Sprite CreatePaddedSpriteGPU(Sprite _originalSprite, float _padding)
        {
            if (_originalSprite == null) return null;

            Rect spriteRect = _originalSprite.rect;
            Texture2D sourceTexture = _originalSprite.texture;

            int pixelPadding = Mathf.CeilToInt(_padding);

            int srcWidth = (int)spriteRect.width;
            int srcHeight = (int)spriteRect.height;
            int paddedWidth = srcWidth + pixelPadding * 2;
            int paddedHeight = srcHeight + pixelPadding * 2;

            // Create a RenderTexture with padding
            RenderTexture rt = new(paddedWidth, paddedHeight, 0, RenderTextureFormat.ARGB32)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            // Calculate UV rect within the original texture
            Rect uvRect = new(
                spriteRect.x / sourceTexture.width,
                spriteRect.y / sourceTexture.height,
                spriteRect.width / sourceTexture.width,
                spriteRect.height / sourceTexture.height
            );

            // Draw the sprite into the padded RenderTexture
            RenderTexture.active = rt;
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, paddedWidth, paddedHeight, 0);
            GL.Clear(true, true, Color.clear);
            Graphics.DrawTexture(new(pixelPadding, pixelPadding, srcWidth, srcHeight), sourceTexture, uvRect, 0, 0, 0, 0);
            GL.PopMatrix();

            // Read the RenderTexture into a Texture2D
            Texture2D paddedTexture = new(paddedWidth, paddedHeight, TextureFormat.RGBA32, false);
            paddedTexture.ReadPixels(new Rect(0, 0, paddedWidth, paddedHeight), 0, 0);
            paddedTexture.Apply();
            paddedTexture.filterMode = sourceTexture.filterMode;

            // Calculate the new pivot based on padding
            Vector2 originalPivot = _originalSprite.pivot;
            Vector2 newPivot = new(
                (originalPivot.x + pixelPadding) / paddedWidth,
                (originalPivot.y + pixelPadding) / paddedHeight
            );

            // Create and return the new sprite
            Sprite paddedSprite = Sprite.Create(
                paddedTexture,
                new Rect(0, 0, paddedWidth, paddedHeight),
                newPivot,
                _originalSprite.pixelsPerUnit,
                0,
                SpriteMeshType.FullRect
            );

            // Cleanup allocations.
            RenderTexture.active = null;
            Destroy(rt);

            return paddedSprite;
        }
        #endregion
    }
}
