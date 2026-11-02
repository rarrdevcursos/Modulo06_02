using MudBlazor;

namespace ClientAppdotnet.Theme;

public static class AppTheme
{
    public static readonly MudTheme Default = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#1565C0",
            Secondary = "#0EA5A4",
            Background = "#F3F6FB",
            Surface = "#FFFFFF",
            AppbarBackground = "#0B1F33",
            AppbarText = "#E2E8F0",
            DrawerBackground = "#0B1F33",
            DrawerText = "#CBD5E1",
            DrawerIcon = "#7DD3FC",
            TextPrimary = "#0F172A",
            TextSecondary = "#5B6B7F",
            ActionDefault = "#5B6B7F",
            Divider = "#E2E8F0",
            Success = "#0F9D58",
            Warning = "#D97706",
            Error = "#DC2626",
            Info = "#0284C7"
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#38BDF8",
            Secondary = "#2DD4BF",
            Background = "#0B1220",
            Surface = "#111C2E",
            AppbarBackground = "#0B1220",
            AppbarText = "#E2E8F0",
            DrawerBackground = "#0B1220",
            DrawerText = "#CBD5E1",
            DrawerIcon = "#7DD3FC",
            TextPrimary = "#E2E8F0",
            TextSecondary = "#94A3B8",
            ActionDefault = "#94A3B8",
            Divider = "#1E293B"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "14px"
        }
    };
}
