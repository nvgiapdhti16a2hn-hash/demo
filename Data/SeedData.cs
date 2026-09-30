using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Models;

namespace QuanLyKhoaHoc.Data;

public static class SeedData
{
    private const string SamplePassword = "Test123!";

    private static readonly string[] ProgramNames =
    [
        "Tiếng Anh giao tiếp",
        "Tiếng Anh thiếu nhi",
        "Tiếng Anh thương mại",
        "Tiếng Nhật sơ cấp",
        "Tin học văn phòng"
    ];

    public static async Task InitializeAsync(IServiceProvider services, AppDbContext db)
    {
        var hasher = services.GetRequiredService<IPasswordHasher<TaiKhoan>>();
        var accounts = await EnsureAccountsAsync(db, hasher);
        var programs = await EnsureProgramsAsync(db);
        var classes = await EnsureClassesAsync(db, programs);
        var applications = await EnsureApplicationsAsync(db, accounts, classes);
        await EnsureSchedulesAndResultsAsync(db, applications);
    }

    private static async Task<List<TaiKhoan>> EnsureAccountsAsync(
        AppDbContext db,
        IPasswordHasher<TaiKhoan> hasher)
    {
        await EnsureAccountAsync(db, hasher, "admin", "Quản trị viên", VaiTro.Admin);
        await EnsureAccountAsync(db, hasher, "admin02", "Quản trị viên mẫu 02", VaiTro.Admin);
        await EnsureAccountAsync(db, hasher, "staff", "Nhân viên đào tạo", VaiTro.NhanVienDaoTao);
        await EnsureAccountAsync(db, hasher, "staff02", "Nhân viên đào tạo mẫu 02", VaiTro.NhanVienDaoTao);

        for (var number = 1; number <= 30; number++)
        {
            var username = StudentUsername(number);
            await EnsureAccountAsync(db, hasher, username, $"Học viên mẫu {number:00}", VaiTro.HocVien);
        }

        await db.SaveChangesAsync();

        var studentUsernames = Enumerable.Range(1, 30).Select(StudentUsername).ToArray();
        var students = await db.TaiKhoans
            .Where(x => studentUsernames.Contains(x.TenDangNhap) && x.VaiTro == VaiTro.HocVien)
            .Include(x => x.HocVien)
            .ToListAsync();

        foreach (var student in students)
        {
            if (student.HocVien is not null)
                continue;

            var number = student.TenDangNhap == "student"
                ? 1
                : int.Parse(student.TenDangNhap["student".Length..]);
            student.HocVien = new HocVien
            {
                HoTen = student.HoTen,
                Email = student.Email,
                NgaySinh = DateTime.Today.AddYears(-18 - number % 15),
                GioiTinh = number % 2 == 0 ? "Nữ" : "Nam",
                SoDienThoai = $"090{number:0000000}",
                DiaChi = $"Địa chỉ mẫu {number:00}",
                TrinhDoHienTai = (number % 3) switch
                {
                    0 => "Cơ bản",
                    1 => "Trung cấp",
                    _ => "Chưa xác định"
                },
                NgheNghiep = number % 2 == 0 ? "Sinh viên" : "Nhân viên",
                SoThangDaHoc = number % 13,
                MucTieuHoc = "Nâng cao kiến thức và kỹ năng."
            };
        }

        await db.SaveChangesAsync();

        if (students.Count < 30)
            throw new InvalidOperationException("Không tạo đủ 30 tài khoản học viên mẫu.");

        return students.OrderBy(x => x.TenDangNhap, StringComparer.Ordinal).ToList();
    }

    private static async Task EnsureAccountAsync(
        AppDbContext db,
        IPasswordHasher<TaiKhoan> hasher,
        string username,
        string fullName,
        VaiTro role)
    {
        if (await db.TaiKhoans.AnyAsync(x => x.TenDangNhap == username))
            return;

        var account = new TaiKhoan
        {
            TenDangNhap = username,
            HoTen = fullName,
            Email = $"{username}@example.com",
            VaiTro = role,
            TrangThai = TrangThaiTaiKhoan.HoatDong
        };
        account.MatKhau = hasher.HashPassword(account, SamplePassword);
        db.TaiKhoans.Add(account);
    }

    private static async Task<List<ChuongTrinhDaoTao>> EnsureProgramsAsync(AppDbContext db)
    {
        var programs = await db.ChuongTrinhDaoTaos
            .OrderBy(x => x.MaChuongTrinhDaoTao)
            .ToListAsync();

        foreach (var name in ProgramNames)
        {
            if (programs.Count >= 5)
                break;
            if (programs.Any(x => x.TenChuongTrinhDaoTao == name))
                continue;

            var program = new ChuongTrinhDaoTao
            {
                TenChuongTrinhDaoTao = name,
                MoTa = $"Chương trình mẫu {name}, dùng để kiểm thử quy trình đăng ký và quản lý lớp.",
                EmailLienHe = "training@example.com",
                TrangThai = TrangThaiChuongTrinh.HoatDong
            };
            db.ChuongTrinhDaoTaos.Add(program);
            programs.Add(program);
        }

        await db.SaveChangesAsync();

        if (programs.Count < 5)
            throw new InvalidOperationException("Không tạo đủ 05 chương trình đào tạo mẫu.");

        return await db.ChuongTrinhDaoTaos
            .OrderBy(x => x.MaChuongTrinhDaoTao)
            .ToListAsync();
    }

    private static async Task<List<LopHoc>> EnsureClassesAsync(
        AppDbContext db,
        IReadOnlyList<ChuongTrinhDaoTao> programs)
    {
        var existingClasses = await db.LopHocs.ToListAsync();
        for (var sampleIndex = 0; sampleIndex < 15; sampleIndex++)
        {
            var className = $"Demo - Lớp {sampleIndex + 1:00} - {programs[sampleIndex % programs.Count].TenChuongTrinhDaoTao}";
            if (existingClasses.Any(x => x.TenLop == className))
                continue;

            var status = sampleIndex switch
            {
                < 5 => TrangThaiLop.DangMoDangKy,
                < 10 => TrangThaiLop.DaDong,
                _ => TrangThaiLop.DangMoDangKy
            };
            var expired = sampleIndex >= 10;
            var registrationStart = expired
                ? DateTime.Today.AddDays(-45)
                : DateTime.Today.AddDays(-2);
            var registrationEnd = expired
                ? DateTime.Today.AddDays(-1)
                : DateTime.Today.AddDays(20 + sampleIndex);
            var program = programs[sampleIndex % programs.Count];

            db.LopHocs.Add(new LopHoc
            {
                TenLop = $"Demo - Lớp {sampleIndex + 1:00} - {program.TenChuongTrinhDaoTao}",
                MaChuongTrinhDaoTao = program.MaChuongTrinhDaoTao,
                SiSoToiDa = 20 + sampleIndex,
                TrinhDoDauVao = (sampleIndex % 3) switch
                {
                    0 => "Cơ bản",
                    1 => "Trung cấp",
                    _ => "Nâng cao"
                },
                DiemDauVaoToiThieu = sampleIndex * 3,
                NgayBatDauDangKy = registrationStart,
                HanDangKy = registrationEnd,
                MoTaLopHoc = "Lớp mẫu để kiểm thử tìm kiếm, lọc, sắp xếp và phân trang.",
                YeuCauHocVien = "Hoàn thiện hồ sơ cá nhân trước khi đăng ký.",
                TrangThai = status
            });
        }

        await db.SaveChangesAsync();
        var classes = await db.LopHocs.OrderBy(x => x.MaLop).ToListAsync();
        if (classes.Count < 15)
            throw new InvalidOperationException("Không tạo đủ 15 lớp học mẫu.");
        return classes;
    }

    private static async Task<List<HoSoDangKyHoc>> EnsureApplicationsAsync(
        AppDbContext db,
        IReadOnlyList<TaiKhoan> studentAccounts,
        IReadOnlyList<LopHoc> classes)
    {
        var students = await db.HocViens
            .Where(x => studentAccounts.Select(a => a.MaTaiKhoan).Contains(x.MaTaiKhoan))
            .OrderBy(x => x.MaHocVien)
            .ToListAsync();
        var existingApplications = await db.HoSoDangKyHocs
            .Include(x => x.LichKiemTras)
            .Include(x => x.KetQuaXepLop)
            .ToListAsync();
        var existingPairs = existingApplications
            .Select(x => (x.MaHocVien, x.MaLop))
            .ToHashSet();
        var applicationsByPair = existingApplications
            .GroupBy(x => (x.MaHocVien, x.MaLop))
            .ToDictionary(x => x.Key, x => x.First());
        var sampleApplications = new List<HoSoDangKyHoc>(45);

        for (var index = 0; index < 45; index++)
        {
            var classIndex = index % 15;
            var studentIndex = (classIndex * 3 + index / 15) % students.Count;
            var student = students[studentIndex];
            var targetClass = classes[classIndex % classes.Count];
            var pair = (student.MaHocVien, targetClass.MaLop);

            if (applicationsByPair.TryGetValue(pair, out var existing))
            {
                sampleApplications.Add(existing);
                continue;
            }

            if (existingPairs.Contains(pair))
                continue;

            var status = GetApplicationStatus(index);
            var application = new HoSoDangKyHoc
            {
                MaHocVien = student.MaHocVien,
                MaLop = targetClass.MaLop,
                NgayNop = DateTime.Today.AddDays(-(index % 35 + 1)).AddHours(9),
                LyDoDangKy = $"Học viên muốn tham gia lớp để nâng cao trình độ. (Mẫu {index + 1:00})",
                GhiChu = $"Dữ liệu minh họa hồ sơ {index + 1:00}.",
                TrangThai = status,
                NgayXuLy = status == TrangThaiHoSo.ChoDuyet
                    ? null
                    : DateTime.Today.AddDays(-(index % 20)).AddHours(15),
                NhanXetXetDuyet = status switch
                {
                    TrangThaiHoSo.DuDieuKien => "Hồ sơ đủ điều kiện theo đánh giá của nhân viên.",
                    TrangThaiHoSo.KhongDuDieuKien => "Hồ sơ chưa đáp ứng yêu cầu lớp học.",
                    TrangThaiHoSo.DaHuy => "Học viên đã hủy hồ sơ mẫu.",
                    _ => null
                }
            };

            db.HoSoDangKyHocs.Add(application);
            existingPairs.Add(pair);
            applicationsByPair.Add(pair, application);
            sampleApplications.Add(application);
        }

        await db.SaveChangesAsync();

        if (await db.HoSoDangKyHocs.CountAsync() < 45)
            throw new InvalidOperationException("Không tạo đủ 45 hồ sơ đăng ký học mẫu.");

        return sampleApplications;
    }

    private static async Task EnsureSchedulesAndResultsAsync(
        AppDbContext db,
        IReadOnlyList<HoSoDangKyHoc> applications)
    {
        var scheduledApplications = applications
            .Where(x => x.TrangThai is TrangThaiHoSo.ChoKiemTraDauVao
                or TrangThaiHoSo.DaKiemTraDauVao
                or TrangThaiHoSo.DuocXepLop
                or TrangThaiHoSo.ChuaDuocXepLop)
            .Take(18)
            .ToList();

        var scheduleCount = await db.LichKiemTraDauVaos.CountAsync();
        for (var index = 0; index < scheduledApplications.Count && scheduleCount < 18; index++)
        {
            var application = scheduledApplications[index];
            if (application.LichKiemTras.Count != 0)
                continue;

            var isUpcoming = application.TrangThai == TrangThaiHoSo.ChoKiemTraDauVao;
            var time = isUpcoming
                ? DateTime.Today.AddDays(index + 1).AddHours(9)
                : DateTime.Today.AddDays(-(index + 1)).AddHours(9);
            db.LichKiemTraDauVaos.Add(new LichKiemTraDauVao
            {
                MaHoSo = application.MaHoSo,
                ThoiGianBatDau = time,
                ThoiGianKetThuc = time.AddHours(1),
                HinhThucKiemTra = index % 2 == 0 ? "Trực tiếp" : "Trực tuyến",
                DiaDiemHoacLienKet = index % 2 == 0
                    ? $"Phòng mẫu {index + 1:00}"
                    : "https://example.com/kiem-tra-mau",
                GiaoVienKiemTra = $"Giáo viên mẫu {index + 1:00}",
                GhiChu = "Lịch mẫu phục vụ kiểm thử.",
                TrangThai = isUpcoming ? TrangThaiLich.DaLenLich : TrangThaiLich.DaHoanThanh
            });
            scheduleCount++;
        }

        var resultCount = await db.KetQuaXepLops.CountAsync();
        foreach (var application in applications
                     .Where(x => x.TrangThai is TrangThaiHoSo.DuocXepLop or TrangThaiHoSo.ChuaDuocXepLop)
                     .Take(10))
        {
            if (application.KetQuaXepLop is not null || resultCount >= 10)
                continue;

            var placed = application.TrangThai == TrangThaiHoSo.DuocXepLop;
            db.KetQuaXepLops.Add(new KetQuaXepLop
            {
                MaHoSo = application.MaHoSo,
                DiemDanhGia = placed ? 75 : 45,
                NhanXet = placed
                    ? "Đạt yêu cầu và được xếp lớp."
                    : "Chưa đạt yêu cầu xếp lớp.",
                DuocXepLop = placed,
                NgayCapNhat = application.NgayXuLy ?? DateTime.Today.AddDays(-1)
            });
            resultCount++;
        }

        await db.SaveChangesAsync();
    }

    private static TrangThaiHoSo GetApplicationStatus(int index) => index switch
    {
        < 9 => TrangThaiHoSo.ChoDuyet,
        < 17 => TrangThaiHoSo.DuDieuKien,
        < 21 => TrangThaiHoSo.KhongDuDieuKien,
        < 25 => TrangThaiHoSo.ChoKiemTraDauVao,
        < 29 => TrangThaiHoSo.DaKiemTraDauVao,
        < 34 => TrangThaiHoSo.DuocXepLop,
        < 39 => TrangThaiHoSo.ChuaDuocXepLop,
        _ => TrangThaiHoSo.DaHuy
    };

    private static string StudentUsername(int number) =>
        number == 1 ? "student" : $"student{number:00}";
}
