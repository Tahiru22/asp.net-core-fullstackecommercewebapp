using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using fullstackecommercewebapp.Models;
using fullstackecommercewebapp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PagedList;

namespace fullstackecommercewebapp.Controllers
{
    [Authorize(Roles = "Administrator, Products Manager")]
    public class ProductController : BaseController
    {
        public static string msg = "";
        private IWebHostEnvironment _hostingEnvironment;
        private readonly Cloudinary _cloudinary;
        public ProductController(IWebHostEnvironment hostingEnvironment, Cloudinary cloudinary)
        {
            _hostingEnvironment = hostingEnvironment;
            _cloudinary = cloudinary;
        }
        public IActionResult Index(string sortOrder, string CurrentCategoryFilter, string CurrentCompanyFilter,
             string CurrentProductFilter, string CategorySearchString,
            string CompanySearchString, string ProductSearchString, int? page)
        {
            ViewBag.msg = msg;
            msg = "";
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";
            if (CategorySearchString != null || CompanySearchString != null || ProductSearchString != null)
            {
                page = 1;
            }
            else
            {
                CategorySearchString = CurrentCategoryFilter;
                CompanySearchString = CurrentCompanyFilter;
                ProductSearchString = CurrentProductFilter;
            }
            ViewBag.CurrentCategoryFilter = CategorySearchString;
            ViewBag.CurrentCompanyFilter = CompanySearchString;
            ViewBag.CurrentProductFilter = ProductSearchString;

            var products = _uow.productRepo.getWithCategories();
            if (!String.IsNullOrEmpty(CategorySearchString))
            {
                products = products.Where(s => s.Category.Name.ToLower().Contains(CategorySearchString.ToLower()));
            }
            if (!String.IsNullOrEmpty(CompanySearchString))
            {
                products = products.Where(s => s.CompanyName.ToLower().Contains(CompanySearchString.ToLower()));
            }
            if (!String.IsNullOrEmpty(ProductSearchString))
            {
                products = products.Where(s => s.Name.ToLower().Contains(ProductSearchString.ToLower()));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(s => s.Name);
                    break;
                case "Price":
                    products = products.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(s => s.Price);
                    break;
                default:
                    products = products.OrderBy(s => s.Name);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(products.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public IActionResult Add()
        {
            ProductViewModel pvm = new ProductViewModel();
            pvm.Categories = _uow.categoryRepo.getAll();
            return View(pvm);
        }

        //[HttpPost]
        //public IActionResult AddAttributes(ProductViewModel pvm)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (pvm.CategoryId == 0)
        //        {
        //            ModelState.AddModelError("CategoryId", "You must select a category");
        //            pvm.Categories = _uow.categoryRepo.getAll();
        //            return View("Add", pvm);
        //        }
        //        if (pvm.Price <= 0)
        //        {
        //            ModelState.AddModelError("Price", "Price can not be below or equal to zero");
        //            pvm.Categories = _uow.categoryRepo.getAll();
        //            return View("Add", pvm);
        //        }
        //        IFormFile file = Request.Form.Files["Image1"];
        //        if (file == null)
        //        {
        //            ModelState.AddModelError("Image1", "You must upload an image for the product");
        //            pvm.Categories = _uow.categoryRepo.getAll();
        //            return View("Add", pvm);
        //        }
        //        if (file.Length > 0)
        //        {
        //            string file_name = Path.GetFileName(file.FileName);
        //            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images" + file_name);
        //            using (Stream fileStream = new FileStream(path, FileMode.Create))
        //            {
        //                file.CopyTo(fileStream);
        //            }
        //        }
        //        pvm.CategoryAttributes = _uow.categoryRepo.getCategoryAttributes(pvm.CategoryId);
        //        pvm.NonCategoryAttributes = _uow.categoryRepo.getNonCategoryAttributes(pvm.CategoryId);
        //        pvm.Image1 = Path.GetFileName(file.FileName);

        //        foreach (var modelState in ModelState.Values)
        //        {
        //            foreach (var error in modelState.Errors)
        //            {
        //                // Log or handle the error as needed
        //                Console.WriteLine(error.ErrorMessage);
        //            }
        //        }

        //        return View(pvm);
        //    }
        //    else
        //    {
        //        pvm.Categories = _uow.categoryRepo.getAll();
        //        return View("Add", pvm);
        //    }

        //}

        [HttpPost]
        public async Task<IActionResult> AddAttributes(ProductViewModel pvm)
        {
            if (ModelState.IsValid)
            {
                if (pvm.CategoryId == 0)
                {
                    ModelState.AddModelError("CategoryId", "You must select a category");
                    pvm.Categories = _uow.categoryRepo.getAll();
                    return View("Add", pvm);
                }
                if (pvm.Price <= 0)
                {
                    ModelState.AddModelError("Price", "Price cannot be below or equal to zero");
                    pvm.Categories = _uow.categoryRepo.getAll();
                    return View("Add", pvm);
                }

                IFormFile file = Request.Form.Files["Image1"];
                if (file == null)
                {
                    ModelState.AddModelError("Image1", "You must upload an image for the product");
                    pvm.Categories = _uow.categoryRepo.getAll();
                    return View("Add", pvm);
                }

                if (file.Length > 0)
                {
                    // Generate a short, unique identifier for the public ID
                    var publicId = $"products/{Guid.NewGuid().ToString().Substring(0, 8)}";
                    // Upload to Cloudinary
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, file.OpenReadStream()),
                        PublicId = publicId

                        //PublicId = $"products/{Path.GetFileNameWithoutExtension(file.FileName)}_{Guid.NewGuid()}"
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        pvm.Image1 = uploadResult.SecureUrl.AbsoluteUri;
                    }
                    else
                    {
                        ModelState.AddModelError("Image1", "There was a problem uploading the image");
                        pvm.Categories = _uow.categoryRepo.getAll();
                        return View("Add", pvm);
                    }
                }

                pvm.CategoryAttributes = _uow.categoryRepo.getCategoryAttributes(pvm.CategoryId);
                pvm.NonCategoryAttributes = _uow.categoryRepo.getNonCategoryAttributes(pvm.CategoryId);

                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        // Log or handle the error as needed
                        Console.WriteLine(error.ErrorMessage);
                    }
                }

                return View(pvm);
            }
            else
            {
                pvm.Categories = _uow.categoryRepo.getAll();
                return View("Add", pvm);
            }
        }

        [HttpPost]
        public IActionResult Add(ProductViewModel pvm)
        {
            if (ModelState.IsValid)
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<ProductViewModel, Product>());
                var mapper = new Mapper(config);
                Product p = mapper.Map<Product>(pvm);

                if (_uow.productRepo.checkUnique(p.Sku) != 0 && Request.Form["llow"] != "yes")
                {
                    ModelState.AddModelError("Sku", "Product SKU must be unique");
                    pvm.Categories = _uow.categoryRepo.getAll();
                    pvm.NonCategoryAttributes = _uow.attributeRepo.getAll();
                    return View(pvm);
                }
                if (pvm.SelectedAttributes != null)
                {
                    foreach (var att in pvm.SelectedAttributes)
                    {
                        if (Convert.ToString(Request.Form[Convert.ToString(att) + "value"]).Trim() == "")
                        {
                            System.Diagnostics.Debug.WriteLine(Convert.ToString(att) + "value");
                            ModelState.AddModelError(Convert.ToString(att) + "value", "Attributes Values can not be empty");
                            pvm.CategoryAttributes = _uow.categoryRepo.getCategoryAttributes(pvm.CategoryId);
                            pvm.NonCategoryAttributes = _uow.categoryRepo.getNonCategoryAttributes(pvm.CategoryId);
                            return View("AddAttributes", pvm);
                        }

                        p.ProductAttributeValue.Add(new ProductAttributeValue()
                        {
                            Product = p,
                            AttributeId = att,
                            Value = Request.Form[Convert.ToString(att) + "value"],
                            Unit = Request.Form[Convert.ToString(att) + "unit"]
                        });
                    }
                }

                if (pvm.AddedAttributes != null)
                {
                    var AttributesandValues = pvm.AddedAttributes.Zip(pvm.AddedProductAttributeValue, (n, w) => new { Att = n, Val = w });

                    foreach (var x in AttributesandValues)
                    {
                        if ((String.IsNullOrEmpty(x.Val.Value)) || Convert.ToString(x.Val.Value).Trim() == "")
                        {
                            ModelState.AddModelError(Convert.ToString(x.Val.Value), "Attributes Value can not be empty");
                            pvm.CategoryAttributes = _uow.categoryRepo.getCategoryAttributes(pvm.CategoryId);
                            pvm.NonCategoryAttributes = _uow.categoryRepo.getNonCategoryAttributes(pvm.CategoryId);
                            return View("AddAttributes", pvm);
                        }


                        if (_uow.attributeRepo.checkUnique(x.Att.Name) != 0)
                        {
                            _uow.attributeRepo.Add(x.Att);
                        }
                        p.ProductAttributeValue.Add(new ProductAttributeValue()
                        {
                            Product = p,
                            Attribute = x.Att,
                            Value = Convert.ToString(x.Val.Value),
                            Unit = x.Val.Unit
                        });
                    }
                }

                _uow.productRepo.Add(p);
                _uow.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                pvm.Categories = _uow.categoryRepo.getAll();
                pvm.NonCategoryAttributes = _uow.attributeRepo.getAll();
                return View(pvm);
            }

            msg = "added";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Product product = _uow.productRepo.getWithCategory(id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.id = id;
            ViewBag.Name = product.Category.Name;
            ViewBag.CategoryId = product.CategoryId;
            ProductViewModel p = new ProductViewModel();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductViewModel>());
            var mapper = new Mapper(config);
            mapper.Map(product, p);
            p.Categories = _uow.categoryRepo.getAll();
            p.NonCategoryAttributes = _uow.attributeRepo.getAll();
            return View(p);
        }

        [HttpPost]
        public async Task<IActionResult> EditAttributes(ProductViewModel pvm)
        {
            if (ModelState.IsValid)
            {
                int previousCategoryId = Convert.ToInt32(Request.Form["previousCategoryId"]);
                ViewBag.id = Convert.ToInt32(Request.Form["id"]);

                IFormFile file = Request.Form.Files["Image1"];
                if (file != null)
                {
                    if (file.Length > 0)
                    {
                        // Upload to Cloudinary
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.FileName, file.OpenReadStream()),
                            PublicId = $"products/{Guid.NewGuid()}"
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            pvm.Image1 = uploadResult.SecureUrl.AbsoluteUri;
                        }
                        else
                        {
                            ModelState.AddModelError("Image1", "There was a problem uploading the image");
                            pvm.Categories = _uow.categoryRepo.getAll();
                            return View("Add", pvm);
                        }
                    }
                }
                else
                {
                    // Retain the existing image if no new image is uploaded
                    pvm.Image1 = _uow.productRepo.getById(Convert.ToInt32(Request.Form["id"])).Image1;
                }

                if (previousCategoryId != pvm.CategoryId)
                {
                    pvm.CategoryAttributes = _uow.categoryRepo.getCategoryAttributes(pvm.CategoryId);
                    pvm.NonCategoryAttributes = _uow.categoryRepo.getNonCategoryAttributes(pvm.CategoryId);
                    return View("AddEditAttributes", pvm);
                }

                pvm.CategoryAttributes = _uow.categoryRepo.getCategoryAttributes(pvm.CategoryId);
                pvm.NonCategoryAttributes = _uow.categoryRepo.getNonCategoryAttributes(pvm.CategoryId);
                pvm.ProductAttributeValues = _uow.productRepo.getProductWithAttributes(Convert.ToInt32(Request.Form["id"]));

                return View(pvm);
            }
            else
            {
                ViewBag.Id = Request.Form["id"];
                pvm.Categories = _uow.categoryRepo.getAll();
                return View(pvm);
            }
        }

        [HttpPost]
        public IActionResult Edit(ProductViewModel pvm)
        {
            if (ModelState.IsValid)
            {
                Product product = _uow.productRepo.getById(Convert.ToInt32(Request.Form["id"]));
                var config = new MapperConfiguration(cfg => cfg.CreateMap<ProductViewModel, Product>());
                var mapper = new Mapper(config);
                string img = product.Image1;
                mapper.Map(pvm, product);
                if (String.IsNullOrEmpty(pvm.Image1))
                {
                    product.Image1 = img;
                }
                int check = _uow.productRepo.checkUnique(product.Sku);
                if (check != 0 && check != product.Id)
                {
                    ModelState.AddModelError("Sku", "Product SKU must be unique");
                    pvm.Categories = _uow.categoryRepo.getAll();
                    pvm.NonCategoryAttributes = _uow.attributeRepo.getAll();
                    return View(pvm);
                }

                _uow.productRepo.DeleteOnProductId(product.Id);

                if (pvm.SelectedAttributes != null)
                {
                    foreach (var item in pvm.SelectedAttributes)
                    {
                        if (Convert.ToString(Request.Form[Convert.ToString(item) + "value"]).Trim() == "")
                        {
                            ModelState.AddModelError(Convert.ToString(Request.Form[Convert.ToString(item) + "value"]), "Attributes Values can not be empty");
                            pvm.CategoryAttributes = _uow.categoryRepo.getCategoryAttributes(pvm.CategoryId);
                            pvm.NonCategoryAttributes = _uow.categoryRepo.getNonCategoryAttributes(pvm.CategoryId);
                            pvm.ProductAttributeValues = _uow.productRepo.getProductWithAttributes(Convert.ToInt32(Request.Form["id"]));
                            return View("EditAttributes", pvm);
                        }

                        product.ProductAttributeValue.Add(new ProductAttributeValue() { Product = product, AttributeId = item, Value = Request.Form[Convert.ToString(item + "value")], Unit = Request.Form[Convert.ToString(item + "unit")] });
                    }
                }
                if (pvm.AddedAttributes != null)
                {
                    var AttributesandValues = pvm.AddedAttributes.Zip(pvm.AddedProductAttributeValue, (n, w) => new { Att = n, Val = w });
                    foreach (var x in AttributesandValues)
                    {
                        if ((String.IsNullOrEmpty(x.Val.Value)) || Convert.ToString(x.Val.Value).Trim() == "")
                        {
                            ModelState.AddModelError(Convert.ToString(x.Val.Value), "Attributes Values can not be empty");
                            pvm.CategoryAttributes = _uow.categoryRepo.getCategoryAttributes(pvm.CategoryId);
                            pvm.NonCategoryAttributes = _uow.categoryRepo.getNonCategoryAttributes(pvm.CategoryId);
                            pvm.ProductAttributeValues = _uow.productRepo.getProductWithAttributes(Convert.ToInt32(Request.Form["id"]));
                            return View("EditAttributes", pvm);
                        }

                        if (_uow.attributeRepo.checkUnique(x.Att.Name) != 0)
                        {
                            _uow.attributeRepo.Add(x.Att);
                        }
                        product.ProductAttributeValue.Add(new ProductAttributeValue()
                        {
                            Product = product,
                            Attribute = x.Att,
                            Value = Convert.ToString(x.Val.Value),
                            Unit = x.Val.Unit
                        });
                    }
                }


                _uow.productRepo.Edit(product);
                _uow.SaveChanges();

            }
            else
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                pvm.Categories = _uow.categoryRepo.getAll();
                pvm.NonCategoryAttributes = _uow.attributeRepo.getAll();
                ViewBag.Id = Request.Form["id"];
                return View(pvm);
            }
            msg = "edited";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            Product product = _uow.productRepo.getById(id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }
            int delete = _uow.productRepo.checkDelete(id);
            if (delete == 0)
            {
                _uow.productRepo.Delete(id);
                _uow.SaveChanges();
                msg = "deleted";
            }
            else
            {
                msg = "NoDelete";
            }
            return RedirectToAction("Index");

        }
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var s = _uow.productRepo.getWithCategory(id);
            if (s == null)
            {
                return RedirectToAction("Index");
            }
            ProductViewModel p = new ProductViewModel();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductViewModel>());
            var mapper = new Mapper(config);
            mapper.Map(s, p);
            p.ProductAttributeValues = _uow.productRepo.getProductWithAttributes(s.Id);
            ViewBag.id = id;
            ViewBag.CategoryName = s.Category.Name;
            ViewBag.CreatedAt = s.CreatedAt;
            ViewBag.ModifiedAt = s.UpdatedAt;
            return View(p);
        }

    }
}
