using DiamondListCreator.Models;
using System.Drawing;

namespace DiamondListCreator.Services
{
    public interface ICreator
    {
        Bitmap Create(DiamondSettings diamond, PathSettings paths);
    }
}
