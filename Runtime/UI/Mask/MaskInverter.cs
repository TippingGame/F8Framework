using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

namespace F8Framework.Core
{
    public sealed class MaskInverter : MonoBehaviour, IMaterialModifier
    {
        private static readonly int _stencilComp = Shader.PropertyToID("_StencilComp");

        public Material GetModifiedMaterial(Material baseMaterial)
        {
            var resultMaterial = new Material(baseMaterial);
            resultMaterial.SetFloat(_stencilComp, Convert.ToSingle(CompareFunction.NotEqual));
            return resultMaterial;
        }
    }
}