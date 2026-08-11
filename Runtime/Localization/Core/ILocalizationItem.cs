using System.Collections.Generic;

namespace F8Framework.Core
{
    /// <summary>
    /// 本地化配置行契约。具体配置程序集只需实现该接口，Core 无需依赖生成的数据类型。
    /// </summary>
    public interface ILocalizationItem
    {
        /// <summary>
        /// 配置行 ID，仅用于定位错误数据。
        /// </summary>
        string Id { get; }

        /// <summary>
        /// 本地化文本 ID。
        /// </summary>
        string TextId { get; }

        /// <summary>
        /// 语言列名称，顺序必须与 <see cref="LanguageValues"/> 一致。
        /// </summary>
        IReadOnlyList<string> LanguageNames { get; }

        /// <summary>
        /// 当前配置行的语言文本，顺序必须与 <see cref="LanguageNames"/> 一致。
        /// </summary>
        IReadOnlyList<string> LanguageValues { get; }
    }
}
