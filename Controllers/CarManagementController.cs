using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CarDelership.Data;
using CarDelership.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace CarDelership.Controllers
{
    [Authorize]
    public class CarManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CarManagementController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // Проверка роли менеджера
        private bool IsManager()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            return userRole == "Менеджер" || userRole == "Администратор";
        }

        // GET: /CarManagement/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsManager())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var cars = await _context.Cars
                .Include(c => c.Model)
                    .ThenInclude(m => m.Manufacturer)
                .Include(c => c.Color)
                .Include(c => c.CarImages)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(cars);
        }

        // GET: /CarManagement/Create (исправленный - убран дубликат)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsManager())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            ViewBag.Models = await _context.Models
                .Include(m => m.Manufacturer)
                .ToListAsync();
            ViewBag.Colors = await _context.Colors.ToListAsync();
            ViewBag.AvailabilityStatuses = new List<string> { "В наличии", "Под заказ", "Нет в наличии", "Скоро в продаже" };
            ViewBag.Manufacturers = await _context.Manufacturers.ToListAsync();
            ViewBag.BodyTypes = await _context.BodyTypes.ToListAsync();
            ViewBag.EngineTypes = await _context.EngineTypes.ToListAsync();
            ViewBag.Transmissions = await _context.Transmissions.ToListAsync();
            ViewBag.Countries = await _context.Countries.ToListAsync();

            return View();
        }

        // POST: /CarManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cars car, List<IFormFile> images)
        {
            if (!IsManager())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (ModelState.IsValid)
            {
                car.CreatedAt = DateTime.Now;
                _context.Cars.Add(car);
                await _context.SaveChangesAsync();

                // Сохраняем изображения
                if (images != null && images.Any())
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    foreach (var image in images)
                    {
                        if (image.Length > 0)
                        {
                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(fileStream);
                            }

                            var carImage = new CarImages
                            {
                                Car_Id = car.Car_Id,
                                ImageName = uniqueFileName
                            };
                            _context.CarImages.Add(carImage);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Автомобиль успешно добавлен!";
                return RedirectToAction("Index");
            }

            // Если модель не валидна, загружаем данные заново
            ViewBag.Models = await _context.Models
                .Include(m => m.Manufacturer)
                .ToListAsync();
            ViewBag.Colors = await _context.Colors.ToListAsync();
            ViewBag.AvailabilityStatuses = new List<string> { "В наличии", "Под заказ", "Нет в наличии", "Скоро в продаже" };
            ViewBag.Manufacturers = await _context.Manufacturers.ToListAsync();
            ViewBag.BodyTypes = await _context.BodyTypes.ToListAsync();
            ViewBag.EngineTypes = await _context.EngineTypes.ToListAsync();
            ViewBag.Transmissions = await _context.Transmissions.ToListAsync();
            ViewBag.Countries = await _context.Countries.ToListAsync();

            return View(car);
        }

        // POST: /CarManagement/CreateModel (для AJAX)
        [HttpPost]
        public async Task<IActionResult> CreateModel([FromBody] CarModels newModel)
        {
            try
            {
                if (newModel == null)
                {
                    return Json(new { success = false, message = "Данные модели не получены" });
                }

                // Валидация
                if (string.IsNullOrEmpty(newModel.Name))
                {
                    return Json(new { success = false, message = "Название модели обязательно" });
                }

                _context.Models.Add(newModel);
                await _context.SaveChangesAsync();

                // Загружаем производителя для отображения
                await _context.Entry(newModel)
                    .Reference(m => m.Manufacturer)
                    .LoadAsync();

                var modelName = $"{newModel.Manufacturer?.Name ?? "Новый"} - {newModel.Name}";

                return Json(new
                {
                    success = true,
                    modelId = newModel.Model_Id,
                    modelName = modelName
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /CarManagement/CreateColor (для AJAX)
        [HttpPost]
        public async Task<IActionResult> CreateColor([FromBody] Color newColor)
        {
            try
            {
                if (newColor == null)
                {
                    return Json(new { success = false, message = "Данные цвета не получены" });
                }

                // Валидация
                if (string.IsNullOrEmpty(newColor.Name))
                {
                    return Json(new { success = false, message = "Название цвета обязательно" });
                }

                _context.Colors.Add(newColor);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    colorId = newColor.Color_Id,
                    colorName = newColor.Name
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /CarManagement/CreateManufacturer (для AJAX)
        [HttpPost]
        public async Task<IActionResult> CreateManufacturer([FromBody] Manufacturers newManufacturer)
        {
            try
            {
                if (newManufacturer == null)
                {
                    return Json(new { success = false, message = "Данные производителя не получены" });
                }

                if (string.IsNullOrEmpty(newManufacturer.Name))
                {
                    return Json(new { success = false, message = "Название производителя обязательно" });
                }

                _context.Manufacturers.Add(newManufacturer);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    manufacturerId = newManufacturer.Manufacturer_Id,
                    manufacturerName = newManufacturer.Name
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /CarManagement/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsManager())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var car = await _context.Cars
                .Include(c => c.Model)
                .Include(c => c.Color)
                .Include(c => c.CarImages)
                .FirstOrDefaultAsync(c => c.Car_Id == id);

            if (car == null)
            {
                return NotFound();
            }

            ViewBag.Models = await _context.Models
                .Include(m => m.Manufacturer)
                .ToListAsync();
            ViewBag.Colors = await _context.Colors.ToListAsync();
            ViewBag.AvailabilityStatuses = new List<string> { "В наличии", "Под заказ", "Нет в наличии", "Скоро в продаже" };

            return View(car);
        }

        // POST: /CarManagement/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cars car, List<IFormFile> images, List<int> deleteImages)
        {
            if (!IsManager())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (id != car.Car_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Получаем существующий автомобиль из базы
                    var existingCar = await _context.Cars
                        .Include(c => c.CarImages)
                        .FirstOrDefaultAsync(c => c.Car_Id == id);

                    if (existingCar == null)
                    {
                        return NotFound();
                    }

                    // Обновляем только изменяемые поля
                    existingCar.Name = car.Name;
                    existingCar.model_Id = car.model_Id;
                    existingCar.Color_Id = car.Color_Id;
                    existingCar.Price = car.Price;
                    existingCar.DiscountPrice = car.DiscountPrice;
                    existingCar.Year = car.Year;
                    existingCar.Mileage = car.Mileage;
                    existingCar.Quantity = car.Quantity;
                    existingCar.AvailabilityStatus = car.AvailabilityStatus;
                    existingCar.VIN = car.VIN;
                    existingCar.Description = car.Description;

                    // Удаляем выбранные изображения
                    if (deleteImages != null && deleteImages.Any())
                    {
                        foreach (var imageId in deleteImages)
                        {
                            var imageToDelete = existingCar.CarImages.FirstOrDefault(i => i.Image_Id == imageId);
                            if (imageToDelete != null)
                            {
                                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", imageToDelete.ImageName);
                                if (System.IO.File.Exists(filePath))
                                {
                                    System.IO.File.Delete(filePath);
                                }
                                _context.CarImages.Remove(imageToDelete);
                            }
                        }
                    }

                    // Добавляем новые изображения
                    if (images != null && images.Any())
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        foreach (var image in images)
                        {
                            if (image.Length > 0)
                            {
                                string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                                using (var fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    await image.CopyToAsync(fileStream);
                                }

                                var carImage = new CarImages
                                {
                                    Car_Id = existingCar.Car_Id,
                                    ImageName = uniqueFileName
                                };
                                _context.CarImages.Add(carImage);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Автомобиль успешно обновлен!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Car_Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction("Index");
            }

            ViewBag.Models = await _context.Models
                .Include(m => m.Manufacturer)
                .ToListAsync();
            ViewBag.Colors = await _context.Colors.ToListAsync();
            ViewBag.AvailabilityStatuses = new List<string> { "В наличии", "Под заказ", "Нет в наличии", "Скоро в продаже" };

            return View(car);
        }

        // POST: /CarManagement/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int carId)
        {
            if (!IsManager())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var car = await _context.Cars
                .Include(c => c.CarImages)
                .FirstOrDefaultAsync(c => c.Car_Id == carId);

            if (car != null)
            {
                // Удаляем файлы изображений
                if (car.CarImages != null && car.CarImages.Any())
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    foreach (var image in car.CarImages)
                    {
                        string filePath = Path.Combine(uploadsFolder, image.ImageName);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                }

                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Автомобиль '{car.Name}' успешно удален!";
            }
            else
            {
                TempData["Error"] = "Автомобиль не найден!";
            }

            return RedirectToAction("Index");
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Car_Id == id);
        }
    }
}