using DisplayProject.Enums;

namespace DisplayProject.WindowManagement;

public static class ResizeHelpers
{
    public static int GetResizedDimension(int inputDimension, ResizeTo resizeTo) => resizeTo switch
    {
        ResizeTo.Full => inputDimension,
        ResizeTo.Half => (int)Math.Round((decimal)inputDimension / 2),
        ResizeTo.Third => (int)Math.Round((decimal)inputDimension / 3),
        ResizeTo.Fourths => (int)Math.Round((decimal)inputDimension / 4),
        _ => throw new NotImplementedException(),
    };
    public static int GetStartingXOrY(int screenWidthOrHeight, ResizeTo resizeTo, int nthPosition) => resizeTo switch
    {
        ResizeTo.Full => 0,
        ResizeTo.Half => (int)Math.Round((decimal)screenWidthOrHeight * nthPosition / 2),
        ResizeTo.Third => (int)Math.Round((decimal)screenWidthOrHeight * nthPosition / 3),
        ResizeTo.Fourths => (int)Math.Round((decimal)screenWidthOrHeight * nthPosition / 4),
        _ => throw new NotImplementedException(),
    };
}
