using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace SmartifyOS.UI.TranslucentImage
{
    public partial class BlurredImage
    {
        [Tooltip("Blend between the sprite and background blur")]
        [Range(0, 1)]
        [FormerlySerializedAs("spriteBlending")]
        public float m_spriteBlending = .65f;

        public float spriteBlending
        {
            get => m_spriteBlending;
            set
            {
                m_spriteBlending = value;
                SetVerticesDirty();
            }
        }

        public virtual void ModifyMesh(VertexHelper vh)
        {
            List<UIVertex> vertices = new List<UIVertex>();
            vh.GetUIVertexStream(vertices);

            for (var i = 0; i < vertices.Count; i++)
            {
                UIVertex moddedVertex = vertices[i];
                moddedVertex.uv1 = new Vector2(spriteBlending,
                                               0 //No use for this yet
                                              );
                vertices[i] = moddedVertex;
            }

            vh.Clear();
            vh.AddUIVertexTriangleStream(vertices);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetVerticesDirty();
#if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Start();
            }
#endif
        }

        protected override void OnDisable()
        {
            SetVerticesDirty();
            base.OnDisable();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            SetVerticesDirty();
            base.OnDidApplyAnimationProperties();
        }

        public virtual void ModifyMesh(Mesh mesh)
        {
            using (var vh = new VertexHelper(mesh))
            {
                ModifyMesh(vh);
                vh.FillMesh(mesh);
            }
        }
    }
}
