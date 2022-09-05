using UnityEngine;

namespace SequenceFeedBack
{
    /// <summary>
    /// 只读属性，能看到值但不能修改
    /// </summary>
    public class MMFReadOnlyAttribute : PropertyAttribute { }
    
    
    /// <summary>
    /// 创建按钮（绑定方法名字）
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class MMFInspectorButtonAttribute : PropertyAttribute
    {
        public readonly string MethodName;

        public MMFInspectorButtonAttribute(string MethodName)
        {
            this.MethodName = MethodName;
        }
    }
}