using QuanLyKhoaHoc.Models;

namespace QuanLyKhoaHoc.Services;

public class CurrentUser(IHttpContextAccessor accessor)
{
    private const string AccountIdKey = "AccountId";
    private const string AccountNameKey = "AccountName";
    private const string RoleKey = "AccountRole";

    public int? AccountId => accessor.HttpContext?.Session.GetInt32(AccountIdKey);
    public string? Name => accessor.HttpContext?.Session.GetString(AccountNameKey);
    public string? Role => accessor.HttpContext?.Session.GetString(RoleKey);
    public bool IsSignedIn => AccountId.HasValue;

    public void SignIn(TaiKhoan account)
    {
        var session = accessor.HttpContext?.Session
            ?? throw new InvalidOperationException("Session is not available.");
        session.SetInt32(AccountIdKey, account.MaTaiKhoan);
        session.SetString(AccountNameKey, account.HoTen);
        session.SetString(RoleKey, account.VaiTro.ToString());
    }

    public void SignOut() => accessor.HttpContext?.Session.Clear();
}
