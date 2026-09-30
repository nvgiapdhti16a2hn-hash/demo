using Microsoft.AspNetCore.Mvc;
using QuanLyKhoaHoc.Services;

namespace QuanLyKhoaHoc.Controllers;

public abstract class AppController(CurrentUser currentUser) : Controller
{
    protected CurrentUser CurrentUser { get; } = currentUser;

    protected bool RequireSignedIn()
    {
        if (CurrentUser.IsSignedIn)
            return true;
        TempData["Error"] = "Vui lòng đăng nhập để tiếp tục.";
        return false;
    }

    protected bool RequireRole(params string[] roles)
    {
        if (!RequireSignedIn())
            return false;
        if (roles.Contains(CurrentUser.Role, StringComparer.Ordinal))
            return true;
        TempData["Error"] = "Bạn không có quyền thực hiện chức năng này.";
        return false;
    }
}
