/* 
1.代码文件中的内容排序：
外部别名指令
使用指令
名称空间
委托
枚举
接口
结构体
类

2.类、记录、结构或接口中的内容排序：
字段
构造函数
析构函数（终结器）
委托
事件
枚举
接口
属性
索引器
方法
结构体
嵌套类和记录
*/

namespace CChess;

internal class Utility
{
    public delegate string Show<T>(T t);
    public static string GetString<T>(List<T> items, Show<T> show, string split = "")
    {
        System.Text.StringBuilder builder = new();
        foreach(T item in items)
            builder.Append(show(item)).Append(split);

        builder.Append($"【{items.Count}】");
        return builder.ToString();
    }
}

