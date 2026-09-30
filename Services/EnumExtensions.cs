using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace QuanLyKhoaHoc.Services;

public static class EnumExtensions
{
    public static string ToDisplayName(this Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        return member?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? value.ToString();
    }
}
