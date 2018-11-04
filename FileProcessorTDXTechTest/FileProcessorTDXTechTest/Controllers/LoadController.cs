using FileProcessorTDXTechTest.FileHelpers;
using Microsoft.AspNetCore.Mvc;
using FileProcessorTDXTechTest.Models;
using FileProcessorTDXTechTestData;
using Microsoft.AspNetCore.Http;

namespace FileProcessorTDXTechTest.Controllers
{
    public class LoadController : Controller
    {
        private readonly FileProcessor _fileProcessor;

        public LoadController(FileProcessor fileProcessor, IOrderRepository orderRepo)
        {
            _fileProcessor = fileProcessor;
        }

        [HttpPost]
        public ActionResult LoadFile(IFormFile file)
        {
            OperationResult<LoadedFileRows> convertedFile = _fileProcessor.FileConversion(file);

            return View(convertedFile);
        }
    }
}
