namespace GFDLibrary.Graphics;

public class ColorSwizzle
{
    public ColorChannel Red { get; set; }
    public ColorChannel Green { get; set; }
    public ColorChannel Blue { get; set; }
    public ColorChannel Alpha { get; set; }

    public ColorSwizzle( ColorChannel red, ColorChannel green, ColorChannel blue, ColorChannel alpha )
    {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = alpha;
    }

    public ColorSwizzle() { }

    public override string ToString()
    {
        return $"Red: {Red}, Green: {Green}, Blue: {Blue}, Alpha: {Alpha}";
    }
}