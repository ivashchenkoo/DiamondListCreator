using DiamondListCreator.Models;
using System.Collections.Generic;

namespace DiamondListCreator.Services
{
    public class CreatorService
    {
        public static void Create(List<DiamondSettings> diamonds, bool createList, bool saveAccounting, bool createListStickers, bool createLegends, bool createStickers, bool createCanvases)
        {
            PathSettings paths = PathSettingsService.ReadSettings();

            if (createList)
            {
                DiamondListService diamondListService = new DiamondListService();
                diamondListService.CreateDiamondsListAsync(diamonds, paths, saveAccounting, createListStickers);
            }

            if (createLegends)
            {
                LegendsService legendsService = new LegendsService();
                legendsService.CreateLegendsPdfAsync(diamonds, paths);
            }

            if (createStickers)
            {
                StickersService stickersService = new StickersService();
                stickersService.CreateStickersPdfAsync(diamonds, paths);
            }

            if (createCanvases)
            {
                CanvasesService canvasesService = new CanvasesService();
                canvasesService.CreateCanvasesFilesAsync(diamonds, paths);
            }
        }
    }
}
