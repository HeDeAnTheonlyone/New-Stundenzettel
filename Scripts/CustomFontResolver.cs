using PdfSharp.Fonts;

public class CustomFontResolver : IFontResolver
{
    public byte[] GetFont(string faceName) => Godot.FileAccess.GetFileAsBytes("user://Arial.ttf");

    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic) => new FontResolverInfo(familyName);
}