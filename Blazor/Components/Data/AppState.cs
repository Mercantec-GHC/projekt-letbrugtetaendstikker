public class AppState
{
    public User CurrentUser { get; private set; }

    public event Action OnChange;

    public void LogIn(User user)
    {
        CurrentUser = user;
        NotifyStateChanged();
    }

    public void LogOut()
    {
        CurrentUser = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}