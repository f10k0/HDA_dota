using HDA.Domain.Entities;
using HDA.Domain.Enums;

namespace HDA.Infrastructure.Services;

public class AppState
{
    public User? CurrentUser { get; private set; }
    public bool IsAuthenticated => CurrentUser != null;
    public bool IsAdmin => CurrentUser?.Role == UserRole.Admin;
    public bool IsProPlayer => CurrentUser?.Role == UserRole.ProPlayer || IsAdmin;

    public event Action? OnChange;

    public void SetUser(User? user)
    {
        CurrentUser = user;
        NotifyStateChanged();
    }

    public void Logout()
    {
        CurrentUser = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
